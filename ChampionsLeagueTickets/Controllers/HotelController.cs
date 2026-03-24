using AutoMapper;
using ChampionsLeagueTickets.Services.Interfaces;
using ChampionsLeagueTickets.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace ChampionsLeagueTickets.Controllers {
    public class HotelController(IHotelService hotelService, IClubService clubService, IMapper mapper) : Controller {
        private readonly IHotelService _hotelService = hotelService;
        private readonly IClubService _clubService = clubService;
        private readonly IMapper _mapper = mapper;

        public async Task<IActionResult> Search() {
            ViewBag.Clubs = await _clubService.GetAllSellableAsync();
            return View(new HotelSearchVM());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Search(HotelSearchVM model) {
            ViewBag.Clubs = await _clubService.GetAllSellableAsync();

            if (!ModelState.IsValid) {
                return View(model);
            }

            if (model.CheckOut <= model.CheckIn) {
                TempData["Error"] = "Check-out date must be after check-in date.";
                return View(model);
            }

            try {
                var results = await _hotelService.SearchHotelsAsync(
                    model.Destination, model.CheckIn, model.CheckOut, model.Adults, model.RoomQuantity);

                if (results == null) {
                    TempData["Error"] = "API Quota exceeded. Tell website owner to upgrade their plan.";
                }else if (results.Count == 0) {
                    TempData["Error"] = "No hotels found for the given criteria. Try a different city.";
                } else {
                    model.Results = _mapper.Map<List<HotelOfferVM>>(results);
                }
            } catch {
                TempData["Error"] = "Could not reach the hotel search service. Please try again later.";
            }

            return View(model);
        }

        public async Task<IActionResult> Book() {
            TempData["Success"] = "Your room(s) has been booked.";

            return RedirectToAction(nameof(Search));
        }
    }
}
