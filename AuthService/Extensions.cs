using System.Net.Mail;
using Microsoft.AspNetCore.Identity;
using Newtonsoft.Json;

namespace AuthService;

public static class Extensions
{
    public static Result ToResult(this IdentityResult identityResult)
    {
        var errors =
            identityResult.Errors.Select(identityError =>
                new Error(identityError.Code, identityError.Description));
        return new Result(identityResult.Succeeded, errors);
    }
    
    public static string ToJsonString<T>(this T data) => JsonConvert.SerializeObject(data);

    public static async Task<Result> SendAsync(this MailMessage mailMessage, SmtpClient smtpClient)
    {
        try
        {
            await smtpClient.SendMailAsync(mailMessage);
            return Result.Success();
        }
        catch (Exception exception)
        {
            return Result.Failure(new[] { new Error("SendEmailFailed", exception.Message) });
        }
    }
}