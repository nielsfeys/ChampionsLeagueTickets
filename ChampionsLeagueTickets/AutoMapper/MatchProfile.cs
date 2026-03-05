using AutoMapper;
using ChampionsLeagueTickets.Domain.Entities;
using ChampionsLeagueTickets.ViewModels;

namespace ChampionsLeagueTickets.AutoMapper;
public class MatchProfile : Profile {

    public MatchProfile() {
        CreateMap<Match, MatchVM>()
            .ForMember(dest => dest.HomeTeam, opt => opt.MapFrom(src => src.HometeamNavigation.Name))
            .ForMember(dest => dest.AwayTeam, opt => opt.MapFrom(src => src.AwayteamNavigation.Name));
    }

}

