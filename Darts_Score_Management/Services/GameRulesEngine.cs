using AutoMapper;
using Darts_Score_Management.CustomExceptions;
using Darts_Score_Management.Data.Models;
using Darts_Score_Management.DTOs.Game;
using Darts_Score_Management.DTOs.Statistic;
using Darts_Score_Management.DTOs.Throw;
using Darts_Score_Management.Enums;
using Darts_Score_Management.Interfaces.ServiceInterfaces;
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

        public GameRulesEngine(
            IGameService gameService,
            ITurnService turnService,
            ILegService legService,
            ISetService setService,
            IStatisticService statisticService,
            IGameValidationService validationService,
            IMapper mapper)
        {
            _gameService = gameService;
            _turnService = turnService;
            _legService = legService;
            _setService = setService;
            _statisticService = statisticService;
            _validationService = validationService;
            _mapper = mapper;
        }
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


        public async Task<GameStateDTO> ProcessTurn(int turnId, List<CreateThrowDTO> throws)
        {
            using var transaction = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);
            try
            {
                var turnDto = await _turnService.GetTurnByIdAsync(turnId);
                if (turnDto == null)
                    throw new ArgumentException($"Turn with ID {turnId} not found");
                var turn = _mapper.Map<Turn>(turnDto);
                if (turn.Leg == null || turn.Leg.Set == null)
                    throw new InvalidOperationException("Turn is not properly associated with leg or set");


                // Validate turn
                if (!await _validationService.ValidateTurnOrder(turnId, turn.PlayerId))
                    throw new GameRuleViolationException("Invalid turn order", "TurnOrder");

                if (!await _validationService.ValidateMaximumThrows(turnId))
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


        private async Task<GameStateDTO> ProcessThrows(Turn turn, List<CreateThrowDTO> throws)
        {
            var gameState = new GameStateDTO();

            foreach (var throwDto in throws)
            {
                var validation = await ValidateThrow(throwDto, turn.Id);
                if (!validation.IsValid)
                {
                    gameState.IsBusted = true;
                    break;
                }

                await _turnService.AddThrowToTurnAsync(turn.Id, throwDto);
                await UpdateStatisticsSafely(turn.Leg.Set.GameId, turn.PlayerId, throwDto);
            }

            return gameState;
        }

        private async Task CheckAndUpdateGameCompletion(Turn turn, GameStateDTO gameState)
        {
            if (turn.EndingScore == 0)
            {
                await CheckLegCompletion(turn, gameState);

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

        private async Task CheckLegCompletion(Turn turn, GameStateDTO gameState)
        {
            await _legService.EndLegAsync(turn.LegId, turn.PlayerId);
            gameState.LegComplete = true;
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

