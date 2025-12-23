using LifeSync.API.Features.Authentication.Refresh.Services;

namespace LifeSync.API.BackgroundJobs;

public sealed class RefreshTokenCleanupJob : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<RefreshTokenCleanupJob> _logger;
    private readonly TimeSpan _period = TimeSpan.FromHours(24);

    public RefreshTokenCleanupJob(
        IServiceProvider serviceProvider,
        ILogger<RefreshTokenCleanupJob> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("RefreshTokenCleanupJob starting. Will run every {Hours} hours.", _period.TotalHours);

        // Wait 1 minute after startup before first cleanup
        await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);

        using PeriodicTimer timer = new(_period);

        while (!stoppingToken.IsCancellationRequested && await timer.WaitForNextTickAsync(stoppingToken))
        {
            try
            {
                _logger.LogInformation("Starting refresh token cleanup...");

                using IServiceScope scope = _serviceProvider.CreateScope();
                IRefreshTokenService refreshTokenService = scope.ServiceProvider
                    .GetRequiredService<IRefreshTokenService>();

                await refreshTokenService.CleanupExpiredTokensAsync();

                _logger.LogInformation("Refresh token cleanup completed successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred during refresh token cleanup.");
            }
        }

        _logger.LogInformation("RefreshTokenCleanupJob stopped.");
    }
}
