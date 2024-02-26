using EcommerceDotNetCore.Configurations;
using MailKit.Net.Smtp;
using Microsoft.Extensions.Options;
using MimeKit;

namespace EcommerceDotNetCore.Services.EmailService;

public class EmailService : IEmailService
{ 
    private readonly Smtp _smtp;
    public EmailService(IOptions<Smtp> smtp)
    {
        _smtp = smtp.Value;
    }

    public async Task SendEmailAsync(string to, string subject, string html, string? from = null)
    {
        var message = new MimeMessage();
        message.From.Add(MailboxAddress.Parse(from ?? _smtp.MailFromAddress));
        message.To.Add(MailboxAddress.Parse(to));
        message.Subject = subject;
        message.Body = new TextPart("html")
        {
            Text = html
        };
        using var client = new SmtpClient();
        await client.ConnectAsync(_smtp.MailHost, _smtp.MailPort, MailKit.Security.SecureSocketOptions.StartTls);
        await client.AuthenticateAsync(_smtp.MailUsername, _smtp.MailPassword);
        await client.SendAsync(message);
        await client.DisconnectAsync(true);
    }
}