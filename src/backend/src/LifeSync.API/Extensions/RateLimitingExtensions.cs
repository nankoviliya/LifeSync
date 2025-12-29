using Microsoft.AspNetCore.RateLimiting;
using System.Threading.RateLimiting;

namespace LifeSync.API.Extensions;

public static class RateLimitingExtensions
{
    public static IServiceCollection AddRateLimitingPolicies(this IServiceCollection services)
    {
        services.AddRateLimiter(options =>
        {
            // Policy for public anonymous endpoints (translations, frontend settings, etc.)
            options.AddFixedWindowLimiter("PublicApi", limiterOptions =>
            {
                limiterOptions.PermitLimit = 100;
                limiterOptions.Window = TimeSpan.FromMinutes(1);
                limiterOptions.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
                limiterOptions.QueueLimit = 10;
            });

            // Strict policy for authentication endpoints (login, refresh)
            options.AddSlidingWindowLimiter("AuthEndpoints", limiterOptions =>
            {
                limiterOptions.PermitLimit = 5;
                limiterOptions.Window = TimeSpan.FromMinutes(5);
                limiterOptions.SegmentsPerWindow = 5; // 1-minute segments
                limiterOptions.QueueLimit = 0;
            });

            // Global fallback rate limit
            options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(httpContext =>
                RateLimitPartition.GetFixedWindowLimiter(
                    httpContext.User.Identity?.Name ??
                    httpContext.Connection.RemoteIpAddress?.ToString() ?? "anonymous",
                    partition => new FixedWindowRateLimiterOptions
                    {
                        AutoReplenishment = true,
                        PermitLimit = 200,
                        QueueLimit = 0,
                        Window = TimeSpan.FromMinutes(1)
                    }));

            options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
        });

        return services;
    }
}