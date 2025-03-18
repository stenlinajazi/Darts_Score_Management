
using AutoMapper;
using Darts_Score_Management.Data.Models;
using Darts_Score_Management.DTOs.Leg;
using Darts_Score_Management.Interfaces.RepositoryInterfaces;
using Darts_Score_Management.Interfaces.ServiceInterfaces;
using Microsoft.EntityFrameworkCore;

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

        public async Task<Leg> GetLegByIdAsync(int id)
        {
            Leg leg = await _legRepository.GetLegWithDetailsAsync(id);
            return leg;
        }

        public async Task<LegDTO> CreateLegAsync(CreateLegDTO createLegDto)
        {
            Leg leg = _mapper.Map<Leg>(createLegDto);
            Leg createdLeg = await _legRepository.AddAsync(leg);
            return _mapper.Map<LegDTO>(createdLeg);
        }

        public async Task<LegDTO> EndLegAsync(int id, int winnerId)
        {
            Leg leg = await _legRepository.GetByIdAsync(id);
            if (leg == null)
                throw new KeyNotFoundException($"Leg with id {id} not found");

            leg.WinnerPlayerId = winnerId;
            await _legRepository.UpdateAsync(leg);
            return _mapper.Map<LegDTO>(leg);
        }

        public async Task<IEnumerable<LegDTO>> GetLegsBySetIdAsync(int setId)
        {
            IEnumerable<Leg> legs = await _legRepository.GetAllAsync();
            IEnumerable<Leg> setLegs = legs.Where(l => l.SetId == setId);
            return _mapper.Map<IEnumerable<LegDTO>>(setLegs);
        }

        public async Task<LegDTO> UpdateLegAsync(Leg leg)
        {
            await _legRepository.UpdateAsync(leg);
            Leg updatedLeg = await _legRepository.GetByIdAsync(leg.Id); 
            return _mapper.Map<LegDTO>(updatedLeg);
        }

        public async Task<List<GamePlayer>> GetGamePlayersForLegAsync(int legId)
        {
            List<GamePlayer> gamePlayers = await _legRepository.GetGamePlayersForLegAsync(legId);
            if (gamePlayers == null || !gamePlayers.Any())
                throw new KeyNotFoundException($"No game players found for Leg with ID {legId}.");
            Leg leg = await _legRepository.GetLegWithDetailsAsync(legId);
            if (leg == null)
                throw new KeyNotFoundException($"Leg with ID {legId} not found.");
            if (leg.Set == null || leg.Set.Game == null)
                throw new KeyNotFoundException($"Leg with ID {legId} has invalid relationships (Set or Game not found).");
            return gamePlayers;
        }
    }
}
