using Redis.Services.EventProcessor;
using Redis.Shared;

var builder = Host.CreateDefaultBuilder(args);

builder.ConfigureServices(services =>
{
    services.AddSingleton<IBrokerAdapter, RedisAdapter>();
    services.AddHostedService<Engine>();
});

var app = builder.Build();

app.Run();
