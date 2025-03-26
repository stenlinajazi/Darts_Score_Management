
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
            if (id <= 0)
                throw new ArgumentException("Set ID must be a positive number.", nameof(id));
            Set set = await _setRepository.GetSetWithLegsAsync(id);
            if (set == null)
                throw new KeyNotFoundException($"Set with ID {id} not found.");

            SetDTO setDto = _mapper.Map<SetDTO>(set);

            if (set.WinnerPlayerId.HasValue)
            {
                Player winner = await _playerRepository.GetByIdAsync(set.WinnerPlayerId.Value);
                setDto.Winner = _mapper.Map<PlayerDTO>(winner);
            }

            return setDto;
        }

        public async Task<SetDTO> CreateSetAsync(CreateSetDTO createSetDto)
        {
            if (createSetDto == null)
                throw new ArgumentNullException(nameof(createSetDto));
            Set set = _mapper.Map<Set>(createSetDto);
            Set createdSet = await _setRepository.AddAsync(set);
            return _mapper.Map<SetDTO>(createdSet);
        }

        public async Task<SetDTO> EndSetAsync(int id, int winnerId)
        {
            if (id <= 0)
                throw new ArgumentException("Set ID must be a positive number.", nameof(id));
            if (winnerId <= 0)
                throw new ArgumentException("Winner ID must be a positive number.", nameof(winnerId));
            Set set = await _setRepository.GetByIdAsync(id);
            if (set == null)
                throw new KeyNotFoundException($"Set with id {id} not found");

            set.WinnerPlayerId = winnerId;
            await _setRepository.UpdateAsync(set);
            return _mapper.Map<SetDTO>(set);
        }

        public async Task<IEnumerable<SetDTO>> GetSetsByGameIdAsync(int gameId)
        {
            if (gameId <= 0)
                throw new ArgumentException("Game ID must be a positive number.", nameof(gameId));
            IEnumerable<Set> sets = await _setRepository.GetSetsForGameAsync(gameId);
            List<SetDTO> result = new List<SetDTO>();

            foreach (var set in sets)
            {
                SetDTO setDto = _mapper.Map<SetDTO>(set);

                if (set.WinnerPlayerId.HasValue)
                {
                    Player winner = await _playerRepository.GetByIdAsync(set.WinnerPlayerId.Value);
                    setDto.Winner = _mapper.Map<PlayerDTO>(winner);
                }

                result.Add(setDto);
            }

            return result;
        }
    }
}

