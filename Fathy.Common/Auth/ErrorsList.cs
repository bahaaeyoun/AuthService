using Fathy.Common.Startup;
using Microsoft.AspNetCore.Http;

namespace Fathy.Common.Auth;

public static class ErrorsList
{
    public static Error SignInFailed() => new(StatusCodes.Status401Unauthorized, nameof(SignInFailed),
        "Wrong email or password.");

    public static Error SignInForbidden() =>
        new(StatusCodes.Status403Forbidden, nameof(SignInForbidden), "Sign In Forbidden.");

    public static Error UserEmailNotFound() => new(StatusCodes.Status404NotFound, nameof(UserEmailNotFound),
        "There is no user with this email.");
}