using ChampionsLeagueTickets.Domain.Entities;
using ChampionsLeagueTickets.Repositories.Interfaces;
using ChampionsLeagueTickets.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChampionsLeagueTickets.Services;
public class MatchService : IService<Match> {
    public readonly IDAO<Match> _matchDAO;
    public MatchService(IDAO<Match> matchDAO) {
        _matchDAO = matchDAO;
    }

    public async Task<IEnumerable<Match>?> GetAllAsync() {
        return await _matchDAO.GetAllAsync();
    }

    public async Task<IEnumerable<Match>?> GetAllByNameAsync(string clubName) {
        return await _matchDAO.GetAllByNameAsync(clubName);
    }


}


