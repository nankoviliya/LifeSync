using Amazon.SecretsManager;
using FastEndpoints;
using FluentValidation;
using FluentValidation.AspNetCore;
using LifeSync.API.Extensions;
using LifeSync.API.OpenApi;
using LifeSync.API.Persistence;
using Microsoft.EntityFrameworkCore;
using Serilog;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .CreateLogger();

builder.Host.UseSerilog();

builder.Services.AddJsonOptions();

builder.Services.AddDefaultAWSOptions(builder.Configuration.GetAWSOptions());

// If env is not development, we use AWS Secrets Manager to get the secrets
if (!builder.Environment.IsDevelopment())
{
    builder.Services.AddAWSService<IAmazonSecretsManager>();
}

builder.Services.AddApplicationSecrets(builder.Environment);

builder.Services.AddIdentityServices();

await builder.Services.AddJwtAuthentication();

builder.Services.AddAuthorization();

builder.Services.AddDbContext<ApplicationDbContext>();

builder.Services.AddGlobalErrorHandling();

builder.Services.AddApplicationServices();

// Add CORS policy to allow the frontend to communicate with the backend
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", builder =>
    {
        builder.WithOrigins(
                "http://localhost:4200",
                "http://localhost:4300",
                "https://localhost:4200",
                "https://localhost:4300")
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();
    });
});

builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddValidatorsFromAssemblyContaining<Program>();

builder.Services.AddRateLimitingPolicies();

builder.Services.AddFastEndpoints();

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });

builder.Services.AddOpenApi(options =>
{
    options.AddDocumentTransformer<BearerSecuritySchemeTransformer>();
});

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    
    await context.Database.MigrateAsync();
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/openapi/v1.json", "My API V1");
    });
}

app.UseCors("AllowFrontend");

app.UseHttpsRedirection();

app.UseRateLimiter();

app.UseAuthentication();
app.UseAuthorization();

app.UseGlobalErrorHandling();

app.UseFastEndpoints();
app.MapControllers();

try
{
    Log.Information("Starting up the web host");

    await app.RunAsync();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Host terminated unexpectedly");
}
finally
{
    await Log.CloseAndFlushAsync();
}
