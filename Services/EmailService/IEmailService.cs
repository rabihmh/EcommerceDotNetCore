namespace EcommerceDotNetCore.Services.EmailService
{
    public interface IEmailService
    {
        public Task SendEmailAsync(string to, string subject, string html, string? from = null);
    }
}
