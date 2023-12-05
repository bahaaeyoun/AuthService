using Fathy.Common.Auth.Email.Utilities;
using Fathy.Common.Startup;

namespace Fathy.Common.Auth.Email.Repositories;

public interface IEmailRepository
{
    Task<Result> SendAsync(Message message);
}