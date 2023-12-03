using Fathy.Common.Auth.CurrentUser.Repositories;
using Fathy.Common.Auth.User.DTOs;
using Fathy.Common.Auth.User.Repositories;
using Fathy.Common.Startup;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Fathy.Common.Auth.User.Controllers;

public class UserController : ApiControllerBase
{
    private readonly IUserRepository _userRepository;
    private readonly ICurrentUserRepository _currentUserRepository;

    public UserController(IUserRepository userRepository, ICurrentUserRepository currentUserRepository)
    {
        _userRepository = userRepository;
        _currentUserRepository = currentUserRepository;
    }

    [HttpGet]
    [ProducesResponseType(typeof(Result), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(IEnumerable<Error>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(IEnumerable<Error>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> ConfirmEmail([FromQuery] string userEmail, [FromQuery] string token) =>
        ResponseToIActionResult(await _userRepository.ConfirmEmailAsync(userEmail, token));

    [HttpPost]
    [ProducesResponseType(typeof(Result), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(IEnumerable<Error>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] UserDto userDto) =>
        ResponseToIActionResult(await _userRepository.CreateAsync(userDto));

    [Authorize]
    [HttpDelete]
    [ProducesResponseType(typeof(Result), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(IEnumerable<Error>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(IEnumerable<Error>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Delete([FromBody] string password) =>
        ResponseToIActionResult(await _userRepository.DeleteAsync(
            new UserDto { Email = _currentUserRepository.UserEmail, Password = password }));

    [HttpGet]
    [ProducesResponseType(typeof(Result), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(IEnumerable<Error>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(IEnumerable<Error>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> ResendConfirmationEmail([FromQuery] string userEmail) =>
        ResponseToIActionResult(await _userRepository.SendConfirmationEmailAsync(userEmail));

    [HttpPost]
    [ProducesResponseType(typeof(Result), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(IEnumerable<Error>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(IEnumerable<Error>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> SignIn([FromBody] UserDto userDto) =>
        ResponseToIActionResult(await _userRepository.SignInAsync(userDto));
}