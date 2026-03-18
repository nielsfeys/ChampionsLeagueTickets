using ChampionsLeagueTickets.Services.DTOs;

namespace ChampionsLeagueTickets.Services.Interfaces;
public interface IHotelService {
    Task<List<HotelOfferDTO>> SearchHotelsAsync(string destination, DateOnly checkIn, DateOnly checkOut, int adults, int roomQuantity);

}

