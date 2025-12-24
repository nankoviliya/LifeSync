using LifeSync.API.Models.RefreshTokens;
using LifeSync.API.Persistence;
using Microsoft.EntityFrameworkCore;

namespace LifeSync.API.BackgroundJobs;

public sealed class RefreshTokenCleanupJob : BackgroundService
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<RefreshTokenCleanupJob> _logger;
    private readonly TimeSpan _period = TimeSpan.FromHours(24);

    public RefreshTokenCleanupJob(
        ApplicationDbContext context,
        ILogger<RefreshTokenCleanupJob> logger)
    {
        _context = context;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        await Task.Delay(TimeSpan.FromMinutes(1), cancellationToken);

        while (!cancellationToken.IsCancellationRequested)
        {
            try
            {
                _logger.LogInformation("Starting refresh token cleanup...");

                DateTime cutoffDate = DateTime.UtcNow;

                // Hard delete tokens that are expired OR revoked
                List<RefreshToken> tokensToDelete = await _context.RefreshTokens
                    .Where(t => t.ExpiresAt < cutoffDate || t.IsRevoked)
                    .ToListAsync(cancellationToken);

                if (tokensToDelete.Count > 0)
                {
                    _context.RefreshTokens.RemoveRange(tokensToDelete);
                    await _context.SaveChangesAsync(cancellationToken);
                }

                _logger.LogInformation("Refresh token cleanup completed successfully.");
            }
            catch (Exception ex) when (ex is not OperationCanceledException)
            {
                _logger.LogError(ex, "Refresh token cleanup failed");
            }

            await Task.Delay(_period, cancellationToken);
        }
    }
}