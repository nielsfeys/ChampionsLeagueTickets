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
        IEnumerable<Club> Clubs = _calendarService.GetAllClubs();
        List<ClubVM>? ClubVMs = null;

        if (Clubs != null) {
            ClubVMs = _mapper.Map<List<ClubVM>>(Clubs);
        }

        return View(ClubVMs);
    }

    [HttpPost]
    public IActionResult Detail(string ClubName) {

        return View();
    }
}

