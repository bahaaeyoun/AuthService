using System.Text;
using DemoWebAPI.Database;
using AuthService;
using AuthService.Models;
using AuthService.Repositories;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// Database definitions.
builder.Services.AddDbContext<DemoAppContext>(optionsAction =>
        optionsAction.UseSqlite(builder.Configuration.GetConnectionString("Sqlite")))
    .AddIdentity<AppUser, IdentityRole>(identityOptions =>
    {
        identityOptions.SignIn.RequireConfirmedAccount =
            builder.Configuration.GetValue<bool>("Identity:SignIn:RequireConfirmedAccount");
        identityOptions.User.RequireUniqueEmail =
            builder.Configuration.GetValue<bool>("Identity:User:RequireUniqueEmail");
    }).AddEntityFrameworkStores<DemoAppContext>().AddDefaultTokenProviders();

// Cors definitions.
builder.Services.AddCors(corsOptions => corsOptions.AddDefaultPolicy(
    policy => policy.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod().AllowCredentials()));

// Swagger definitions.
builder.Services.AddSwaggerGen(swaggerGenOption =>
{
    swaggerGenOption.OperationFilter<SwaggerOperationFilter>();

    swaggerGenOption.SwaggerDoc(
        builder.Configuration.GetValue<string>("Swagger:OpenApiInfo:Version"), new OpenApiInfo
        {
            Title = builder.Configuration.GetValue<string>("Swagger:OpenApiInfo:Title"),
            Version = builder.Configuration.GetValue<string>("Swagger:OpenApiInfo:Version"),
            Description = builder.Configuration.GetValue<string>("Swagger:OpenApiInfo:Description")
        });

    swaggerGenOption.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Name = "JWT Authentication",
        Scheme = "Bearer",
        Description = "Please enter JWT Bearer token **only**.",
        BearerFormat = "JWT"
    });

    swaggerGenOption.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Id = "Bearer",
                    Type = ReferenceType.SecurityScheme
                }
            },
            Array.Empty<string>()
        }
    });
});

// Authentication definitions.
builder.Services.AddAuthentication(configureOptions =>
{
    configureOptions.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
    configureOptions.DefaultForbidScheme = JwtBearerDefaults.AuthenticationScheme;
    configureOptions.DefaultSignInScheme = JwtBearerDefaults.AuthenticationScheme;
    configureOptions.DefaultSignOutScheme = JwtBearerDefaults.AuthenticationScheme;
    configureOptions.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    configureOptions.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(configureOptions =>
{
    // TODO: test.
    configureOptions.SaveToken = true;
    configureOptions.TokenValidationParameters = new TokenValidationParameters
    {
        ClockSkew = TimeSpan.Zero,
        ValidateIssuer = true,
        ValidIssuer = builder.Configuration.GetValue<string>("JWT:ValidIssuer"),
        ValidateAudience = true,
        ValidAudience = builder.Configuration.GetValue<string>("JWT:ValidAudience"),
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(
            builder.Configuration.GetValue<string>("JWT:Key") ?? string.Empty))
    };
});

builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IAdminRepository, AdminRepository>();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();