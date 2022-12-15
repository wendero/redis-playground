using StackExchange.Redis;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using System.Text.Json;

namespace Redis.Shared;

public class RedisAdapter : IBrokerAdapter
{

    private readonly ILogger<RedisAdapter> _logger;

    private ConnectionMultiplexer Muxer { get; }
    private IDatabase Redis { get; }

    public RedisAdapter(IConfiguration configuration, ILogger<RedisAdapter> logger)
    {
        _logger = logger;

        this.Muxer = ConnectionMultiplexer.Connect(configuration.GetConnectionString("Redis"));
        this.Redis = this.Muxer.GetDatabase();
    }
    
    public void Publish<T>(string channel, T message) where T : class
    {
        try
        {
            if (message is null) throw new ArgumentNullException(nameof(message));

            var json = JsonSerializer.Serialize(message);

            this.Redis.Publish(channel, new RedisValue(json));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sending message.");
            throw;
        }
    }
    public void Subscribe<T>(string channel, Action<T> action) where T : class
    {
        try
        {
            var subscriber = this.Muxer.GetSubscriber();
            subscriber.Subscribe(channel, (redisChannel, message) =>
            {
                var obj = JsonSerializer.Deserialize<T>(message);
                action(obj);
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error receiving message.");
            throw;
        }
    }
}
