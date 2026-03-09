using ChampionsLeagueTickets.Extensions;
using ChampionsLeagueTickets.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ChampionsLeagueTickets.Controllers {
    [Authorize]
    public class ShoppingCartController : Controller {
        public IActionResult Index() {
            var shoppingCartVM = HttpContext.Session.GetObject<ShoppingCartVM>("ShoppingCart") ?? new ShoppingCartVM();
            return View(shoppingCartVM);
        }
    }
}
