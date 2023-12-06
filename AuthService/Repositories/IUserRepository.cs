using AuthService.DTOs;

namespace AuthService.Repositories;

public interface IUserRepository
{
    Task<Result> ConfirmEmailAsync(string userEmail, string token);
    Task<Result> CreateAsync(UserDto userDto);
    Task<Result> DeleteAsync(SignInDto signInDto);
    Task<Result<AuthDto>> NewRefreshTokenAsync(string refreshToken);
    Task<Result> RevokeRefreshTokenAsync(string refreshToken);
    Task<Result> SendConfirmationEmailAsync(string userEmail);
    Task<Result<AuthDto>> SignInAsync(SignInDto signInDto);
}