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
public class MatchDAO(ChampionsLeagueDbContext dbContext) : IMatchDAO {
    private readonly ChampionsLeagueDbContext _dbContext = dbContext;

    public IEnumerable<Match> GetMatches(string club) {
        return _dbContext.Matches
            .Include(m => m.HometeamNavigation)
            .Include(m => m.AwayteamNavigation)
            .Where(m =>
                m.HometeamNavigation.Name == club ||
                m.AwayteamNavigation.Name == club)
            .OrderBy(m => m.Date)
            .ToList();
    }

    public IEnumerable<Match> GetAllMatches() {
        return _dbContext.Matches
            .Include(m => m.HometeamNavigation)
            .Include(m => m.AwayteamNavigation)
            .ToList();
    }

}

