using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ChampionsLeagueTickets.Controllers {
    public class ShoppingCartController : Controller {

        [Authorize]
        public IActionResult Index() {
            return View();
        }
    }
}
