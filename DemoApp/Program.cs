using DemoApp.Database;
using Fathy.Common.Auth;
using Fathy.Common.Auth.User.Models;
using Fathy.Common.Startup;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddHttpContextAccessor();
builder.Services.AddEndpointsApiExplorer();

// Database definitions.
builder.Services.AddDbContext<IDemoAppContext, DemoAppContext>(optionsAction =>
        optionsAction.UseSqlite(builder.Configuration.GetConnectionString("Sqlite")))
    .AddIdentity<AppUser, IdentityRole>(identityOptions =>
    {
        identityOptions.SignIn.RequireConfirmedAccount =
            builder.Configuration.GetValue<bool>("Database:IdentityOptions:SignIn:RequireConfirmedAccount");
        identityOptions.User.RequireUniqueEmail =
            builder.Configuration.GetValue<bool>("Database:IdentityOptions:User:RequireUniqueEmail");
    }).AddEntityFrameworkStores<DemoAppContext>().AddDefaultTokenProviders();

// Cors definitions.
builder.Services.AddCors(corsOptions => corsOptions.AddDefaultPolicy(
    policy => policy.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod()));

// Swagger definitions.
builder.Services.AddSwaggerService(new OpenApiInfo
{
    Title = builder.Configuration.GetValue<string>("Swagger:OpenApiInfo:Title"),
    Version = builder.Configuration.GetValue<string>("Swagger:OpenApiInfo:Version"),
    Description = builder.Configuration.GetValue<string>("Swagger:OpenApiInfo:Description")
});

// Authentication definitions.
builder.Services.AddAuthenticationService();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.UseStaticFiles();
app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();