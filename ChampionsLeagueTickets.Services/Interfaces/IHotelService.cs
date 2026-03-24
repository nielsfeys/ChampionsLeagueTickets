using ChampionsLeagueTickets.Services.DTOs;

namespace ChampionsLeagueTickets.Services.Interfaces;
public interface IHotelService {
    Task<List<HotelOfferDTO>?> SearchHotelsAsync(int cityId, DateOnly checkIn, DateOnly checkOut, int adults, int roomQuantity);

}

