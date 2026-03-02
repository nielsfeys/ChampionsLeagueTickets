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

    public CalendarService(IClubDAO clubDAO) {
        _clubDAO = clubDAO;
    }

    public IEnumerable<Club> GetAllClubs() {
        return _clubDAO.GetAllClubs();
    }
}

