using AuthService.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AuthService.Controllers;

//[Authorize(Roles = Roles.Admin)]
public class AdminController(IAdminRepository adminRepository) : ApiControllerBase
{
    [HttpPost]
    [ProducesResponseType(typeof(Result), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(IEnumerable<Error>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(IEnumerable<Error>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> AddToRoleAdmin([FromQuery] string userEmail) =>
        ToIActionResult(await adminRepository.AddToRoleAsync(userEmail, Roles.Admin));

    [HttpPost]
    [ProducesResponseType(typeof(Result), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(IEnumerable<Error>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(IEnumerable<Error>), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> CreateAdminRole() =>
        ToIActionResult(await adminRepository.CreateRoleAsync(Roles.Admin));
}