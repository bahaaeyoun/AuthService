using System.Net;
using System.Net.Mail;
using Fathy.Common.Startup;
using Microsoft.Extensions.Configuration;

namespace Fathy.Common.Email;

public class EmailRepository : IEmailRepository
{
    private readonly string _from;
    private readonly SmtpClient _smtpClient;

    public EmailRepository(IConfiguration configuration)
    {
        _smtpClient = new SmtpClient
        {
            Host = configuration.GetValue<string>("SmtpClient:Host"),
            Port = configuration.GetValue<int>("SmtpClient:Port"),
            EnableSsl = configuration.GetValue<bool>("SmtpClient:EnableSsl"),
            Credentials = new NetworkCredential(
                configuration.GetValue<string>("SmtpClient:Credentials:UserName"),
                configuration.GetValue<string>("SmtpClient:Credentials:Password"))
        };
        _from = configuration.GetValue<string>("SmtpClient:Credentials:UserName");
    }

    public async Task<Result> SendAsync(Message message)
    {
        var mailMessage = new MailMessage(_from, message.To,
            message.Subject, message.Body)
        {
            IsBodyHtml = message.IsBodyHtml
        };

        try
        {
            await _smtpClient.SendMailAsync(mailMessage);
            return Result.Success();
        }
        catch (Exception exception)
        {
            return Result.Failure(new[] { new Error("SendEmailFailed", exception.Message) });
        }
    }
}