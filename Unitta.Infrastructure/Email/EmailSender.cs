using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using SendGrid.Helpers.Mail;
using Unitta.Application.Interfaces;
namespace Unitta.Infrastructure.Email;

public class EmailSender : IEmailSender
{
    public string _secretKey { get; set; }
    private readonly ILogger<EmailSender> _logger;
    public EmailSender(IConfiguration _config, ILogger<EmailSender> logger)
    {
        _secretKey = _config["SendGrid:SecretKey"] ?? string.Empty;
        _logger = logger;
    }
    public async Task SendEmailAsync(string to, string subject, string body)
    {
        var client = new SendGrid.SendGridClient(_secretKey);
        var from = new EmailAddress("ziadmuhammed422@gmail.com", "Unitta");
        var toEmail = new EmailAddress(to);
        var msg = MailHelper.CreateSingleEmail(from, toEmail, subject, "", body);

        _logger.LogInformation($"Sending Email to: {to}");
        _logger.LogInformation($"Subject: {subject}");
        _logger.LogInformation($"Body: {body}");
        var response = await client.SendEmailAsync(msg);

        if (response.IsSuccessStatusCode)
        {
            _logger.LogInformation("Email to {To} queued successfully!", to);
        }
        else
        {
            _logger.LogError("Failed to send email. Status Code: {StatusCode}. Reason: {Reason}",
                response.StatusCode,
                await response.Body.ReadAsStringAsync());
        }
    }
}
