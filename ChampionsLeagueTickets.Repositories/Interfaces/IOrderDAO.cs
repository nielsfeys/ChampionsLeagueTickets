using ChampionsLeagueTickets.Domain.Entities;

namespace ChampionsLeagueTickets.Repositories.Interfaces;

public interface IOrderDAO {
    Task<Order> AddAsync(Order order);
}
