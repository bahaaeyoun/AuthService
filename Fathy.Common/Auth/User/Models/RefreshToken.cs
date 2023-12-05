using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace Fathy.Common.Auth.User.Models;

[Owned]
public class RefreshToken
{
    [Key]
    public int Id { get; set; }
    
    public string Token { get; set; } = null!;
    public DateTime CreatedOn { get; set; }
    
    public DateTime ExpiresIn { get; set; }
    public bool IsExpired => DateTime.UtcNow >= ExpiresIn;

    public DateTime? RevokedOn { get; set; }
    public bool IsActive => RevokedOn == null && !IsExpired;
}