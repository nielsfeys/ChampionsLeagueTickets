using ChampionsLeagueTickets.Domain.Entities;

namespace ChampionsLeagueTickets.Repositories.Interfaces;
public interface ITicketDAO {
    public Task AddListAsync(List<Ticket> ticketList);
    public Task<List<Ticket>?> GetOwnedSeasonTicketsAsync(string userId);
    public Task<List<Ticket>?> GetOwnedDayTicketsAsync(string userId);

}
