using ChampionsLeagueTickets.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChampionsLeagueTickets.Repositories.Interfaces;
public interface IStadiumSectionDAO {
    public Task<IEnumerable<StadiumSection>?> GetAllByClubNameAsync(string clubName);
    public Task<StadiumSection?> FindByIdAsync(int id);
}

