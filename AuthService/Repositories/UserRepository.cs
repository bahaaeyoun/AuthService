using System.Globalization;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Net.Mail;
using System.Security.Claims;
using System.Text;
using System.Web;
using AuthService.DTOs;
using AuthService.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace AuthService.Repositories;

public class UserRepository(UserManager<AppUser> userManager, IConfiguration configuration,
    SignInManager<AppUser> signInManager) : IUserRepository
{
    private readonly string _from =
        configuration.GetValue<string>("SmtpClient:Credentials:UserName") ?? string.Empty;

    private readonly SmtpClient _smtpClient = new()
    {
        Host = configuration.GetValue<string>("SmtpClient:Host") ?? string.Empty,
        Port = configuration.GetValue<int>("SmtpClient:Port"),
        EnableSsl = configuration.GetValue<bool>("SmtpClient:EnableSsl"),
        Credentials = new NetworkCredential(
            configuration.GetValue<string>("SmtpClient:Credentials:UserName"),
            configuration.GetValue<string>("SmtpClient:Credentials:Password"))
    };

    public async Task<Result> ConfirmEmailAsync(string userEmail, string token)
    {
        var user = await userManager.FindByEmailAsync(userEmail);

        return user is null
            ? Result.Failure(new[] { Error.UserEmailNotFound() })
            : (await userManager.ConfirmEmailAsync(user, token)).ToResult();
    }

    public async Task<Result> CreateAsync(UserDto userDto)
    {
        var createResult = await userManager.CreateAsync(new AppUser
        {
            UserName = userDto.Email,
            Email = userDto.Email,
            FirstName = userDto.FirstName,
            LastName = userDto.LastName
        }, userDto.Password);

        return createResult.Succeeded
            ? await SendConfirmationEmailAsync(userDto.Email)
            : createResult.ToResult();
    }

    public async Task<Result> DeleteAsync(SignInDto signInDto)
    {
        var user = await userManager.FindByEmailAsync(signInDto.Email);

        return user is null || !await userManager.CheckPasswordAsync(user, signInDto.Password)
            ? Result.Failure(new[] { Error.WrongEmailOrPassword() })
            : (await userManager.DeleteAsync(user)).ToResult();
    }

    public async Task<Result<AuthDto>> NewRefreshTokenAsync(string refreshToken)
    {
        var user = await userManager.Users.SingleOrDefaultAsync(user =>
            user.RefreshTokens.Any(token => token.Token == refreshToken));

        if (user is null)
            return Result<AuthDto>.Failure(new[] { Error.InvalidRefreshToken() });

        var token = user.RefreshTokens.Single(token => token.Token == refreshToken);

        if (!token.IsActive)
            return Result<AuthDto>.Failure(new[] { Error.InactiveRefreshToken() });

        token.RevokedAt = DateTime.UtcNow;

        var newRefreshToken = GenerateRefreshToken();
        user.RefreshTokens.Add(newRefreshToken);
        await userManager.UpdateAsync(user);

        return Result<AuthDto>.Success(new AuthDto
        {
            RefreshToken = newRefreshToken.Token,
            JwtSecurityToken =
                new JwtSecurityTokenHandler().WriteToken(await GenerateJwtSecurityTokenAsync(user)),
            RefreshTokenExpiresAt = newRefreshToken.ExpiresAt
        });
    }

    public async Task<Result> RevokeRefreshTokenAsync(string refreshToken)
    {
        var user = await userManager.Users.SingleOrDefaultAsync(user =>
            user.RefreshTokens.Any(token => token.Token == refreshToken));

        if (user is null)
            return Result.Failure(new[] { Error.InvalidRefreshToken() });

        var token = user.RefreshTokens.Single(token => token.Token == refreshToken);

        if (!token.IsActive)
            return Result.Failure(new[] { Error.InactiveRefreshToken() });

        token.RevokedAt = DateTime.UtcNow;
        await userManager.UpdateAsync(user);
        return Result.Success();
    }

    public async Task<Result> SendConfirmationEmailAsync(string userEmail)
    {
        var user = await userManager.FindByEmailAsync(userEmail);
        if (user is null) return Result.Failure(new[] { Error.UserEmailNotFound() });

        var token = await userManager.GenerateEmailConfirmationTokenAsync(user);

        var confirmEmailUri =
            $"{configuration.GetValue<string>("ConfirmEmailEndpoint")}?userEmail={userEmail}&token={HttpUtility.UrlEncode(token)}";

        var body = "<h1>Welcome</h1><br>" +
                   "<p> Thanks for registering please click " +
                   $"<strong><a href=\"{confirmEmailUri}\" target=\"_blank\">here</a></strong>" +
                   " to confirm your email</p>";

        return await new MailMessage(_from, userEmail, "Confirmation Email", body)
        {
            IsBodyHtml = true
        }.SendAsync(_smtpClient);
    }

    public async Task<Result<AuthDto>> SignInAsync(SignInDto signInDto)
    {
        var passwordSignInResult = await signInManager.PasswordSignInAsync(signInDto.Email,
            signInDto.Password, isPersistent: true, lockoutOnFailure: false);

        if (passwordSignInResult.IsNotAllowed)
            return Result<AuthDto>.Failure(new[] { Error.SignInForbidden() });

        if (!passwordSignInResult.Succeeded)
            return Result<AuthDto>.Failure(new[] { Error.WrongEmailOrPassword() });

        var user = await userManager.FindByEmailAsync(signInDto.Email);
        var authDto = new AuthDto
        {
            JwtSecurityToken =
                new JwtSecurityTokenHandler().WriteToken(await GenerateJwtSecurityTokenAsync(user!))
        };

        if (user!.RefreshTokens.Any(refreshToken => refreshToken.IsActive))
        {
            var activeRefreshToken =
                user.RefreshTokens.FirstOrDefault(refreshToken => refreshToken.IsActive);

            authDto.RefreshToken = activeRefreshToken!.Token;
            authDto.RefreshTokenExpiresAt = activeRefreshToken.ExpiresAt;
        }
        else
        {
            var refreshToken = GenerateRefreshToken();

            authDto.RefreshToken = refreshToken.Token;
            authDto.RefreshTokenExpiresAt = refreshToken.ExpiresAt;

            user.RefreshTokens.Add(refreshToken);
            await userManager.UpdateAsync(user);
        }

        return Result<AuthDto>.Success(authDto);
    }

    private async Task<JwtSecurityToken> GenerateJwtSecurityTokenAsync(AppUser user)
    {
        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, user.UserName!),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new(JwtRegisteredClaimNames.Iat, DateTime.UtcNow.ToString(CultureInfo.InvariantCulture))
        };

        var roles = await userManager.GetRolesAsync(user);
        claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));

        return new JwtSecurityToken(
            issuer: configuration.GetValue<string>("JWT:ValidIssuer"),
            audience: configuration.GetValue<string>("JWT:ValidAudience"),
            claims: claims,
            expires: DateTime.UtcNow.AddHours(
                configuration.GetValue<int>("JWT:ExpirationDurationInHours")),
            signingCredentials: new SigningCredentials(
                new SymmetricSecurityKey(Encoding.UTF8.GetBytes(
                    configuration.GetValue<string>("JWT:Key") ?? string.Empty)),
                SecurityAlgorithms.HmacSha256)
        );
    }

    private RefreshToken GenerateRefreshToken() => new()
    {
        Token = Convert.ToBase64String(Guid.NewGuid().ToByteArray()),
        ExpiresAt =
            DateTime.UtcNow.AddDays(
                configuration.GetValue<int>("RefreshTokenExpirationDurationInDays"))
    };
}