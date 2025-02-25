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
        public async Task<Data.Models.ValidationResult> ValidateThrow(CreateThrowDTO throwDto, int turnId)
        {
            if (throwDto == null)
                return new Data.Models.ValidationResult { IsValid = false, Message = "Throw data is required" };

            var turn = await _turnService.GetTurnByIdAsync(turnId);
            if (turn == null)
                return new Data.Models.ValidationResult { IsValid = false, Message = "Turn not found" };
            if (turn.Leg == null)
                return new Data.Models.ValidationResult { IsValid = false, Message = "Turn is not properly associated with a leg" };

            if (turn.Leg.Set == null)
                return new Data.Models.ValidationResult { IsValid = false, Message = "Leg is not properly associated with a set" };


            // Check if it's a valid dart segment and multiplier
            if (!IsValidDartSegment(throwDto.Segment, throwDto.Multiplier))
                return new Data.Models.ValidationResult { IsValid = false, Message = "Invalid dart segment or multiplier" };

            // Calculate potential score
            int points = throwDto.Segment * throwDto.Multiplier;
            int newScore = turn.EndingScore - points;

            // Check for bust conditions
            if (newScore < 0)
                return new Data.Models.ValidationResult { IsValid = false, Message = "Bust - score would go below 0" };

            // Check finish on double rule if applicable
            var game = await _gameService.GetGameByIdAsync(turn.Leg.Set.GameId);
            if (game.Settings.MustFinishOnDouble && newScore == 0 && throwDto.Multiplier != 2)
                return new Data.Models.ValidationResult { IsValid = false, Message = "Must finish on a double" };

            return new Data.Models.ValidationResult { IsValid = true };
        }

        //Processes a turn (up to 3 throws), validates it, updates the game state, and checks for completion (leg, set, game)
        public async Task<GameStateDTO> ProcessTurnForLeg(int legId, List<CreateThrowDTO> throws)
        {
            using var transaction = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);
            try
            {
                var leg = await _legService.GetLegByIdAsync(legId);
                if (leg == null)
                    throw new ArgumentException($"Leg with ID {legId} not found");

                // Determine the current player (based on turn order in the leg)
                var lastTurnDto = await _turnService.GetLastTurnByLegAsync(legId);
                Turn lastTurn =  _mapper.Map<Turn>(lastTurnDto); // Map TurnDTO to Turn

                int playerId = DetermineNextPlayer(leg, lastTurn);
                int turnNumber = lastTurn?.TurnNumber + 1 ?? 1; // Increment turn number or start at 1
                                                                                                            
                // Get the last turn for the current player in this leg to determine their starting score
                var lastPlayerTurnDto = await _turnService.GetLastTurnByPlayerAndLegAsync(playerId, legId);
                Turn lastPlayerTurn = _mapper.Map<Turn>(lastPlayerTurnDto);

                int startingScore = lastPlayerTurn?.EndingScore ?? leg.Set.Game.StartingScore;

                // Create a new turn
                var createTurnDto = new CreateTurnDTO
                {
                    LegId = legId,
                    PlayerId = playerId,
                    TurnNumber = turnNumber,
                    StartingScore = startingScore
                };
                var turnDto = await _turnService.CreateTurnAsync(createTurnDto);
                var turn = _mapper.Map<Turn>(turnDto);



                // Validate turn
                if (!await _validationService.ValidateTurnOrder(turn.Id, turn.PlayerId))
                    throw new GameRuleViolationException("Invalid turn order", "TurnOrder");

                // Ensure exactly 3 throws
                if (throws.Count != 3)
                    throw new GameRuleViolationException("A turn must contain exactly 3 throws", "ThrowCount");


                if (!await _validationService.ValidateMaximumThrows(turn.Id))
                    throw new GameRuleViolationException("Maximum throws exceeded", "MaxThrows");

                var gameState = await ProcessThrows(turn, throws);

                if (!gameState.IsBusted)
                {
                    await CheckAndUpdateGameCompletion(turn, gameState);
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

        private int DetermineNextPlayer(Leg leg, Turn lastTurn)
        {
            // Logic to determine the next player based on turn order in the leg
            var gamePlayers = leg.Set.Game.GamePlayers.OrderBy(gp => gp.TurnOrder).ToList();
            if (lastTurn == null)
                return gamePlayers[0].PlayerId; // First turn, start with lowest TurnOrder

            int currentPlayerIndex = gamePlayers.FindIndex(gp => gp.PlayerId == lastTurn.PlayerId);
            int nextIndex = (currentPlayerIndex + 1) % gamePlayers.Count;
            return gamePlayers[nextIndex].PlayerId;
        }


        private async Task<GameStateDTO> ProcessThrows(Turn turn, List<CreateThrowDTO> throws)
        {
            var gameState = new GameStateDTO();
            bool bustDetected = false;
            CreateThrowDTO bustingThrow = null;
            int bustThrowIndex = -1;

            // Single pass: Validate and track bust status for all throws before applying
            for (int i = 0; i < throws.Count; i++)
            {
                var throwDto = throws[i];
                var validation = await ValidateThrow(throwDto, turn.Id);

                if (!validation.IsValid)
                {
                    bustDetected = true;
                    bustingThrow = throwDto;
                    bustThrowIndex = i;
                    gameState.IsBusted = true;
                    gameState.Message = validation.Message;
                    break;
                }
            }

            // Process throws within the transaction scope (assumed from ProcessTurnForLeg)
            if (bustDetected)
            {
                // Reset score and total points for the entire turn if any throw busts
                var turnToUpdate = await _turnRepository.GetByIdAsync(turn.Id);
                turnToUpdate.EndingScore = turnToUpdate.StartingScore;
                turnToUpdate.TotalPoints = 0;

                // Process each throw, marking only the busting throw as IsBusted
                for (int i = 0; i < throws.Count; i++)
                {
                    var throwDto = throws[i];
                    var processedThrow = new CreateThrowDTO
                    {
                        Segment = throwDto.Segment,
                        Multiplier = throwDto.Multiplier
                    };

                    // Use AddThrowToTurnAsync, which will handle IsBusted based on the bust logic
                    await _turnService.AddThrowToTurnAsync(turn.Id, processedThrow);

                    // If this is the busting throw, ensure it’s marked as busted in the database
                    if (i == bustThrowIndex)
                    {
                        // Fetch the last throw added to verify/update IsBusted
                        var lastThrow = await _turnService.GetLastThrowForTurnAsync(turn.Id);
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

                    // Update statistics even for busted turns
                    await UpdateStatisticsSafely(turn.Leg.Set.GameId, turn.PlayerId, throwDto);
                }

                await _turnRepository.UpdateAsync(turnToUpdate); // Save the turn reset
            }
            else
            {
                // Process all throws normally if no bust occurs
                foreach (var throwDto in throws)
                {
                    await _turnService.AddThrowToTurnAsync(turn.Id, throwDto);
                    await UpdateStatisticsSafely(turn.Leg.Set.GameId, turn.PlayerId, throwDto);
                }

                // Reload the turn to get the latest state
                var updatedTurnDto = await _turnService.GetTurnByIdAsync(turn.Id);
                var updatedTurn = _mapper.Map<Turn>(updatedTurnDto);

                // Check if ending score is exactly 0 (potential win)
                if (updatedTurn.EndingScore == 0)
                {
                    var lastThrow = await _turnService.GetLastThrowForTurnAsync(turn.Id);
                    var game = await _gameService.GetGameByIdAsync(turn.Leg.Set.GameId);

                    // Check finish-on-double rule if applicable
                    if (game.Settings.MustFinishOnDouble && lastThrow?.Multiplier != 2)
                    {
                        gameState.IsBusted = true;
                        gameState.Message = "Must finish on a double";

                        // Reset score if finishing throw wasn't a double
                        var turnToUpdate = await _turnRepository.GetByIdAsync(turn.Id);
                        turnToUpdate.EndingScore = turnToUpdate.StartingScore;
                        turnToUpdate.TotalPoints = 0;
                        await _turnRepository.UpdateAsync(turnToUpdate);
                    }
                    else
                    {
                        // Valid win condition
                        await CheckLegCompletion(updatedTurn, gameState, lastThrow);
                    }
                }
            }

            return gameState;
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

        private async Task CheckLegCompletion(Turn turn, GameStateDTO gameState, ThrowDTO lastThrow)
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

        private async Task<StatisticDTO> UpdatePPDStatistic(IEnumerable<StatisticDTO> stats, int gamePlayerId, CreateThrowDTO throwDto)
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

