using ChampionsLeagueTickets.Services.Interfaces;
using ChampionsLeagueTickets.Repositories.Interfaces;
using ChampionsLeagueTickets.Domain.Entities;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace ChampionsLeagueTickets.Services;
public class CalendarService : ICalendarService{
    public readonly IClubDAO _clubDAO;
    public readonly IMatchDAO _matchDAO;

    public CalendarService(IClubDAO clubDAO, IMatchDAO matchDAO) {
        _clubDAO = clubDAO;
        _matchDAO = matchDAO;
    }

    public IEnumerable<Club> GetAllClubs() {
        return _clubDAO.GetAllClubs();
    }

    public IEnumerable<Match> GetMatches(string club) {
        return _matchDAO.GetMatches(club);
    }

    public IEnumerable<Match> GetAllMatches() {
        return _matchDAO.GetAllMatches();
    }
}

