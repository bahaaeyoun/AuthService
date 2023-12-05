using Fathy.Common.Auth.Admin.Repositories;
using Fathy.Common.Auth.Email.Repositories;
using Fathy.Common.Auth.JWT.Repositories;
using Fathy.Common.Auth.JWT.Utilities;
using Fathy.Common.Auth.User.Repositories;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

namespace Fathy.Common.Auth;

public static class DependencyInjection
{
    public static void AddAuthenticationService(this IServiceCollection services)
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
            configureOptions.SaveToken = true;
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
        services.AddSingleton<IEmailRepository, EmailRepository>();
    }
}