using System.Globalization;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Fathy.Common.Auth.JWTGenerator.Utilities;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;

namespace Fathy.Common.Auth.JWTGenerator.Repositories;

public class JwtGeneratorRepository : IJwtGeneratorRepository
{
    private readonly UserManager<IdentityUser> _userManager;

    public JwtGeneratorRepository(UserManager<IdentityUser> userManager)
    {
        _userManager = userManager;
    }

    public async Task<string> GenerateJwtSecurityToken(IdentityUser user)
    {
        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Email, user.Email),
            new(JwtRegisteredClaimNames.Sub, user.UserName),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new(JwtRegisteredClaimNames.Iat, DateTime.UtcNow.ToString(CultureInfo.InvariantCulture))
        };

        var roles = await _userManager.GetRolesAsync(user);

        claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));
        claims.AddRange(await _userManager.GetClaimsAsync(user));

        return new JwtSecurityTokenHandler().WriteToken(new JwtSecurityToken(
            issuer: JwtParameters.ValidIssuer,
            audience: JwtParameters.ValidAudience,
            claims: claims,
            expires: DateTime.UtcNow.AddDays(30),
            signingCredentials: new SigningCredentials(JwtParameters.IssuerSigningKey,
                SecurityAlgorithms.HmacSha256)
        ));
    }
}