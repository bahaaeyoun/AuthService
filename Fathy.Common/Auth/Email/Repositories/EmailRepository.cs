using System.Net;
using System.Net.Mail;
using Fathy.Common.Auth.Email.Utilities;
using Fathy.Common.Startup;
using Microsoft.Extensions.Configuration;

namespace Fathy.Common.Auth.Email.Repositories;

public class EmailRepository(IConfiguration configuration) : IEmailRepository
{
    private readonly string _from = configuration.GetValue<string>("SmtpClient:Credentials:UserName") ?? string.Empty;
    private readonly SmtpClient _smtpClient = new()
    {
        Host = configuration.GetValue<string>("SmtpClient:Host") ?? string.Empty,
        Port = configuration.GetValue<int>("SmtpClient:Port"),
        EnableSsl = configuration.GetValue<bool>("SmtpClient:EnableSsl"),
        Credentials = new NetworkCredential(
            configuration.GetValue<string>("SmtpClient:Credentials:UserName"),
            configuration.GetValue<string>("SmtpClient:Credentials:Password"))
    };

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