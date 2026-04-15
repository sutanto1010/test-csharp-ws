namespace RealtimeDemo.Api.Services;

public sealed class WebSocketRealtimeNotifier(WebSocketConnectionManager connectionManager) : IRealtimeNotifier
{
    public Task BroadcastAsync<T>(string eventName, T payload, CancellationToken cancellationToken = default)
    {
        var message = new
        {
            Type = eventName,
            Data = payload
        };

        return connectionManager.BroadcastAsync(message, cancellationToken);
    }
}
