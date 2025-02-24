using AutoMapper;
using Darts_Score_Management.Data.Models;
using Darts_Score_Management.DTOs.Game;
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
            //// Player mappings
            //CreateMap<Player, PlayerDTO>().ReverseMap();
            //CreateMap<UpsertPlayerDTO, Player>();

            //// Game mappings
            //CreateMap<Game, GameDTO>()
            //    .ForMember(dest => dest.Settings, opt => opt.MapFrom(src => src.Settings))
            //    .ForMember(dest => dest.Players, opt => opt.MapFrom(src => src.GamePlayers))  
            //    .ForMember(dest => dest.Sets, opt => opt.MapFrom(src => src.Sets));
            //CreateMap<GameDTO, Game>();
            //CreateMap<CreateGameDTO, Game>()
            //    .ForMember(dest => dest.Settings, opt => opt.MapFrom(src => src.Settings));

            //// GameSettings mappings
            //CreateMap<GameSettings, GameSettingsDTO>().ReverseMap();

            //// GamePlayer mappings
            //CreateMap<GamePlayer, GamePlayerDTO>()
            //    .ForMember(dest => dest.Player, opt => opt.MapFrom(src => src.Player))
            //    .ForMember(dest => dest.Statistics, opt => opt.MapFrom(src => src.Statistics));
            //CreateMap<GamePlayerDTO, GamePlayer>();

            //// Statistic mappings
            //CreateMap<Statistic, StatisticDTO>().ReverseMap();

            //// Set mappings (assuming you have a SetDTO)
            //CreateMap<Set, SetDTO>().ReverseMap();
            //CreateMap<CreateSetDTO, Set>();

            //// Add other mappings as needed for Leg, Turn, Throw models
            //CreateMap<Leg, LegDTO>().ReverseMap();
            //CreateMap<CreateLegDTO, Leg>();

            ////CreateMap<Turn, TurnDTO>().ReverseMap();
            ////CreateMap<CreateTurnDTO, Turn>();

            //CreateMap<Turn, TurnDTO>()
            //.ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
            //.ForMember(dest => dest.LegId, opt => opt.MapFrom(src => src.LegId))
            //.ForMember(dest => dest.Leg, opt => opt.MapFrom(src => src.Leg))
            //.ForMember(dest => dest.PlayerId, opt => opt.MapFrom(src => src.PlayerId))
            //.ForMember(dest => dest.TurnNumber, opt => opt.MapFrom(src => src.TurnNumber))
            //.ForMember(dest => dest.StartingScore, opt => opt.MapFrom(src => src.StartingScore))
            //.ForMember(dest => dest.EndingScore, opt => opt.MapFrom(src => src.EndingScore))
            //.ForMember(dest => dest.TotalPoints, opt => opt.MapFrom(src => src.TotalPoints))
            //.ForMember(dest => dest.Throws, opt => opt.MapFrom(src => src.Throws));

            //// TurnDTO -> Turn mapping
            //CreateMap<TurnDTO, Turn>()
            //    .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
            //    .ForMember(dest => dest.LegId, opt => opt.MapFrom(src => src.LegId))
            //    .ForMember(dest => dest.PlayerId, opt => opt.MapFrom(src => src.PlayerId))
            //    .ForMember(dest => dest.TurnNumber, opt => opt.MapFrom(src => src.TurnNumber))
            //    .ForMember(dest => dest.StartingScore, opt => opt.MapFrom(src => src.StartingScore))
            //    .ForMember(dest => dest.EndingScore, opt => opt.MapFrom(src => src.EndingScore))
            //    .ForMember(dest => dest.TotalPoints, opt => opt.MapFrom(src => src.TotalPoints))
            //    .ForMember(dest => dest.Throws, opt => opt.MapFrom(src => src.Throws))
            //    // Handle navigation properties
            //    .ForMember(dest => dest.Leg, opt => opt.MapFrom(src => src.Leg))
            //    .ForMember(dest => dest.Player, opt => opt.Ignore()); // Player is not present in DTO

            //CreateMap<Throw, ThrowDTO>().ReverseMap();
            //CreateMap<CreateThrowDTO, Throw>();

            // Player mappings
            CreateMap<Player, PlayerDTO>().ReverseMap();
            CreateMap<UpsertPlayerDTO, Player>();
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
            CreateMap<GamePlayer, GameStatisticsDTO>();

            // Statistic mappings
            CreateMap<Statistic, StatisticDTO>().ReverseMap();
            CreateMap<Statistic, StatisticDTO>();

            // Set mappings
            CreateMap<Set, SetDTO>().ReverseMap();
            CreateMap<CreateSetDTO, Set>();

            // Leg mappings
            CreateMap<Leg, LegDTO>().ReverseMap();
            CreateMap<CreateLegDTO, Leg>();

            // Turn mappings
            CreateMap<Turn, TurnDTO>();
            CreateMap<TurnDTO, Turn>()
                .ForMember(dest => dest.Player, opt => opt.Ignore()); // Player is not in DTO

            // Throw mappings
            CreateMap<Throw, ThrowDTO>().ReverseMap();
            CreateMap<CreateThrowDTO, Throw>();
        }
    }
}
