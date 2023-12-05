using Microsoft.IdentityModel.Tokens;

namespace Fathy.Common.Auth.User.Utilities;

public static class JwtParameters
{
    public const string ValidAudience = "User";
    public const string ValidIssuer = "DemoAuthService";
    
    public static SymmetricSecurityKey IssuerSigningKey => new(key: "xm7zdYq8KIlFftHJ6NCLYOarogKX41qv"u8.ToArray());
}