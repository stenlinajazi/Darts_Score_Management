using AutoMapper;
using Darts_Score_Management.Data.Models;
using Darts_Score_Management.DTOs.Player;
using Darts_Score_Management.Interfaces.RepositoryInterfaces;
using Darts_Score_Management.Interfaces.ServiceInterfaces;
using Darts_Score_Management.Repositories;
using System.ComponentModel.DataAnnotations;
using System.Numerics;

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
            if (id <= 0)
                throw new ArgumentException("Player ID must be a positive number.", nameof(id));
            Player player = await _playerRepository.GetByIdAsync(id);
            if (player == null)
                throw new KeyNotFoundException($"Player with Id {id} was not found.");
            return _mapper.Map<PlayerDTO>(player);
        }

        public async Task<IEnumerable<PlayerDTO>> GetAllPlayersAsync()
        {
            var players = await _playerRepository.GetAllPlayerDTOsAsync();
            if (players == null || !players.Any())
                throw new KeyNotFoundException("No players found.");
            return players;
        }

        public async Task<PlayerDTO> CreatePlayerAsync(UpsertPlayerDTO createPlayerDto)
        {
            if (createPlayerDto == null)
                throw new ArgumentNullException(nameof(createPlayerDto));

            var existingPlayer = await _playerRepository.GetAllAsync();
            if (existingPlayer.Any(p => p.Username == createPlayerDto.Username && !p.IsDeleted))
                throw new ValidationException($"A player with the username '{createPlayerDto.Username}' already exists.");

            var player = _mapper.Map<Player>(createPlayerDto);
            player.IsActive = true;

            var createdPlayer = await _playerRepository.AddAsync(player);
            return _mapper.Map<PlayerDTO>(createdPlayer);
        }


        public async Task<PlayerDTO> UpdatePlayerAsync(int id, UpsertPlayerDTO upsertplayerDto)
        {
            if (id <= 0)
                throw new ArgumentException("Player ID must be a positive number.", nameof(id));

            if (upsertplayerDto == null)
                throw new ArgumentNullException(nameof(upsertplayerDto));

            var player = await _playerRepository.GetByIdAsync(id);
            if (player == null)
                throw new KeyNotFoundException($"Player with Id {id} was not found.");

            var existingPlayers = await _playerRepository.GetAllAsync();
            if (existingPlayers.Any(p => p.Username == upsertplayerDto.Username && p.Id != id && !p.IsDeleted))
                throw new ValidationException($"A player with the username '{upsertplayerDto.Username}' already exists.");

            _mapper.Map(upsertplayerDto, player);
            await _playerRepository.UpdateAsync(player);
            return _mapper.Map<PlayerDTO>(player);
        }

        public async Task DeletePlayerAsync(int id)
        {
            if (id <= 0)
                throw new ArgumentException("Player ID must be a positive number.", nameof(id));

            var player = await _playerRepository.GetByIdAsync(id);
            if (player == null)
                throw new KeyNotFoundException($"Player with Id {id} was not found.");

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
