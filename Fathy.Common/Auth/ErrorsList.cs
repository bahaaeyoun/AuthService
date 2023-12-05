using Fathy.Common.Startup;
using Microsoft.AspNetCore.Http;

namespace Fathy.Common.Auth;

public static class ErrorsList
{
    public static Error WrongEmailOrPassword() => new(nameof(WrongEmailOrPassword),
        "Wrong email or password.", StatusCodes.Status400BadRequest);

    public static Error SignInForbidden() =>
        new(nameof(SignInForbidden), "Sign In Forbidden.", StatusCodes.Status403Forbidden);

    public static Error UserEmailNotFound() => new(nameof(UserEmailNotFound),
        "There is no user with this email.", StatusCodes.Status404NotFound);

    public static Error InvalidRefreshToken() => new(nameof(InvalidRefreshToken),
        "Invalid Refresh Token.", StatusCodes.Status400BadRequest);

    public static Error InactiveRefreshToken() => new(nameof(InactiveRefreshToken),
        "Inactive Refresh Token.", StatusCodes.Status400BadRequest);
}