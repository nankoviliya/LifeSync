using LifeSync.API.Features.Authentication.Helpers;
using LifeSync.API.Features.Authentication.Services;
using LifeSync.API.Features.ExpenseTracking.EventHandlers;
using LifeSync.API.Features.ExpenseTracking.Services;
using LifeSync.API.Features.IncomeTracking.EventHandlers;
using LifeSync.API.Features.IncomeTracking.Services;
using LifeSync.API.Features.Users.Services;
using LifeSync.API.Infrastructure.DomainEvents;
using LifeSync.API.Models.ApplicationUser;
using LifeSync.API.Models.Events;
using LifeSync.API.Persistence;
using LifeSync.API.Secrets;
using LifeSync.API.Secrets.Contracts;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http.Json;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace LifeSync.API.Extensions;

public static class ApplicationExtensions
{
    public static IServiceCollection AddJsonOptions(this IServiceCollection services)
    {
        services.Configure<JsonOptions>(options =>
        {
            options.SerializerOptions.PropertyNameCaseInsensitive = true;
        });

        return services;
    }

    public static IServiceCollection AddIdentityServices(this IServiceCollection services)
    {
        services.AddIdentity<User, IdentityRole>()
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddDefaultTokenProviders();

        return services;
    }

    public static IServiceCollection AddApplicationSecrets(this IServiceCollection services, IWebHostEnvironment environment)
    {
        if (environment.IsDevelopment())
        {
            services.AddScoped<ISecretsProvider, LocalSecretsProvider>();
        }
        else
        {
            services.AddScoped<ISecretsProvider, CloudSecretsProvider>();
        }

        services.AddScoped<ISecretsProviderFactory, SecretsProviderFactory>();

        services.AddScoped<ISecretsManager, SecretsManager>();

        return services;
    }

    public static async Task<IServiceCollection> AddJwtAuthentication(this IServiceCollection services)
    {
        var serviceProvider = services.BuildServiceProvider();
        var secretsManager = serviceProvider.GetRequiredService<ISecretsManager>();

        var jwtSecrets = await secretsManager.GetJwtSecretsAsync();

        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = jwtSecrets.Issuer,
                ValidAudience = jwtSecrets.Audience,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecrets.SecretKey))
            };
        });

        return services;
    }

    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddTransient<IDomainEventDispatcher, DomainEventDispatcher>();

        services.AddTransient<IDomainEventHandler<IncomeTransactionCreatedDomainEvent>, IncomeTransactionCreatedDomainEventHandler>();
        services.AddTransient<IDomainEventHandler<ExpenseTransactionCreatedDomainEvent>, ExpenseTransactionCreatedDomainEventHandler>();

        services.AddScoped<IUsersService, UsersService>();
        services.AddScoped<IExpenseTrackingService, ExpenseTrackingService>();
        services.AddScoped<IIncomeTrackingService, IncomeTrackingService>();

        services.AddTransient<JwtTokenGenerator>();

        services.AddScoped<IAuthService, AuthService>();

        return services;
    }
}