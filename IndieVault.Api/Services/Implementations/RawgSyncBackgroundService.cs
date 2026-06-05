using IndieVault.Api.Services.Interfaces.ExternalApis;

namespace IndieVault.Api.Services.Implementations
{
    public class RawgSyncBackgroundService : BackgroundService
    {
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly ILogger<RawgSyncBackgroundService> _logger;
        public RawgSyncBackgroundService(IServiceScopeFactory serviceScopeFactory, ILogger<RawgSyncBackgroundService> logger)
        {
            _serviceScopeFactory = serviceScopeFactory;
            _logger = logger;
        }
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    // Create a new scope to resolve scoped services
                    using var scope = _serviceScopeFactory.CreateScope();

                    _logger.LogInformation("Starting synchronization with RAWG API at: {time}", DateTimeOffset.Now);
                    // Resolve the RAWG API service and perform synchronization
                    var rawgSyncService = scope.ServiceProvider.GetRequiredService<IRawgApiService>();
                    // Synchronize games from the RAWG API
                    await rawgSyncService.SynchronizeGamesFromApiAsync();
                    _logger.LogInformation("Finished synchronization with RAWG API at: {time}", DateTimeOffset.Now);

                    // Wait for 24 hours before the next synchronization
                    await Task.Delay(TimeSpan.FromHours(24), stoppingToken);
                }
                catch(TaskCanceledException)
                {
                    // The task was canceled, likely due to application shutdown.
                    _logger.LogInformation("RAWG API background synchronization was canceled.");
                    break;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "An error occurred during RAWG API background synchronization.");
                    await Task.Delay(TimeSpan.FromMinutes(10), stoppingToken);
                }
            }
        }
    }
}
