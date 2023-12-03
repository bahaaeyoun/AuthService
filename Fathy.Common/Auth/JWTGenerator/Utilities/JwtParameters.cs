using Microsoft.IdentityModel.Tokens;

namespace Fathy.Common.Auth.JWTGenerator.Utilities;

public static class JwtParameters
{
    public const string ValidIssuer = "Fathy.Common";
    public const string ValidAudience = "User";

    public static readonly SymmetricSecurityKey IssuerSigningKey = new(key: "xm7zdYq8KIlFftHJ6NCLYOarogKX41qv"u8.ToArray());
}