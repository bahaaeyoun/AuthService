namespace Fathy.Common.Auth.Email.Utilities;

public class Message(string to, string subject, string body)
{
    public string To { get; } = to;
    public string Subject { get; } = subject;
    public string Body { get; } = body;
    public bool IsBodyHtml { get; init; }
}