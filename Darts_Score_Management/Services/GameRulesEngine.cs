using AutoMapper;
using Darts_Score_Management.CustomExceptions;
using Darts_Score_Management.Data;
using Darts_Score_Management.Data.Models;
using Darts_Score_Management.DTOs.Game;
using Darts_Score_Management.DTOs.Leg;
using Darts_Score_Management.DTOs.Set;
using Darts_Score_Management.DTOs.Statistic;
using Darts_Score_Management.DTOs.Throw;
using Darts_Score_Management.DTOs.Turn;
using Darts_Score_Management.Enums;
using Darts_Score_Management.Interfaces.RepositoryInterfaces;
using Darts_Score_Management.Interfaces.ServiceInterfaces;
using Darts_Score_Management.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
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

        //Processes a turn (up to 3 throws), validates it, updates the game state, and checks for completion (leg, set, game)
        public async Task<GameStateDTO> ProcessTurnForLeg(int legId, List<CreateThrowDTO> throws)
        {
            using var transaction = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);
            try
            {

                var turn = await SetupNewTurn(legId);

                if (!await ValidateLegIsActive(legId, turn.PlayerId))
                    throw new GameRuleViolationException("Cannot make throws to this leg - you must complete active legs and sets first", "LegSequence");

                if (!await _validationService.ValidateTurnOrder(turn.Id, turn.PlayerId))
                    throw new GameRuleViolationException("Invalid turn order", "TurnOrder");

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
        private async Task<Turn> SetupNewTurn(int legId)
        {
            var leg = await _legService.GetLegByIdAsync(legId);
            if (leg == null)
                throw new ArgumentException($"Leg with ID {legId} not found");

            // Check if the leg is already complete before setting up a new turn
            if (leg.WinnerPlayerId != null)
                throw new GameRuleViolationException($"Leg {legId} is already complete and cannot accept further throws", "LegComplete");

            var lastTurnDto = await _turnService.GetLastTurnByLegAsync(legId);
            Turn lastTurn = _mapper.Map<Turn>(lastTurnDto);

            int playerId = DetermineNextPlayer(leg, lastTurn);
            int turnNumber = lastTurn?.TurnNumber + 1 ?? 1;

            // Get starting score for this player
            var lastPlayerTurnDto = await _turnService.GetLastTurnByPlayerAndLegAsync(playerId, legId);
            Turn lastPlayerTurn = _mapper.Map<Turn>(lastPlayerTurnDto);
            int startingScore = lastPlayerTurn?.EndingScore ?? leg.Set.Game.StartingScore;

            var createTurnDto = new CreateTurnDTO
            {
                LegId = legId,
                PlayerId = playerId,
                TurnNumber = turnNumber,
                StartingScore = startingScore
            };

            var turnDto = await _turnService.CreateTurnAsync(createTurnDto);
            return await _turnRepository.GetTurnWithThrowsAsync(turnDto.Id);
        }
        // Process all throws in a turn
        private async Task<GameStateDTO> ProcessThrows(Turn turn, List<CreateThrowDTO> throws)
        {
            var gameState = new GameStateDTO();
            var bustAnalysis = AnalyzeThrowsForBust(turn, throws);

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

        private BustAnalysisResult AnalyzeThrowsForBust(Turn turn, List<CreateThrowDTO> throws)
        {
            int simulatedScore = turn.StartingScore;

            for (int i = 0; i < throws.Count; i++)
            {
                var throwDto = throws[i];
                if (!IsValidDartSegment(throwDto.Segment, throwDto.Multiplier))
                {
                    return new BustAnalysisResult
                    {
                        HasBust = true,
                        BustIndex = i,
                        BustMessage = "Invalid dart segment or multiplier"
                    };
                }
                int points = CalculatePoints(throwDto.Segment, throwDto.Multiplier);
                int newScore = simulatedScore - points; 
                var game = _gameService.GetGameByIdAsync(turn.Leg.Set.GameId).Result;
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

                if (newScore == 1 && game.Settings.MustFinishOnDouble)
                {
                    return new BustAnalysisResult
                    {
                        HasBust = true,
                        BustIndex = i,
                        BustMessage = "Bust - cannot finish on 1 with finish-on-double rule"
                    };
                }

                // Check for finish-on-double rule
                if (newScore == 0)
                {
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

        private async Task ProcessBustedTurn(Turn turn, List<CreateThrowDTO> throws, int bustIndex, string bustMessage, GameStateDTO gameState)
        {
            // Reset turn score
            turn.EndingScore = turn.StartingScore;
            turn.TotalPoints = 0;

            // Process throws up to and including the bust
            for (int i = 0; i < throws.Count; i++)
            {
                var throwDto = throws[i];
                // Create and add the throw directly to the turn entity
                var newThrow = new Throw
                {
                    TurnId = turn.Id,
                    ThrowNumber = turn.Throws.Count + 1,
                    Segment = throwDto.Segment,
                    Multiplier = throwDto.Multiplier,
                    IsBusted = (i == bustIndex)
                };

                turn.Throws.Add(newThrow);
            }

            await _turnRepository.UpdateAsync(turn);

            gameState.IsBusted = true;
            gameState.Message = bustMessage;
            gameState.RemainingScore = turn.EndingScore;
        }

        
        private async Task ProcessSuccessfulTurn(Turn turn, List<CreateThrowDTO> throws, GameStateDTO gameState)
        {
            int currentScore = turn.StartingScore;
            var game = await _gameService.GetGameByIdAsync(turn.Leg.Set.GameId);
            ThrowDTO lastThrow = null;

            turn.IsCheckoutAttempt = IsCheckoutPossible(turn.StartingScore);

            foreach (var throwDto in throws)
            {
                int points = CalculatePoints(throwDto.Segment, throwDto.Multiplier);
                currentScore -= points;
     
                var newThrow = new Throw
                {
                    TurnId = turn.Id,
                    ThrowNumber = turn.Throws.Count + 1,
                    Segment = throwDto.Segment,
                    Multiplier = throwDto.Multiplier,
                    IsBusted = false
                };
                turn.Throws.Add(newThrow);
                turn.TotalPoints += points;
            }

            turn.EndingScore = currentScore;

            if (currentScore == 0)
            {
                turn.IsCheckoutSuccessful = true;
                await CompleteLeg(turn, gameState, lastThrow);
            }

            await _turnRepository.UpdateAsync(turn);
            gameState.RemainingScore = turn.EndingScore;
        }
        private async Task CompleteLeg(Turn turn, GameStateDTO gameState, ThrowDTO lastThrow)
        {
            // Update leg with winner
            var leg = await _legService.GetLegByIdAsync(turn.LegId);
            if (leg != null)
            {
                leg.WinnerPlayerId = turn.PlayerId;
                await _legService.UpdateLegAsync(leg);

                // Update leg-level statistics after processing the turn
                var gamePlayers = await _legService.GetGamePlayersForLegAsync(turn.LegId);
                if (gamePlayers == null || !gamePlayers.Any())
                    throw new KeyNotFoundException($"No game players found for Leg with ID {turn.LegId}.");
                await _statisticService.UpdateLegStatsAsync(turn.LegId, gamePlayers);
            }

            gameState.LegComplete = true;
            gameState.Message = $"Leg {turn.LegId} completed by Player {turn.PlayerId}";
        }

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

         private async Task CheckSetCompletion(Turn turn, GameStateDTO gameState)
         {
            var leg = await _legService.GetLegByIdAsync(turn.LegId);
            var set = await _setService.GetSetByIdAsync(leg.SetId);
            var game = await _gameService.GetGameByIdAsync(set.GameId);
            var legsWon = set.Legs.Count(l => l.WinnerPlayerId == turn.PlayerId);
            var totalLegs = set.Legs.Count;
           

            // Count legs won by each player to determine if there's a tie
            var players = set.Legs.Select(l => l.WinnerPlayerId).Distinct().Where(id => id.HasValue).Select(id => id.Value).ToList();
            var legsWonPerPlayer = players.ToDictionary(p => p, p => set.Legs.Count(l => l.WinnerPlayerId == p));
            bool isTie = players.Count > 1 && players.All(p => legsWonPerPlayer[p] == legsWonPerPlayer[players[0]]);

            if (legsWon >= game.Settings.LegsPerSet)
            {
                // Update set-level statistics before ending the set
                await _statisticService.UpdateSetStatsAsync(set.Id, legsWonPerPlayer);
                await _setService.EndSetAsync(set.Id, turn.PlayerId);
                gameState.SetComplete = true;
            }
            else if (isTie && totalLegs == game.Settings.LegsPerSet)
            {
                // If there's a tie (e.g., 1-1 for LegsPerSet = 2), create a tiebreaker leg
                await CreateTiebreakerLeg(set.Id, game.Settings.LegsPerSet);
            }
        }

        private async Task CheckGameCompletion(Turn turn, GameStateDTO gameState)
        {
            var game = await _gameService.GetGameByIdAsync(turn.Leg.Set.GameId);
            var setsWon = game.Sets.Count(s => s.WinnerPlayerId == turn.PlayerId);
            var totalSets = game.Sets.Count;

            // Count sets won by each player to determine if there's a tie
            var players = game.Sets.Select(s => s.WinnerPlayerId).Distinct().Where(id => id.HasValue).Select(id => id.Value).ToList();
            var setsPerPlayer = players.ToDictionary(p => p, p => game.Sets.Count(s => s.WinnerPlayerId == p));
            bool isTie = players.Count > 1 && players.All(p => setsPerPlayer[p] == setsPerPlayer[players[0]]);

            if (setsWon >= game.Settings.SetsToWin)
            {
                // Update game-level statistics before ending the game
                await _statisticService.UpdateGameStatsAsync(game.Id, setsPerPlayer);
                await _gameService.EndGameAsync(game.Id, turn.PlayerId);
                gameState.GameComplete = true;
            }
            else if (isTie && totalSets == game.Settings.SetsToWin)
            {
                // If there's a tie (e.g., 1-1 for SetsToWin = 2), create a tiebreaker set
                await CreateTiebreakerSet(game.Id, game.Settings.SetsToWin);
            }   
        }

        private async Task CreateTiebreakerLeg(int setId, int legsPerSet)
        {
            var set = await _setService.GetSetByIdAsync(setId);
            var totalLegs = set.Legs.Count;
            var newLegNumber = totalLegs + 1;

            var createLegDto = new CreateLegDTO
            {
                SetId = setId,
                LegNumber = newLegNumber
            };

            await _legService.CreateLegAsync(createLegDto);
        }

        private async Task CreateTiebreakerSet(int gameId, int setsToWin)
        {
            var game = await _gameService.GetGameByIdAsync(gameId);
            var totalSets = game.Sets.Count;
            var newSetNumber = totalSets + 1;

            var createSetDto = new CreateSetDTO
            {
                GameId = gameId,
                SetNumber = newSetNumber
            };

            var setDto = await _setService.CreateSetAsync(createSetDto);

            // Create initial legs for the new set based on LegsPerSet
            int initialLegs = game.Settings.LegsPerSet;
            for (int legNumber = 1; legNumber <= initialLegs; legNumber++)
            {
                var createLegDto = new CreateLegDTO
                {
                    SetId = setDto.Id,
                    LegNumber = legNumber
                };

                await _legService.CreateLegAsync(createLegDto);
            }
        }
        private async Task<bool> ValidateLegIsActive(int legId, int playerId)
        {
            var leg = await _legService.GetLegByIdAsync(legId);
            if (leg == null)
                return false;

            var set = await _setService.GetSetByIdAsync(leg.SetId);
            if (set == null)
                return false;

            var game = await _gameService.GetGameByIdAsync(set.GameId);
            if (game == null)
                return false;

            // Check if the game is still active
            var winner = game.Players.FirstOrDefault(gp => gp.IsWinner);
            if (winner != null)
                return false;

            // Check if the set is active (previous sets are all completed)
            var sets = await _setService.GetSetsByGameIdAsync(game.Id);
            var orderedSets = sets.OrderBy(s => s.SetNumber).ToList();

            // Find the current set's position
            int currentSetIndex = orderedSets.FindIndex(s => s.Id == set.Id);

            // Check if any previous set is incomplete
            for (int i = 0; i < currentSetIndex; i++)
            {
                if (orderedSets[i].WinnerPlayerId == null)
                    return false; // A previous set is not complete
            }

            // Check if the leg is active (previous legs in this set are all completed)
            var legs = await _legService.GetLegsBySetIdAsync(set.Id);
            var orderedLegs = legs.OrderBy(l => l.LegNumber).ToList();

            // Find the current leg's position
            int currentLegIndex = orderedLegs.FindIndex(l => l.Id == legId);

            // Check if any previous leg is incomplete
            for (int i = 0; i < currentLegIndex; i++)
            {
                if (orderedLegs[i].WinnerPlayerId == null)
                    return false; // A previous leg is not complete
            }
            return true;
        }
        private bool IsValidDartSegment(int segment, int multiplier)
        {
           if (segment == 0 && multiplier == 1) return true;
           if (segment < 1 || (segment > 20 && segment != 25)) return false;
           if (multiplier < 1 || multiplier > 3) return false;
           if (segment == 25 && multiplier > 2) return false; 
           return true;
        }

        private static bool IsCheckoutPossible(int score)
        {
            if (score < 170 && score != 169 && score != 168 && score != 166 &&
         score != 165 && score != 163 && score != 162 && score != 159)
            {
                return true;
            }
            return false;
        }
    }
} 

