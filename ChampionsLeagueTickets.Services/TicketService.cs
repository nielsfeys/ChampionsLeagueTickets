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

    public async Task<int> GetSeasonTicketCountBySectionAsync(int sectionId) {
        return await _ticketDAO.GetSeasonTicketCountBySectionAsync(sectionId);
    }

    public async Task<int> GetDayTicketCountByMatchAndSectionAsync(int matchId, int sectionId) {
        return await _ticketDAO.GetDayTicketCountByMatchAndSectionAsync(matchId, sectionId);
    }

    public async Task<List<Ticket>> GetAllUserTicketsAsync(string userId) {
        return await _ticketDAO.GetAllUserTicketsAsync(userId);
    }
    public async Task<bool> CancelTicketAsync(int ticketId, string userId) {
        return await _ticketDAO.CancelTicketAsync(ticketId, userId);
    }
    public async Task<int> GetMaxDayTicketsBySectionAsync(int sectionId) {
        return await _ticketDAO.GetMaxDayTicketsBySectionAsync(sectionId);
    }
}

