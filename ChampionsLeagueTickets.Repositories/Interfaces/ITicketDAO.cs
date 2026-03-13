using ChampionsLeagueTickets.Domain.Entities;

namespace ChampionsLeagueTickets.Repositories.Interfaces;
public interface ITicketDAO {
    public Task AddListAsync(List<Ticket> ticketList);
    public Task<List<Ticket>?> GetOwnedSeasonTicketsAsync(string userId);
    public Task<List<Ticket>?> GetOwnedDayTicketsAsync(string userId);
    public Task<Dictionary<int, int>> GetSeasonTicketCountsBySectionsAsync(List<int> sectionIds);
    public Task<Dictionary<(int MatchId, int SectionId), int>> GetDayTicketCountsByMatchAndSectionsAsync(List<(int MatchId, int SectionId)> matchSectionPairs);

}
