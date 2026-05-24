using AutoMapper;
using ChampionsLeagueTickets.Domain.Entities;
using ChampionsLeagueTickets.Extensions;
using ChampionsLeagueTickets.Services;
using ChampionsLeagueTickets.Services.Interfaces;
using ChampionsLeagueTickets.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace ChampionsLeagueTickets.Controllers;
public class SeasonTicketsController(IClubService clubService, IStadiumSectionService stadiumSectionService, IMapper mapper) : Controller {

    private readonly IClubService _clubService = clubService;
    private readonly IStadiumSectionService _stadiumSectionService = stadiumSectionService;
    private readonly IMapper _mapper = mapper;

    public async Task<IActionResult> Index() {
        ViewBag.Clubs = await _clubService.GetAllSellableAsync();
        return View();
    }

    public async Task<IActionResult> IndexWithFilter(string? clubName) {
        if (clubName == null) {
            return RedirectToAction(nameof(Index));
        }

        IEnumerable<StadiumSection>? StadiumSections = await _stadiumSectionService.GetAllByClubNameAsync(clubName);
        StadiumVM StadiumVM = new();

        if (StadiumSections != null) {
            StadiumVM.SectionVMs = _mapper.Map<List<SectionVM>>(StadiumSections);
        }

        return PartialView("_seasonTicketsDetails", StadiumVM);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AddToCart(int? sectionId) {
        //Should never get triggered
        if (!sectionId.HasValue) {
            return NotFound();
        }

        //Should never get triggered
        StadiumSection? stadiumSection = await _stadiumSectionService.FindByIdAsync(sectionId.Value);
        if (stadiumSection == null) {
            return NotFound();
        }

        var shoppingCartVM = HttpContext.Session.GetObject<ShoppingCartVM>("ShoppingCart") ?? new ShoppingCartVM();

        var existingItem = shoppingCartVM.SeasonTickets.FirstOrDefault(x => x.HomeClubName == stadiumSection.HomeTeamNavigation.Name);

        //Only 1 season ticket per team
        if (existingItem != null) {
            TempData["Error"] = "You already have a season ticket for this club.";
            return RedirectToAction(nameof(Index));
        }

        shoppingCartVM.SeasonTickets.Add(new SeasonTicketVM {
            SectionId = sectionId.Value,
            HomeClubName = stadiumSection.HomeTeamNavigation.Name,
            Ring = stadiumSection.Ring,
            Location = stadiumSection.Location,
            Price = stadiumSection.Price * SeasonTicketVM.PriceMultiplier,
            DateCreated = DateOnly.FromDateTime(DateTime.Now)
        });

        HttpContext.Session.SetObject("ShoppingCart", shoppingCartVM);

        TempData["Success"] = "Season ticket added to cart.";
        return RedirectToAction(nameof(Index));

    }
}

