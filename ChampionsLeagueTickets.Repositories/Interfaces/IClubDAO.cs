using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using ChampionsLeagueTickets.Domain.Entities;

namespace ChampionsLeagueTickets.Repositories.Interfaces;
public interface IClubDAO {
    public IEnumerable<Club> GetAllClubs();

}

