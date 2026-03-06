using ChampionsLeagueTickets.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChampionsLeagueTickets.Repositories.Interfaces;
public interface IDAO<T> where T : class {
    Task<IEnumerable<T>?> GetAllAsync();
    Task<IEnumerable<T>?> GetAllByNameAsync(string name);
}

