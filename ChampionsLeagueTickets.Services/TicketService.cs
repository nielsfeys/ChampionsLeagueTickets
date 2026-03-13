using ChampionsLeagueTickets.Domain.Entities;
using ChampionsLeagueTickets.Repositories.Interfaces;
using ChampionsLeagueTickets.Services.Interfaces;
using Org.BouncyCastle.Asn1.IsisMtt.X509;


namespace ChampionsLeagueTickets.Services;
public class TicketService(ITicketDAO ticketDAO) : ITicketService {
    public readonly ITicketDAO _ticketDAO = ticketDAO;

    public async Task AddListAsync(List<Ticket> ticketList) {
        await _ticketDAO.AddListAsync(ticketList);
    }

    public async Task<List<Ticket>?> GetOwnedSeasonTicketsAsync(string userId) {
        return await _ticketDAO.GetOwnedSeasonTicketsAsync(userId);
    }

    public async Task<List<Ticket>?> GetOwnedDayTicketsAsync(string userId) {
        return await _ticketDAO.GetOwnedDayTicketsAsync(userId);
    }

    public async Task<Dictionary<int, int>> GetSeasonTicketCountsBySectionsAsync(List<int> sectionIds) {
        return await _ticketDAO.GetSeasonTicketCountsBySectionsAsync(sectionIds);
    }

    public async Task<Dictionary<(int MatchId, int SectionId), int>> GetDayTicketCountsByMatchAndSectionsAsync(List<(int MatchId, int SectionId)> matchSectionPairs) {
        return await _ticketDAO.GetDayTicketCountsByMatchAndSectionsAsync(matchSectionPairs);
    }
}

