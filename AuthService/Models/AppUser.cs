using Microsoft.AspNetCore.Identity;

namespace AuthService.Models;

public class AppUser : IdentityUser
{
    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;

    public List<RefreshToken> RefreshTokens { get; } = new();
}