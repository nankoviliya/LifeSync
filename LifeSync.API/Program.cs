using Amazon.SecretsManager;
using FluentValidation;
using FluentValidation.AspNetCore;
using LifeSync.API.Extensions;
using LifeSync.API.Persistence;
using Serilog;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .CreateLogger();

builder.Host.UseSerilog();

// Add services to the container.
builder.Services.AddJsonOptions();

builder.Services.AddDefaultAWSOptions(builder.Configuration.GetAWSOptions());

// If env is not development, we use AWS Secrets Manager to get the secrets
if (!builder.Environment.IsDevelopment())
{
    builder.Services.AddAWSService<IAmazonSecretsManager>();
}

builder.Services.AddApplicationSecrets(builder.Environment);

builder.Services.AddIdentityServices();

// Add authentication service
await builder.Services.AddJwtAuthentication();

builder.Services.AddAuthorization();

builder.Services.AddDbContext<ApplicationDbContext>();

builder.Services.AddApplicationServices();

// Add CORS policy to allow the frontend to communicate with the backend
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", builder =>
    {
        builder.WithOrigins("http://localhost:4200")
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();
    });
});

builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddValidatorsFromAssemblyContaining<Program>();

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    }); ;

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("AllowFrontend");

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

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
    Log.CloseAndFlush();
}
