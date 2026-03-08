using AutoMapper;
using ChampionsLeagueTickets.Domain.Entities;
using ChampionsLeagueTickets.Extensions;
using ChampionsLeagueTickets.Services;
using ChampionsLeagueTickets.Services.Interfaces;
using ChampionsLeagueTickets.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace ChampionsLeagueTickets.Controllers;
public class SeasonTicketsController(IService<Club> clubService, IService<StadiumSection> stadiumSectionService, IMapper mapper) : Controller {

    private readonly IService<Club> _clubService = clubService;
    private readonly IService<StadiumSection> _stadiumSectionService = stadiumSectionService;
    private readonly IMapper _mapper = mapper;

    public async Task<IActionResult> Index() {
        ViewBag.Clubs = await _clubService.GetAllAsync();
        return View();
    }

    public async Task<IActionResult> IndexWithFilter(string? clubName) {
        if (clubName == null) {
            return RedirectToAction(nameof(Index));
        }

        IEnumerable<StadiumSection>? StadiumSections = await _stadiumSectionService.GetAllByNameAsync(clubName);
        StadiumVM StadiumVM = new();

        if (StadiumSections != null) {
            StadiumVM.SectionVMs = _mapper.Map<List<SectionVM>>(StadiumSections);
        }

        return PartialView("_seasonTicketsDetails", StadiumVM);
    }

    public async Task<IActionResult> Select(int? sectionId) {
        if (!sectionId.HasValue) {
            return NotFound();
        }

        StadiumSection? stadiumSection = await _stadiumSectionService.FindByIdAsync(sectionId.Value);
        if (stadiumSection == null) {
            return NotFound();
        }

        var shoppingCartVM = HttpContext.Session.GetObject<ShoppingCartVM>("ShoppingCart") ?? new ShoppingCartVM();

        var existingItem = shoppingCartVM.Tickets.FirstOrDefault(x => x.ClubName == stadiumSection.HomeTeamNavigation.Name);

        //Only 1 season ticket per team
        if (existingItem != null) {
            TempData["Error"] = "You already have a season ticket for this club.";
            return RedirectToAction(nameof(Index));
        }

        shoppingCartVM.Tickets.Add(new SeasonTicketVM {
            SectionId = sectionId.Value,
            ClubName = stadiumSection.HomeTeamNavigation.Name,
            Ring = stadiumSection.Ring,
            Location = stadiumSection.Location,
            Price = 800.00,
            DateCreated = DateTime.Now
        });

        HttpContext.Session.SetObject("ShoppingCart", shoppingCartVM);

        TempData["Success"] = "Season ticket added to cart.";
        return RedirectToAction(nameof(Index));

    }
}

