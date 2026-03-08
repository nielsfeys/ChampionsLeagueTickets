using AutoMapper;
using ChampionsLeagueTickets.Domain.Entities;
using ChampionsLeagueTickets.Services.Interfaces;
using ChampionsLeagueTickets.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace ChampionsLeagueTickets.Controllers;
public class CalendarController(IService<Club> clubService, IService<Match> matchService, IMapper mapper) : Controller {

    private readonly IService<Club> _clubService = clubService;
    private readonly IService<Match> _matchService = matchService;
    private readonly IMapper _mapper = mapper;

    public async Task<IActionResult> Index() {
        ViewBag.Clubs = await _clubService.GetAllAsync();
        var Matches = await _matchService.GetAllAsync();
        var MatchVMs = MatchToMatchVM(Matches);
        return View(MatchVMs);
    }

    public async Task<IActionResult> IndexWithFilter(string clubName) {
        IEnumerable<Match>? Matches;
        if (clubName == "All") {
            Matches = await _matchService.GetAllAsync();
        } else {
            Matches = await _matchService.GetAllByNameAsync(clubName);
        }

        var MatchVMs = MatchToMatchVM(Matches);
       

        return PartialView("_TeamMatchDetails", MatchVMs);
    }

    private List<MatchVM>? MatchToMatchVM(IEnumerable<Match>? matches) {
        List<MatchVM>? MatchVMs = null;

        if (matches != null) {
            MatchVMs = _mapper.Map<List<MatchVM>>(matches);
        }

        return MatchVMs;

    }
}

