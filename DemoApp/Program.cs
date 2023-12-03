using DemoApp.Database;
using Fathy.Common.Auth;
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
        optionsAction.UseSqlite(builder.Configuration.GetValue<string>("ConnectionStrings:Sqlite")))
    .AddIdentity<IdentityUser, IdentityRole>(identityOptions =>
    {
        identityOptions.SignIn.RequireConfirmedAccount =
            builder.Configuration.GetValue<bool>("Database:IdentityOptions:SignIn:RequireConfirmedAccount");
        identityOptions.User.RequireUniqueEmail =
            builder.Configuration.GetValue<bool>("Database:IdentityOptions:User:RequireUniqueEmail");
    }).AddEntityFrameworkStores<DemoAppContext>().AddDefaultTokenProviders();

// Cors definitions.
builder.Services.AddCors(corsOptions => corsOptions.AddDefaultPolicy(
    policy => policy.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod().AllowCredentials()));

// Swagger definitions.
builder.Services.AddSwaggerService(new OpenApiInfo
{
    Title = "DemoAuthService",
    Version = "v1",
    Description = "An ASP.NET Core Web API for authentication services."
});

// Authentication definitions.
builder.Services.AddAuthService();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseStaticFiles();
app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();