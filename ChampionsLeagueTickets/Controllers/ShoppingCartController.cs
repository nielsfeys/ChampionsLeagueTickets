using AutoMapper;
using ChampionsLeagueTickets.Domain.Entities;
using ChampionsLeagueTickets.Extensions;
using ChampionsLeagueTickets.Services.Interfaces;
using ChampionsLeagueTickets.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Text;
using QRCoder;


namespace ChampionsLeagueTickets.Controllers {
    public class ShoppingCartController(ITicketService ticketService, IStadiumSectionService stadiumSectionService, IMapper mapper, IEmailSender emailSender) : Controller {
        private readonly ITicketService _ticketService = ticketService;
        private readonly IStadiumSectionService _stadiumSectionService = stadiumSectionService;
        private readonly IMapper _mapper = mapper;
        private readonly IEmailSender _emailSender = emailSender;

        public IActionResult Index() {
            var shoppingCartVM = HttpContext.Session.GetObject<ShoppingCartVM>("ShoppingCart") ?? new ShoppingCartVM();
            return View(shoppingCartVM);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> Checkout() {
            List<Ticket>? ticketList = await MakeTicketList();

            if (ticketList == null) {
                return RedirectToAction("Index");
            }

            try {
                await _ticketService.AddListAsync(ticketList);
            } catch(Exception) {
                TempData["Error"] = "An error occurred while processing your order. Please try again.";
                return RedirectToAction("Index");
            }

            await SendEmail(ticketList);

            TempData["Succes"] = "Tickets succesfully added to your account. Thank you for your purchase!";
            
            HttpContext.Session.Remove("ShoppingCart");
            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteSeasonTicket(int sectionId) {
            var shoppingCart = HttpContext.Session.GetObject<ShoppingCartVM>("ShoppingCart") ?? new ShoppingCartVM();

            var itemToRemove = shoppingCart.SeasonTickets.FirstOrDefault(t => t.SectionId == sectionId);
            if (itemToRemove != null) {
                shoppingCart.SeasonTickets.Remove(itemToRemove);
                HttpContext.Session.SetObject("ShoppingCart", shoppingCart);
                TempData["Success"] = "Season ticket removed from cart.";
            }

            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteDayTicket(int sectionId, int matchId) {
            var shoppingCart = HttpContext.Session.GetObject<ShoppingCartVM>("ShoppingCart") ?? new ShoppingCartVM();

            var itemToRemove = shoppingCart.DayTickets.FirstOrDefault(t => t.SectionId == sectionId && t.MatchId == matchId);
            if (itemToRemove != null) {
                shoppingCart.DayTickets.Remove(itemToRemove);
                HttpContext.Session.SetObject("ShoppingCart", shoppingCart);
                TempData["Success"] = "Ticket removed from cart.";
            }

            return RedirectToAction("Index");
        }

        private async Task<List<Ticket>?> MakeTicketList() {
            ShoppingCartVM shoppingCart = HttpContext.Session.GetObject<ShoppingCartVM>("ShoppingCart") ?? new ShoppingCartVM();

            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var options = new Action<IMappingOperationOptions>(opts => opts.Items["UserId"] = userId);

            if (!await CheckTickets(shoppingCart)) {
                return null;
            }

            bool duplicateSeasonTickets = await CheckDuplicateSeasonTickets(shoppingCart.SeasonTickets);
            if (duplicateSeasonTickets) {
                return null;
            }

            List<Ticket> ticketList = _mapper.Map<List<Ticket>>(shoppingCart.SeasonTickets, options);
            var dayTickets = new List<DayTicketVM>();
            foreach (var dayTicket in shoppingCart.DayTickets) {
                for (int i = 0; i < dayTicket.Quantity; i++) {
                    dayTickets.Add(dayTicket);
                }
            }
            ticketList.AddRange(_mapper.Map<List<Ticket>>(dayTickets, options));

            return ticketList;
        }

        private async Task SendEmail(List<Ticket> ticketList) {
            var userEmail = User.FindFirst(ClaimTypes.Email)?.Value;

            if (string.IsNullOrEmpty(userEmail)) {
                return;
            }

            var attachments = new List<(byte[] Content, string FileName, string ContentType)>();
            var emailBody = new StringBuilder();
            emailBody.AppendLine("<h2>Thank you for your purchase!</h2>");
            emailBody.AppendLine("<p>Your tickets are attached as QR codes. Please download and save them to your device.</p>");
            emailBody.AppendLine("<p>You'll need to present these QR codes at the stadium entrance.</p>");

            for (int i = 0; i < ticketList.Count; i++) {
                var ticket = ticketList[i];
                var qrCodeBytes = GenerateQRCode(ticket.Code);
                string fileName;
                var homeClub = ticket.Match?.HometeamNavigation?.Name ?? "Unknown";
                var ring = ticket.Section?.Ring ?? "Unknown";
                var location = ticket.Section?.Location ?? "Unknown";
                if (ticket.Type == "Season") {
                    
                    fileName = $"SeasonTicket_{homeClub}_{ring}_{location}.png";
                } else {
                    var awayClub = ticket.Match?.AwayteamNavigation?.Name ?? "Unknown";
                    var date = ticket.Match?.Date.ToString("yyyy-MM-dd") ?? "Unknown";
                    fileName = $"Ticket_{homeClub}_{awayClub}_{date}_{ring}_{location}.png";
                }

                attachments.Add((qrCodeBytes, fileName, "image/png"));
            }

            emailBody.AppendLine("<p>Please keep these QR codes safe and bring them to the stadium.</p>");

            await _emailSender.SendEmailWithAttachmentsAsync(userEmail, "Your Champions League Tickets", emailBody.ToString(), attachments); 
        }

        private static byte[] GenerateQRCode(string ticketCode) {
            using var qrGenerator = new QRCodeGenerator();
            using var qrCodeData = qrGenerator.CreateQrCode(ticketCode, QRCodeGenerator.ECCLevel.Q);
            using var qrCode = new PngByteQRCode(qrCodeData);
            return qrCode.GetGraphic(20);
        }

        //Returns true if all checks passed and tickets can be bought
        private async Task<bool> CheckTickets(ShoppingCartVM shoppingCart) {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null) {
                return false;
            }

            if (await CheckDuplicateSeasonTickets(shoppingCart.SeasonTickets)) {
                return false;
            }

            if (CheckSeasonTicketsBeforeSeasonStart(shoppingCart.SeasonTickets)) {
                return false;
            }

            // Get user's existing day tickets
            List<Ticket>? ownedDayTickets = await _ticketService.GetOwnedDayTicketsAsync(userId);

            if (ownedDayTickets == null) {
                TempData["Error"] = "Faulty userId";
                return false;
            }

            if (CheckDayTicketsBuyDate(shoppingCart.DayTickets)) {
                return false;
            }

            if (await CheckDayTicketsForMultipleMatchSameDay(shoppingCart.DayTickets, ownedDayTickets)) {
                return false;
            }

            if (await CheckDayTicketsForQuantity(shoppingCart.DayTickets, ownedDayTickets)) {
                return false;
            }

            if (await CheckTooManyTickets(shoppingCart.DayTickets)) {
                return false;
            }

            return true;
        }

        //Returns true if duplicates found
        //Returns false if duplicates not found
        private async Task<bool> CheckDuplicateSeasonTickets(List<SeasonTicketVM> seasonTicketVMs) {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (userId == null) {
                return true;
            }

            List<Ticket>? seasonTickets = await _ticketService.GetOwnedSeasonTicketsAsync(userId);

            if (seasonTickets == null) {
                return false;
            }

            // For each seasonticket in the shopping cart, check that the user does not yet have one for this club
            foreach (var cartTicket in seasonTicketVMs) {
                foreach (var ownedTicket in seasonTickets) {
                    var ownedHomeClub = ownedTicket.Section?.HomeTeamNavigation?.Name;
                    if (!string.IsNullOrEmpty(ownedHomeClub) && ownedHomeClub == cartTicket.HomeClubName) {
                        TempData["Error"] = $"You already have a season ticket for this club: {ownedHomeClub}. Please remove it from your cart.";
                        return true;
                    }
                }
            }
            return false;
        }

        //Returns true if season has already started and user is trying to buy seasontickets
        //Returns false otherwise
        private bool CheckSeasonTicketsBeforeSeasonStart(List<SeasonTicketVM> seasonTicketVMs) {
            if (seasonTicketVMs.Count > 0 && SeasonTicketVM.SeasonStart <= DateOnly.FromDateTime(DateTime.Today)) {
                TempData["Error"] = "You can only buy season tickets before the start of the season. Please remove any season tickets from your cart.";
                return true;   
            }

            return false;
        }

        //Returns true if user tries to buy tickets for multiple different matches on the same day
        //Returns false otherwise
        private async Task<bool> CheckDayTicketsForMultipleMatchSameDay(List<DayTicketVM> dayTicketVMs, List<Ticket> ownedDayTickets) {
            if (dayTicketVMs.Count == 0) {
                return false;
            }
            
            // Group cart tickets by date and match
            var cartTicketsByDate = dayTicketVMs
                .GroupBy(t => new { t.MatchDate, t.MatchId })
                .ToList();

            // Check for conflicts within the cart itself
            var datesInCart = cartTicketsByDate
                .GroupBy(g => g.Key.MatchDate)
                .Where(g => g.Count() > 1)
                .ToList();

            if (datesInCart.Count >0) {
                var conflictDate = datesInCart.First().Key;
                TempData["Error"] = $"You cannot buy tickets for different matches on the same day {conflictDate}. Please remove one from your cart.";
                return true;
            }

            // Check cart tickets against owned tickets
            if (ownedDayTickets != null && ownedDayTickets.Count > 0) {
                foreach (var cartTicket in dayTicketVMs) {
                    foreach (var ownedTicket in ownedDayTickets) {
                        var ownedMatchDate = ownedTicket.Match?.Date;
                        var ownedMatchId = ownedTicket.MatchId;
                        
                        if (ownedMatchDate.HasValue && 
                            ownedMatchDate.Value == cartTicket.MatchDate && 
                            ownedMatchId != cartTicket.MatchId) {
                            TempData["Error"] = $"You already have a ticket for a different match on {cartTicket.MatchDate}. You cannot buy tickets for multiple matches on the same day.";
                            return true;
                        }
                    }
                }
            }

            return false;
        }

        private async Task<bool> CheckDayTicketsForQuantity(List<DayTicketVM> dayTicketVMs, List<Ticket> ownedDayTickets) {
            const int MAX_TICKETS = 4;
    
            if (dayTicketVMs.Count == 0) {
                return false;
            }

            // Group cart tickets by match and sum quantities
            var cartTicketsByMatch = dayTicketVMs
                .GroupBy(t => t.MatchId)
                .Select(g => new { 
                    MatchId = g.Key, 
                    Quantity = g.Sum(t => t.Quantity),
                    MatchInfo = g.First()
                })
                .ToList();

            foreach (var cartGroup in cartTicketsByMatch) {
                int totalQuantity = cartGroup.Quantity;
    
                // Add owned tickets for this match
                if (ownedDayTickets != null) {
                    int ownedCount = ownedDayTickets.Count(t => t.MatchId == cartGroup.MatchId);
                    totalQuantity += ownedCount;
                }
    
                if (totalQuantity > MAX_TICKETS) {
                    int ownedCount = ownedDayTickets?.Count(t => t.MatchId == cartGroup.MatchId) ?? 0;
                    int availableTickets = MAX_TICKETS - ownedCount;
                    
                    TempData["Error"] = $"You cannot buy more than {MAX_TICKETS} tickets for the match on {cartGroup.MatchInfo.MatchDate}. You already own {ownedCount} ticket(s) for this match, so you can only buy {availableTickets} more." +
                        $"Please remove some tickets for this date.";
                    return true;
                }
            }

            return false;
        }

        //Returns true if today is more than 1 month before the match or is after they day of the match
        //Returns false otherwise
        private bool CheckDayTicketsBuyDate(List<DayTicketVM> dayTicketVMs) {
            if (dayTicketVMs.Count > 0) {
                foreach (var ticket in dayTicketVMs) {
                    if (ticket.MatchDate > DateOnly.FromDateTime(DateTime.Today.AddMonths(1)) || ticket.MatchDate < DateOnly.FromDateTime(DateTime.Today)) {
                        TempData["Error"] = $"You can't buy tickets yet for the match on {ticket.MatchDate}";
                        return true;
                    }
                }
            }

            return false;

        }

        //Returns true if any of the tickets would mean the stadiumsection goes over capacity for that match
        //Returns false otherwise
        private async Task<bool> CheckTooManyTickets(List<DayTicketVM> dayTicketVMs) {
            if (dayTicketVMs.Count == 0) {
                return false;
            }

            // Group cart tickets by match and section
            var cartTicketsByMatchAndSection = dayTicketVMs
                .GroupBy(t => new { t.MatchId, t.SectionId })
                .Select(g => new {
                    g.Key.MatchId,
                    g.Key.SectionId,
                    Quantity = g.Sum(t => t.Quantity),
                    TicketInfo = g.First()
                })
                .ToList();

            // Get all unique section IDs and match IDs from cart
            var sectionIds = cartTicketsByMatchAndSection.Select(c => c.SectionId).Distinct().ToList();
            var matchIds = cartTicketsByMatchAndSection.Select(c => c.MatchId).Distinct().ToList();

            // Fetch all sections at once
            var sections = await _stadiumSectionService.FindByIdsAsync(sectionIds);
            var sectionLookup = sections.ToDictionary(s => s.Id);

            // Get season ticket counts for all sections at once
            var seasonTicketCounts = await _ticketService.GetSeasonTicketCountsBySectionsAsync(sectionIds);

            // Get day ticket counts for all match/section combinations at once
            var matchSectionPairs = cartTicketsByMatchAndSection
                .Select(c => (c.MatchId, c.SectionId))
                .ToList();
            var dayTicketCounts = await _ticketService.GetDayTicketCountsByMatchAndSectionsAsync(matchSectionPairs);

            // Now check each cart group using pre-fetched data
            foreach (var cartGroup in cartTicketsByMatchAndSection) {
                // Get section from lookup
                if (!sectionLookup.TryGetValue(cartGroup.SectionId, out var section)) {
                    continue;
                }

                int totalSeats = section.Seats;

                // Get season ticket count from pre-fetched data
                int seasonTicketCount = seasonTicketCounts.TryGetValue(cartGroup.SectionId, out var seasonCount)
                    ? seasonCount
                    : 0;

                // Get day ticket count from pre-fetched data
                var key = (cartGroup.MatchId, cartGroup.SectionId);
                int existingDayTickets = dayTicketCounts.TryGetValue(key, out var dayCount)
                    ? dayCount
                    : 0;

                // Calculate total tickets including what's in cart
                int totalTickets = seasonTicketCount + existingDayTickets + cartGroup.Quantity;

                if (totalTickets > totalSeats) {
                    int availableSeats = totalSeats - seasonTicketCount - existingDayTickets;
                    TempData["Error"] = $"Not enough seats available in {cartGroup.TicketInfo.Ring} {cartGroup.TicketInfo.Location} for the match on {cartGroup.TicketInfo.MatchDate}. " +
                        $"Only {availableSeats} seat(s) available, but you're trying to buy {cartGroup.Quantity}. Please reduce the quantity.";
                    return true;
                }
            }

            return false;
        }
    }
}
