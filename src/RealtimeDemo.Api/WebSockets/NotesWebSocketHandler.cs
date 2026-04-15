using System.Net.WebSockets;
using RealtimeDemo.Api.Services;

namespace RealtimeDemo.Api.WebSockets;

public sealed class NotesWebSocketHandler(WebSocketConnectionManager connectionManager, ILogger<NotesWebSocketHandler> logger)
{
    public async Task HandleAsync(WebSocket socket, CancellationToken cancellationToken)
    {
        var connectionId = connectionManager.Add(socket);

        try
        {
            await connectionManager.SendAsync(
                socket,
                new
                {
                    Type = "connected",
                    Data = new
                    {
                        ConnectionId = connectionId,
                        ActiveConnections = connectionManager.ConnectionCount
                    }
                },
                cancellationToken);

            while (socket.State == WebSocketState.Open && !cancellationToken.IsCancellationRequested)
            {
                var message = await WebSocketConnectionManager.ReceiveTextAsync(socket, cancellationToken);

                if (message is null)
                {
                    break;
                }

                if (string.Equals(message.Trim(), "ping", StringComparison.OrdinalIgnoreCase))
                {
                    await connectionManager.SendAsync(
                        socket,
                        new
                        {
                            Type = "pong",
                            Data = new
                            {
                                Message = "pong"
                            }
                        },
                        cancellationToken);

                    continue;
                }

                await connectionManager.SendAsync(
                    socket,
                    new
                    {
                        Type = "echo",
                        Data = new
                        {
                            Message = message
                        }
                    },
                    cancellationToken);
            }
        }
        catch (OperationCanceledException)
        {
            logger.LogInformation("WebSocket connection {ConnectionId} cancelled.", connectionId);
        }
        finally
        {
            await connectionManager.RemoveAsync(connectionId);
            logger.LogInformation("WebSocket connection {ConnectionId} closed.", connectionId);
        }
    }
}
