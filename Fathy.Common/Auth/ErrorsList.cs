using Fathy.Common.Startup;
using Microsoft.AspNetCore.Http;

namespace Fathy.Common.Auth;

public static class ErrorsList
{
    public static Error SignInFailed() => new(StatusCodes.Status400BadRequest, nameof(SignInFailed),
        "Sign In failed, wrong email or password.");

    public static Error SignInNotAllowed() =>
        new(StatusCodes.Status400BadRequest, nameof(SignInNotAllowed), "Sign In not allowed.");

    public static Error UserEmailNotFound(string email) => new(StatusCodes.Status404NotFound, nameof(UserEmailNotFound),
        "There is no user with this email.");
}