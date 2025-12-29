using LifeSync.API.Features.Account.GetAccount.Services;
using LifeSync.API.Features.Account.UpdateAccount.Services;
using LifeSync.API.Features.AccountExport;
using LifeSync.API.Features.AccountExport.DataExporters;
using LifeSync.API.Features.AccountImport;
using LifeSync.API.Features.AccountImport.DataReaders;
using LifeSync.API.Features.Authentication.Helpers;
using LifeSync.API.Features.Authentication.Login.Services;
using LifeSync.API.Features.Authentication.Refresh.Services;
using LifeSync.API.Features.Authentication.Register.Services;
using LifeSync.API.Features.Finances.Expenses.Services;
using LifeSync.API.Features.Finances.Incomes.Services;
using LifeSync.API.Features.Finances.Search.Services;
using LifeSync.API.Features.FrontendSettings.Services;
using LifeSync.API.Features.Translations.Services;
using LifeSync.API.Features.Translations.Services.Contracts;
using LifeSync.API.Models.ApplicationUser;
using LifeSync.API.Persistence;
using LifeSync.API.Secrets;
using LifeSync.API.Secrets.Contracts;
using LifeSync.API.Secrets.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace LifeSync.API.Extensions;

public static class ServiceCollectionExtensions
{
    extension(IServiceCollection services)
    {
        public IServiceCollection LifeSync => services;

        public IServiceCollection AddJsonOptions()
        {
            void ConfigureSerializer(JsonSerializerOptions serializerOptions)
            {
                serializerOptions.PropertyNameCaseInsensitive = true;
                serializerOptions.NumberHandling = JsonNumberHandling.Strict;
                serializerOptions.AllowTrailingCommas = false;
                serializerOptions.ReadCommentHandling = JsonCommentHandling.Disallow;
                serializerOptions.Converters.Add(new JsonStringEnumConverter());
            }

            services.Configure<JsonOptions>(options =>
            {
                ConfigureSerializer(options.JsonSerializerOptions);
            });

            services.ConfigureHttpJsonOptions(options =>
            {
                ConfigureSerializer(options.SerializerOptions);
            });

            return services;
        }

        public IServiceCollection AddIdentityServices()
        {
            services.AddIdentity<User, IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();

            services.ConfigureApplicationCookie(options =>
            {
                options.Events.OnRedirectToLogin = context =>
                {
                    context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                    return Task.CompletedTask;
                };
                options.Events.OnRedirectToAccessDenied = context =>
                {
                    context.Response.StatusCode = StatusCodes.Status403Forbidden;
                    return Task.CompletedTask;
                };
            });

            return services;
        }

        public IServiceCollection AddApplicationSecrets(IWebHostEnvironment environment)
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

        public async Task<IServiceCollection> AddJwtAuthentication()
        {
            services.AddSingleton<JwtSecurityTokenHandler>();

            ServiceProvider serviceProvider = services.BuildServiceProvider();
            ISecretsManager secretsManager = serviceProvider.GetRequiredService<ISecretsManager>();
            JwtSecrets jwtSecrets = await secretsManager.GetJwtSecretsAsync();

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

                    options.Events = new JwtBearerEvents
                    {
                        OnMessageReceived = context =>
                        {
                            // Support JWT from cookie OR Authorization header
                            context.Token = context.Request.Cookies["access_token"];
                            return Task.CompletedTask;
                        }
                    };
                });

            return services;
        }

        public IServiceCollection AddApplicationServices()
        {
            services.AddScoped<ITranslationsLoader, TranslationsFileLoader>();
            services.AddScoped<ITranslationsService, TranslationsService>();

            services.AddScoped<IFrontendSettingsService, FrontendSettingsService>();

            services.AddScoped<IGetAccountService, GetAccountService>();
            services.AddScoped<IUpdateAccountService, UpdateAccountService>();

            services.AddScoped<IAccountDataExporter, JsonAccountDataExporter>();
            services.AddScoped<IAccountExportService, AccountExportService>();

            services.AddScoped<IAccountDataReader, JsonAccountDataReader>();
            services.AddScoped<IAccountImportService, AccountImportService>();

            services.AddScoped<IExpenseService, ExpenseService>();
            services.AddScoped<IIncomeService, IncomeService>();
            services.AddScoped<ITransactionsSearchService, TransactionsSearchService>();

            services.AddTransient<JwtTokenGenerator>();

            services.AddScoped<ILoginService, LoginService>();
            services.AddScoped<IRegisterService, RegisterService>();
            services.AddScoped<IRefreshTokenService, RefreshTokenService>();

            return services;
        }

        public IServiceCollection AddGlobalErrorHandling()
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