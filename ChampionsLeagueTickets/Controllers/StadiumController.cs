using AutoMapper;
using ChampionsLeagueTickets.Domain.Entities;
using ChampionsLeagueTickets.Services.Interfaces;
using ChampionsLeagueTickets.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace ChampionsLeagueTickets.Controllers;
public class StadiumController(IService<Club> clubService, IService<StadiumSection> stadiumSectionService, IMapper mapper) : Controller {
    private readonly IService<Club> _clubService = clubService;
    private readonly IService<StadiumSection> _stadiumSectionService = stadiumSectionService;
    private readonly IMapper _mapper = mapper;

    public async Task<IActionResult> Index() {
        ViewBag.Clubs = await _clubService.GetAllAsync();

        return View();
    }

    public async Task<IActionResult> IndexWithFilter(string clubName) {
        IEnumerable<StadiumSection>? StadiumSections = await _stadiumSectionService.GetAllByNameAsync(clubName);
        StadiumVM StadiumVM = new();

        if (StadiumSections != null) {
            StadiumVM.SectionVMs = _mapper.Map<List<SectionVM>>(StadiumSections);
        }

        return PartialView("_stadiumDetails", StadiumVM);
    }
    
}
