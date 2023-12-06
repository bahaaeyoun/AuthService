using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace AuthService.Models;

[Owned]
public class RefreshToken
{
    [Key]
    public int Id { get; init; }
    
    public string Token { get; init; } = null!;
    public DateTime ExpiresAt { get; init; }
    public DateTime? RevokedAt { get; set; }
    
    public bool IsActive => RevokedAt == null && ExpiresAt < DateTime.UtcNow;
}