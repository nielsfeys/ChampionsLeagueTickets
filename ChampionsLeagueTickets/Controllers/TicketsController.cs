using AutoMapper;
using ChampionsLeagueTickets.Domain.Entities;
using ChampionsLeagueTickets.Extensions;
using ChampionsLeagueTickets.Services.Interfaces;
using ChampionsLeagueTickets.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace ChampionsLeagueTickets.Controllers;
public class TicketsController(IStadiumSectionService stadiumSectionService, IMatchService matchService, IMapper mapper) : Controller {

    private readonly IStadiumSectionService _stadiumSectionService = stadiumSectionService;
    private readonly IMatchService _matchService = matchService;
    private readonly IMapper _mapper = mapper;

    public async Task<IActionResult> Index(int? matchId) {
        if (matchId == null) return NotFound();

        try {
            Match? match = await _matchService.GetByIdAsync(matchId.Value);
            if (match == null) return NotFound();

            if (match.Date <= DateOnly.FromDateTime(DateTime.Today) && match.Date < DateOnly.FromDateTime(DateTime.Today.AddMonths(1))) {
                return Forbid();
            }

            string homeClubName = match.HometeamNavigation.Name;
            ViewBag.HomeClub = homeClubName;
            ViewBag.AwayClub = match.AwayteamNavigation.Name;
            ViewBag.MatchDate = match.Date;
            ViewBag.MatchId = match.Id;

            IEnumerable<StadiumSection>? StadiumSections = await _stadiumSectionService.GetAllByClubNameAsync(homeClubName);
            StadiumVM stadiumVM = new();

            if (StadiumSections != null) {
                stadiumVM.SectionVMs = _mapper.Map<List<SectionVM>>(StadiumSections);
            }
            return View(stadiumVM);
        } catch (Exception) {
            TempData["Error"] = "Could not load the ticket page. Please try again.";
            return RedirectToAction(nameof(HomeController.Index), "Home");
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AddToCart(int? matchId, int? sectionId, int? quantity) {
        if (!matchId.HasValue || !sectionId.HasValue || !quantity.HasValue) return NotFound();

        try {
            Match? match = await _matchService.GetByIdAsync(matchId.Value);
            StadiumSection? stadiumSection = await _stadiumSectionService.FindByIdAsync(sectionId.Value);
            if (stadiumSection == null || match == null) return NotFound();

            if (match.Hometeam != stadiumSection.HomeTeam) return NotFound();

            if (quantity.Value <= 0) {
                TempData["Error"] = "You need to add at least 1 ticket.";
                return RedirectToAction(nameof(Index), new { matchId = matchId.Value });
            }

            if (quantity.Value > 4) {
                TempData["Error"] = "You can't buy more than 4 tickets.";
                return RedirectToAction(nameof(Index), new { matchId = matchId.Value });
            }

            var shoppingCartVM = HttpContext.Session.GetObject<ShoppingCartVM>("ShoppingCart") ?? new ShoppingCartVM();
            var existingItem = shoppingCartVM.DayTickets.FirstOrDefault(x => x.MatchId == matchId.Value && x.SectionId == sectionId.Value);

            if (existingItem != null) {
                existingItem.Quantity += quantity.Value;
                if (existingItem.Quantity > 4) {
                    existingItem.Quantity = 4;
                    TempData["Error"] = "You added too many tickets! Tickets were added up to the limit";
                    return RedirectToAction(nameof(Index), new { matchId = matchId.Value });
                }
            } else {
                shoppingCartVM.DayTickets.Add(new DayTicketVM {
                    SectionId = sectionId.Value,
                    HomeClubName = stadiumSection.HomeTeamNavigation.Name,
                    Ring = stadiumSection.Ring,
                    Location = stadiumSection.Location,
                    DateCreated = DateOnly.FromDateTime(DateTime.Now),
                    Quantity = quantity.Value,
                    MatchId = matchId.Value,
                    AwayClubName = match.AwayteamNavigation.Name,
                    MatchDate = match.Date,
                    Price = stadiumSection.Price
                });
            }

            HttpContext.Session.SetObject("ShoppingCart", shoppingCartVM);
            TempData["Success"] = "Ticket(s) added to cart.";
            return RedirectToAction(nameof(Index), new { matchId = matchId.Value });
        } catch (Exception) {
            TempData["Error"] = "Could not add ticket to cart. Please try again.";
            return RedirectToAction(nameof(Index), new { matchId = matchId.Value });
        }
    }
}

