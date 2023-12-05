using Fathy.Common.Auth.User.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace DemoApp.Database;

public class DemoAppContext(DbContextOptions<DemoAppContext> option) : IdentityDbContext<AppUser>(option),
    IDemoAppContext;