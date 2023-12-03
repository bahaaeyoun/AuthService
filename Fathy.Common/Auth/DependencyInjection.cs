using Fathy.Common.Auth.Admin.Repositories;
using Fathy.Common.Auth.CurrentUser.Repositories;
using Fathy.Common.Auth.JWTGenerator.Repositories;
using Fathy.Common.Auth.JWTGenerator.Utilities;
using Fathy.Common.Auth.User.Repositories;
using Fathy.Common.Email;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

namespace Fathy.Common.Auth;

public static class DependencyInjection
{
    public static void AddAuthService(this IServiceCollection services)
    {
        services.AddAuthentication(configureOptions =>
        {
            configureOptions.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            configureOptions.DefaultForbidScheme = JwtBearerDefaults.AuthenticationScheme;
            configureOptions.DefaultSignInScheme = JwtBearerDefaults.AuthenticationScheme;
            configureOptions.DefaultSignOutScheme = JwtBearerDefaults.AuthenticationScheme;
            configureOptions.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            configureOptions.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        }).AddJwtBearer(configureOptions =>
        {
            configureOptions.TokenValidationParameters = new TokenValidationParameters
            {
                ClockSkew = TimeSpan.Zero,
                ValidateIssuer = true,
                ValidIssuer = JwtParameters.ValidIssuer,
                ValidateAudience = true,
                ValidAudience = JwtParameters.ValidAudience,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = JwtParameters.IssuerSigningKey
            };
        });

        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IAdminRepository, AdminRepository>();
        services.AddScoped<IJwtGeneratorRepository, JwtGeneratorRepository>();
        services.AddSingleton<ICurrentUserRepository, CurrentUserRepository>();
        services.AddSingleton<IEmailRepository, EmailRepository>();
    }
}