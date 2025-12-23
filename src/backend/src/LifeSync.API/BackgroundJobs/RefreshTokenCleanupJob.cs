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
        await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                _logger.LogInformation("Starting refresh token cleanup...");

                using IServiceScope scope = _serviceProvider.CreateScope();
                await scope.ServiceProvider
                    .GetRequiredService<IRefreshTokenService>()
                    .CleanupExpiredTokensAsync();

                _logger.LogInformation("Refresh token cleanup completed successfully.");
            }
            catch (Exception ex) when (ex is not OperationCanceledException)
            {
                _logger.LogError(ex, "Refresh token cleanup failed");
            }

            await Task.Delay(_period, stoppingToken);
        }
    }
}