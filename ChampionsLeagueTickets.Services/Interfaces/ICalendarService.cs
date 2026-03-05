using ChampionsLeagueTickets.Domain.Entities;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChampionsLeagueTickets.Services.Interfaces {
    public interface ICalendarService {
        public IEnumerable<Club> GetAllClubs();
        public IEnumerable<Match> GetMatches(string club);
        public IEnumerable<Match> GetAllMatches();
    }
}
