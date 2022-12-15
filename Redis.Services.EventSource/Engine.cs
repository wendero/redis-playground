using Redis.Shared;

namespace Redis.Services.EventSource;

public class Engine : IHostedService
{
    private readonly ILogger<Engine> _logger;
    public IBrokerAdapter Broker { get; }
    private string ServiceName { get; }
    public CancellationTokenSource CancellationToken { get; }

    public Engine(ILogger<Engine> logger, IBrokerAdapter adapter, IConfiguration configuration)
    {
        _logger = logger;
        Broker = adapter;
        ServiceName = configuration["ServiceName"];
        CancellationToken = new CancellationTokenSource();
    }

    public bool IsRunning { get; set; } = false;
    public Task StartAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation($"Engine started for service: {ServiceName}");
        DoSomething();
        return Task.CompletedTask;
    }
    public async void DoSomething()
    {
        IsRunning = true;
        _logger.LogInformation("Loop started");

        while (!CancellationToken.IsCancellationRequested)
        {
            Broker.Publish($"SomethingHappened", new { Text = "Something just happened", Timestamp = DateTime.Now.ToString() });
            await Task.Delay(5000);
        }
        _logger.LogInformation("Loop stopped");
        IsRunning = false;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.LogWarning("Engine is stopping");
        CancellationToken.Cancel();
        Task.Run(() =>
        {
            while (IsRunning) Task.Delay(50).Wait();
            return Task.CompletedTask;
        }).Wait();
        _logger.LogWarning("Engine is stopped");
        return Task.CompletedTask;
    }
}
