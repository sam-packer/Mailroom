using System.Configuration;
using System.Net;
using System.Net.Mail;
using System.Text;
using Microsoft.AspNetCore.Mvc;

namespace Mailroom;

public class EmailUtil
{
    private IConfiguration _configuration;

    public EmailUtil(IConfiguration configuration)
    {
        configuration = new ConfigurationBuilder()
            .AddUserSecrets<Program>()
            .Build();
        _configuration = configuration;
    }

    public async Task SendEmail(string to)
    {
        try
        {
            MailMessage message = new MailMessage();
            message.From = new MailAddress(_configuration["Mail:From"]);
            message.To.Add(to);
            message.Subject = "Package Available for Pickup";
            string htmlContent = @"
            <html>
                <body>
                    <p>Dear Resident,</p>
                    <p>We have received a package for you at the leasing office. Due to limited storage space, please pick up your package within <strong>5 days</strong>.</p>
                    <p>If the package is not picked up within this time frame, it will be returned to the post office.</p>
                    <p>Thank you!</p>
                    <p>Apartment Leasing Office</p>
                </body>
            </html>
            ";
            message.Body = htmlContent;
            message.IsBodyHtml = true;
            SmtpClient smtpClient =
                new SmtpClient(_configuration["Mail:Server"], int.Parse(_configuration["Mail:Port"]));
            smtpClient.EnableSsl = true;
            smtpClient.UseDefaultCredentials = false;
            smtpClient.Credentials =
                new NetworkCredential(_configuration["Mail:Username"], _configuration["Mail:Password"]);
            await smtpClient.SendMailAsync(message);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Failed to send email: {ex.Message}");
        }
    }
}