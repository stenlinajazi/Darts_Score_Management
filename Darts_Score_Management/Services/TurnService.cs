
using AutoMapper;
using Darts_Score_Management.Data;
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
        private readonly IThrowRepository _throwRepository;

        public TurnService(ITurnRepository turnRepository, IMapper mapper, IThrowRepository throwRepository)
        {
            _turnRepository = turnRepository;
            _mapper = mapper;
            _throwRepository = throwRepository;
        }

        public async Task<TurnDTO> GetTurnByIdAsync(int turnId)
        {
            Turn turn = await _turnRepository.GetByIdAsync(turnId);
            if (turn == null)
                throw new KeyNotFoundException($"Turn with id {turnId} not found");

            return _mapper.Map<TurnDTO>(turn);
        }

        public async Task<TurnDTO> CreateTurnAsync(CreateTurnDTO createTurnDto)
        {
            Turn turn = new Turn
            {
                LegId = createTurnDto.LegId,
                PlayerId = createTurnDto.PlayerId,
                TurnNumber = createTurnDto.TurnNumber,
                StartingScore = createTurnDto.StartingScore,
                EndingScore = createTurnDto.StartingScore 
            };

            Turn createdTurn = await _turnRepository.AddAsync(turn);
            return _mapper.Map<TurnDTO>(createdTurn);
        }

        public async Task<TurnDTO> AddThrowToTurnAsync(int turnId, CreateThrowDTO throwDto)
        {
            Turn turn = await _turnRepository.GetTurnWithThrowsAsync(turnId);
            if (turn == null)
                throw new KeyNotFoundException($"Turn with id {turnId} not found");

            Throw newThrow = new Throw
            {
                TurnId = turnId,
                ThrowNumber = turn.Throws.Count + 1,
                Segment = throwDto.Segment,
                Multiplier = throwDto.Multiplier,
                IsBusted = false 
            };

        
            turn.Throws.Add(newThrow);
            await _turnRepository.UpdateAsync(turn);
            return _mapper.Map<TurnDTO>(turn);
        }

        public async Task<TurnDTO> GetLastTurnByLegAsync(int legId)
        {
            Turn turn = await _turnRepository.GetLastTurnByLegAsync(legId);
            return  _mapper.Map<TurnDTO>(turn);
        }

        public async Task<TurnDTO> GetLastTurnByPlayerAndLegAsync(int playerId, int legId)
        {
            Turn turn = await _turnRepository.GetLastTurnByPlayerAndLegAsync(playerId, legId);
            return _mapper.Map<TurnDTO>(turn);
        }

        public async Task<IEnumerable<ThrowDTO>> GetThrowsForTurnAsync(int turnId)
        {
            IEnumerable<Throw> throws = await _throwRepository.GetThrowsForTurnAsync(turnId);
            return _mapper.Map<IEnumerable<ThrowDTO>>(throws);
        }

        public async Task<ThrowDTO> GetLastThrowForTurnAsync(int turnId)
        {
            IEnumerable<ThrowDTO> throws = await GetThrowsForTurnAsync(turnId);
            return throws?.OrderByDescending(t => t.ThrowNumber).FirstOrDefault();
        }

        public async Task<IEnumerable<Turn>> GetPlayerTurnsInLegAsync(int playerId, int legId)
        {
            return await _turnRepository.GetTurnsByPlayerAndLegAsync(playerId, legId);
        }
    }
}
