using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Fathy.Common.Startup;

[ApiController]
[Route("api/[controller]/[action]")]
public class ApiControllerBase : ControllerBase
{
    protected IActionResult ResultToIActionResult(Result result)
    {
        if (result.Succeeded) return Ok();

        if (result.Errors!.Any(x => x.StatusCode == StatusCodes.Status404NotFound))
            return NotFound(result.Errors.ToJsonStringContent());

        return BadRequest(result.Errors.ToJsonStringContent());
    }

    protected IActionResult ResultToIActionResult<T>(Result<T> result)
    {
        if (result.Succeeded) return Ok(result.Data.ToJsonStringContent());

        if (result.Errors!.Any(x => x.StatusCode == StatusCodes.Status404NotFound))
            return NotFound(result.Errors.ToJsonStringContent());

        return BadRequest(result.Errors.ToJsonStringContent());
    }
}