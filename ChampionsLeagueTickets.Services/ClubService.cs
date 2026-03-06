using ChampionsLeagueTickets.Services.Interfaces;
using ChampionsLeagueTickets.Repositories.Interfaces;
using ChampionsLeagueTickets.Domain.Entities;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace ChampionsLeagueTickets.Services;
public class ClubService : IService<Club> {
    public readonly IDAO<Club> _clubDAO;

    public ClubService(IDAO<Club> clubDAO) {
        _clubDAO = clubDAO;
    }

    public async Task<IEnumerable<Club>?> GetAllAsync() {
        return await _clubDAO.GetAllAsync();
    }

    public async Task<IEnumerable<Club>?> GetAllByNameAsync(string name) {
        throw new NotImplementedException();
    }
}

