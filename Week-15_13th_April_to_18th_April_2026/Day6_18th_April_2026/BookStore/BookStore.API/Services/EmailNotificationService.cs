using System.Net;
using System.Net.Mail;
using Azure;
using Azure.Communication.Email;
using BookStore.API.Data;
using BookStore.API.Models;

namespace BookStore.API.Services;

public sealed class EmailNotificationService
{
    private const string ProviderAzureCommunication = "AzureCommunication";
    private const string ProviderSmtp = "Smtp";

    private readonly BookStoreDbContext _dbContext;
    private readonly IConfiguration _configuration;
    private readonly ILogger<EmailNotificationService> _logger;

    public EmailNotificationService(BookStoreDbContext dbContext, IConfiguration configuration, ILogger<EmailNotificationService> logger)
    {
        _dbContext = dbContext;
        _configuration = configuration;
        _logger = logger;
    }

    public async Task LogOrderConfirmationAsync(string toEmail, int orderId, CancellationToken cancellationToken = default)
    {
        var log = CreateQueuedLog(toEmail, $"Order Confirmation - #{orderId}", "Order confirmation queued.");
        _dbContext.EmailLogs.Add(log);

        await _dbContext.SaveChangesAsync(cancellationToken);
        await SendAsync(log, $"Your order #{orderId} was placed successfully.", cancellationToken);
    }

    public async Task LogInvoiceAsync(string toEmail, int orderId, CancellationToken cancellationToken = default)
    {
        var log = CreateQueuedLog(toEmail, $"Invoice - Order #{orderId}", "Invoice email queued.");
        _dbContext.EmailLogs.Add(log);

        await _dbContext.SaveChangesAsync(cancellationToken);
        await SendAsync(log, $"Your invoice for order #{orderId} is generated.", cancellationToken);
    }

    public async Task LogLowStockAsync(string toEmail, string bookTitle, int stock, CancellationToken cancellationToken = default)
    {
        var log = CreateQueuedLog(toEmail, "Low Stock Alert", $"Book '{bookTitle}' has low stock ({stock}).");
        _dbContext.EmailLogs.Add(log);

        await _dbContext.SaveChangesAsync(cancellationToken);
        await SendAsync(log, $"Book '{bookTitle}' has low stock ({stock}).", cancellationToken);
    }

    private static EmailLog CreateQueuedLog(string toEmail, string subject, string note) => new()
    {
        ToEmail = toEmail,
        Subject = subject,
        Status = "Queued",
        Note = note
    };

    private async Task SendAsync(EmailLog log, string content, CancellationToken cancellationToken)
    {
        var provider = _configuration["Email:Provider"] ?? ProviderAzureCommunication;

        try
        {
            if (string.Equals(provider, ProviderSmtp, StringComparison.OrdinalIgnoreCase))
            {
                await SendViaSmtpAsync(log, content, cancellationToken);
            }
            else
            {
                await SendViaAzureCommunicationAsync(log, content, cancellationToken);
            }

            log.Status = "Sent";
            log.Note = $"Sent successfully via {provider}.";
            await _dbContext.SaveChangesAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            log.Status = "Failed";
            log.Note = ex.Message.Length > 500 ? ex.Message[..500] : ex.Message;
            await _dbContext.SaveChangesAsync(cancellationToken);

            _logger.LogWarning(ex, "Email send failed for {Email} using provider {Provider}", log.ToEmail, provider);
        }
    }

    private async Task SendViaAzureCommunicationAsync(EmailLog log, string content, CancellationToken cancellationToken)
    {
        var connectionString = _configuration["AzureCommunication:ConnectionString"];
        var senderAddress = _configuration["AzureCommunication:SenderAddress"];

        if (string.IsNullOrWhiteSpace(connectionString) || string.IsNullOrWhiteSpace(senderAddress))
        {
            throw new InvalidOperationException("Azure Communication email configuration is missing.");
        }

        var client = new EmailClient(connectionString);
        var message = new EmailMessage(
            senderAddress,
            new EmailRecipients([new EmailAddress(log.ToEmail)]),
            new EmailContent(log.Subject)
            {
                PlainText = content
            });

        await client.SendAsync(WaitUntil.Completed, message, cancellationToken);
    }

    private async Task SendViaSmtpAsync(EmailLog log, string content, CancellationToken cancellationToken)
    {
        var host = _configuration["Smtp:Host"];
        var port = _configuration.GetValue<int?>("Smtp:Port");
        var username = _configuration["Smtp:Username"];
        var password = _configuration["Smtp:Password"];
        var fromAddress = _configuration["Smtp:FromAddress"];
        var enableSsl = _configuration.GetValue<bool?>("Smtp:EnableSsl") ?? true;

        if (string.IsNullOrWhiteSpace(host) || port is null || string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password) || string.IsNullOrWhiteSpace(fromAddress))
        {
            throw new InvalidOperationException("SMTP email configuration is missing.");
        }

        using var client = new SmtpClient(host, port.Value)
        {
            EnableSsl = enableSsl,
            Credentials = new NetworkCredential(username, password)
        };

        using var message = new MailMessage(fromAddress, log.ToEmail, log.Subject, content)
        {
            IsBodyHtml = false
        };

        await client.SendMailAsync(message, cancellationToken);
    }
}
