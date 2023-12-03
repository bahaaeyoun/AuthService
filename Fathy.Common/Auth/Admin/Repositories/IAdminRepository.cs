using Fathy.Common.Startup;

namespace Fathy.Common.Auth.Admin.Repositories;

public interface IAdminRepository
{
    Task<Result> AddToRoleAsync(string userEmail, string role);
    Task<Result> CreateRoleAsync(string role);
}