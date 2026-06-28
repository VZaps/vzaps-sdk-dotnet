namespace VZaps.Models;

public sealed class WebhookSetRequest : InstanceScopedRequest
{
    public string WebhookURL { get; set; } = string.Empty;

    public object? Events { get; set; }
}

public sealed class WebhookLogSearchRequest : InstanceScopedRequest
{
}

public sealed class WebhookLogRequest : InstanceScopedRequest
{
    public string LogId { get; set; } = string.Empty;
}

public sealed class GroupListRequest : PagedInstanceRequest
{
}

public class GroupInfoRequest : InstanceScopedRequest
{
    public string GroupId { get; set; } = string.Empty;
}

public sealed class GroupInviteLinkRequest : GroupInfoRequest
{
    public bool? Reset { get; set; }
}

public sealed class GroupMutationRequest : InstanceScopedRequest
{
    public string? GroupId { get; set; }

    public string? Image { get; set; }

    public string? Name { get; set; }

    public string? Description { get; set; }

    public bool? AdminOnlyMessage { get; set; }

    public bool? AdminOnlySettings { get; set; }

    public int? DelayMessage { get; set; }

    public string? GroupName { get; set; }

    public string? GroupDescription { get; set; }

    public string? GroupImage { get; set; }

    public IReadOnlyList<string>? Participants { get; set; }
}

public sealed class QueueRequest : InstanceScopedRequest
{
}

public sealed class QueueMessageRequest : InstanceScopedRequest
{
    public string MessageId { get; set; } = string.Empty;
}

public class TypeBotRequest : InstanceScopedRequest
{
    public bool? Enabled { get; set; }

    public string? Description { get; set; }

    public string? TypebotUrl { get; set; }

    public string? PublicId { get; set; }

    public string? TriggerType { get; set; }

    public string? TriggerOperator { get; set; }

    public string? TriggerValue { get; set; }

    public int? Priority { get; set; }

    public int? ExpireInMinutes { get; set; }

    public string? KeywordFinish { get; set; }

    public int? DefaultDelayMs { get; set; }

    public string? UnknownMessage { get; set; }

    public bool? ListenFromMe { get; set; }

    public bool? StopBotFromMe { get; set; }

    public bool? KeepOpen { get; set; }

    public int? DebounceMs { get; set; }

    public bool? IgnoreGroups { get; set; }

    public bool? TranscribeAudio { get; set; }
}

public sealed class TypeBotMutationRequest : TypeBotRequest
{
    public string TypebotId { get; set; } = string.Empty;
}

public sealed class TypeBotSessionRequest : InstanceScopedRequest
{
    public string Session { get; set; } = string.Empty;
}

public sealed class TypeBotStartSessionRequest : InstanceScopedRequest
{
    public string? TypebotId { get; set; }

    public string? PublicId { get; set; }

    public string Phone { get; set; } = string.Empty;

    public string? PushName { get; set; }

    public string Message { get; set; } = string.Empty;
}

public sealed class ChatwootSetRequest : InstanceScopedRequest
{
    public bool? Enabled { get; set; }

    public string? Url { get; set; }

    public string? AccountId { get; set; }

    public string? Token { get; set; }

    public string? NameInbox { get; set; }

    public bool? SignMsg { get; set; }

    public string? SignDelimiter { get; set; }

    public string? Number { get; set; }

    public bool? ReopenConversation { get; set; }

    public bool? ConversationPending { get; set; }

    public bool? ImportContacts { get; set; }

    public bool? ImportMessages { get; set; }

    public int? DaysLimitImportMessages { get; set; }

    public bool? AutoCreate { get; set; }

    public string? Organization { get; set; }

    public string? Logo { get; set; }

    public object? IgnoreJids { get; set; }

    public bool? IgnoreGroups { get; set; }
}

public sealed class ChatwootImportRequest : InstanceScopedRequest
{
    public string What { get; set; } = "all";
}

public class ChatRequest : InstanceScopedRequest
{
    public string Phone { get; set; } = string.Empty;
}

public sealed class ChatListRequest : PagedInstanceRequest
{
}

public sealed class ChatDeleteRequest : ChatRequest
{
    public bool? DeleteMedia { get; set; }
}

public sealed class ChatMuteRequest : ChatRequest
{
    public int? DurationSeconds { get; set; }
}

public sealed class ChatClearRequest : ChatRequest
{
    public bool? DeleteMedia { get; set; }
}

public sealed class ChatExpirationRequest : ChatRequest
{
    public string Expiration { get; set; } = string.Empty;
}

public sealed class SessionBusinessCategory
{
    public string Id { get; set; } = string.Empty;

    public string Name { get; set; } = string.Empty;
}

public sealed class SessionBusinessProfile
{
    public string? BusinessHoursTimezone { get; set; }

    public List<SessionBusinessCategory>? Categories { get; set; }

    public Dictionary<string, string>? ProfileOptions { get; set; }

    public string? Address { get; set; }

    public string? Email { get; set; }
}

public sealed class SessionStatusData
{
    public bool Connected { get; set; }

    public string? Phone { get; set; }

    public string? WhatsappJid { get; set; }

    public string? PushName { get; set; }

    public string? BusinessName { get; set; }

    public SessionBusinessProfile? BusinessProfile { get; set; }

    public string? ProfilePictureId { get; set; }

    public string? ProfilePictureUrl { get; set; }

    public string? ProfileUrl { get; set; }

    public string? VerifiedName { get; set; }

    public string? About { get; set; }

    public string? Website { get; set; }
}

public sealed class SessionStatusResponse
{
    public int Code { get; set; }

    public bool Success { get; set; }

    public SessionStatusData Data { get; set; } = new();
}
