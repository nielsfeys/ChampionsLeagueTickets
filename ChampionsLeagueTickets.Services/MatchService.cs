using ChampionsLeagueTickets.Domain.Entities;
using ChampionsLeagueTickets.Repositories.Interfaces;
using ChampionsLeagueTickets.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChampionsLeagueTickets.Services;
public class MatchService : IMatchService {
    public readonly IMatchDAO _matchDAO;
    public MatchService(IMatchDAO matchDAO) {
        _matchDAO = matchDAO;
    }

    public async Task<IEnumerable<Match>?> GetAllFutureAsync() {
        return await _matchDAO.GetAllFutureAsync();
    }

    public async Task<IEnumerable<Match>?> GetAllFutureByNameAsync(string clubName) {
        return await _matchDAO.GetAllFutureByNameAsync(clubName);
    }

    public async Task<Match?> GetByIdAsync(int id) {
        return await _matchDAO.GetByIdAsync(id);
    }

}


