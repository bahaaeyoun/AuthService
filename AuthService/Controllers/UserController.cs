using System.Security.Claims;
using AuthService.DTOs;
using AuthService.Models;
using AuthService.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AuthService.Controllers;

public class UserController(IUserRepository userRepository)
    : ApiControllerBase
{
    private string CurrentUserEmail =>
        Request.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier) ?? string.Empty;

    // private string CurrentUserRefreshToken =>
    //     Request.HttpContext.User.FindFirstValue(nameof(RefreshToken)) ?? string.Empty;

    [HttpGet]
    [ProducesResponseType(typeof(Result), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Result), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(Result), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> ConfirmEmail([FromQuery] string userEmail, [FromQuery] string token) =>
        ToIActionResult(await userRepository.ConfirmEmailAsync(userEmail, token));

    [HttpPost]
    [ProducesResponseType(typeof(Result), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Result), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(Result), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] UserDto userDto) =>
        ToIActionResult(await userRepository.CreateAsync(userDto));

    [Authorize]
    [HttpDelete]
    [ProducesResponseType(typeof(Result), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Result), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(Result), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(Result), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Delete([FromBody] string password) => ToIActionResult(
        await userRepository.DeleteAsync(new SignInDto { Email = CurrentUserEmail, Password = password }));

    [HttpGet]
    [ProducesResponseType(typeof(Result<AuthDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Result), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(Result), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(Result), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> NewRefreshToken()
    {
        var refreshToken = Request.Cookies[nameof(RefreshToken)];

        var newRefreshTokenResult = await userRepository.NewRefreshTokenAsync(refreshToken!);

        if (!newRefreshTokenResult.Succeeded)
            return ToIActionResult(newRefreshTokenResult);

        SetRefreshTokenInCookie(newRefreshTokenResult.Data!.RefreshToken,
            newRefreshTokenResult.Data.RefreshTokenExpiresAt);

        return ToIActionResult(newRefreshTokenResult);
    }

    [HttpGet]
    [ProducesResponseType(typeof(Result), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Result), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(Result), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> ResendConfirmationEmail([FromQuery] string userEmail) =>
        ToIActionResult(await userRepository.SendConfirmationEmailAsync(userEmail));

    [HttpPost]
    [ProducesResponseType(typeof(Result), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Result), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(Result), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(Result), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> RevokeRefreshToken() =>
        ToIActionResult(await userRepository.RevokeRefreshTokenAsync(Request.Cookies[nameof(RefreshToken)]!));

    [HttpPost]
    [ProducesResponseType(typeof(Result<AuthDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Result), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(Result), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(Result), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> SignIn([FromBody] SignInDto signInDto)
    {
        var signInResult = await userRepository.SignInAsync(signInDto);

        if (!signInResult.Succeeded)
            return ToIActionResult(signInResult);

        SetRefreshTokenInCookie(signInResult.Data!.RefreshToken,
            signInResult.Data.RefreshTokenExpiresAt);

        return ToIActionResult(signInResult);
    }

    private void SetRefreshTokenInCookie(string refreshToken, DateTime expiresIn) => Response.Cookies.Append(
        nameof(RefreshToken), refreshToken, new CookieOptions
        {
            HttpOnly = true,
            Expires = expiresIn.ToLocalTime(),
            Secure = true,
            IsEssential = true,
        });
}