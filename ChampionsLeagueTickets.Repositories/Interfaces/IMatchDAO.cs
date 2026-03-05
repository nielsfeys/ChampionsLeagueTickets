using ChampionsLeagueTickets.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChampionsLeagueTickets.Repositories.Interfaces;
public interface IMatchDAO {
    public IEnumerable<Match> GetMatches(string club);
    public IEnumerable<Match> GetAllMatches();
}

