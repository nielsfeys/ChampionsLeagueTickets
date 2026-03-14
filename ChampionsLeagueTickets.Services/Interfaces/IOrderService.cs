using ChampionsLeagueTickets.Domain.Entities;

namespace ChampionsLeagueTickets.Services.Interfaces;

public interface IOrderService {
    Task<Order> CreateOrderWithOrderlinesAsync(string userId, List<Ticket> tickets);
}

