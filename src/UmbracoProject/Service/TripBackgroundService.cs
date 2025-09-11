using UmbracoProject.Repository;

namespace UmbracoProject.Service
{
    public class TripBackgroundService : BackgroundService
    {
        private readonly ILogger<TripBackgroundService> _log;
        private readonly IServiceScopeFactory _serviceScopeFactory;

        public TripBackgroundService(ILogger<TripBackgroundService> log, IServiceScopeFactory serviceScopeFactory)
        {
            _log = log;
            _serviceScopeFactory = serviceScopeFactory;
        }

        protected override async Task ExecuteAsync(CancellationToken ct)
        {
            while (!ct.IsCancellationRequested)
            {
                try
                {
                    using var scope = _serviceScopeFactory.CreateScope();
                    var repo = scope.ServiceProvider.GetRequiredService<ITripRepository>();
                    _log.LogInformation("Current  time: {CurrentTime}", DateTime.Now);

                    var changed = await repo.UpdateTripStatusBackgroundServiceAsync();
                    _log.LogInformation("TripStatusWorker: updated {Changed} trips", changed);
                }
                catch (Exception ex)
                {
                    _log.LogError(ex, "TripStatusWorker failed");
                }
                await Task.Delay(TimeSpan.FromMinutes(1), ct);
            }
        }
    }
}