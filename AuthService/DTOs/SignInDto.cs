using System.ComponentModel.DataAnnotations;

namespace AuthService.DTOs;

public class SignInDto
{
    [Required(ErrorMessage = "Email is required."), EmailAddress] public string Email { get; init; } = null!;
    [Required(ErrorMessage = "Password is required.")] public string Password { get; init; } = null!;
}