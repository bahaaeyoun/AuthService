using System.ComponentModel.DataAnnotations;

namespace Fathy.Common.Auth.User.DTOs;

public class UserDto
{
    [Required] public string Email { get; init; } = null!;
    [Required] public string Password { get; init; } = null!;
}