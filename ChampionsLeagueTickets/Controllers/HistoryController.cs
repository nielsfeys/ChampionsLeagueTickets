using ChampionsLeagueTickets.Services.Interfaces;
using ChampionsLeagueTickets.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace ChampionsLeagueTickets.Controllers;

[Authorize]
public class HistoryController(ITicketService ticketService, UserManager<IdentityUser> userManager) : Controller
{
    private readonly ITicketService _ticketService = ticketService;
    private readonly UserManager<IdentityUser> _userManager = userManager;

    public async Task<IActionResult> Index() {
        var user = await _userManager.GetUserAsync(User);

        // We know user is logged in because of [Authorize]
        var tickets = await _ticketService.GetAllUserTicketsAsync(user!.Id);

        var History = new HistoryVM();

        if (tickets == null) {
            TempData["Error"] = "You don't own any tickets yet";
            History.Tickets = [];
        } else {
            History.Tickets = tickets;
        }

        return View(History);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Cancel(int? ticketId) {
        var user = await _userManager.GetUserAsync(User);

        //if ticketId is null, we would already get a 400 bad request
        //we know user is logged in because of [authorize]
        if (await _ticketService.CancelTicketAsync(ticketId!.Value, user!.Id)) {
            TempData["Success"] = "Your ticket has been cancelled.";
        } else {
            TempData["Error"] = "Could not cancel your ticket. Please try again.";
        }

        return RedirectToAction(nameof(Index));
    }
}
