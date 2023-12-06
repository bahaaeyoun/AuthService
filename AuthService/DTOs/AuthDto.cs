namespace AuthService.DTOs;

public class AuthDto
{
    public string RefreshToken { get; set; } = null!;
    public DateTime RefreshTokenExpiresAt { get; set; }
    public string JwtSecurityToken { get; set; } = null!;
}