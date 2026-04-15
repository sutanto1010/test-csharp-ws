namespace RealtimeDemo.Api.Services;

public interface IRealtimeNotifier
{
    Task BroadcastAsync<T>(string eventName, T payload, CancellationToken cancellationToken = default);
}
