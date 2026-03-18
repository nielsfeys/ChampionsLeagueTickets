using ChampionsLeagueTickets.Services.Interfaces;
using ChampionsLeagueTickets.Repositories.Interfaces;
using ChampionsLeagueTickets.Domain.Entities;

namespace ChampionsLeagueTickets.Services;
public class ClubService(IClubDAO clubDAO) : IClubService {
    public readonly IClubDAO _clubDAO = clubDAO;

    public async Task<IEnumerable<Club>?> GetAllSellableAsync() {
        return await _clubDAO.GetAllSellableAsync();
    }
}

