using System.Text.Json;
using System.Text.Json.Serialization;

namespace VZaps.Models;

public sealed class VZapsEventSubscribeRequest
{
    public string InstanceId { get; set; } = string.Empty;

    public string InstanceToken { get; set; } = string.Empty;

    public IReadOnlyList<VZapsEventType>? Events { get; set; }

    public bool Reconnect { get; set; } = true;

    public int MaxRetries { get; set; } = 10;

    public TimeSpan RetryDelay { get; set; } = TimeSpan.FromSeconds(1);

    public string? LastEventId { get; set; }
}

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum VZapsEventType
{
    Message,
    ReadReceipt,
    Presence,
    HistorySync,
    ChatPresence,
    Connected,
    Disconnected,
    GroupParticipantsAdd,
    GroupParticipantsRemove,
    All,
}

public sealed class VZapsEvent<TData> : VZapsModel
{
    public string Id { get; set; } = string.Empty;

    public string Type { get; set; } = string.Empty;

    public string InstanceId { get; set; } = string.Empty;

    public DateTimeOffset? CreatedAt { get; set; }

    public TData? Data { get; set; }
}

public sealed class VZapsEvent : VZapsModel
{
    public string Id { get; set; } = string.Empty;

    public string Type { get; set; } = string.Empty;

    public string InstanceId { get; set; } = string.Empty;

    public DateTimeOffset? CreatedAt { get; set; }

    public JsonElement Data { get; set; }
}
