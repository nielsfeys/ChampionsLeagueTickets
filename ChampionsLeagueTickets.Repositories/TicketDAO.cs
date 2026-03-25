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

    public async Task<int> GetSeasonTicketCountBySectionAsync(int sectionId) {
        return await _dbContext.Tickets
            .Where(t => t.Type == "Season" && sectionId == t.SectionId)
            .CountAsync();
    }

    public async Task<int> GetDayTicketCountByMatchAndSectionAsync(int matchId, int sectionId) {
        return await _dbContext.Tickets
            .Where(t => t.Type == "Day" && t.MatchId == matchId && t.SectionId == sectionId)
            .CountAsync();
    }

    public async Task<List<Ticket>> GetAllUserTicketsAsync(string userId) {
        return await _dbContext.Tickets
            .Include(t => t.Match)
                .ThenInclude(m => m.HometeamNavigation)
            .Include(t => t.Match)
                .ThenInclude(m => m.AwayteamNavigation)
            .Include(t => t.Section)
                .ThenInclude(m => m.HomeTeamNavigation)
            .Where(t => t.UserId == userId)
            .OrderByDescending(t => t.Type)
            .ThenBy(t => t.Match.Date)
            .ToListAsync();
    }

    public async Task<bool> CancelTicketAsync(int ticketId, string userId) {
        var ticket = await _dbContext.Tickets
            .Include(t => t.Match)
            .FirstOrDefaultAsync(t => t.Id == ticketId);
        
        if (ticket == null || ticket.UserId != userId) {
            return false;
        }

        if (ticket.Type == "Day" && ticket.Match != null && 
            ticket.Match.Date < DateOnly.FromDateTime(DateTime.Today.AddDays(7))) {
            return false;
        }

        ticket.Status = "Cancelled";
        await _dbContext.SaveChangesAsync();
        
        return true;
    }

    public async Task<int> GetMaxDayTicketsBySectionAsync(int sectionId) {
        return await _dbContext.Tickets
            .Where(t => t.Type == "Day" && sectionId == t.SectionId)
            .Select(t => (int?)t.Seat)
            .MaxAsync() ?? 0;
    }
}

