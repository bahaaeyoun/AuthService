using Fathy.Common.Auth.User.DTOs;
using Fathy.Common.Auth.User.Repositories;
using Fathy.Common.Startup;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Fathy.Common.Auth.User.Controllers;

public class UserController(IUserRepository userRepository)
    : ApiControllerBase
{
    [HttpGet]
    [ProducesResponseType(typeof(Result), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Result), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(Result), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> ConfirmEmail([FromQuery] string userEmail, [FromQuery] string token) =>
        ResultToIActionResult(await userRepository.ConfirmEmailAsync(userEmail, token));

    [HttpPost]
    [ProducesResponseType(typeof(Result), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Result), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(Result), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] UserDto userDto) =>
        ResultToIActionResult(await userRepository.CreateAsync(userDto));

    [Authorize]
    [HttpDelete]
    [ProducesResponseType(typeof(Result), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Result), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(Result), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(Result), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Delete([FromBody] string password) =>
        ResultToIActionResult(await userRepository.DeleteAsync(
            new SignInDto { Email = userRepository.CurrentUserEmail, Password = password }));

    [HttpGet]
    [ProducesResponseType(typeof(Result), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Result), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(Result), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> ResendConfirmationEmail([FromQuery] string userEmail) =>
        ResultToIActionResult(await userRepository.SendConfirmationEmailAsync(userEmail));

    [HttpPost]
    [ProducesResponseType(typeof(Result<AuthDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Result), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(Result), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(Result), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> SignIn([FromBody] SignInDto signInDto) =>
        ResultToIActionResult(await userRepository.SignInAsync(signInDto));
}