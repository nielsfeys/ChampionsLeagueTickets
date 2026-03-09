using ChampionsLeagueTickets.Domain.Entities;
using ChampionsLeagueTickets.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChampionsLeagueTickets.Services.Interfaces;
public interface IMatchService {
    public Task<IEnumerable<Match>?> GetAllAsync();
    public Task<IEnumerable<Match>?> GetAllByNameAsync(string clubName);
    public Task<Match?> GetByIdAsync(int id);
}

