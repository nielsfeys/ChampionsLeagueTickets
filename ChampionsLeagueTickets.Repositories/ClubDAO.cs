using ChampionsLeagueTickets.Domain.Data;
using ChampionsLeagueTickets.Domain.Entities;
using ChampionsLeagueTickets.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChampionsLeagueTickets.Repositories;
public class ClubDAO(ChampionsLeagueDbContext dbContext) : IClubDAO {
    private readonly ChampionsLeagueDbContext _dbContext = dbContext;

    public IEnumerable<Club> GetAllClubs() {
        return _dbContext.Clubs.ToList();
    }

}

