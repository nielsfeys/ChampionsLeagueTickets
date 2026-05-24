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

        try {
            var tickets = await _ticketService.GetAllUserTicketsAsync(user!.Id);
            var History = new HistoryVM {
                Tickets = tickets ?? []
            };
            return View(History);
        } catch (Exception) {
            TempData["Error"] = "Could not load your ticket history. Please try again.";
            return View(new HistoryVM { Tickets = [] });
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Cancel(int? ticketId) {
        var user = await _userManager.GetUserAsync(User);

        try {
            if (await _ticketService.CancelTicketAsync(ticketId!.Value, user!.Id)) {
                TempData["Success"] = "Your ticket has been cancelled.";
            } else {
                TempData["Error"] = "Could not cancel your ticket. Please try again.";
            }
        } catch (Exception) {
            TempData["Error"] = "An unexpected error occurred. Please try again.";
        }

        return RedirectToAction(nameof(Index));
    }
}
