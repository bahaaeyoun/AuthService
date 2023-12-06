using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AuthService;

[ApiController]
[Route("api/[controller]/[action]")]
public class ApiControllerBase : ControllerBase
{
    protected IActionResult ToIActionResult(Result result)
    {
        if (result.Succeeded) return Ok();

        if (result.Errors!.Any(error => error.StatusCode == StatusCodes.Status404NotFound))
            return NotFound(result.Errors);

        return BadRequest(result.Errors);
    }

    protected IActionResult ToIActionResult<T>(Result<T> result)
    {
        if (result.Succeeded) return Ok(result.Data);

        if (result.Errors!.Any(error => error.StatusCode == StatusCodes.Status404NotFound))
            return NotFound(result.Errors);

        return BadRequest(result.Errors);
    }
}