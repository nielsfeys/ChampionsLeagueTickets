using ChampionsLeagueTickets.Domain.Data;
using ChampionsLeagueTickets.Domain.Entities;
using ChampionsLeagueTickets.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ChampionsLeagueTickets.Repositories;
public class StadiumSectionDAO(ChampionsLeagueDbContext dbContext) : IStadiumSectionDAO {
    private readonly ChampionsLeagueDbContext _dbContext = dbContext;

    public async Task<IEnumerable<StadiumSection>?> GetAllByClubNameAsync(string clubName) {
        return await _dbContext.Stadiumsections
            .Include(s => s.HomeTeamNavigation)
            .Where(s => s.HomeTeamNavigation.Name == clubName)
            .ToListAsync();
    }

    public async Task<StadiumSection?> FindByIdAsync(int id) {
        return await _dbContext.Stadiumsections
            .Include(s => s.HomeTeamNavigation)
            .Where(s => s.Id == id)
            .FirstOrDefaultAsync();
    }

}

