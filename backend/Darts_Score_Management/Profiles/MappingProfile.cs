using AutoMapper;
using Darts_Score_Management.Data.Models;
using Darts_Score_Management.DTOs.Game.Core;
using Darts_Score_Management.DTOs.Game.Statistics;
using Darts_Score_Management.DTOs.GamePlayer;
using Darts_Score_Management.DTOs.Leg;
using Darts_Score_Management.DTOs.Player;
using Darts_Score_Management.DTOs.Set;
using Darts_Score_Management.DTOs.Statistic;
using Darts_Score_Management.DTOs.Throw;
using Darts_Score_Management.DTOs.Turn;

namespace Darts_Score_Management.Profiles
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // Player mappings
            CreateMap<Player, PlayerDTO>().ReverseMap();
            CreateMap<UpsertPlayerDTO, Player>().ReverseMap();
            CreateMap<Player, PlayerStatsDTO>();

            // Game mappings
            CreateMap<Game, GameDTO>()
                .ForMember(dest => dest.Players, opt => opt.MapFrom(src => src.GamePlayers));
            CreateMap<GameDTO, Game>();
            CreateMap<CreateGameDTO, Game>();
            

            // GameSettings mappings
            CreateMap<GameSettings, GameSettingsDTO>().ReverseMap();

            // GamePlayer mappings
            CreateMap<GamePlayer, GamePlayerDTO>();
            CreateMap<GamePlayerDTO, GamePlayer>();
            

            // Set mappings
            CreateMap<Set, SetDTO>().ReverseMap();
            CreateMap<CreateSetDTO, Set>()
                 .ForMember(dest => dest.Id, opt => opt.Ignore())
                 .ForMember(dest => dest.Legs, opt => opt.Ignore())
                 .ForMember(dest => dest.WinnerPlayerId, opt => opt.Ignore());

            // Leg mappings
            CreateMap<Leg, LegDTO>().ReverseMap();
            CreateMap<CreateLegDTO, Leg>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.WinnerPlayerId, opt => opt.Ignore())
                .ForMember(dest => dest.Turns, opt => opt.Ignore());

            // Turn mappings
            CreateMap<Turn, TurnDTO>();
            CreateMap<TurnDTO, Turn>()
                .ForMember(dest => dest.Player, opt => opt.Ignore()); 

            // Throw mappings
            CreateMap<Throw, ThrowDTO>().ReverseMap();
            CreateMap<CreateThrowDTO, Throw>();

            // LegStats mappings
            CreateMap<LegStats, LegStatsDTO>().ReverseMap();

            // SetStats mappings
            CreateMap<SetStats, SetStatsDTO>().ReverseMap();

            // GameStats mappings
            CreateMap<GameStats, GameStatsDTO>().ReverseMap();

            // PlayerStats mappings (derived on-demand)
            CreateMap<Player, PlayerStatsDTO>()
                .ForMember(dest => dest.PlayerName, opt => opt.MapFrom(src => src.Name));
     

            CreateMap<PlayerStatsDTO, PlayerStatsDTO>().ReverseMap();
        }
    } 
}
