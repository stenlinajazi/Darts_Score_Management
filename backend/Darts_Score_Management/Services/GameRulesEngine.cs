using AutoMapper;
using Darts_Score_Management.CustomExceptions;
using Darts_Score_Management.Data;
using Darts_Score_Management.Data.Models;
using Darts_Score_Management.DTOs.Game.Core;
using Darts_Score_Management.DTOs.Game.State;
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

                Turn turn = await SetupNewTurn(legId);

                if (!await _validationService.ValidateTurnOrder(turn.Id, turn.PlayerId))
                    throw new GameRuleViolationException("Invalid turn order", "TurnOrder");

                GameStateDTO gameState = await ProcessThrows(turn, throws);

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
            Leg leg = await _legService.GetLegByIdAsync(legId);
            if (leg == null)
                throw new ArgumentException($"Leg with ID {legId} not found");

            // Check if the leg is already complete before setting up a new turn
            if (leg.WinnerPlayerId != null)
                throw new GameRuleViolationException($"Leg {legId} is already complete and cannot accept further throws", "LegComplete");

            TurnDTO lastTurnDto = await _turnService.GetLastTurnByLegAsync(legId);
            Turn lastTurn = _mapper.Map<Turn>(lastTurnDto);

            int playerId = DetermineNextPlayer(leg, lastTurn);
            int turnNumber = lastTurn?.TurnNumber + 1 ?? 1;

            // Get starting score for this player
            TurnDTO lastPlayerTurnDto = await _turnService.GetLastTurnByPlayerAndLegAsync(playerId, legId);
            Turn lastPlayerTurn = _mapper.Map<Turn>(lastPlayerTurnDto);
            int startingScore = lastPlayerTurn?.EndingScore ?? leg.Set.Game.StartingScore;

            CreateTurnDTO createTurnDto = new CreateTurnDTO
            {
                LegId = legId,
                PlayerId = playerId,
                TurnNumber = turnNumber,
                StartingScore = startingScore
            };

            TurnDTO turnDto = await _turnService.CreateTurnAsync(createTurnDto);
            return await _turnRepository.GetTurnWithThrowsAsync(turnDto.Id);
        }
        // Process all throws in a turn
        private async Task<GameStateDTO> ProcessThrows(Turn turn, List<CreateThrowDTO> throws)
        {
            GameStateDTO gameState = new GameStateDTO();
            BustAnalysisResult bustAnalysis = AnalyzeThrowsForBust(turn, throws);

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
                CreateThrowDTO throwDto = throws[i];
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
                GameDTO game = _gameService.GetGameByIdAsync(turn.Leg.Set.GameId).Result;
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
            turn.IsCheckoutAttempt = IsCheckoutPossible(turn.StartingScore);

            // Process throws up to and including the bust
            for (int i = 0; i < throws.Count; i++)
            {
                CreateThrowDTO throwDto = throws[i];
                // Create and add the throw directly to the turn entity
                Throw newThrow = new Throw
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
            GameDTO game = await _gameService.GetGameByIdAsync(turn.Leg.Set.GameId);
            ThrowDTO lastThrow = null;

            turn.IsCheckoutAttempt = IsCheckoutPossible(turn.StartingScore);

            foreach (CreateThrowDTO throwDto in throws)
            {
                int points = CalculatePoints(throwDto.Segment, throwDto.Multiplier);
                currentScore -= points;
     
                Throw newThrow = new Throw
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
            Leg leg = await _legService.GetLegByIdAsync(turn.LegId);
            if (leg != null)
            {
                leg.WinnerPlayerId = turn.PlayerId;
                await _legService.UpdateLegAsync(leg);

                // Update leg-level statistics after processing the turn
                List<GamePlayer> gamePlayers = await _legService.GetGamePlayersForLegAsync(turn.LegId);
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
            List<GamePlayer> gamePlayers = leg.Set.Game.GamePlayers.OrderBy(gp => gp.TurnOrder).ToList();//retrieve all players in the game ordered by their turn order
            if (lastTurn == null)
                return gamePlayers[0].PlayerId; // First turn, start with lowest TurnOrder

            int currentPlayerIndex = gamePlayers.FindIndex(gp => gp.PlayerId == lastTurn.PlayerId);
            int nextIndex = (currentPlayerIndex + 1) % gamePlayers.Count; // The modulo operation ensures that after the last player, the index wraps around to the first player, maintaining a circular order
            return gamePlayers[nextIndex].PlayerId;
        }

         private async Task CheckSetCompletion(Turn turn, GameStateDTO gameState)
         {
            Leg leg = await _legService.GetLegByIdAsync(turn.LegId);
            SetDTO set = await _setService.GetSetByIdAsync(leg.SetId);
            GameDTO game = await _gameService.GetGameByIdAsync(set.GameId);
            int legsWon = set.Legs.Count(l => l.WinnerPlayerId == turn.PlayerId);
            int totalLegs = set.Legs.Count;
           
            var players = set.Legs.Select(l => l.WinnerPlayerId).Distinct().Where(id => id.HasValue).Select(id => id.Value).ToList();
            Dictionary<int, int> legsWonPerPlayer = players.ToDictionary(p => p, p => set.Legs.Count(l => l.WinnerPlayerId == p));
            bool isTie = players.Count > 1 && players.All(p => legsWonPerPlayer[p] == legsWonPerPlayer[players[0]]);

            if (totalLegs < game.Settings.LegsPerSet && !isTie)
            {
                // Create the next leg in the current set
                await _gameService.CreateNextLegAsync(set.Id);
                return;
            }

            if (legsWon >= game.Settings.LegsPerSet)
            {
                // Update set-level statistics before ending the set
                await _statisticService.UpdateSetStatsAsync(set.Id, legsWonPerPlayer);
                await _setService.EndSetAsync(set.Id, turn.PlayerId);
                gameState.SetComplete = true;
            }
            else if (isTie && totalLegs == game.Settings.LegsPerSet)
            {
                await _gameService.CreateNextLegAsync(set.Id);
            }
        }

        private async Task CheckGameCompletion(Turn turn, GameStateDTO gameState)
        {
            GameDTO game = await _gameService.GetGameByIdAsync(turn.Leg.Set.GameId);
            int setsWon = game.Sets.Count(s => s.WinnerPlayerId == turn.PlayerId);
            int totalSets = game.Sets.Count;
      
            var players = game.Sets.Select(s => s.WinnerPlayerId).Distinct().Where(id => id.HasValue).Select(id => id.Value).ToList();
            Dictionary<int, int> setsPerPlayer = players.ToDictionary(p => p, p => game.Sets.Count(s => s.WinnerPlayerId == p));
            bool isTie = players.Count > 1 && players.All(p => setsPerPlayer[p] == setsPerPlayer[players[0]]);

            if (totalSets < game.Settings.SetsToWin && !isTie)
            {
                await _gameService.CreateNextSetAsync(game.Id);
                return;
            }

            if (setsWon >= game.Settings.SetsToWin)
            {
                // Update game-level statistics before ending the game
                await _statisticService.UpdateGameStatsAsync(game.Id, setsPerPlayer);
                await _gameService.EndGameAsync(game.Id, turn.PlayerId);
                gameState.GameComplete = true;
            }
            else if (isTie && totalSets == game.Settings.SetsToWin)
            {
                await _gameService.CreateNextSetAsync(game.Id);
            }   
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
            if (score <= 170 && score != 169 && score != 168 && score != 166 &&
         score != 165 && score != 163 && score != 162 && score != 159)
            {
                return true;
            }
            return false;
        }
    }
} 

