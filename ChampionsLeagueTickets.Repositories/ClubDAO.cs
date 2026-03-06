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
public class ClubDAO(ChampionsLeagueDbContext dbContext) : IDAO<Club> {
    private readonly ChampionsLeagueDbContext _dbContext = dbContext;

    public async Task<IEnumerable<Club>?> GetAllAsync() {
        return await _dbContext.Clubs.ToListAsync();
    }

    public Task<IEnumerable<Club>?> GetAllByNameAsync(string name) {
        throw new NotImplementedException();
    }

}