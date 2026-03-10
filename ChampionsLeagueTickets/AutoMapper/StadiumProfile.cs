using AutoMapper;
using ChampionsLeagueTickets.Domain.Entities;
using ChampionsLeagueTickets.ViewModels;

namespace ChampionsLeagueTickets.AutoMapper;
public class StadiumProfile : Profile {

    public StadiumProfile() {
        CreateMap<StadiumSection, SectionVM>()
            .ForMember(dest => dest.HomeTeam,
                        opt => opt.MapFrom(src => src.HomeTeamNavigation.Name));
    }
}

