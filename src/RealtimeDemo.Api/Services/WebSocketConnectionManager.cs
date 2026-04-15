using System.Collections.Concurrent;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;

namespace RealtimeDemo.Api.Services;

public sealed class WebSocketConnectionManager
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);
    private readonly ConcurrentDictionary<Guid, WebSocket> _connections = new();

    public int ConnectionCount => _connections.Count;

    public Guid Add(WebSocket socket)
    {
        var connectionId = Guid.NewGuid();
        _connections[connectionId] = socket;
        return connectionId;
    }

    public async Task RemoveAsync(Guid connectionId)
    {
        if (_connections.TryRemove(connectionId, out var socket))
        {
            if (socket.State is WebSocketState.Open or WebSocketState.CloseReceived)
            {
                await socket.CloseAsync(
                    WebSocketCloseStatus.NormalClosure,
                    "Connection closed.",
                    CancellationToken.None);
            }

            socket.Dispose();
        }
    }

    public async Task SendAsync(WebSocket socket, object message, CancellationToken cancellationToken)
    {
        if (socket.State != WebSocketState.Open)
        {
            return;
        }

        var payload = JsonSerializer.SerializeToUtf8Bytes(message, JsonOptions);
        await socket.SendAsync(
            new ArraySegment<byte>(payload),
            WebSocketMessageType.Text,
            endOfMessage: true,
            cancellationToken);
    }

    public async Task BroadcastAsync(object message, CancellationToken cancellationToken)
    {
        foreach (var connection in _connections.ToArray())
        {
            var socket = connection.Value;

            if (socket.State != WebSocketState.Open)
            {
                await RemoveAsync(connection.Key);
                continue;
            }

            try
            {
                await SendAsync(socket, message, cancellationToken);
            }
            catch (WebSocketException)
            {
                await RemoveAsync(connection.Key);
            }
        }
    }

    public static async Task<string?> ReceiveTextAsync(WebSocket socket, CancellationToken cancellationToken)
    {
        var buffer = new ArraySegment<byte>(new byte[4 * 1024]);
        using var stream = new MemoryStream();

        while (true)
        {
            var result = await socket.ReceiveAsync(buffer, cancellationToken);

            if (result.MessageType == WebSocketMessageType.Close)
            {
                return null;
            }

            stream.Write(buffer.Array!, buffer.Offset, result.Count);

            if (result.EndOfMessage)
            {
                return Encoding.UTF8.GetString(stream.ToArray());
            }
        }
    }
}
