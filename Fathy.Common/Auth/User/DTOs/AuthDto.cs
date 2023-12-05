namespace Fathy.Common.Auth.User.DTOs;

public class AuthDto
{
    public string RefreshToken { get; set; } = null!;
    public DateTime RefreshTokenExpiresIn { get; set; }
    public string JwtSecurityToken { get; set; } = null!;
}