using AuthService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DemoWebAPI.Controllers;

[Authorize]
public class AuthorizedController : ApiControllerBase
{
    [HttpGet]
    public IActionResult HelloWorld() => Ok("Hello, World!");
}