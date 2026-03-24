using ChampionsLeagueTickets.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChampionsLeagueTickets.Services.Interfaces;
public interface ITicketService {
    public Task AddListAsync(List<Ticket> ticketList);
    public Task<List<Ticket>?> GetOwnedSeasonTicketsAsync(string userId);
    public Task<List<Ticket>?> GetOwnedDayTicketsAsync(string userId);
    public Task<int> GetSeasonTicketCountBySectionAsync(int sectionId);
    public Task<int> GetDayTicketCountByMatchAndSectionAsync(int matchId, int SectionId);    
    public Task<List<Ticket>> GetAllUserTicketsAsync(string userId);
    public Task<bool> CancelTicketAsync(int ticketId, string userId);
}

