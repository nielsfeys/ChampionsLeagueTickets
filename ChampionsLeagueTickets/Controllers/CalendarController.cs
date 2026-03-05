using AutoMapper;
using ChampionsLeagueTickets.Domain.Entities;
using ChampionsLeagueTickets.Services.Interfaces;
using ChampionsLeagueTickets.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace ChampionsLeagueTickets.Controllers;
public class CalendarController : Controller {

    private readonly ICalendarService _calendarService;
    private readonly IMapper _mapper;

    public CalendarController(ICalendarService calendarService, IMapper mapper) {
        _calendarService = calendarService;
        _mapper = mapper;
    }

    public IActionResult Index() {
        ViewBag.Clubs = _calendarService.GetAllClubs();
        var Matches = _calendarService.GetAllMatches();
        var MatchVMs = MatchToMatchVM(Matches);
        return View(MatchVMs);
    }

    [HttpPost]
    public IActionResult Index(string clubName) {
        IEnumerable<Match> Matches;
        if (clubName == "All") {
            Matches = _calendarService.GetAllMatches();
        } else {
            Matches = _calendarService.GetMatches(clubName);
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

