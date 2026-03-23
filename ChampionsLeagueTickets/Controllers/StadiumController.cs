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
        ViewBag.Clubs = await _clubService.GetAllSellableAsync();

        return View();
    }

    public async Task<IActionResult> IndexWithFilter(string? clubName) {
        if (clubName == null) {
            return NotFound();
        }

        IEnumerable<StadiumSection>? StadiumSections = await _stadiumSectionService.GetAllByClubNameAsync(clubName);
        
        if (StadiumSections == null || !StadiumSections.Any()) {
            return NotFound();
        }

        StadiumVM StadiumVM = new();
        StadiumVM.SectionVMs = _mapper.Map<List<SectionVM>>(StadiumSections);

        return PartialView("_stadiumDetails", StadiumVM);
    }    
}
