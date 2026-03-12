using ChampionsLeagueTickets.Domain.Entities;
using ChampionsLeagueTickets.Repositories.Interfaces;
using ChampionsLeagueTickets.Services.Interfaces;


namespace ChampionsLeagueTickets.Services;
public class TicketService(ITicketDAO ticketDAO) : ITicketService {
    public readonly ITicketDAO _ticketDAO = ticketDAO;

    public async Task AddListAsync(List<Ticket> ticketList) {
        await _ticketDAO.AddListAsync(ticketList);
    }

}

