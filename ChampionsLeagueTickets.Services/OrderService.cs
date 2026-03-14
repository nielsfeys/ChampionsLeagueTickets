using ChampionsLeagueTickets.Domain.Entities;
using ChampionsLeagueTickets.Repositories.Interfaces;
using ChampionsLeagueTickets.Services.Interfaces;

namespace ChampionsLeagueTickets.Services;

public class OrderService(IOrderDAO orderDAO) : IOrderService {
    private readonly IOrderDAO _orderDAO = orderDAO;

    public async Task<Order> CreateOrderWithOrderlinesAsync(string userId, List<Ticket> tickets) {
        var order = new Order {
            UserId = userId,
            Date = DateOnly.FromDateTime(DateTime.Now),
            Orderlines = new List<Orderline>()
        };

        // Create an orderline for each ticket
        foreach (var ticket in tickets) {
            order.Orderlines.Add(new Orderline {
                TicketId = ticket.Id,
                Order = order
            });
        }

        return await _orderDAO.AddAsync(order);
    }
}
