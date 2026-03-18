using ChampionsLeagueTickets.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace ChampionsLeagueTickets.Repositories.Interfaces;
public interface IClubDAO {
    public Task<IEnumerable<Club>?> GetAllSellableAsync();
}

