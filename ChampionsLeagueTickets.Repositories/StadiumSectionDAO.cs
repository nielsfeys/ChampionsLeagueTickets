using ChampionsLeagueTickets.Domain.Data;
using ChampionsLeagueTickets.Domain.Entities;
using ChampionsLeagueTickets.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChampionsLeagueTickets.Repositories;
public class StadiumSectionDAO(ChampionsLeagueDbContext dbContext) : IDAO<StadiumSection> {
    private readonly ChampionsLeagueDbContext _dbContext = dbContext;

    public async Task<IEnumerable<StadiumSection>?> GetAllAsync() {
        throw new NotImplementedException();
    }

    public async Task<IEnumerable<StadiumSection>?> GetAllByNameAsync(string clubName) {
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

