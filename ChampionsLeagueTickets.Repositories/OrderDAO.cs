using ChampionsLeagueTickets.Domain.Data;
using ChampionsLeagueTickets.Domain.Entities;
using ChampionsLeagueTickets.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ChampionsLeagueTickets.Repositories;

public class OrderDAO(ChampionsLeagueDbContext context) : IOrderDAO {
    private readonly ChampionsLeagueDbContext _context = context;

    public async Task<Order> AddAsync(Order order) {
        await _context.Orders.AddAsync(order);
        await _context.SaveChangesAsync();
        return order;
    }
}
