using ChampionsLeagueTickets.Domain.Entities;

namespace ChampionsLeagueTickets.Services.Interfaces;
public interface IMatchService {
    public Task<IEnumerable<Match>?> GetAllFutureAsync();
    public Task<IEnumerable<Match>?> GetAllFutureByNameAsync(string clubName);
    public Task<Match?> GetByIdAsync(int id);
}

