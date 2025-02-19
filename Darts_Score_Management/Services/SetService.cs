/*
using AutoMapper;
using Darts_Score_Management.Data.Models;
using Darts_Score_Management.DTOs.Set;
using Darts_Score_Management.Interfaces.RepositoryInterfaces;
using Darts_Score_Management.Interfaces.ServiceInterfaces;

namespace Darts_Score_Management.Services
{
    public class SetService : ISetService
    {
        private readonly ISetRepository _setRepository;
        private readonly IMapper _mapper;

        public SetService(ISetRepository setRepository, IMapper mapper)
        {
            _setRepository = setRepository;
            _mapper = mapper;
        }

        public async Task<SetDTO> GetSetByIdAsync(int id)
        {
            var set = await _setRepository.GetSetWithLegsAsync(id);
            return _mapper.Map<SetDTO>(set);
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

            set.WinnerId = winnerId;
            await _setRepository.UpdateAsync(set);
            return _mapper.Map<SetDTO>(set);
        }

        public async Task<IEnumerable<SetDTO>> GetSetsByGameIdAsync(int gameId)
        {
            var sets = await _setRepository.GetSetsForGameAsync(gameId);
            return _mapper.Map<IEnumerable<SetDTO>>(sets);
        }
    }
}
*/
