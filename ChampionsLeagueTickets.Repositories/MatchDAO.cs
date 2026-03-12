using ChampionsLeagueTickets.Domain.Data;
using ChampionsLeagueTickets.Domain.Entities;
using ChampionsLeagueTickets.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ChampionsLeagueTickets.Repositories;
public class MatchDAO(ChampionsLeagueDbContext dbContext) : IMatchDAO{
    private readonly ChampionsLeagueDbContext _dbContext = dbContext;

    public async Task<IEnumerable<Match>?> GetAllByNameAsync(string clubName) {
        return  await _dbContext.Matches
            .Include(m => m.HometeamNavigation)
            .Include(m => m.AwayteamNavigation)
            .Where(m =>
                m.HometeamNavigation.Name == clubName ||
                m.AwayteamNavigation.Name == clubName)
            .OrderBy(m => m.Date)
            .ToListAsync();
    }

    public async Task<IEnumerable<Match>?> GetAllAsync() {
        return await _dbContext.Matches
            .Include(m => m.HometeamNavigation)
            .Include(m => m.AwayteamNavigation)
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

