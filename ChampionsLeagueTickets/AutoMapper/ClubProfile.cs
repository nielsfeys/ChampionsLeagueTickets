using AutoMapper;
using ChampionsLeagueTickets.Domain.Entities;
using ChampionsLeagueTickets.ViewModels;

namespace ChampionsLeagueTickets.AutoMapper;
public class ClubProfile : Profile {
    public ClubProfile() {
        CreateMap<Club, ClubVM>();
    }
}

