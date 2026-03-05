using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using MailKit.Net.Smtp;
using MimeKit;
using Microsoft.Extensions.Options;
using ChampionsLeagueTickets.Services.Interfaces;
using ChampionsLeagueTickets.Models;
using Microsoft.AspNetCore.Identity.UI.Services;

namespace ChampionsLeagueTickets.Services;
public class EmailSender : IEmailSender {
    private readonly SmtpSettings _smtpSettings;

    public EmailSender(IOptions<SmtpSettings> smtpSettings) {
        _smtpSettings = smtpSettings.Value;
    }

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
}

