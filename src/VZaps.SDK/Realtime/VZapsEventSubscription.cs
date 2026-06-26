using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using VZaps.Models;
using VZaps.Serialization;

namespace VZaps.Realtime;

public sealed class VZapsEventSubscription : IAsyncDisposable
{
    private readonly VZapsClientOptions _options;
    private readonly VZapsEventSubscribeRequest _request;
    private readonly string _accessToken;
    private readonly CancellationTokenSource _disposeCts = new();
    private readonly Dictionary<string, List<Func<VZapsEvent, Task>>> _handlers = new(StringComparer.OrdinalIgnoreCase);
    private readonly List<Func<Exception, Task>> _errorHandlers = new();
    private Task? _runTask;
    private string? _lastEventId;

    internal VZapsEventSubscription(VZapsClientOptions options, VZapsEventSubscribeRequest request, string accessToken)
    {
        _options = options;
        _request = request;
        _accessToken = accessToken;
        _lastEventId = request.LastEventId;
    }

    public void On(VZapsEventType eventType, Func<VZapsEvent, Task> handler)
    {
        On(eventType.ToString(), handler);
    }

    public void On(VZapsEventType eventType, Action<VZapsEvent> handler)
    {
        On(eventType.ToString(), handler);
    }

    public void On(string eventType, Func<VZapsEvent, Task> handler)
    {
        if (string.IsNullOrWhiteSpace(eventType))
        {
            throw new ArgumentException("Event type is required.", nameof(eventType));
        }

        if (handler is null)
        {
            throw new ArgumentNullException(nameof(handler));
        }

        lock (_handlers)
        {
            if (!_handlers.TryGetValue(eventType, out var handlers))
            {
                handlers = new List<Func<VZapsEvent, Task>>();
                _handlers[eventType] = handlers;
            }

            handlers.Add(handler);
        }
    }

    public void On(string eventType, Action<VZapsEvent> handler)
    {
        if (handler is null)
        {
            throw new ArgumentNullException(nameof(handler));
        }

        On(eventType, evt =>
        {
            handler(evt);
            return Task.CompletedTask;
        });
    }

    public void OnError(Func<Exception, Task> handler)
    {
        if (handler is null)
        {
            throw new ArgumentNullException(nameof(handler));
        }

        lock (_errorHandlers)
        {
            _errorHandlers.Add(handler);
        }
    }

    public Task WaitForCloseAsync(CancellationToken cancellationToken = default)
    {
        var task = _runTask ?? Task.CompletedTask;
        if (!cancellationToken.CanBeCanceled || task.IsCompleted)
        {
            return task;
        }

        return WaitWithCancellationAsync(task, cancellationToken);
    }

    internal void Start(CancellationToken cancellationToken)
    {
        using var linked = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, _disposeCts.Token);
        _runTask = RunAsync(_disposeCts.Token);
    }

    private async Task RunAsync(CancellationToken cancellationToken)
    {
        var attempt = 0;
        while (!cancellationToken.IsCancellationRequested)
        {
            try
            {
                using var socket = CreateSocket();
                await socket.ConnectAsync(BuildUri(), cancellationToken).ConfigureAwait(false);
                attempt = 0;
                await ReceiveLoopAsync(socket, cancellationToken).ConfigureAwait(false);

                if (!_request.Reconnect)
                {
                    return;
                }
            }
            catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
            {
                return;
            }
            catch (Exception ex)
            {
                await DispatchErrorAsync(ex).ConfigureAwait(false);
                if (!_request.Reconnect || attempt >= _request.MaxRetries)
                {
                    throw new VZapsRealtimeException("The VZaps realtime subscription closed unexpectedly.", ex);
                }
            }

            attempt++;
            await Task.Delay(ComputeDelay(attempt), cancellationToken).ConfigureAwait(false);
        }
    }

    private ClientWebSocket CreateSocket()
    {
        var socket = new ClientWebSocket();
        socket.Options.SetRequestHeader("Authorization", $"Bearer {_accessToken}");
        socket.Options.SetRequestHeader("X-Client-Token", _options.ClientToken);
        socket.Options.SetRequestHeader("X-Instance-Token", _request.InstanceToken);
        return socket;
    }

    private async Task ReceiveLoopAsync(ClientWebSocket socket, CancellationToken cancellationToken)
    {
        var buffer = new byte[16 * 1024];
        while (socket.State == WebSocketState.Open && !cancellationToken.IsCancellationRequested)
        {
            using var message = new MemoryStream();
            WebSocketReceiveResult result;
            do
            {
                result = await socket.ReceiveAsync(new ArraySegment<byte>(buffer), cancellationToken).ConfigureAwait(false);
                if (result.MessageType == WebSocketMessageType.Close)
                {
                    await socket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Closing", cancellationToken).ConfigureAwait(false);
                    return;
                }

                message.Write(buffer, 0, result.Count);
            }
            while (!result.EndOfMessage);

            var json = Encoding.UTF8.GetString(message.ToArray());
            var evt = JsonSerializer.Deserialize<VZapsEvent>(json, VZapsJson.RealtimeOptions);
            if (evt is null)
            {
                continue;
            }

            _lastEventId = evt.Id;
            await DispatchAsync(evt).ConfigureAwait(false);
            await AckAsync(socket, evt.Id, cancellationToken).ConfigureAwait(false);
        }
    }

    private async Task DispatchAsync(VZapsEvent evt)
    {
        var handlers = new List<Func<VZapsEvent, Task>>();
        lock (_handlers)
        {
            if (_handlers.TryGetValue(evt.Type, out var typed))
            {
                handlers.AddRange(typed);
            }

            if (_handlers.TryGetValue(VZapsEventType.All.ToString(), out var all))
            {
                handlers.AddRange(all);
            }
        }

        foreach (var handler in handlers)
        {
            await handler(evt).ConfigureAwait(false);
        }
    }

    private async Task DispatchErrorAsync(Exception exception)
    {
        List<Func<Exception, Task>> handlers;
        lock (_errorHandlers)
        {
            handlers = _errorHandlers.ToList();
        }

        foreach (var handler in handlers)
        {
            await handler(exception).ConfigureAwait(false);
        }
    }

    private async Task AckAsync(ClientWebSocket socket, string eventId, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(eventId) || socket.State != WebSocketState.Open)
        {
            return;
        }

        var payload = JsonSerializer.Serialize(new { type = "ack", id = eventId }, VZapsJson.RealtimeOptions);
        var bytes = Encoding.UTF8.GetBytes(payload);
        await socket.SendAsync(new ArraySegment<byte>(bytes), WebSocketMessageType.Text, endOfMessage: true, cancellationToken).ConfigureAwait(false);
    }

    private Uri BuildUri()
    {
        var builder = new UriBuilder(new Uri(_options.RealtimeUrl, "/events/ws"));
        var query = new List<string>
        {
            "instance_id=" + Uri.EscapeDataString(_request.InstanceId),
        };

        var events = _request.Events is null || _request.Events.Count == 0
            ? null
            : string.Join(",", _request.Events.Select(evt => evt.ToString()));
        if (!string.IsNullOrWhiteSpace(events))
        {
            query.Add("events=" + Uri.EscapeDataString(events));
        }

        if (!string.IsNullOrWhiteSpace(_lastEventId))
        {
            query.Add("last_event_id=" + Uri.EscapeDataString(_lastEventId));
        }

        builder.Query = string.Join("&", query);
        return builder.Uri;
    }

    private TimeSpan ComputeDelay(int attempt)
    {
        var multiplier = Math.Min(attempt, 6);
        var delayMs = _request.RetryDelay.TotalMilliseconds * Math.Pow(2, multiplier - 1);
        return TimeSpan.FromMilliseconds(Math.Min(delayMs, 30_000));
    }

    private static async Task WaitWithCancellationAsync(Task task, CancellationToken cancellationToken)
    {
        var completion = new TaskCompletionSource<object?>();
        using (cancellationToken.Register(static state => ((TaskCompletionSource<object?>)state!).TrySetResult(null), completion))
        {
            var winner = await Task.WhenAny(task, completion.Task).ConfigureAwait(false);
            if (winner == completion.Task)
            {
                throw new OperationCanceledException(cancellationToken);
            }

            await task.ConfigureAwait(false);
        }
    }

    public async ValueTask DisposeAsync()
    {
        _disposeCts.Cancel();
        if (_runTask is not null)
        {
            try
            {
                await _runTask.ConfigureAwait(false);
            }
            catch (OperationCanceledException)
            {
            }
            catch (VZapsRealtimeException)
            {
            }
        }

        _disposeCts.Dispose();
    }
}
