using Microsoft.AspNetCore.Identity;

namespace Fathy.Common.Auth.User.Models;

public class AppUser : IdentityUser
{
    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;
    
    public List<RefreshToken>? RefreshTokens { get; set; }
}