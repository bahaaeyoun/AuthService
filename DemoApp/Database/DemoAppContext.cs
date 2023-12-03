using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace DemoApp.Database;

public class DemoAppContext : IdentityDbContext, IDemoAppContext
{
    public DemoAppContext(DbContextOptions option) : base(option)
    {
    }
}