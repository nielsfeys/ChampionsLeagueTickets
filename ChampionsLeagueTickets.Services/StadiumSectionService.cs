using ChampionsLeagueTickets.Domain.Entities;
using ChampionsLeagueTickets.Repositories.Interfaces;
using ChampionsLeagueTickets.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChampionsLeagueTickets.Services;
public class StadiumSectionService : IService<StadiumSection> {
    private readonly IDAO<StadiumSection> _stadiumSectionDAO;

    public StadiumSectionService(IDAO<StadiumSection> stadiumSectionDAO) {
        _stadiumSectionDAO = stadiumSectionDAO;
    }

    public async Task<IEnumerable<StadiumSection>?> GetAllAsync() {
        throw new NotImplementedException();
    }

    public async Task<IEnumerable<StadiumSection>?> GetAllByNameAsync(string clubName) {
        return await _stadiumSectionDAO.GetAllByNameAsync(clubName);
    }

}

