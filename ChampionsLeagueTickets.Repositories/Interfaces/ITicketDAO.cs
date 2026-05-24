using ChampionsLeagueTickets.Domain.Entities;

namespace ChampionsLeagueTickets.Repositories.Interfaces;
public interface ITicketDAO {
    public Task AddListAsync(List<Ticket> ticketList);
    public Task<List<Ticket>?> GetOwnedSeasonTicketsAsync(string userId);
    public Task<List<Ticket>?> GetOwnedDayTicketsAsync(string userId);
    public Task<int> GetSeasonTicketCountBySectionAsync(int sectionId);
    public Task<int> GetDayTicketCountByMatchAndSectionAsync(int matchId, int sectionId);
    public Task<List<Ticket>> GetAllUserTicketsAsync(string userId);
    public Task<bool> CancelTicketAsync(int ticketId, string userId);
    public Task<int> GetMaxDayTicketsBySectionAsync(int sectionId);
}
