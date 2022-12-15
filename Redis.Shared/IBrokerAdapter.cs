namespace Redis.Shared;

public interface IBrokerAdapter
{
    public void Publish<T>(string channel, T message) where T : class;
    public void Subscribe<T>(string channel, Action<T> action) where T : class;
}
