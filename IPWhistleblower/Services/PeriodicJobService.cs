public class PeriodicJobService : IHostedService, IDisposable
{
    private Timer _timer;
    private readonly UpdateIPInformationService _updateIPInformationService;

    public PeriodicJobService(UpdateIPInformationService updateIPInformationService)
    {
        _updateIPInformationService = updateIPInformationService;
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        _timer = new Timer(DoWork, null, TimeSpan.Zero, TimeSpan.FromHours(1));
        return Task.CompletedTask;
    }

    private async void DoWork(object state)
    {
        await _updateIPInformationService.UpdateIPInformationAsync();
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
