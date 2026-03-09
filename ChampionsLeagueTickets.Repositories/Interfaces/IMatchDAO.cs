using ChampionsLeagueTickets.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChampionsLeagueTickets.Repositories.Interfaces;
public interface IMatchDAO {
    public Task<IEnumerable<Match>?> GetAllByNameAsync(string clubName);

    public Task<IEnumerable<Match>?> GetAllAsync();

    public Task<Match?> GetByIdAsync(int id);
}

