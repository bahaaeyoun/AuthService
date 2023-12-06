using AuthService.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace DemoWebAPI.Database;

public class DemoAppContext(DbContextOptions<DemoAppContext> option) : IdentityDbContext<AppUser>(option);