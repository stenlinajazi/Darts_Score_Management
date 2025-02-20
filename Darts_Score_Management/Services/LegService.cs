
using AutoMapper;
using Darts_Score_Management.Data.Models;
using Darts_Score_Management.DTOs.Leg;
using Darts_Score_Management.Interfaces.RepositoryInterfaces;
using Darts_Score_Management.Interfaces.ServiceInterfaces;

namespace Darts_Score_Management.Services
{
    public class LegService : ILegService
    {
        private readonly ILegRepository _legRepository;
        private readonly IMapper _mapper;

        public LegService(ILegRepository legRepository, IMapper mapper)
        {
            _legRepository = legRepository;
            _mapper = mapper;
        }

        public async Task<LegDTO> GetLegByIdAsync(int id)
        {
            var leg = await _legRepository.GetLegWithDetailsAsync(id);
            return _mapper.Map<LegDTO>(leg);
        }

        public async Task<LegDTO> CreateLegAsync(CreateLegDTO createLegDto)
        {
            var leg = _mapper.Map<Leg>(createLegDto);
            var createdLeg = await _legRepository.AddAsync(leg);
            return _mapper.Map<LegDTO>(createdLeg);
        }

        public async Task<LegDTO> EndLegAsync(int id, int winnerId)
        {
            var leg = await _legRepository.GetByIdAsync(id);
            if (leg == null)
                throw new KeyNotFoundException($"Leg with id {id} not found");

            leg.WinnerPlayerId = winnerId;
            await _legRepository.UpdateAsync(leg);
            return _mapper.Map<LegDTO>(leg);
        }

        public async Task<IEnumerable<LegDTO>> GetLegsBySetIdAsync(int setId)
        {
            var legs = await _legRepository.GetAllAsync();
            var setLegs = legs.Where(l => l.SetId == setId);
            return _mapper.Map<IEnumerable<LegDTO>>(setLegs);
        }
    }
}
