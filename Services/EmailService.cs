using BlogApi.ServiceContracts;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;
using MimeKit.Text;
using System.Security.Authentication;

namespace BlogApi.Services {
    public class EmailService : IEmailService {
        private readonly EmailOptions _emailOptions;

        public EmailService(IOptions<EmailOptions> emailOptions)
        {
            _emailOptions = emailOptions.Value;
        }
        public async Task SendActivationEmail(string toEmail, string token, DateTime tokenExpiration) {
            var email = new MimeMessage();
            email.From.Add(MailboxAddress.Parse(_emailOptions.From));
            email.To.Add(MailboxAddress.Parse(toEmail));
            email.Subject = "Activate Your Account";
            email.Body = new TextPart(TextFormat.Html) { Text = EmailTemplate.ActivateTemplate(token, tokenExpiration) };

            await _sendEmail(email);
        }

        public async Task SendResetPassowrdEmail(string toEmail, string token, DateTime tokenExpiration) {
            var email = new MimeMessage();
            email.From.Add(MailboxAddress.Parse(_emailOptions.From));
            email.To.Add(MailboxAddress.Parse(toEmail));
            email.Subject = "Reset Passowrd";
            email.Body = new TextPart(TextFormat.Html) { Text = EmailTemplate.ResetPasswordTemplate(token, tokenExpiration) };
            await _sendEmail(email);
            
        }

        private async Task _sendEmail(MimeMessage email) {
            using var smtpClient = new SmtpClient();
            smtpClient.SslProtocols = SslProtocols.Ssl3 | SslProtocols.Tls | SslProtocols.Tls11 | SslProtocols.Tls12 | SslProtocols.Tls13;
            smtpClient.CheckCertificateRevocation = false;
            await smtpClient.ConnectAsync(_emailOptions.Smtp, _emailOptions.Port, SecureSocketOptions.Auto);
            await smtpClient.AuthenticateAsync(_emailOptions.Username, _emailOptions.Password);
            await smtpClient.SendAsync(email);
            await smtpClient.DisconnectAsync(true);
        }
    }
}
