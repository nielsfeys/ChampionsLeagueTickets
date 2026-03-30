using AutoMapper;
using ChampionsLeagueTickets.Domain.Entities;
using ChampionsLeagueTickets.Services.Interfaces;
using ChampionsLeagueTickets.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace ChampionsLeagueTickets.Controllers;
public class StadiumController(IClubService clubService, IStadiumSectionService stadiumSectionService, IMapper mapper) : Controller {
    private readonly IClubService _clubService = clubService;
    private readonly IStadiumSectionService _stadiumSectionService = stadiumSectionService;
    private readonly IMapper _mapper = mapper;

    public async Task<IActionResult> Index() {
        try {
            ViewBag.Clubs = await _clubService.GetAllSellableAsync();
        } catch (Exception) {
            TempData["Error"] = "Could not load clubs. Please try again.";
            ViewBag.Clubs = new List<Club>();
        }
        return View();
    }

    public async Task<IActionResult> IndexWithFilter(string? clubName) {
        if (clubName == null) {
            return NotFound();
        }

        try {
            IEnumerable<StadiumSection>? StadiumSections = await _stadiumSectionService.GetAllByClubNameAsync(clubName);

            if (StadiumSections == null || !StadiumSections.Any()) {
                return NotFound();
            }

            StadiumVM StadiumVM = new();
            StadiumVM.SectionVMs = _mapper.Map<List<SectionVM>>(StadiumSections);

            return PartialView("_stadiumDetails", StadiumVM);
        } catch (Exception) {
            return PartialView("_stadiumDetails", new StadiumVM());
        }
    }    
}
