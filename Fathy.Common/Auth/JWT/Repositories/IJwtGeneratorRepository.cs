using Fathy.Common.Auth.User.Models;

namespace Fathy.Common.Auth.JWT.Repositories;

public interface IJwtGeneratorRepository
{
    Task<string> GenerateJwtSecurityTokenAsync(AppUser user);
}