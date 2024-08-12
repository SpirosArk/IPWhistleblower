public class PeriodicJobService : IHostedService, IDisposable
{
    private Timer _timer;
    private readonly IServiceScopeFactory _serviceScopeFactory;

    public PeriodicJobService(IServiceScopeFactory serviceScopeFactory)
    {
        _serviceScopeFactory = serviceScopeFactory;
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        _timer = new Timer(DoWork, null, TimeSpan.Zero, TimeSpan.FromHours(1));
        return Task.CompletedTask;
    }

    private async void DoWork(object state)
    {
        //This was introduced to avoid the di lifetime conflict, that Singleton PeriodicJobService had with a scoped ApplicationDbContext 
        using (var scope = _serviceScopeFactory.CreateScope())
        {
            var updateIPInformationService = scope.ServiceProvider.GetRequiredService<UpdateIPInformationService>();
            await updateIPInformationService.UpdateIPInformationAsync();
        }
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        _timer?.Change(Timeout.Infinite, 0);
        return Task.CompletedTask;
    }

    public void Dispose()
    {
        _timer?.Dispose();
    }
}
