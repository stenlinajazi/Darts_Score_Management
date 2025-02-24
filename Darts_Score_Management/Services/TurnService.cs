
using AutoMapper;
using Darts_Score_Management.Data.Models;
using Darts_Score_Management.DTOs.Throw;
using Darts_Score_Management.DTOs.Turn;
using Darts_Score_Management.Interfaces.RepositoryInterfaces;
using Darts_Score_Management.Interfaces.ServiceInterfaces;
using Microsoft.EntityFrameworkCore;

namespace Darts_Score_Management.Services
{
    public class TurnService : ITurnService
    {
        private readonly ITurnRepository _turnRepository;
        private readonly IMapper _mapper;

        public TurnService(ITurnRepository turnRepository, IMapper mapper)
        {
            _turnRepository = turnRepository;
            _mapper = mapper;
        }

        public async Task<TurnDTO> GetTurnByIdAsync(int turnId)
        {
            var turn = await _turnRepository.GetByIdAsync(turnId);
            if (turn == null)
                throw new KeyNotFoundException($"Turn with id {turnId} not found");

            return _mapper.Map<TurnDTO>(turn);
        }

        public async Task<TurnDTO> CreateTurnAsync(CreateTurnDTO createTurnDto)
        {
            var turn = new Turn
            {
                LegId = createTurnDto.LegId,
                PlayerId = createTurnDto.PlayerId,
                TurnNumber = createTurnDto.TurnNumber,
                StartingScore = createTurnDto.StartingScore,
                EndingScore = createTurnDto.StartingScore // Will be updated as throws are added
            };

            var createdTurn = await _turnRepository.AddAsync(turn);
            return _mapper.Map<TurnDTO>(createdTurn);
        }

        public async Task<TurnDTO> AddThrowToTurnAsync(int turnId, CreateThrowDTO throwDto)
        {
            var turn = await _turnRepository.GetTurnWithThrowsAsync(turnId);
            if (turn == null)
                throw new KeyNotFoundException($"Turn with id {turnId} not found");

            var newThrow = new Throw
            {
                TurnId = turnId,
                ThrowNumber = turn.Throws.Count + 1,
                Segment = throwDto.Segment,
                Multiplier = throwDto.Multiplier
            };

            // Calculate new score
            var points = newThrow.Points;
            var newScore = turn.EndingScore - points;

            // Check for bust
            if (newScore < 0)
            {
                newThrow.IsBusted = true;
                // Reset ending score to starting score if busted
                turn.EndingScore = turn.StartingScore;
            }
            else
            {
                turn.EndingScore = newScore;
                turn.TotalPoints += points;
            }

            turn.Throws.Add(newThrow);
            await _turnRepository.UpdateAsync(turn);
            return _mapper.Map<TurnDTO>(turn);
        }

        public async Task<TurnDTO> GetLastTurnByLegAsync(int legId)
        {
            var turn = await _turnRepository.GetLastTurnByLegAsync(legId);
            return  _mapper.Map<TurnDTO>(turn);
        }

        public async Task<TurnDTO> GetLastTurnByPlayerAndLegAsync(int playerId, int legId)
        {
            var turn = await _turnRepository.GetLastTurnByPlayerAndLegAsync(playerId, legId);
            return _mapper.Map<TurnDTO>(turn);
        }
    }
}
