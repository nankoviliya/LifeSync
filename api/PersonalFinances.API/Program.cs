using System.Text;
using System.Text.Json.Serialization;
using Amazon.SecretsManager;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http.Json;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using PersonalFinances.API.Features.Authentication.Helpers;
using PersonalFinances.API.Features.Authentication.Models;
using PersonalFinances.API.Features.Authentication.Services;
using PersonalFinances.API.Features.ExpenseTracking.EventHandlers;
using PersonalFinances.API.Features.ExpenseTracking.Services;
using PersonalFinances.API.Features.IncomeTracking.EventHandlers;
using PersonalFinances.API.Features.IncomeTracking.Services;
using PersonalFinances.API.Infrastructure.DomainEvents;
using PersonalFinances.API.Models;
using PersonalFinances.API.Models.Events;
using PersonalFinances.API.Persistence;
using PersonalFinances.API.Secrets;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.Configure<JsonOptions>(options =>
{
    options.SerializerOptions.PropertyNameCaseInsensitive = true;
});

builder.Services.AddIdentity<User, IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

builder.Services.AddDefaultAWSOptions(builder.Configuration.GetAWSOptions());

builder.Services.AddAWSService<IAmazonSecretsManager>();

builder.Services.AddSingleton<ISecretsManager, SecretsManager>();

// Get JwtSettings from AWS Secrets Manager
var serviceProvider = builder.Services.BuildServiceProvider();
var secretsManager = serviceProvider.GetService<ISecretsManager>();
var jwtSecrets = secretsManager.GetJwtSecretAsync().Result;
builder.Services.AddSingleton<JwtSecrets>(jwtSecrets);

// Add authentication service
builder.Services.AddAuthentication(options =>
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
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecrets.SecretKey)) // use a secret key here
    };
});

builder.Services.AddAuthorization();

builder.Services.AddDbContext<ApplicationDbContext>();

builder.Services.AddTransient<IDomainEventDispatcher, DomainEventDispatcher>();

builder.Services.AddTransient<IDomainEventHandler<IncomeTransactionCreatedDomainEvent>, IncomeTransactionCreatedDomainEventHandler>();
builder.Services.AddTransient<IDomainEventHandler<ExpenseTransactionCreatedDomainEvent>, ExpenseTransactionCreatedDomainEventHandler>();

builder.Services.AddScoped<IExpenseTrackingService, ExpenseTrackingService>();

builder.Services.AddScoped<IIncomeTrackingService, IncomeTrackingService>();

builder.Services.AddTransient<JwtTokenGenerator>();

builder.Services.AddScoped<IAuthService, AuthService>();

// Add CORS policy to allow the frontend to communicate with the backend
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", builder =>
    {
        builder.WithOrigins("http://localhost:4200") // Update with your React app's URL
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

builder.Services.AddControllers() 
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });;

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

app.Run();
