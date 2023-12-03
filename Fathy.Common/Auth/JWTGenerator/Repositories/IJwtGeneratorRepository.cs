using Microsoft.AspNetCore.Identity;

namespace Fathy.Common.Auth.JWTGenerator.Repositories;

public interface IJwtGeneratorRepository
{
    Task<string> GenerateJwtSecurityToken(IdentityUser user);
}