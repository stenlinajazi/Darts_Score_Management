using AutoMapper;
using Darts_Score_Management.CustomExceptions;
using Darts_Score_Management.Data.Models;
using Darts_Score_Management.DTOs.Game;
using Darts_Score_Management.DTOs.Statistic;
using Darts_Score_Management.DTOs.Throw;
using Darts_Score_Management.DTOs.Turn;
using Darts_Score_Management.Enums;
using Darts_Score_Management.Interfaces.RepositoryInterfaces;
using Darts_Score_Management.Interfaces.ServiceInterfaces;
using Darts_Score_Management.Repositories;
using System.Text.Json;
using System.Transactions;


namespace Darts_Score_Management.Services
{
    public class GameRulesEngine : IGameRulesEngine
    {
        private readonly IGameService _gameService;
        private readonly ITurnService _turnService;
        private readonly ILegService _legService;
        private readonly ISetService _setService;
        private readonly IStatisticService _statisticService;
        private readonly IGameValidationService _validationService;
        private readonly IMapper _mapper;
        private readonly IThrowRepository _throwRepository;
        private readonly ITurnRepository _turnRepository;

        public GameRulesEngine(
            IGameService gameService,
            ITurnService turnService,
            ILegService legService,
            ISetService setService,
            IStatisticService statisticService,
            IGameValidationService validationService,
            IMapper mapper,
            IThrowRepository throwRepository,
            ITurnRepository turnRepository)
        {
            _gameService = gameService;
            _turnService = turnService;
            _legService = legService;
            _setService = setService;
            _statisticService = statisticService;
            _validationService = validationService;
            _mapper = mapper;
            _throwRepository = throwRepository;
            _turnRepository = turnRepository;
        }

        //Validates a single dart throw against game rules (e.g., valid segments, bust conditions, finish-on-double rule) before it’s processed
        public async Task<ValidationResult> ValidateThrow(CreateThrowDTO throwDto, int turnId)
        {
            if (throwDto == null)
                return new ValidationResult { IsValid = false, Message = "Throw data is required" };

            var turn = await _turnService.GetTurnByIdAsync(turnId);
            if (turn == null)
                return new ValidationResult { IsValid = false, Message = "Turn not found" };
            if (turn.Leg == null || turn.Leg.Set == null)
                return new ValidationResult { IsValid = false, Message = "Invalid turn relationships" };


            // Check if it's a valid dart segment and multiplier
            if (!IsValidDartSegment(throwDto.Segment, throwDto.Multiplier))
                return new ValidationResult { IsValid = false, Message = "Invalid dart segment or multiplier" };

            // Calculate potential score
            int points = CalculatePoints(throwDto.Segment, throwDto.Multiplier);
            int newScore = turn.EndingScore - points;

            // Check for bust conditions
            if (newScore < 0)
                return new ValidationResult { IsValid = false, Message = "Bust - score would go below 0" };

            // Check finish on double rule if applicable
            var gameDto = await _gameService.GetGameByIdAsync(turn.Leg.Set.GameId);
            var game = _mapper.Map<Game>(gameDto);
            if (newScore == 0 && gameDto.Settings.MustFinishOnDouble && throwDto.Multiplier != 2)
                return new ValidationResult { IsValid = false, Message = "Must finish on a double" };
            return new ValidationResult { IsValid = true };
        }

        //Processes a turn (up to 3 throws), validates it, updates the game state, and checks for completion (leg, set, game)
        public async Task<GameStateDTO> ProcessTurnForLeg(int legId, List<CreateThrowDTO> throws)
        {
            using var transaction = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);
            try
            {
                if (throws == null || throws.Count != 3)
                    throw new GameRuleViolationException("A turn must contain exactly 3 throws", "ThrowCount");

                var turn = await SetupNewTurn(legId);

                // Validate turn
                if (!await _validationService.ValidateTurnOrder(turn.Id, turn.PlayerId))
                    throw new GameRuleViolationException("Invalid turn order", "TurnOrder");

                if (!await _validationService.ValidateMaximumThrows(turn.Id))
                    throw new GameRuleViolationException("Maximum throws exceeded", "MaxThrows");

                var gameState = await ProcessThrows(turn, throws);

                if (!gameState.IsBusted && gameState.LegComplete)
                {
                    await CheckForSetAndGameCompletion(turn, gameState);
                }

                transaction.Complete();
                return gameState;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in ProcessTurn: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
                throw;
            }
        }
        // Creates a new turn for the given leg
        private async Task<Turn> SetupNewTurn(int legId)
        {
            var leg = await _legService.GetLegByIdAsync(legId);
            if (leg == null)
                throw new ArgumentException($"Leg with ID {legId} not found");

            var lastTurnDto = await _turnService.GetLastTurnByLegAsync(legId);
            Turn lastTurn = _mapper.Map<Turn>(lastTurnDto);

            int playerId = DetermineNextPlayer(leg, lastTurn);
            int turnNumber = lastTurn?.TurnNumber + 1 ?? 1;

            // Get starting score for this player
            var lastPlayerTurnDto = await _turnService.GetLastTurnByPlayerAndLegAsync(playerId, legId);
            Turn lastPlayerTurn = _mapper.Map<Turn>(lastPlayerTurnDto);
            int startingScore = lastPlayerTurn?.EndingScore ?? leg.Set.Game.StartingScore;

            // Create the turn
            var createTurnDto = new CreateTurnDTO
            {
                LegId = legId,
                PlayerId = playerId,
                TurnNumber = turnNumber,
                StartingScore = startingScore
            };

            var turnDto = await _turnService.CreateTurnAsync(createTurnDto);
            return _mapper.Map<Turn>(turnDto);
        }
        // Process all throws in a turn
        private async Task<GameStateDTO> ProcessThrows(Turn turn, List<CreateThrowDTO> throws)
        {
            var gameState = new GameStateDTO();
            // Analyze throws to detect any busts without modifying data
            var bustAnalysis = AnalyzeThrowsForBust(turn, throws);

            // Process throws based on bust status
            if (bustAnalysis.HasBust)
            {
                await ProcessBustedTurn(turn, throws, bustAnalysis.BustIndex, bustAnalysis.BustMessage, gameState);
            }
            else
            {
                await ProcessSuccessfulTurn(turn, throws, gameState);
            }
            return gameState;
        }


        // Analyze throws for any potential busts without modifying data
        private BustAnalysisResult AnalyzeThrowsForBust(Turn turn, List<CreateThrowDTO> throws)
        {
            int simulatedScore = turn.StartingScore;

            for (int i = 0; i < throws.Count; i++)
            {
                var throwDto = throws[i];
                int points = CalculatePoints(throwDto.Segment, throwDto.Multiplier);
                int newScore = simulatedScore - points;

                // Check for bust conditions
                if (newScore < 0)
                {
                    return new BustAnalysisResult
                    {
                        HasBust = true,
                        BustIndex = i,
                        BustMessage = "Bust - score would go below 0"
                    };
                }

                // Check for finish-on-double rule
                if (newScore == 0)
                {
                    var game = _gameService.GetGameByIdAsync(turn.Leg.Set.GameId).Result;
                    if (game.Settings.MustFinishOnDouble && throwDto.Multiplier != 2)
                    {
                        return new BustAnalysisResult
                        {
                            HasBust = true,
                            BustIndex = i,
                            BustMessage = "Must finish on a double"
                        };
                    }
                }

                simulatedScore = newScore;
            }

            return new BustAnalysisResult { HasBust = false };
        }


        // Process a turn that has a bust
        private async Task ProcessBustedTurn(Turn turn, List<CreateThrowDTO> throws, int bustIndex, string bustMessage, GameStateDTO gameState)
        {
            // Reset turn score
            var turnToUpdate = await _turnRepository.GetTurnWithThrowsAsync(turn.Id);
            turnToUpdate.EndingScore = turnToUpdate.StartingScore;
            turnToUpdate.TotalPoints = 0;

            // Process throws up to and including the bust
            for (int i = 0; i < throws.Count; i++)
            {
                var throwDto = throws[i];
                await _turnService.AddThrowToTurnAsync(turn.Id, throwDto);

                // Mark the busting throw
                if (i == bustIndex)
                {
                    await MarkThrowAsBusted(turn.Id);
                }

                // Always update statistics
                await UpdateStatisticsSafely(turn.Leg.Set.GameId, turn.PlayerId, throwDto);
            }

            await _turnRepository.UpdateAsync(turnToUpdate);

            // Set game state for bust
            gameState.IsBusted = true;
            gameState.Message = bustMessage;
        }


        private async Task MarkThrowAsBusted(int turnId)
        {
            var lastThrow = await _turnService.GetLastThrowForTurnAsync(turnId);
            if (lastThrow != null)
            {
                var throwEntity = await _throwRepository.GetByIdAsync(lastThrow.Id);
                if (throwEntity != null)
                {
                    throwEntity.IsBusted = true;
                    await _throwRepository.UpdateAsync(throwEntity);
                }
            }
        }

        // Process a turn with no busts
        private async Task ProcessSuccessfulTurn(Turn turn, List<CreateThrowDTO> throws, GameStateDTO gameState)
        {
            var turnToUpdate = await _turnRepository.GetTurnWithThrowsAsync(turn.Id);
            int currentScore = turnToUpdate.StartingScore;
            var game = await _gameService.GetGameByIdAsync(turn.Leg.Set.GameId);
            ThrowDTO lastThrow = null;
            // Process all throws
            foreach (var throwDto in throws)
            {
                int points = CalculatePoints(throwDto.Segment, throwDto.Multiplier);
                currentScore -= points;

                // Capture the ThrowDTO returned by AddThrowToTurnAsync
                var addedThrowDto = await _turnService.AddThrowToTurnAsync(turn.Id, throwDto);
                lastThrow = addedThrowDto.Throws.Last(); 
                await UpdateStatisticsSafely(turn.Leg.Set.GameId, turn.PlayerId, throwDto);
                if (currentScore < 0)
                {
                    turnToUpdate.EndingScore = turnToUpdate.StartingScore;
                    turnToUpdate.TotalPoints = 0;
                    await MarkThrowAsBusted(turn.Id);
                    gameState.IsBusted = true;
                    gameState.Message = "Bust - score went below 0";
                    break;
                }
                else if (currentScore == 0)
                {
                    if (game.Settings.MustFinishOnDouble && throwDto.Multiplier != 2)
                    {
                        turnToUpdate.EndingScore = turnToUpdate.StartingScore;
                        turnToUpdate.TotalPoints = 0;
                        await MarkThrowAsBusted(turn.Id);
                        gameState.IsBusted = true;
                        gameState.Message = "Must finish on a double";
                        break;
                    }
                    else
                    {
                        turnToUpdate.EndingScore = currentScore;
                        turnToUpdate.TotalPoints = turnToUpdate.StartingScore;
                        await CompleteLeg(turnToUpdate, gameState, lastThrow);
                    }
                }
                else
                {
                    turnToUpdate.EndingScore = currentScore;
                    turnToUpdate.TotalPoints += points;
                }
            }


            await _turnRepository.UpdateAsync(turnToUpdate);
        }

        // Unified leg completion method
        private async Task CompleteLeg(Turn turn, GameStateDTO gameState, ThrowDTO lastThrow)
        {
            // Update leg with winner
            var leg = await _legService.GetLegByIdAsync(turn.LegId);
            if (leg != null)
            {
                leg.WinnerPlayerId = turn.PlayerId;
                await _legService.UpdateLegAsync(leg);
            }

            gameState.LegComplete = true;
            gameState.Message = $"Leg {turn.LegId} completed by Player {turn.PlayerId}";
        }

        // Check for set and game completion
        private async Task CheckForSetAndGameCompletion(Turn turn, GameStateDTO gameState)
        {
            if (gameState.LegComplete)
            {
                await CheckSetCompletion(turn, gameState);

                if (gameState.SetComplete)
                {
                    await CheckGameCompletion(turn, gameState);
                }
            }
        }

        private int CalculatePoints(int segment, int multiplier)
        {
            return segment * multiplier;
        }

        private int DetermineNextPlayer(Leg leg, Turn lastTurn)
        {
            // Logic to determine the next player based on turn order in the leg
            var gamePlayers = leg.Set.Game.GamePlayers.OrderBy(gp => gp.TurnOrder).ToList();//retrieve all players in the game ordered by their turn order
            if (lastTurn == null)
                return gamePlayers[0].PlayerId; // First turn, start with lowest TurnOrder

            int currentPlayerIndex = gamePlayers.FindIndex(gp => gp.PlayerId == lastTurn.PlayerId);
            int nextIndex = (currentPlayerIndex + 1) % gamePlayers.Count; // The modulo operation ensures that after the last player, the index wraps around to the first player, maintaining a circular order
            return gamePlayers[nextIndex].PlayerId;
        }

        private async Task CheckAndUpdateGameCompletion(Turn turn, GameStateDTO gameState) 
        {
            if (turn.EndingScore == 0)
            {
                await CheckLegCompletion(turn, gameState, null);

                if (gameState.LegComplete)
                {
                    await CheckSetCompletion(turn, gameState);

                    if (gameState.SetComplete)
                    {
                        await CheckGameCompletion(turn, gameState);
                    }
                }
            }
        }

         private async Task CheckLegCompletion(Turn turn, GameStateDTO gameState, ThrowDTO lastThrow)//Its called in line 285 and in the method above  Why?Is 1 time not enough
         {
            // Verify the last throw meets the double requirement if needed
            var game = await _gameService.GetGameByIdAsync(turn.Leg.Set.GameId);
            if (game.Settings.MustFinishOnDouble && turn.EndingScore == 0 && lastThrow.Multiplier != 2)
            {
                throw new GameRuleViolationException("Leg must end on a double", "FinishOnDouble");
            }
            // Update the leg with the winner
            var leg = await _legService.GetLegByIdAsync(turn.LegId);
            if (leg != null)
            {
                leg.WinnerPlayerId = turn.PlayerId;
                await _legService.UpdateLegAsync(leg); // Assume UpdateLegAsync exists or update EndLegAsync
            }
            gameState.LegComplete = true;
            gameState.Message = $"Leg {turn.LegId} completed by Player {turn.PlayerId} with double finish on {lastThrow?.Segment} (multiplier: {lastThrow?.Multiplier})";
         }
    

         private async Task CheckSetCompletion(Turn turn, GameStateDTO gameState)
         {
            var leg = await _legService.GetLegByIdAsync(turn.LegId);
            var set = await _setService.GetSetByIdAsync(leg.SetId);
            var legsWon = set.Legs.Count(l => l.WinnerPlayerId == turn.PlayerId);

            if (legsWon >= set.Game.Settings.LegsPerSet)
            {
                await _setService.EndSetAsync(set.Id, turn.PlayerId);
                gameState.SetComplete = true;
            }
         }

        private async Task CheckGameCompletion(Turn turn, GameStateDTO gameState)
        {
            var game = await _gameService.GetGameByIdAsync(turn.Leg.Set.GameId);
            var setsWon = game.Sets.Count(s => s.WinnerPlayerId == turn.PlayerId);

            if (setsWon >= game.Settings.SetsToWin)
            {
                await _gameService.EndGameAsync(game.Id, turn.PlayerId);
                gameState.GameComplete = true;
            }
        }

        private async Task UpdateStatisticsSafely(int gameId, int playerId, CreateThrowDTO throwDto)
        {
            try
            {
                var game = await _gameService.GetGameByIdAsync(gameId);
                var gamePlayer = game.Players.First(gp => gp.Player.Id == playerId);
                var stats = await _statisticService.GetPlayerGameStatisticsAsync(gamePlayer.Id);
                var updatedStats = new List<StatisticDTO>();

                // Update PPD
                try
                {
                    var ppdStat = await UpdatePPDStatistic(stats, gamePlayer.Id, throwDto);
                    if (ppdStat != null) updatedStats.Add(ppdStat);
                }
                catch (Exception ex)
                {
                    throw new StatisticsUpdateException("Failed to update PPD statistic", gamePlayer.Id, StatisticType.PPD);
                }

                // Update other statistics
                await UpdateHighScoreStatistics(stats, gamePlayer.Id, throwDto, updatedStats);

                if (updatedStats.Any())
                {
                    await _statisticService.UpdateStatisticsAsync(gamePlayer.Id, updatedStats);
                }
            }
            catch (Exception ex)
            {
                // Log the error but don't fail the turn
                // Consider implementing retry logic here
                // Throw custom exception if critical
            }
        }

        private async Task<StatisticDTO> UpdatePPDStatistic(IEnumerable<StatisticDTO> stats, int gamePlayerId, CreateThrowDTO throwDto)//Needs to be implemented
        {
            var ppdStat = stats.FirstOrDefault(s => s.Type == StatisticType.PPD) ??
                          new StatisticDTO { Type = StatisticType.PPD, Value = "0", GamePlayerId = gamePlayerId };

            var currentPPD = decimal.Parse(ppdStat.Value);
            var points = throwDto.Segment * throwDto.Multiplier;
            // Calculate new PPD...

            return ppdStat;
        }

        private async Task UpdateHighScoreStatistics(IEnumerable<StatisticDTO> stats, int gamePlayerId, CreateThrowDTO throwDto, List<StatisticDTO> updatedStats)
        {
            var points = throwDto.Segment * throwDto.Multiplier;

            if (points >= 140)
            {
                try
                {
                    var count140Stat = await Update140PlusStatistic(stats, gamePlayerId);
                    if (count140Stat != null) updatedStats.Add(count140Stat);
                }
                catch (Exception ex)
                {
                    throw new StatisticsUpdateException("Failed to update 140+ statistic", gamePlayerId, StatisticType.Count140Plus);
                }
            }

            if (points == 180)
            {
                try
                {
                    var count180Stat = await Update180Statistic(stats, gamePlayerId);
                    if (count180Stat != null) updatedStats.Add(count180Stat);
                }
                catch (Exception ex)
                {
                    throw new StatisticsUpdateException("Failed to update 180s statistic", gamePlayerId, StatisticType.Count180s);
                }
            }
        }

        private bool IsValidDartSegment(int segment, int multiplier)
        {
           if (segment < 1 || (segment > 20 && segment != 25)) return false;
           if (multiplier < 1 || multiplier > 3) return false;
           if (segment == 25 && multiplier > 2) return false; // Bullseye can only be single or double
           return true;
        }

        private async Task<StatisticDTO> Update140PlusStatistic(IEnumerable<StatisticDTO> stats, int gamePlayerId)
        {
            var count140Stat = stats.FirstOrDefault(s => s.Type == StatisticType.Count140Plus) ??
                              new StatisticDTO
                              {
                                  Type = StatisticType.Count140Plus,
                                  Value = "0",
                                  GamePlayerId = gamePlayerId
                              };

            count140Stat.Value = (int.Parse(count140Stat.Value) + 1).ToString();
            return count140Stat;
        }

        private async Task<StatisticDTO> Update180Statistic(IEnumerable<StatisticDTO> stats, int gamePlayerId)
        {
            var count180Stat = stats.FirstOrDefault(s => s.Type == StatisticType.Count180s) ??
                              new StatisticDTO
                              {
                                  Type = StatisticType.Count180s,
                                  Value = "0",
                                  GamePlayerId = gamePlayerId
                              };

            count180Stat.Value = (int.Parse(count180Stat.Value) + 1).ToString();
            return count180Stat;
        }



    }
} 

