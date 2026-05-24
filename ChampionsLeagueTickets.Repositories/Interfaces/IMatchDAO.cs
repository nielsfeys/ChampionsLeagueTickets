using ChampionsLeagueTickets.Domain.Entities;

namespace ChampionsLeagueTickets.Repositories.Interfaces;
public interface IMatchDAO {
    public Task<IEnumerable<Match>?> GetAllFutureByNameAsync(string clubName);

    public Task<IEnumerable<Match>?> GetAllFutureAsync();

    public Task<Match?> GetByIdAsync(int id);
}

