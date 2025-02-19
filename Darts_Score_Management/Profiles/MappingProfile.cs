using AutoMapper;
using Darts_Score_Management.Data.Models;
using Darts_Score_Management.DTOs.Game;
using Darts_Score_Management.DTOs.GamePlayer;
using Darts_Score_Management.DTOs.Player;
using Darts_Score_Management.DTOs.Set;
using Darts_Score_Management.DTOs.Statistic;

namespace Darts_Score_Management.Profiles
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // Player mappings
            CreateMap<Player, PlayerDTO>().ReverseMap();
            CreateMap<UpsertPlayerDTO, Player>();

            // Game mappings
            CreateMap<Game, GameDTO>()
                .ForMember(dest => dest.Settings, opt => opt.MapFrom(src => src.Settings))
                .ForMember(dest => dest.Players, opt => opt.MapFrom(src => src.GamePlayers))  
                .ForMember(dest => dest.Sets, opt => opt.MapFrom(src => src.Sets));
            CreateMap<GameDTO, Game>();
            CreateMap<CreateGameDTO, Game>()
                .ForMember(dest => dest.Settings, opt => opt.MapFrom(src => src.Settings));

            // GameSettings mappings
            CreateMap<GameSettings, GameSettingsDTO>().ReverseMap();

            // GamePlayer mappings
            CreateMap<GamePlayer, GamePlayerDTO>()
                .ForMember(dest => dest.Player, opt => opt.MapFrom(src => src.Player))
                .ForMember(dest => dest.Statistics, opt => opt.MapFrom(src => src.Statistics));
            CreateMap<GamePlayerDTO, GamePlayer>();

            // Statistic mappings
            CreateMap<Statistic, StatisticDTO>().ReverseMap();

            // Set mappings (assuming you have a SetDTO)
            CreateMap<Set, SetDTO>().ReverseMap();

            // Add other mappings as needed for Leg, Turn, Throw models
        }
    }
}
