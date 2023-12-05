using Fathy.Common.Startup;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DemoApp.Controllers;

[Authorize]
public class SecuredController : ApiControllerBase
{
    [HttpGet]
    public IActionResult Get() => Ok("Hello, World!");
}