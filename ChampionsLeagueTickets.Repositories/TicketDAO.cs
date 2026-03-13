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

            // Load navigation properties AFTER saving
            foreach (var ticket in ticketList) {
                if (ticket.Type == "Season") {
                    await _dbContext.Entry(ticket).Reference(t => t.Section).LoadAsync();
                    await _dbContext.Entry(ticket.Section).Reference(s => s.HomeTeamNavigation).LoadAsync();
                } else {
                    await _dbContext.Entry(ticket).Reference(t => t.Match).LoadAsync();
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
}

