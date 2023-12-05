namespace Fathy.Common.Auth.User.DTOs;

public class AuthDto
{
    public string Token { get; set; } = null!;
    public string? RefreshToken { get; set; }
}