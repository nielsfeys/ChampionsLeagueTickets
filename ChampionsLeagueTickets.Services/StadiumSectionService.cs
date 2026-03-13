using ChampionsLeagueTickets.Domain.Entities;
using ChampionsLeagueTickets.Repositories.Interfaces;
using ChampionsLeagueTickets.Services.Interfaces;

namespace ChampionsLeagueTickets.Services;
public class StadiumSectionService(IStadiumSectionDAO stadiumSectionDAO) : IStadiumSectionService {
    private readonly IStadiumSectionDAO _stadiumSectionDAO = stadiumSectionDAO;

    public async Task<IEnumerable<StadiumSection>?> GetAllAsync() {
        throw new NotImplementedException();
    }

    public async Task<IEnumerable<StadiumSection>?> GetAllByClubNameAsync(string clubName) {
        return await _stadiumSectionDAO.GetAllByClubNameAsync(clubName);
    }

    public async Task<StadiumSection?> FindByIdAsync(int id) {
        return await _stadiumSectionDAO.FindByIdAsync(id); 
    }

    public async Task<List<StadiumSection>> FindByIdsAsync(List<int> sectionIds)
    {
        return await _stadiumSectionDAO.FindByIdsAsync(sectionIds);
    }
}

