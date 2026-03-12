using ChampionsLeagueTickets.Domain.Entities;

namespace ChampionsLeagueTickets.Repositories.Interfaces;
public interface ITicketDAO {
    public Task AddListAsync(List<Ticket> ticketList);
}
