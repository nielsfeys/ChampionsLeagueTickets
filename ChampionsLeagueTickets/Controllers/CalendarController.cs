using AutoMapper;
using ChampionsLeagueTickets.Domain.Entities;
using ChampionsLeagueTickets.Services.Interfaces;
using ChampionsLeagueTickets.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace ChampionsLeagueTickets.Controllers;
public class CalendarController(IClubService clubService, IMatchService matchService, IMapper mapper) : Controller {

    private readonly IClubService _clubService = clubService;
    private readonly IMatchService _matchService = matchService;
    private readonly IMapper _mapper = mapper;

    public async Task<IActionResult> Index() {
        try {
            ViewBag.Clubs = await _clubService.GetAllSellableAsync();
            var Matches = await _matchService.GetAllFutureAsync();
            var MatchVMs = MatchToMatchVM(Matches);
            return View(MatchVMs);
        } catch (Exception) {
            TempData["Error"] = "Could not load the calendar. Please try again.";
            ViewBag.Clubs = new List<Club>();
            return View(new List<MatchVM>());
        }
    }

    public async Task<IActionResult> IndexWithFilter(string? clubName) {
        if (clubName == null) {
            return NotFound();
        }

        try {
            IEnumerable<Match>? Matches;
            if (clubName == "All") {
                Matches = await _matchService.GetAllFutureAsync();
            } else {
                Matches = await _matchService.GetAllFutureByNameAsync(clubName);
            }

            var MatchVMs = MatchToMatchVM(Matches);
            return PartialView("_TeamMatchDetails", MatchVMs);
        } catch (Exception) {
            return PartialView("_TeamMatchDetails", new List<MatchVM>());
        }
    }

    private List<MatchVM>? MatchToMatchVM(IEnumerable<Match>? matches) {
        List<MatchVM>? MatchVMs = null;

        if (matches != null) {
            MatchVMs = _mapper.Map<List<MatchVM>>(matches);
        }

        return MatchVMs;

    }
}

