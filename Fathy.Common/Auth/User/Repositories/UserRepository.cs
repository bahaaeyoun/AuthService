using System.Web;
using Fathy.Common.Auth.JWTGenerator.Repositories;
using Fathy.Common.Auth.User.DTOs;
using Fathy.Common.Email;
using Fathy.Common.Startup;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;

namespace Fathy.Common.Auth.User.Repositories;

public class UserRepository : IUserRepository
{
    private readonly IConfiguration _configuration;
    private readonly IEmailRepository _emailRepository;
    private readonly UserManager<IdentityUser> _userManager;
    private readonly SignInManager<IdentityUser> _signInManager;
    private readonly IJwtGeneratorRepository _jwtGeneratorRepository;

    public UserRepository(UserManager<IdentityUser> userManager, IConfiguration configuration,
        SignInManager<IdentityUser> signInManager,
        IEmailRepository emailRepository, IJwtGeneratorRepository jwtGeneratorRepository)
    {
        _userManager = userManager;
        _configuration = configuration;
        _signInManager = signInManager;
        _emailRepository = emailRepository;
        _jwtGeneratorRepository = jwtGeneratorRepository;
    }

    public async Task<Result> ConfirmEmailAsync(string userEmail, string token)
    {
        var user = await _userManager.FindByEmailAsync(userEmail);
        return user is null
            ? Result.Failure(new[] { ErrorsList.UserEmailNotFound(userEmail) })
            : (await _userManager.ConfirmEmailAsync(user, token)).ToApplicationResult();
    }

    public async Task<Result> CreateAsync(UserDto userDto)
    {
        var createResult = await _userManager.CreateAsync(
            new IdentityUser(userDto.Email) { Email = userDto.Email }, userDto.Password);
        return createResult.Succeeded
            ? await SendConfirmationEmailAsync(userDto.Email)
            : createResult.ToApplicationResult();
    }

    public async Task<Result> DeleteAsync(UserDto userDto)
    {
        var user = await _userManager.FindByEmailAsync(userDto.Email);
        if (user is null) return Result.Failure(new[] { ErrorsList.UserEmailNotFound(userDto.Email) });

        var passwordSignInResult = await PasswordSignInAsync(user, userDto.Password);
        return passwordSignInResult.Succeeded
            ? (await _userManager.DeleteAsync(user)).ToApplicationResult()
            : passwordSignInResult;
    }

    private async Task<Result> PasswordSignInAsync(IdentityUser user, string password)
    {
        var passwordSignInAsyncResult = await _signInManager.PasswordSignInAsync(user, password, true, false);

        if (passwordSignInAsyncResult.IsNotAllowed)
            return Result.Failure(new[] { ErrorsList.SignInNotAllowed() });

        return passwordSignInAsyncResult.Succeeded == false
            ? Result.Failure(new[] { ErrorsList.SignInFailed() })
            : Result.Success();
    }

    public async Task<Result> SendConfirmationEmailAsync(string userEmail)
    {
        var user = await _userManager.FindByEmailAsync(userEmail);
        if (user is null) return Result.Failure(new[] { ErrorsList.UserEmailNotFound(userEmail) });

        var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);

        var confirmEmailUri =
            $"{_configuration.GetValue<string>("ConfirmEmailEndpoint")}?userEmail={userEmail}&token={HttpUtility.UrlEncode(token)}";

        var body = "<h1>Welcome</h1><br>" +
                   "<p> Thanks for registering please click " +
                   $"<strong><a href=\"{confirmEmailUri}\" target=\"_blank\">here</a></strong>" +
                   " to confirm your email</p>";

        var message = new Message(userEmail, "Confirmation Email", body)
        {
            IsBodyHtml = true
        };

        return await _emailRepository.SendAsync(message);
    }

    public async Task<Result> SignInAsync(UserDto userDto)
    {
        var user = await _userManager.FindByEmailAsync(userDto.Email);
        if (user is null) return Result.Failure(new[] { ErrorsList.UserEmailNotFound(userDto.Email) });

        var passwordSignInResult = await PasswordSignInAsync(user, userDto.Password);
        return !passwordSignInResult.Succeeded
            ? Result.Failure(passwordSignInResult.Errors)
            : Result<string>.Success(await _jwtGeneratorRepository.GenerateJwtSecurityToken(user));
    }
}