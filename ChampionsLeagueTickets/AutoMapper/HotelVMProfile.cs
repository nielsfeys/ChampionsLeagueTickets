using AutoMapper;
using ChampionsLeagueTickets.Services.DTOs;
using ChampionsLeagueTickets.ViewModels;

namespace ChampionsLeagueTickets.AutoMapper;
public class HotelVMProfile : Profile {
    public HotelVMProfile() {
        CreateMap<HotelOfferDTO, HotelOfferVM>();
    }
}