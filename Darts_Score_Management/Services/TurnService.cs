/*
using AutoMapper;
using Darts_Score_Management.Data.Models;
using Darts_Score_Management.DTOs.Turn;
using Darts_Score_Management.Interfaces.RepositoryInterfaces;
using Darts_Score_Management.Interfaces.ServiceInterfaces;

namespace Darts_Score_Management.Services
{
    public class TurnService : ITurnService
    {
        private readonly ITurnRepository _turnRepository;
        private readonly ILegRepository _legRepository;
        private readonly IMapper _mapper;

        public TurnService(ITurnRepository turnRepository, ILegRepository legRepository, IMapper mapper)
        {
            _turnRepository = turnRepository;
            _legRepository = legRepository;
            _mapper = mapper;
        }

        public async Task<TurnDTO> GetTurnByIdAsync(int id)
        {
            var turn = await _turnRepository.GetTurnWithThrowsAsync(id);
            return _mapper.Map<TurnDTO>(turn);
        }

        public async Task<TurnDTO> CreateTurnAsync(CreateTurnDTO createTurnDto)
        {
            var turn = _mapper.Map<Turn>(createTurnDto);

            // Calculate score after turn
            int turnScore = 0;
            foreach (var throw_ in turn.Throws)
            {
                if (!throw_.IsBounceOut)
                {
                    throw_.Score = CalculateThrowScore(throw_.Segment, throw_.Value);
                    turnScore += throw_.Score;
                }
            }

            turn.ScoreAfterTurn = turn.ScoreBeforeTurn - turnScore;

            // Check if this is a checkout
            if (turn.ScoreAfterTurn == 0)
            {
                turn.IsCheckout = true;
            }

            var createdTurn = await _turnRepository.AddTurnWithThrowsAsync(turn);
            return _mapper.Map<TurnDTO>(createdTurn);
        }

        public async Task<IEnumerable<TurnDTO>> GetTurnsByLegIdAsync(int legId)
        {
            var turns = await _legRepository.GetLegTurnsAsync(legId);
            return _mapper.Map<IEnumerable<TurnDTO>>(turns);
        }

        private int CalculateThrowScore(DartSegment segment, int value)
        {
            switch (segment)
            {
                case DartSegment.Single:
                    return value;
                case DartSegment.Double:
                    return value * 2;
                case DartSegment.Triple:
                    return value * 3;
                case DartSegment.InnerBullseye:
                    return 25;
                case DartSegment.OuterBullseye:
                    return 50;
                case DartSegment.Missed:
                default:
                    return 0;
            }
        }
    }
}
*/