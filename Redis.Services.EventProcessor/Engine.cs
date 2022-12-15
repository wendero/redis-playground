using Redis.Shared;

namespace Redis.Services.EventProcessor;

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
        WhenSomethingHappened();
        return Task.CompletedTask;
    }
    internal record Something(string Text, string Timestamp);
    public async void WhenSomethingHappened()
    {
        Broker.Subscribe<Something>("SomethingHappened", (message) =>
        {
            _logger.LogInformation($"I think something happened! :: {message.Text} :: {message.Timestamp}");
        });
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
