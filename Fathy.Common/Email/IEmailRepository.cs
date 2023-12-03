using Fathy.Common.Startup;

namespace Fathy.Common.Email;

public interface IEmailRepository
{
    Task<Result> SendAsync(Message message);
}