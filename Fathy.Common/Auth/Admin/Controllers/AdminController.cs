using Fathy.Common.Auth.Admin.Repositories;
using Fathy.Common.Auth.Admin.Utilities;
using Fathy.Common.Startup;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Fathy.Common.Auth.Admin.Controllers;

[Authorize(Roles = Roles.Admin)]
public class AdminController : ApiControllerBase
{
    private readonly IAdminRepository _adminRepository;

    public AdminController(IAdminRepository adminRepository)
    {
        _adminRepository = adminRepository;
    }

    [HttpPost]
    [ProducesResponseType(typeof(Result), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(IEnumerable<Error>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(IEnumerable<Error>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> AddToRoleAdmin([FromQuery] string userEmail) =>
        ResponseToIActionResult(await _adminRepository.AddToRoleAsync(userEmail, Roles.Admin));

    [HttpPost]
    [ProducesResponseType(typeof(Result), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(IEnumerable<Error>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateAdminRole() =>
        ResponseToIActionResult(await _adminRepository.CreateRoleAsync(Roles.Admin));
}