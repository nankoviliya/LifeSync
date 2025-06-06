﻿using LifeSync.API.Features.Account.Services;
using LifeSync.API.Features.Account.Services.Contracts;
using LifeSync.API.Features.AccountExport;
using LifeSync.API.Features.AccountExport.Exporters;
using LifeSync.API.Features.AccountImport;
using LifeSync.API.Features.AccountImport.Importers;
using LifeSync.API.Features.Authentication.Helpers;
using LifeSync.API.Features.Authentication.Services;
using LifeSync.API.Features.Finances.EventHandlers;
using LifeSync.API.Features.Finances.Services;
using LifeSync.API.Features.Finances.Services.Contracts;
using LifeSync.API.Features.FrontendSettings.Services;
using LifeSync.API.Features.Translations.Services;
using LifeSync.API.Features.Translations.Services.Contracts;
using LifeSync.API.Infrastructure.DomainEvents;
using LifeSync.API.Models.ApplicationUser;
using LifeSync.API.Models.Expenses.Events;
using LifeSync.API.Models.Incomes.Events;
using LifeSync.API.Persistence;
using LifeSync.API.Secrets;
using LifeSync.API.Secrets.Contracts;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http.Json;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Text;

namespace LifeSync.API.Extensions
{
    public static class ServiceCollectionExtensions
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
            services.AddSingleton<JwtSecurityTokenHandler>();

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

            services.AddScoped<ITranslationsLoader, TranslationsFileLoader>();
            services.AddScoped<ITranslationsService, TranslationsService>();

            services.AddScoped<IFrontendSettingsService, FrontendSettingsService>();
            services.AddScoped<IAccountService, AccountService>();

            services.AddScoped<IAccountExporter, JsonAccountExporter>();
            services.AddScoped<IAccountExportService, AccountExportService>();

            services.AddScoped<IAccountImporter, JsonAccountImporter>();
            services.AddScoped<IAccountImportService, AccountImportService>();

            services.AddScoped<IExpenseTransactionsManagement, ExpenseTransactionsManagement>();
            services.AddScoped<IIncomeTransactionsManagement, IncomeTransactionsManagement>();
            services.AddScoped<ITransactionsSearchService, TransactionsSearchService>();

            services.AddTransient<JwtTokenGenerator>();

            services.AddScoped<IAuthService, AuthService>();

            return services;
        }

        public static IServiceCollection AddGlobalErrorHandling(this IServiceCollection services)
        {
            services.AddProblemDetails(options =>
            {
                options.CustomizeProblemDetails = context =>
                {
                    context.ProblemDetails.Extensions["traceId"] = context.HttpContext.TraceIdentifier;
                    context.ProblemDetails.Status = StatusCodes.Status500InternalServerError;
                    context.ProblemDetails.Title = "Internal Server Error";
                    context.ProblemDetails.Detail = "An unexpected error occurred.";
                };
            });

            return services;
        }
    }
}
