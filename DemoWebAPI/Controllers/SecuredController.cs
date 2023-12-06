using AuthService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DemoWebAPI.Controllers;

[Authorize]
public class SecuredController : ApiControllerBase
{
    [HttpGet]
    public IActionResult HelloWorld() => Ok("Hello, World!");
}