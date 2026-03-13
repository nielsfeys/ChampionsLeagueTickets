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
    public Task<Dictionary<int, int>> GetSeasonTicketCountsBySectionsAsync(List<int> sectionIds);
    public Task<Dictionary<(int MatchId, int SectionId), int>> GetDayTicketCountsByMatchAndSectionsAsync(List<(int MatchId, int SectionId)> matchSectionPairs);
}

