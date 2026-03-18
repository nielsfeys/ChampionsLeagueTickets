using ChampionsLeagueTickets.Domain.Data;
using ChampionsLeagueTickets.Domain.Entities;
using ChampionsLeagueTickets.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ChampionsLeagueTickets.Repositories;
public class MatchDAO(ChampionsLeagueDbContext dbContext) : IMatchDAO{
    private readonly ChampionsLeagueDbContext _dbContext = dbContext;

    public async Task<IEnumerable<Match>?> GetAllFutureByNameAsync(string clubName) {
        return  await _dbContext.Matches
            .Include(m => m.HometeamNavigation)
            .Include(m => m.AwayteamNavigation)
            .Where(m =>
                m.Date > DateOnly.FromDateTime(DateTime.Today) &&
                (m.HometeamNavigation.Name == clubName ||
                m.AwayteamNavigation.Name == clubName))
            .OrderBy(m => m.Date)
            .ToListAsync();
    }

    public async Task<IEnumerable<Match>?> GetAllFutureAsync() {
        return await _dbContext.Matches
            .Include(m => m.HometeamNavigation)
            .Include(m => m.AwayteamNavigation)
            .Where(m => m.Date > DateOnly.FromDateTime(DateTime.Today))
            .OrderBy(m => m.Date)
            .ToListAsync();
    }

    public async Task<Match?> GetByIdAsync(int id) {
        return await _dbContext.Matches
            .Include(m => m.HometeamNavigation)
            .Include(m => m.AwayteamNavigation)
            .Where(m => m.Id == id)
            .FirstOrDefaultAsync();
    }

}

