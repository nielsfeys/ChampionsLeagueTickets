using AutoMapper;
using ChampionsLeagueTickets.Domain.Entities;
using ChampionsLeagueTickets.Extensions;
using ChampionsLeagueTickets.Services.Interfaces;
using ChampionsLeagueTickets.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QRCoder;
using System.Security.Claims;
using System.Text;


namespace ChampionsLeagueTickets.Controllers {
    public class ShoppingCartController(ITicketService ticketService, IStadiumSectionService stadiumSectionService, IMapper mapper, IEmailSender emailSender, IOrderService orderService) : Controller {
        private readonly ITicketService _ticketService = ticketService;
        private readonly IStadiumSectionService _stadiumSectionService = stadiumSectionService;
        private readonly IMapper _mapper = mapper;
        private readonly IEmailSender _emailSender = emailSender;
        private readonly IOrderService _orderService = orderService;

        public IActionResult Index() {
            var shoppingCartVM = HttpContext.Session.GetObject<ShoppingCartVM>("ShoppingCart") ?? new ShoppingCartVM();
            return View(shoppingCartVM);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> Checkout() {
            List<Ticket>? ticketList;

            try {
                ticketList = await MakeTicketList();
            } catch (Exception) {
                TempData["Error"] = "An error occurred while preparing your order. Please try again.";
                return RedirectToAction(nameof(Index));
            }

            if (ticketList == null) {
                return RedirectToAction(nameof(Index));
            }

            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId)) {
                TempData["Error"] = "User not found.";
                return RedirectToAction(nameof(Index));
            }

            try {
                await _ticketService.AddListAsync(ticketList);
                await _orderService.CreateOrderWithOrderlinesAsync(userId, ticketList);
            } catch (Exception) {
                TempData["Error"] = "An error occurred while processing your order. Please try again.";
                return RedirectToAction(nameof(Index));
            }

            var userEmail = User.FindFirst(ClaimTypes.Email)?.Value;
            if (!string.IsNullOrEmpty(userEmail)) {
                _ = Task.Run(async () => {
                    try {
                        await SendEmail(ticketList, userEmail);
                    } catch (Exception ex) {
                        Console.WriteLine($"Email failed: {ex.Message}");
                    }
                    
                });
            }

            TempData["Success"] = "Tickets succesfully added to your account. Thank you for your purchase!";
            
            HttpContext.Session.Remove("ShoppingCart");
            return RedirectToAction(nameof(Index));
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

            return RedirectToAction(nameof(Index));
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

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult UpdateDayTicketQuantity(int sectionId, int matchId, int quantity) {
            if (quantity < 1 || quantity > 4) {
                TempData["Error"] = "Quantity must be between 1 and 4.";
                return RedirectToAction(nameof(Index));
            }

            var shoppingCart = HttpContext.Session.GetObject<ShoppingCartVM>("ShoppingCart") ?? new ShoppingCartVM();
            
            var itemToUpdate = shoppingCart.DayTickets.FirstOrDefault(t => t.SectionId == sectionId && t.MatchId == matchId);
            if (itemToUpdate != null) {
                itemToUpdate.Quantity = quantity;
                HttpContext.Session.SetObject("ShoppingCart", shoppingCart);
            }

            return RedirectToAction(nameof(Index));
        }

        private async Task<List<Ticket>?> MakeTicketList() {
            ShoppingCartVM shoppingCart = HttpContext.Session.GetObject<ShoppingCartVM>("ShoppingCart") ?? new ShoppingCartVM();

            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (!await CheckTickets(shoppingCart)) {
                return null;
            }

            List<Ticket> ticketList = [];

            // Map Season Tickets
            foreach (var seasonTicketVM in shoppingCart.SeasonTickets) {
                var section = await _stadiumSectionService.FindByIdAsync(seasonTicketVM.SectionId);

                if (section == null) {
                    TempData["Error"] = "Something went wrong.";
                    Console.WriteLine($"Could not find section with sectionId {seasonTicketVM.SectionId}");
                    return null;
                }

                int seasonTicketsSold = await _ticketService.GetSeasonTicketCountBySectionAsync(seasonTicketVM.SectionId);

                var options = new Action<IMappingOperationOptions>(opts => {
                    opts.Items["TotalSeats"] = section.Seats;
                    opts.Items["SeasonTicketsSold"] = seasonTicketsSold;
                });

                var ticket = _mapper.Map<Ticket>(seasonTicketVM, options);
                ticketList.Add(ticket);
            }
            

            // Map Day Tickets
           foreach (var dayTicketVM in shoppingCart.DayTickets) {
                int matchTicketsSold = await _ticketService.GetDayTicketCountByMatchAndSectionAsync(dayTicketVM.MatchId, dayTicketVM.SectionId);

                var options = new Action<IMappingOperationOptions>(opts => {
                    opts.Items["DayTicketsSold"] = matchTicketsSold;
                });

                var ticket = _mapper.Map<Ticket>(dayTicketVM, options);
                ticketList.Add(ticket);
            }

            return ticketList;
        }

        private async Task SendEmail(List<Ticket> ticketList, string userEmail) {
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
                var seat = ticket.Seat;
                if (ticket.Type == "Season") {
                    fileName = $"SeasonTicket_{homeClub}_{ring}_{location}_{seat}.png";
                } else {
                    var awayClub = ticket.Match?.AwayteamNavigation?.Name ?? "Unknown";
                    var date = ticket.Match?.Date.ToString("yyyy-MM-dd") ?? "Unknown";
                    fileName = $"Ticket_{homeClub}_{awayClub}_{date}_{ring}_{location}_{seat}.png";
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

            if (!await CheckNoDuplicateSeasonTickets(shoppingCart.SeasonTickets)) {
                return false;
            }

            if (!CheckBeforeSeasonStart(shoppingCart.SeasonTickets)) {
                return false;
            }

            if (!await CheckSeasonTicketSeatAvailable(shoppingCart.SeasonTickets)) {
                return false;
            }

            List<Ticket>? ownedDayTickets = await _ticketService.GetOwnedDayTicketsAsync(userId);

            if (ownedDayTickets == null) {
                TempData["Error"] = "Faulty userId";
                return false;
            }

            if (!CheckDayTicketsBuyDate(shoppingCart.DayTickets)) {
                return false;
            }

            if (!await CheckNoDayTicketsForMultipleMatchSameDay(shoppingCart.DayTickets, ownedDayTickets)) {
                return false;
            }

            if (!await CheckDayTicketQuantityAllowed(shoppingCart.DayTickets, ownedDayTickets)) {
                return false;
            }

            if (!await CheckDayTicketsAmount(shoppingCart.DayTickets)) {
                return false;
            }

            return true;
        }

        //Returns true if no duplicates found
        //Returns false otherwise
        private async Task<bool> CheckNoDuplicateSeasonTickets(List<SeasonTicketVM> seasonTicketVMs) {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (userId == null) {
                return false;
            }

            List<Ticket>? seasonTickets = await _ticketService.GetOwnedSeasonTicketsAsync(userId);

            if (seasonTickets == null) {
                return true;
            }

            // For each seasonticket in the shopping cart, check that the user does not yet have one for this club
            foreach (var cartTicket in seasonTicketVMs) {
                foreach (var ownedTicket in seasonTickets) {
                    var ownedHomeClub = ownedTicket.Section?.HomeTeamNavigation?.Name;
                    if (!string.IsNullOrEmpty(ownedHomeClub) && ownedHomeClub == cartTicket.HomeClubName) {
                        TempData["Error"] = $"You already have a season ticket for this club: {ownedHomeClub}. Please remove it from your cart.";
                        return false;
                    }
                }
            }
            return true;
        }

        //Returns true if season not yet started when trying to buy a seasonTicket
        //Returns false otherwise
        private bool CheckBeforeSeasonStart(List<SeasonTicketVM> seasonTicketVMs) {
            if (seasonTicketVMs.Count > 0 && SeasonTicketVM.SeasonStart <= DateOnly.FromDateTime(DateTime.Today)) {
                TempData["Error"] = "You can only buy season tickets before the start of the season. Please remove any season tickets from your cart.";
                return false;   
            }

            return true;
        }

        //Returns true if there is a seat available for this season ticket
        //Returns false otherwise
        private async Task<bool> CheckSeasonTicketSeatAvailable(List<SeasonTicketVM> seasonTicketVMs) {
            if (seasonTicketVMs.Count == 0) {
                return true; 
            }

            foreach (var seasonTicket in seasonTicketVMs) {
                var section = await _stadiumSectionService.FindByIdAsync(seasonTicket.SectionId);
                if (section == null) {
                    Console.WriteLine($"Invalid section id: {seasonTicket.SectionId}");
                    TempData["Error"] = $"Something went wrong with season ticket for {seasonTicket.HomeClubName}";
                    return false;
                }

                var seats = section.Seats;
                var maxDayTicket = await _ticketService.GetMaxDayTicketsBySectionAsync(seasonTicket.SectionId);
                var seasonTicketCount = await _ticketService.GetSeasonTicketCountBySectionAsync(seasonTicket.SectionId);

                // Next seat to be assigned to a season ticket is already assigned to one or more day tickets
                if (seats - seasonTicketCount <= maxDayTicket ) {
                    return false;
                }
            }

            return true;

            

        }

        //Returns true if there are no tickets for multiple matches on the same day
        //Returns false otherwise
        private async Task<bool> CheckNoDayTicketsForMultipleMatchSameDay(List<DayTicketVM> dayTicketVMs, List<Ticket> ownedDayTickets) {
            if (dayTicketVMs.Count == 0) {
                return true;
            }
            
            var cartTicketsByDate = dayTicketVMs
                .GroupBy(t => new { t.MatchDate, t.MatchId })
                .ToList();

            // Check within cart itself for multiple matches on the same day
            var datesInCart = cartTicketsByDate
                .GroupBy(g => g.Key.MatchDate)
                .Where(g => g.Count() > 1)
                .ToList();

            if (datesInCart.Count >0) {
                var conflictDate = datesInCart.First().Key;
                TempData["Error"] = $"You cannot buy tickets for different matches on {conflictDate}. Please remove one from your cart.";
                return false;
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
                            return false;
                        }
                    }
                }
            }

            return true;
        }

        //Returns true if the quantity of tickets that user is trying to buy is allowed.
        //Returns false otherwise
        private async Task<bool> CheckDayTicketQuantityAllowed(List<DayTicketVM> dayTicketVMs, List<Ticket> ownedDayTickets) {
            const int MAX_TICKETS = 4;
    
            if (dayTicketVMs.Count == 0) {
                return true;
            }

            // Group cart tickets by match and sum quantities
            var cartTicketsByMatch = dayTicketVMs
                .GroupBy(t => t.MatchId)
                .Select(g => new { 
                    MatchId = g.Key, 
                    Quantity = g.Sum(t => t.Quantity),
                    MatchDate = g.First().MatchDate
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
                    
                    TempData["Error"] = $"You cannot buy more than {MAX_TICKETS} tickets for the match on {cartGroup.MatchDate}. You already own {ownedCount} ticket(s) for this match, so you can only buy {availableTickets} more." +
                        $"Please remove some tickets for this date.";
                    return false;
                }
            }

            return true;
        }

        //Returns true if today is less than 1 month before match dates and before match dates
        //Returns false otherwise
        private bool CheckDayTicketsBuyDate(List<DayTicketVM> dayTicketVMs) {
            if (dayTicketVMs.Count > 0) {
                foreach (var ticket in dayTicketVMs) {
                    if (ticket.MatchDate > DateOnly.FromDateTime(DateTime.Today.AddMonths(1)) || ticket.MatchDate < DateOnly.FromDateTime(DateTime.Today)) {
                        TempData["Error"] = $"You can't buy tickets yet for the match on {ticket.MatchDate}";
                        return false;
                    }
                }
            }

            return true;

        }

        //Returns true if none of the tickets would mean the stadiumsection goes over capacity for that match
        //Returns false otherwise
        private async Task<bool> CheckDayTicketsAmount(List<DayTicketVM> dayTicketVMs) {
            if (dayTicketVMs.Count == 0) {
                return true;
            }

            var cartTicketsByMatchAndSection = dayTicketVMs
                .GroupBy(t => new { t.MatchId, t.SectionId })
                .Select(g => new {
                    g.Key.MatchId,
                    g.Key.SectionId,
                    Quantity = g.Sum(t => t.Quantity),
                    TicketInfo = g.First()
                })
                .ToList();

            var sectionIds = cartTicketsByMatchAndSection.Select(c => c.SectionId).Distinct().ToList();
            var sections = await _stadiumSectionService.FindByIdsAsync(sectionIds);
            var sectionLookup = sections.ToDictionary(s => s.Id);

            foreach (var cartGroup in cartTicketsByMatchAndSection) {
                sectionLookup.TryGetValue(cartGroup.SectionId, out var section);

                if (section == null) {
                    continue;
                }

                int seasonTicketCount = await _ticketService.GetSeasonTicketCountBySectionAsync(cartGroup.SectionId);
                int existingDayTickets = await _ticketService.GetDayTicketCountByMatchAndSectionAsync(cartGroup.MatchId, cartGroup.SectionId);

                int totalTickets = seasonTicketCount + existingDayTickets + cartGroup.Quantity;

                if (totalTickets > section.Seats) {
                    int availableSeats = section.Seats - seasonTicketCount - existingDayTickets;
                    TempData["Error"] = $"Not enough seats available in {cartGroup.TicketInfo.Ring} {cartGroup.TicketInfo.Location} for the match on {cartGroup.TicketInfo.MatchDate}. " +
                        $"Only {availableSeats} seat(s) available, but you're trying to buy {cartGroup.Quantity}. Please reduce the quantity.";
                    return false;
                }
            }


            return true;
        }
    }
}
