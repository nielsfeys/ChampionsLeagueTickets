using ChampionsLeagueTickets.Domain.Entities;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChampionsLeagueTickets.Services.Interfaces;
public interface IService<T> where T : class {
    Task<IEnumerable<T>?> GetAllAsync();
    Task<IEnumerable<T>?> GetAllByNameAsync(string name);
}

