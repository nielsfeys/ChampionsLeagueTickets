namespace ChampionsLeagueTickets.Services.Interfaces;

public interface IEmailSender : Microsoft.AspNetCore.Identity.UI.Services.IEmailSender {
    Task SendEmailWithAttachmentsAsync(string to, string subject, string htmlMessage, List<(byte[] Content, string FileName, string ContentType)>? attachments);
}