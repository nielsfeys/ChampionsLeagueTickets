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
}

