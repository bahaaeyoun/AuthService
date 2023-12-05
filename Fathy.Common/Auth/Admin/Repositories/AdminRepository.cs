using Fathy.Common.Auth.User.Models;
using Fathy.Common.Startup;
using Microsoft.AspNetCore.Identity;

namespace Fathy.Common.Auth.Admin.Repositories;

public class AdminRepository(UserManager<AppUser> userManager, RoleManager<IdentityRole> roleManager)
    : IAdminRepository
{
    public async Task<Result> AddToRoleAsync(string userEmail, string role)
    {
        var user = await userManager.FindByEmailAsync(userEmail);
        
        return user is null
            ? Result.Failure(new[] { ErrorsList.UserEmailNotFound() })
            : (await userManager.AddToRoleAsync(user, role)).ToApplicationResult();
    }

    public async Task<Result> CreateRoleAsync(string role) =>
        (await roleManager.CreateAsync(new IdentityRole(role))).ToApplicationResult();
}