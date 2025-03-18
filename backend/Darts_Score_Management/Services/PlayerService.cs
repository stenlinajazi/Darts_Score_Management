using AutoMapper;
using Darts_Score_Management.Data.Models;
using Darts_Score_Management.DTOs.Player;
using Darts_Score_Management.Interfaces.RepositoryInterfaces;
using Darts_Score_Management.Interfaces.ServiceInterfaces;

namespace Darts_Score_Management.Services
{
    public class PlayerService : IPlayerService
    {
        private readonly IPlayerRepository _playerRepository;
        private readonly IMapper _mapper;

        public PlayerService(IPlayerRepository playerRepository, IMapper mapper)
        {
            _playerRepository = playerRepository;
            _mapper = mapper;
        }

        public async Task<PlayerDTO> GetPlayerByIdAsync(int id)
        {
            Player player = await _playerRepository.GetByIdAsync(id);
            return _mapper.Map<PlayerDTO>(player);
        }

        public async Task<IEnumerable<PlayerDTO>> GetAllPlayersAsync()
        {
            return await _playerRepository.GetAllPlayerDTOsAsync();
        }

        public async Task<PlayerDTO> CreatePlayerAsync(UpsertPlayerDTO createPlayerDto)
        {
            Player player = _mapper.Map<Player>(createPlayerDto);
            player.IsActive = true;
         
            Player createdPlayer = await _playerRepository.AddAsync(player);
            return _mapper.Map<PlayerDTO>(createdPlayer);
        }

        public async Task<PlayerDTO> UpdatePlayerAsync(int id, UpsertPlayerDTO upsertplayerDto)
        {
            Player player = await _playerRepository.GetByIdAsync(id);
            if (player == null)
            {
                throw new KeyNotFoundException($"Player with id {id} not found");
            }

            _mapper.Map(upsertplayerDto, player);
            await _playerRepository.UpdateAsync(player);
            return _mapper.Map<PlayerDTO>(player);
        }

        public async Task DeletePlayerAsync(int id)
        {
            await _playerRepository.DeleteAsync(id);
        }

        public async Task<IEnumerable<PlayerDTO>> GetActivePlayersAsync()
        {
            IEnumerable<Player> players = await _playerRepository.GetActivePlayersAsync();
            return _mapper.Map<IEnumerable<PlayerDTO>>(players);
        }

        //public async Task<PlayerStatsDTO> GetPlayerWithStatsAsync(int id)
        //{
        //    var player = await _playerRepository.GetPlayerWithStatsAsync(id);
        //    if (player == null)
        //        throw new KeyNotFoundException($"Player with id {id} not found");

        //    return _mapper.Map<PlayerStatsDTO>(player);
        //}
    }
}
