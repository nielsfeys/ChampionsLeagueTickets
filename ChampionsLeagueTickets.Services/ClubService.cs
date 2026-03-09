using ChampionsLeagueTickets.Services.Interfaces;
using ChampionsLeagueTickets.Repositories.Interfaces;
using ChampionsLeagueTickets.Domain.Entities;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace ChampionsLeagueTickets.Services;
public class ClubService : IClubService {
    public readonly IClubDAO _clubDAO;

    public ClubService(IClubDAO clubDAO) {
        _clubDAO = clubDAO;
    }

    public async Task<IEnumerable<Club>?> GetAllAsync() {
        return await _clubDAO.GetAllAsync();
    }
}

