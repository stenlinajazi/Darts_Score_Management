
using AutoMapper;
using Darts_Score_Management.Data.Models;
using Darts_Score_Management.DTOs.Player;
using Darts_Score_Management.DTOs.Set;
using Darts_Score_Management.Interfaces.RepositoryInterfaces;
using Darts_Score_Management.Interfaces.ServiceInterfaces;
using Darts_Score_Management.Repositories;

namespace Darts_Score_Management.Services
{
    public class SetService : ISetService
    {
        private readonly ISetRepository _setRepository;
        private readonly IPlayerRepository _playerRepository;
        private readonly IMapper _mapper;

        public SetService(ISetRepository setRepository, IPlayerRepository playerRepository, IMapper mapper)
        {
            _setRepository = setRepository;
            _playerRepository = playerRepository;
            _mapper = mapper;
        }

        public async Task<SetDTO> GetSetByIdAsync(int id)
        {
            var set = await _setRepository.GetSetWithLegsAsync(id);
            if (set == null)
                return null;

            var setDto = _mapper.Map<SetDTO>(set);

            if (set.WinnerPlayerId.HasValue)
            {
                var winner = await _playerRepository.GetByIdAsync(set.WinnerPlayerId.Value);
                setDto.Winner = _mapper.Map<PlayerDTO>(winner);
            }

            return setDto;
        }

        public async Task<SetDTO> CreateSetAsync(CreateSetDTO createSetDto)
        {
            var set = _mapper.Map<Set>(createSetDto);
            var createdSet = await _setRepository.AddAsync(set);
            return _mapper.Map<SetDTO>(createdSet);
        }

        public async Task<SetDTO> EndSetAsync(int id, int winnerId)
        {
            var set = await _setRepository.GetByIdAsync(id);
            if (set == null)
                throw new KeyNotFoundException($"Set with id {id} not found");

            set.WinnerPlayerId = winnerId;
            await _setRepository.UpdateAsync(set);
            return _mapper.Map<SetDTO>(set);
        }

        public async Task<IEnumerable<SetDTO>> GetSetsByGameIdAsync(int gameId)
        {
            var sets = await _setRepository.GetSetsForGameAsync(gameId);
            var result = new List<SetDTO>();

            foreach (var set in sets)
            {
                var setDto = _mapper.Map<SetDTO>(set);

                if (set.WinnerPlayerId.HasValue)
                {
                    var winner = await _playerRepository.GetByIdAsync(set.WinnerPlayerId.Value);
                    setDto.Winner = _mapper.Map<PlayerDTO>(winner);
                }

                result.Add(setDto);
            }

            return result;
        }
    }
}

