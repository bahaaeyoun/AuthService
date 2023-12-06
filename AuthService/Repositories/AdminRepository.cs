using AuthService.Models;
using Microsoft.AspNetCore.Identity;

namespace AuthService.Repositories;

public class AdminRepository(UserManager<AppUser> userManager, RoleManager<IdentityRole> roleManager)
    : IAdminRepository
{
    public async Task<Result> AddToRoleAsync(string userEmail, string role)
    {
        var user = await userManager.FindByEmailAsync(userEmail);
        
        return user is null
            ? Result.Failure(new[] { Error.UserEmailNotFound() })
            : (await userManager.AddToRoleAsync(user, role)).ToResult();
    }

    public async Task<Result> CreateRoleAsync(string role) =>
        (await roleManager.CreateAsync(new IdentityRole(role))).ToResult();
}