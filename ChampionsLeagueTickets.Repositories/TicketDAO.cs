using ChampionsLeagueTickets.Domain.Data;
using ChampionsLeagueTickets.Domain.Entities;
using ChampionsLeagueTickets.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ChampionsLeagueTickets.Repositories;
public class TicketDAO (ChampionsLeagueDbContext dbContext): ITicketDAO {
    private readonly ChampionsLeagueDbContext _dbContext = dbContext;
    public async Task AddListAsync(List<Ticket> ticketList) {
        foreach (var ticket in ticketList) {
            _dbContext.Entry(ticket).State = EntityState.Added;
        }

        try {
            await _dbContext.SaveChangesAsync();

            foreach (var ticket in ticketList) {
                if (ticket.Type == "Season") {
                    await _dbContext.Entry(ticket).Reference(t => t.Section).LoadAsync();
                    await _dbContext.Entry(ticket.Section).Reference(s => s.HomeTeamNavigation).LoadAsync();
                } else {
                    await _dbContext.Entry(ticket).Reference(t => t.Match).LoadAsync();
                    await _dbContext.Entry(ticket).Reference(t => t.Section).LoadAsync();
                    await _dbContext.Entry(ticket.Match).Reference(m => m.HometeamNavigation).LoadAsync();
                    await _dbContext.Entry(ticket.Match).Reference(m => m.AwayteamNavigation).LoadAsync();
                }
            }
        } catch (Exception ex) {
            Console.WriteLine(ex);
            throw;
        }
    }

    public async Task<List<Ticket>?> GetOwnedSeasonTicketsAsync(string userId) {
        return await _dbContext.Tickets
            .Include(t => t.Section)
            .ThenInclude(s => s.HomeTeamNavigation)
            .Where(t => t.UserId == userId && t.Type == "Season")
            .ToListAsync();
    }

    public async Task<List<Ticket>?> GetOwnedDayTicketsAsync(string userId) {
        return await _dbContext.Tickets
            .Include(t => t.Match)
            .Include(t => t.Section)
            .ThenInclude(s => s.HomeTeamNavigation)
            .Where(t => t.UserId == userId && t.Type == "Day")
            .ToListAsync();
    }

    public async Task<Dictionary<int, int>> GetSeasonTicketCountsBySectionsAsync(List<int> sectionIds) {
        if (sectionIds == null || sectionIds.Count == 0) {
            return new Dictionary<int, int>();
        }

        return await _dbContext.Tickets
            .Where(t => t.Type == "Season" && sectionIds.Contains(t.SectionId))
            .GroupBy(t => t.SectionId)
            .Select(g => new { SectionId = g.Key, Count = g.Count() })
            .ToDictionaryAsync(x => x.SectionId, x => x.Count);
    }

    public async Task<Dictionary<(int MatchId, int SectionId), int>> GetDayTicketCountsByMatchAndSectionsAsync(List<(int MatchId, int SectionId)> matchSectionPairs) {
        if (matchSectionPairs == null || matchSectionPairs.Count == 0) {
            return new Dictionary<(int, int), int>();
        }

        // Create a list of match IDs and section IDs for efficient querying
        var matchIds = matchSectionPairs.Select(p => p.MatchId).Distinct().ToList();
        var sectionIds = matchSectionPairs.Select(p => p.SectionId).Distinct().ToList();

        // Query all relevant tickets in one go
        var ticketCounts = await _dbContext.Tickets
            .Where(t => t.Type == "Day" && 
                   t.MatchId.HasValue && 
                   matchIds.Contains(t.MatchId.Value) && 
                   sectionIds.Contains(t.SectionId))
            .GroupBy(t => new { MatchId = t.MatchId.Value, t.SectionId })
            .Select(g => new { g.Key.MatchId, g.Key.SectionId, Count = g.Count() })
            .ToListAsync();

        return ticketCounts.ToDictionary(
            x => (x.MatchId, x.SectionId),
            x => x.Count
        );
    }
}

