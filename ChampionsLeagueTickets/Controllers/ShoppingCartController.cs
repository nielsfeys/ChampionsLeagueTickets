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
    public class ShoppingCartController(ITicketService ticketService, IMapper mapper, IEmailSender emailSender) : Controller {
        private readonly ITicketService _ticketService = ticketService;
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
            List<Ticket> ticketList = MakeTicketList();

            try {
                await _ticketService.AddListAsync(ticketList);
            } catch(Exception) {
                TempData["Error"] = "An error occurred while processing your order. Please try again.";
                return RedirectToAction("Index");
            }

            await SendEmail(ticketList);
            
            HttpContext.Session.Remove("ShoppingCart");
            return RedirectToAction("Index");
        }

        private List<Ticket> MakeTicketList() {
            ShoppingCartVM shoppingCart = HttpContext.Session.GetObject<ShoppingCartVM>("ShoppingCart") ?? new ShoppingCartVM();

            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var options = new Action<IMappingOperationOptions>(opts => opts.Items["UserId"] = userId);

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
                var homeClub = ticket.Section?.HomeTeamNavigation?.Name ?? "Unknown";
                var ring = ticket.Section?.Ring ?? "Unknown";
                var location = ticket.Section?.Ring ?? "Unknown";
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

    }
}
