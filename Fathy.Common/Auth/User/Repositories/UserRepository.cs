using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using System.Web;
using Fathy.Common.Auth.Email.Repositories;
using Fathy.Common.Auth.Email.Utilities;
using Fathy.Common.Auth.JWT.Repositories;
using Fathy.Common.Auth.User.DTOs;
using Fathy.Common.Auth.User.Models;
using Fathy.Common.Startup;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;

namespace Fathy.Common.Auth.User.Repositories;

public class UserRepository(UserManager<AppUser> userManager, IConfiguration configuration,
        SignInManager<AppUser> signInManager, IHttpContextAccessor httpContextAccessor,
        IEmailRepository emailRepository, IJwtGeneratorRepository jwtGeneratorRepository)
    : IUserRepository
{
    public string CurrentUserEmail =>
        httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier) ?? string.Empty;

    public async Task<Result> ConfirmEmailAsync(string userEmail, string token)
    {
        var user = await userManager.FindByEmailAsync(userEmail);
        
        return user is null
            ? Result.Failure(new[] { ErrorsList.UserEmailNotFound() })
            : (await userManager.ConfirmEmailAsync(user, token)).ToApplicationResult();
    }

    public async Task<Result> CreateAsync(UserDto userDto)
    {
        var createResult = await userManager.CreateAsync(
            new AppUser { UserName = userDto.Email, Email = userDto.Email }, userDto.Password);
        
        return createResult.Succeeded
            ? await SendConfirmationEmailAsync(userDto.Email)
            : createResult.ToApplicationResult();
    }

    public async Task<Result> DeleteAsync(SignInDto signInDto)
    {
        var user = await userManager.FindByEmailAsync(signInDto.Email);
        if (user is null) return Result.Failure(new[] { ErrorsList.UserEmailNotFound() });
        
        return await userManager.CheckPasswordAsync(user, signInDto.Password)
            ? (await userManager.DeleteAsync(user)).ToApplicationResult()
            : Result.Failure(new[] { ErrorsList.SignInFailed() });
    }

    public async Task<Result> SendConfirmationEmailAsync(string userEmail)
    {
        var user = await userManager.FindByEmailAsync(userEmail);
        if (user is null) return Result.Failure(new[] { ErrorsList.UserEmailNotFound() });

        var token = await userManager.GenerateEmailConfirmationTokenAsync(user);

        var confirmEmailUri =
            $"{configuration.GetValue<string>("ConfirmEmailEndpoint")}?userEmail={userEmail}&token={HttpUtility.UrlEncode(token)}";

        var body = "<h1>Welcome</h1><br>" +
                   "<p> Thanks for registering please click " +
                   $"<strong><a href=\"{confirmEmailUri}\" target=\"_blank\">here</a></strong>" +
                   " to confirm your email</p>";

        var message = new Message(userEmail, "Confirmation Email", body)
        {
            IsBodyHtml = true
        };

        return await emailRepository.SendAsync(message);
    }

    public async Task<Result<AuthDto>> SignInAsync(SignInDto signInDto)
    {
        var user = await userManager.FindByEmailAsync(signInDto.Email);
        if (user is null) return Result<AuthDto>.Failure(new[] { ErrorsList.UserEmailNotFound() });

        var passwordSignInResult = await signInManager.PasswordSignInAsync(user, signInDto.Password, true, false);

        if (passwordSignInResult.IsNotAllowed)
            return Result<AuthDto>.Failure(new[] { ErrorsList.SignInForbidden() });

        if (!passwordSignInResult.Succeeded)
            return Result<AuthDto>.Failure(new[] { ErrorsList.SignInFailed() });

        var jwtSecurityToken = await jwtGeneratorRepository.GenerateJwtSecurityTokenAsync(user);
        return Result<AuthDto>.Success(new AuthDto { Token = jwtSecurityToken });
    }

    private RefreshToken GenerateRefreshToken() => new()
    {
        Token = Convert.ToBase64String(Guid.NewGuid().ToByteArray()),
        ExpiresIn = DateTime.UtcNow.AddDays(10),
        CreatedOn = DateTime.UtcNow
    };
}