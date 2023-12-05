using Fathy.Common.Startup;
using Microsoft.AspNetCore.Identity;

namespace Fathy.Common.Auth;

public static class Extensions
{
    public static Result ToResult(this IdentityResult identityResult)
    {
        var errors =
            identityResult.Errors.Select(identityError =>
                new Error(identityError.Code, identityError.Description));
        return new Result(identityResult.Succeeded, errors);
    }
}