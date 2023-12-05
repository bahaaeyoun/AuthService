using System.ComponentModel.DataAnnotations;

namespace Fathy.Common.Auth.User.DTOs;

public class SignInDto
{
    [Required] public string Password { get; init; } = null!;
    [Required, EmailAddress] public string Email { get; init; } = null!;
}