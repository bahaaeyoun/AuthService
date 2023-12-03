using System.Security.Claims;
using Microsoft.AspNetCore.Http;

namespace Fathy.Common.Auth.CurrentUser.Repositories;

public class CurrentUserRepository : ICurrentUserRepository
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CurrentUserRepository(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public string UserEmail =>
        _httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier) ?? string.Empty;
}