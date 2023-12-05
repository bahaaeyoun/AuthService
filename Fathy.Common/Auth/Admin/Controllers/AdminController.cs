using Fathy.Common.Auth.Admin.Repositories;
using Fathy.Common.Auth.Admin.Utilities;
using Fathy.Common.Startup;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Fathy.Common.Auth.Admin.Controllers;

[Authorize(Roles = Roles.Admin)]
public class AdminController(IAdminRepository adminRepository) : ApiControllerBase
{
    [HttpPost]
    [ProducesResponseType(typeof(Result), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(IEnumerable<Error>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(IEnumerable<Error>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> AddToRoleAdmin([FromQuery] string userEmail) =>
        ResultToIActionResult(await adminRepository.AddToRoleAsync(userEmail, Roles.Admin));

    [HttpPost]
    [ProducesResponseType(typeof(Result), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(IEnumerable<Error>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(IEnumerable<Error>), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> CreateAdminRole() =>
        ResultToIActionResult(await adminRepository.CreateRoleAsync(Roles.Admin));
}