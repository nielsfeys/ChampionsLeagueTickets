using ChampionsLeagueTickets.Models;
using MailKit.Net.Smtp;
using Microsoft.Extensions.Options;
using MimeKit;

namespace ChampionsLeagueTickets.Services;
public class EmailSender(IOptions<SmtpSettings> smtpSettings) : Interfaces.IEmailSender {
    private readonly SmtpSettings _smtpSettings = smtpSettings.Value;

    public async Task SendEmailAsync(string to, string subject, string htmlMessage) {
        try {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress(_smtpSettings.FromName, _smtpSettings.FromEmail));
            message.To.Add(new MailboxAddress("", to));
            message.Subject = subject;

            var bodyBuilder = new BodyBuilder {
                HtmlBody = htmlMessage
            };

            message.Body = bodyBuilder.ToMessageBody();

            using var client = new SmtpClient();
            await client.ConnectAsync(_smtpSettings.SmtpServer, _smtpSettings.SmtpPort, _smtpSettings.EnableSsl);
            await client.AuthenticateAsync(_smtpSettings.SmtpUsername, _smtpSettings.SmtpPassword);
            await client.SendAsync(message);
            await client.DisconnectAsync(true);
        } catch (Exception ex) {
            // Log the exception
            throw new Exception($"Failed to send email: {ex.Message}", ex);
        }
    }
  

    public async Task SendEmailWithAttachmentsAsync(string to, string subject, string htmlMessage, List<(byte[] Content, string FileName, string ContentType)>? attachments) {
        try {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress(_smtpSettings.FromName, _smtpSettings.FromEmail));
            message.To.Add(new MailboxAddress("", to));
            message.Subject = subject;

            var bodyBuilder = new BodyBuilder {
                HtmlBody = htmlMessage
            };

            if (attachments != null) {
                foreach (var attachment in attachments) {
                    bodyBuilder.Attachments.Add(attachment.FileName, attachment.Content, ContentType.Parse(attachment.ContentType));
                }
            }

            message.Body = bodyBuilder.ToMessageBody();

            using var client = new SmtpClient();
            await client.ConnectAsync(_smtpSettings.SmtpServer, _smtpSettings.SmtpPort, _smtpSettings.EnableSsl);
            await client.AuthenticateAsync(_smtpSettings.SmtpUsername, _smtpSettings.SmtpPassword);
            await client.SendAsync(message);
            await client.DisconnectAsync(true);
        } catch (Exception ex) {
            // Log the exception
            throw new Exception($"Failed to send email: {ex.Message}", ex);
        }
    }
}

