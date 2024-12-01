using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using PlanningPoker.Api.Services;

namespace PlanningPoker.Api.Services;

public class GameCleanupService : IHostedService, IDisposable
{
    private readonly IEstimationService _estimationService;
    private readonly ILogger<GameCleanupService> _logger;
    private Timer? _timer;
    private const int CleanupIntervalMinutes = 5; // Run cleanup every 5 minutes
    private const int GameTimeoutMinutes = 30; // Remove games inactive for 30 minutes

    public GameCleanupService(
        IEstimationService estimationService,
        ILogger<GameCleanupService> logger)
    {
        _estimationService = estimationService;
        _logger = logger;
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Game Cleanup Service is starting");
        
        _timer = new Timer(DoCleanup, null, 
            TimeSpan.Zero, 
            TimeSpan.FromMinutes(CleanupIntervalMinutes));

        return Task.CompletedTask;
    }

    private void DoCleanup(object? state)
    {
        _logger.LogInformation("Running game cleanup task");
        
        try
        {
            var inactiveGames = _estimationService.GetInactiveGames(TimeSpan.FromMinutes(GameTimeoutMinutes));
            
            foreach (var gameId in inactiveGames)
            {
                _logger.LogInformation("Removing inactive game: {GameId}", gameId);
                _estimationService.RemoveGame(gameId);
            }

            if (inactiveGames.Any())
            {
                _logger.LogInformation("Removed {Count} inactive games", inactiveGames.Count());
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while cleaning up inactive games");
        }
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Game Cleanup Service is stopping");

        _timer?.Change(Timeout.Infinite, 0);

        return Task.CompletedTask;
    }

    public void Dispose()
    {
        _timer?.Dispose();
    }
}
