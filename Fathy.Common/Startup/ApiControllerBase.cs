using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Fathy.Common.Startup;

[ApiController]
[Route("api/[controller]/[action]")]
public class ApiControllerBase : ControllerBase
{
    protected IActionResult ResponseToIActionResult(Result response)
    {
        if (response.Succeeded) return Ok();

        if (response.Errors!.Any(x => x.StatusCode == StatusCodes.Status404NotFound))
            return NotFound(response.Errors.ToJsonStringContent());

        return BadRequest(response.Errors.ToJsonStringContent());
    }

    protected IActionResult ResponseToIActionResult<T>(Result<T> response)
    {
        if (response.Succeeded) return Ok(response.Data.ToJsonStringContent());

        if (response.Errors!.Any(x => x.StatusCode == StatusCodes.Status404NotFound))
            return NotFound(response.Errors.ToJsonStringContent());

        return BadRequest(response.Errors.ToJsonStringContent());
    }
}