using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Options;
using SendGrid;
using SendGrid.Helpers.Mail;

namespace SiteWebJO2.Services
{

    /// <summary>
    /// Implement IEmailSender class
    /// code from 
    /// https://learn.microsoft.com/en-us/aspnet/core/security/authentication/accconfirm?view=aspnetcore-8.0&tabs=visual-studio
    /// TODO : change email address to an email address with the domain name
    /// </summary>
    public class EmailSender : IEmailSender
    {
        private readonly ILogger _logger;

        public EmailSender(IOptions<AuthMessageSenderOptions> optionsAccessor,
                           ILogger<EmailSender> logger)
        {
            Options = optionsAccessor.Value;
            _logger = logger;
        }

        public AuthMessageSenderOptions Options { get; } //Set with Secret Manager.


        /// <summary>
        /// add API key to send email
        /// </summary>
        /// <param name="toEmail">destination email</param>
        /// <param name="subject">email subject</param>
        /// <param name="message">email message</param>
        /// <returns>Task to execute email sending</returns>
        /// <exception cref="Exception">exception thrown if SendGrid key is unfound</exception>
        public async Task SendEmailAsync(string toEmail, string subject, string message)
        {
            if (string.IsNullOrEmpty(Options.SendGridKey))
            {
                throw new Exception("Null SendGridKey");
            }
            await Execute(Options.SendGridKey, subject, message, toEmail);
        }

        /// <summary>
        /// Send email. Only used for Identity purposes
        /// </summary>
        /// <param name="apiKey">SendGrid API key</param>
        /// <param name="subject">email subject</param>
        /// <param name="message">email message</param>
        /// <param name="toEmail">destination email</param>
        /// <returns>Task to send email</returns>
        public async Task Execute(string apiKey, string subject, string message, string toEmail)
        {
            var client = new SendGridClient(apiKey);
            var msg = new SendGridMessage()
            {
                From = new EmailAddress("gevie46000@gmail.com", "Jo2024Tickets.fly.dev"),
                Subject = subject,
                PlainTextContent = message,
                HtmlContent = message
            };

            msg.AddTo(new EmailAddress(toEmail));

            // Disable click tracking.
            // See https://sendgrid.com/docs/User_Guide/Settings/tracking.html
            msg.SetClickTracking(false, false);
            var response = await client.SendEmailAsync(msg);
            _logger.LogInformation(response.IsSuccessStatusCode
                                   ? $"Email to {toEmail} queued successfully!"
                                   : $"Failure Email to {toEmail}");
        }
    }
}
