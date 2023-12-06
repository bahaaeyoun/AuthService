using System.ComponentModel.DataAnnotations;

namespace AuthService.DTOs;

public class UserDto
{
    [Required] public string FirstName { get; init; } = null!;
    [Required] public string LastName { get; init; } = null!;
    
    [Required, EmailAddress] public string Email { get; init; } = null!;
    [Required] public string Password { get; init; } = null!;
}