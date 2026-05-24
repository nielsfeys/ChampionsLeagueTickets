using AutoMapper;
using ChampionsLeagueTickets.Domain.Entities;
using ChampionsLeagueTickets.ViewModels;

namespace ChampionsLeagueTickets.AutoMapper;
public class TicketVMProfile : Profile {
    public TicketVMProfile() {
        CreateMap<SeasonTicketVM, Ticket>()
            .ForMember(dest => dest.Code, opt => opt.MapFrom(src => Guid.NewGuid().ToString()))
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => "Available"))
            .ForMember(dest => dest.Type, opt => opt.MapFrom(src => "Season"))
            .ForMember(dest => dest.UserId, opt => opt.MapFrom((_, _, _, context) => context.Items["UserId"]))
            .ForMember(dest => dest.Seat, opt => opt.MapFrom((_, _, _, context) => 
            {
                var totalSeats = (int)context.Items["TotalSeats"];
                var seasonTicketsSold = (int)context.Items["SeasonTicketsSold"];
                return totalSeats - seasonTicketsSold;
            }));

        CreateMap<DayTicketVM, Ticket>()
            .ForMember(dest => dest.Code, opt => opt.MapFrom(src => Guid.NewGuid().ToString()))
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => "Available"))
            .ForMember(dest => dest.Type, opt => opt.MapFrom(src => "Day"))
            .ForMember(dest => dest.UserId, opt => opt.MapFrom((_, _, _, context) => context.Items["UserId"]))
            .ForMember(dest => dest.Seat, opt => opt.MapFrom((_, _, _, context) => 
            {
                var dayTicketsSold = (int)context.Items["DayTicketsSold"];
                return dayTicketsSold + 1;
            }));
    }
}
