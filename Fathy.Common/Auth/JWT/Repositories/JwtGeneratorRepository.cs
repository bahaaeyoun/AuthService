using System.Globalization;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Fathy.Common.Auth.JWT.Utilities;
using Fathy.Common.Auth.User.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;

namespace Fathy.Common.Auth.JWT.Repositories;

public class JwtGeneratorRepository(UserManager<AppUser> userManager) : IJwtGeneratorRepository
{
    public async Task<string> GenerateJwtSecurityTokenAsync(AppUser user)
    {
        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Email, user.Email),
            new(JwtRegisteredClaimNames.Sub, user.UserName),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new(JwtRegisteredClaimNames.Iat, DateTime.UtcNow.ToLocalTime().ToString(CultureInfo.InvariantCulture))
        };

        var roles = await userManager.GetRolesAsync(user);

        claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));
        claims.AddRange(await userManager.GetClaimsAsync(user));

        return new JwtSecurityTokenHandler().WriteToken(new JwtSecurityToken(
            issuer: JwtParameters.ValidIssuer,
            audience: JwtParameters.ValidAudience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(1),
            signingCredentials: new SigningCredentials(JwtParameters.IssuerSigningKey,
                SecurityAlgorithms.HmacSha256)
        ));
    }
}