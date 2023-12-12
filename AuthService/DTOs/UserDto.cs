using System.ComponentModel.DataAnnotations;

namespace AuthService.DTOs;

public class UserDto
{
    [Required(ErrorMessage = "First name is required.")] public string FirstName { get; init; } = null!;
    [Required(ErrorMessage = "Last name is required.")] public string LastName { get; init; } = null!;
    
    [Required(ErrorMessage = "Email is required."), EmailAddress] public string Email { get; init; } = null!;
    [Required(ErrorMessage = "Password is required.")] public string Password { get; init; } = null!;
}