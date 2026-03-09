using ChampionsLeagueTickets.Domain.Entities;
using ChampionsLeagueTickets.Repositories;

namespace ChampionsLeagueTickets.Services.Interfaces;
public interface IClubService {
    public Task<IEnumerable<Club>?> GetAllAsync();

}


