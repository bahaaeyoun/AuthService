using Fathy.Common.Auth.User.DTOs;
using Fathy.Common.Startup;

namespace Fathy.Common.Auth.User.Repositories;

public interface IUserRepository
{
    Task<Result> ConfirmEmailAsync(string userEmail, string token);
    Task<Result> CreateAsync(UserDto userDto);
    Task<Result> DeleteAsync(UserDto userDto);
    Task<Result> SendConfirmationEmailAsync(string userEmail);
    Task<Result> SignInAsync(UserDto userDto);
}