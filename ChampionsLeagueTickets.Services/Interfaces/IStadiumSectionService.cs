using ChampionsLeagueTickets.Domain.Entities;

namespace ChampionsLeagueTickets.Services.Interfaces;
public interface IStadiumSectionService {
    public Task<IEnumerable<StadiumSection>?> GetAllByClubNameAsync(string clubName);
    public Task<StadiumSection?> FindByIdAsync(int id);
    Task<List<StadiumSection>> FindByIdsAsync(List<int> sectionIds);
}

