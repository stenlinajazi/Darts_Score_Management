
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
        private readonly IThrowRepository _throwRepository;

        public TurnService(ITurnRepository turnRepository, IMapper mapper, IThrowRepository throwRepository)
        {
            _turnRepository = turnRepository;
            _mapper = mapper;
            _throwRepository = throwRepository;
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
                Multiplier = throwDto.Multiplier,
                IsBusted = false // Default to false, will be set to true if this throw busts
            };

            //// Calculate points for this throw
            //int points = newThrow.Points;//Why do we need to calculate again the points?

            //// Check if any previous throw in this turn has already caused a bust
            //bool previousBust = turn.Throws.Any(t => t.IsBusted);//Why do we check again for bust condition when the throws that we passed this method we chacked before and they werent busts

            //if (previousBust)
            //{
            //    // If a previous throw already busted, don't change the score but add the throw
            //    turn.Throws.Add(newThrow);
            //    await _turnRepository.UpdateAsync(turn);
            //    return _mapper.Map<TurnDTO>(turn);
            //}

            //// Calculate new score after this throw
            //int newScore = turn.EndingScore - points;

            //// Check for bust on this specific throw
            //if (newScore < 0)//Why do we still continoue to check for bust
            //{
            //    // Mark only this throw as busted
            //    newThrow.IsBusted = true;//Wasnt this set using the for loop in ProcesThrowss method and getting the index of the throw which did it

            //    // Reset turn score to starting score
            //    turn.EndingScore = turn.StartingScore;
            //    turn.TotalPoints = 0;
            //}
            //else
            //{
            //    // Update score if no bust occurs
            //    turn.EndingScore = newScore;
            //    turn.TotalPoints += points;
            //}

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

        public async Task<IEnumerable<ThrowDTO>> GetThrowsForTurnAsync(int turnId)
        {
            var throws = await _throwRepository.GetThrowsForTurnAsync(turnId);
            return _mapper.Map<IEnumerable<ThrowDTO>>(throws);
        }

        public async Task<ThrowDTO> GetLastThrowForTurnAsync(int turnId)
        {
            var throws = await GetThrowsForTurnAsync(turnId);
            return throws?.OrderByDescending(t => t.ThrowNumber).FirstOrDefault();
        }
    }
}
