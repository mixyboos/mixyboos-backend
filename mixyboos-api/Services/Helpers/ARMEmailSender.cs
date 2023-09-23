using System;
using System.Threading.Tasks;
using Azure;
using Microsoft.AspNetCore.Identity.UI.Services;
using Azure.Communication.Email;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace MixyBoos.Api.Services.Helpers;

public class ARMMailSender : IEmailSender {
  private readonly IConfiguration _config;
  private readonly ILogger<ARMMailSender> _logger;

  public ARMMailSender(IConfiguration config, ILogger<ARMMailSender> logger) {
    _config = config;
    _logger = logger;
  }

  public async Task SendEmailAsync(string email, string subject, string htmlMessage) {
    try {
      var connectionString = _config["Email:ARMKey"];
      if (string.IsNullOrEmpty(connectionString)) {
        throw new InvalidOperationException("ARM key is missing");
      }

      var client = new EmailClient(connectionString);
      var operation = await client.SendAsync(
        wait: WaitUntil.Completed,
        senderAddress: _config["Email:FromAddress"].ToString(),
        recipientAddress: email,
        subject: subject,
        htmlContent: htmlMessage
      );
      _logger.LogDebug(
        "Send email to {Email} with subject {Subject} with tracking id {TrackingId}",
        email,
        subject,
        operation?.Id);
    } catch (Exception e) {
      _logger.LogError("Error sending email {Error}", e.Message);
    }
  }
}
