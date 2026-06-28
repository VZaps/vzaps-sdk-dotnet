using VZaps.Http;
using VZaps.Models;

namespace VZaps.Resources;

public sealed class SessionsResource : BaseResource
{
    internal SessionsResource(VZapsHttpClient http) : base(http) { }

    public Task<SessionStatusResponse?> StatusAsync(string instanceId, InstanceRequestOptions? options = null, CancellationToken cancellationToken = default) => StatusAsync<SessionStatusResponse>(instanceId, options, cancellationToken);

    public Task<TResponse?> StatusAsync<TResponse>(string instanceId, InstanceRequestOptions? options = null, CancellationToken cancellationToken = default) => SendAsync<TResponse>(HttpMethod.Get, $"/instances/{Escape(instanceId)}/session/status", instanceToken: options?.InstanceToken, cancellationToken: cancellationToken);

    public Task<TResponse?> QrAsync<TResponse>(string instanceId, InstanceRequestOptions? options = null, CancellationToken cancellationToken = default) => SendAsync<TResponse>(HttpMethod.Get, $"/instances/{Escape(instanceId)}/session/qr", instanceToken: options?.InstanceToken, cancellationToken: cancellationToken);

    public Task<TResponse?> PairCodeAsync<TResponse>(string instanceId, string phone, InstanceRequestOptions? options = null, CancellationToken cancellationToken = default) => SendAsync<TResponse>(HttpMethod.Get, $"/instances/{Escape(instanceId)}/session/paircode/{Escape(phone)}", instanceToken: options?.InstanceToken, cancellationToken: cancellationToken);

    public Task<TResponse?> DisconnectAsync<TResponse>(string instanceId, InstanceRequestOptions? options = null, CancellationToken cancellationToken = default) => SendAsync<TResponse>(HttpMethod.Post, $"/instances/{Escape(instanceId)}/session/disconnect", instanceToken: options?.InstanceToken, cancellationToken: cancellationToken);
}

public sealed class WebhooksResource : BaseResource
{
    internal WebhooksResource(VZapsHttpClient http) : base(http) { }

    public Task<TResponse?> GetAsync<TResponse>(string instanceId, InstanceRequestOptions? options = null, CancellationToken cancellationToken = default) => SendAsync<TResponse>(HttpMethod.Get, $"/instances/{Escape(instanceId)}/webhook", instanceToken: options?.InstanceToken, cancellationToken: cancellationToken);

    public Task<TResponse?> SetAsync<TResponse>(WebhookSetRequest request, CancellationToken cancellationToken = default) => InstanceRequestAsync<TResponse>(HttpMethod.Post, "/webhook", request, cancellationToken);

    public Task<TResponse?> SearchLogsAsync<TResponse>(WebhookLogSearchRequest request, CancellationToken cancellationToken = default) => InstanceRequestAsync<TResponse>(HttpMethod.Post, "/webhook/logs/search", request, cancellationToken);

    public Task<TResponse?> GetLogAsync<TResponse>(WebhookLogRequest request, CancellationToken cancellationToken = default) => SendAsync<TResponse>(HttpMethod.Get, $"/instances/{Escape(request.InstanceId)}/webhook/logs/{Escape(request.LogId)}", instanceToken: request.InstanceToken, cancellationToken: cancellationToken);

    public Task<TResponse?> RetryLogAsync<TResponse>(WebhookLogRequest request, CancellationToken cancellationToken = default) => SendAsync<TResponse>(HttpMethod.Post, $"/instances/{Escape(request.InstanceId)}/webhook/logs/{Escape(request.LogId)}/retry", instanceToken: request.InstanceToken, cancellationToken: cancellationToken);

    private Task<TResponse?> InstanceRequestAsync<TResponse>(HttpMethod method, string suffix, InstanceScopedRequest request, CancellationToken cancellationToken)
    {
        return SendAsync<TResponse>(method, $"/instances/{Escape(request.InstanceId)}{suffix}", BodyWithoutInstance(request), request.InstanceToken, cancellationToken: cancellationToken);
    }
}

public sealed class ContactsResource : BaseResource
{
    internal ContactsResource(VZapsHttpClient http) : base(http) { }

    public Task<TResponse?> ListAsync<TResponse>(string instanceId, InstanceRequestOptions? options = null, CancellationToken cancellationToken = default) => SendAsync<TResponse>(HttpMethod.Get, $"/instances/{Escape(instanceId)}/contact/list", instanceToken: options?.InstanceToken, cancellationToken: cancellationToken);

    public Task<TResponse?> AddAsync<TResponse>(ContactAddRequest request, CancellationToken cancellationToken = default) => SendAsync<TResponse>(HttpMethod.Post, $"/instances/{Escape(request.InstanceId)}/contact/add", BodyWithoutInstance(request), request.InstanceToken, cancellationToken: cancellationToken);
}

public sealed class GroupsResource : BaseResource
{
    internal GroupsResource(VZapsHttpClient http) : base(http) { }

    public Task<TResponse?> ListAsync<TResponse>(GroupListRequest request, CancellationToken cancellationToken = default) => SendAsync<TResponse>(HttpMethod.Get, $"/instances/{Escape(request.InstanceId)}/group/list", instanceToken: request.InstanceToken, query: new Dictionary<string, object?> { ["page"] = request.Page, ["pageSize"] = request.PageSize }, cancellationToken: cancellationToken);

    public Task<TResponse?> GetAsync<TResponse>(GroupInfoRequest request, CancellationToken cancellationToken = default) => SendAsync<TResponse>(HttpMethod.Get, $"/instances/{Escape(request.InstanceId)}/group/info", instanceToken: request.InstanceToken, query: new Dictionary<string, object?> { ["groupId"] = request.GroupId }, cancellationToken: cancellationToken);

    public Task<TResponse?> InviteLinkAsync<TResponse>(GroupInviteLinkRequest request, CancellationToken cancellationToken = default) => SendAsync<TResponse>(HttpMethod.Get, $"/instances/{Escape(request.InstanceId)}/group/invitelink", instanceToken: request.InstanceToken, query: new Dictionary<string, object?> { ["groupId"] = request.GroupId, ["reset"] = request.Reset }, cancellationToken: cancellationToken);

    public Task<TResponse?> SetPhotoAsync<TResponse>(GroupMutationRequest request, CancellationToken cancellationToken = default) => PostAsync<TResponse>("/group/photo", request, cancellationToken);

    public Task<TResponse?> SetNameAsync<TResponse>(GroupMutationRequest request, CancellationToken cancellationToken = default) => PostAsync<TResponse>("/group/name", request, cancellationToken);

    public Task<TResponse?> SetDescriptionAsync<TResponse>(GroupMutationRequest request, CancellationToken cancellationToken = default) => PostAsync<TResponse>("/group/description", request, cancellationToken);

    public Task<TResponse?> SetSettingsAsync<TResponse>(GroupMutationRequest request, CancellationToken cancellationToken = default) => PostAsync<TResponse>("/group/settings", request, cancellationToken);

    public Task<TResponse?> CreateAsync<TResponse>(GroupMutationRequest request, CancellationToken cancellationToken = default) => PostAsync<TResponse>("/group/create", request, cancellationToken);

    public Task<TResponse?> AddAdminAsync<TResponse>(GroupMutationRequest request, CancellationToken cancellationToken = default) => PostAsync<TResponse>("/group/add-admin", request, cancellationToken);

    public Task<TResponse?> RemoveAdminAsync<TResponse>(GroupMutationRequest request, CancellationToken cancellationToken = default) => PostAsync<TResponse>("/group/remove-admin", request, cancellationToken);

    private Task<TResponse?> PostAsync<TResponse>(string suffix, GroupMutationRequest request, CancellationToken cancellationToken)
    {
        return SendAsync<TResponse>(HttpMethod.Post, $"/instances/{Escape(request.InstanceId)}{suffix}", BodyWithoutInstance(request), request.InstanceToken, cancellationToken: cancellationToken);
    }
}

public sealed class UsersResource : BaseResource
{
    internal UsersResource(VZapsHttpClient http) : base(http) { }

    public Task<TResponse?> InfoAsync<TResponse>(UserPhonesRequest request, CancellationToken cancellationToken = default) => PostAsync<TResponse>("/user/info", request, cancellationToken);

    public Task<TResponse?> CheckAsync<TResponse>(UserPhonesRequest request, CancellationToken cancellationToken = default) => PostAsync<TResponse>("/user/check", request, cancellationToken);

    public Task<TResponse?> AvatarAsync<TResponse>(UserAvatarRequest request, CancellationToken cancellationToken = default) => PostAsync<TResponse>("/user/avatar", request, cancellationToken);

    public Task<TResponse?> ContactsAsync<TResponse>(string instanceId, InstanceRequestOptions? options = null, CancellationToken cancellationToken = default) => SendAsync<TResponse>(HttpMethod.Get, $"/instances/{Escape(instanceId)}/user/contacts", instanceToken: options?.InstanceToken, cancellationToken: cancellationToken);

    private Task<TResponse?> PostAsync<TResponse>(string suffix, InstanceScopedRequest request, CancellationToken cancellationToken)
    {
        return SendAsync<TResponse>(HttpMethod.Post, $"/instances/{Escape(request.InstanceId)}{suffix}", BodyWithoutInstance(request), request.InstanceToken, cancellationToken: cancellationToken);
    }
}

public sealed class QueuesResource : BaseResource
{
    internal QueuesResource(VZapsHttpClient http) : base(http) { }

    public Task<TResponse?> ListMessagesAsync<TResponse>(QueueRequest request, CancellationToken cancellationToken = default) => InstanceAsync<TResponse>(HttpMethod.Get, "/queue/messages", request, cancellationToken);

    public Task<TResponse?> PurgeMessagesAsync<TResponse>(QueueRequest request, CancellationToken cancellationToken = default) => InstanceAsync<TResponse>(HttpMethod.Delete, "/queue/messages", request, cancellationToken);

    public Task<TResponse?> RemoveMessageAsync<TResponse>(QueueMessageRequest request, CancellationToken cancellationToken = default) => InstanceAsync<TResponse>(HttpMethod.Delete, $"/queue/messages/{Escape(request.MessageId)}", request, cancellationToken, "MessageId");

    public Task<TResponse?> ListOperationsAsync<TResponse>(QueueRequest request, CancellationToken cancellationToken = default) => InstanceAsync<TResponse>(HttpMethod.Get, "/queue/operations", request, cancellationToken);

    public Task<TResponse?> PurgeOperationsAsync<TResponse>(QueueRequest request, CancellationToken cancellationToken = default) => InstanceAsync<TResponse>(HttpMethod.Delete, "/queue/operations", request, cancellationToken);

    public Task<TResponse?> RemoveOperationAsync<TResponse>(QueueMessageRequest request, CancellationToken cancellationToken = default) => InstanceAsync<TResponse>(HttpMethod.Delete, $"/queue/operations/{Escape(request.MessageId)}", request, cancellationToken, "MessageId");

    private Task<TResponse?> InstanceAsync<TResponse>(HttpMethod method, string suffix, InstanceScopedRequest request, CancellationToken cancellationToken, params string[] excluded)
    {
        return SendAsync<TResponse>(method, $"/instances/{Escape(request.InstanceId)}{suffix}", BodyWithoutInstance(request, excluded), request.InstanceToken, cancellationToken: cancellationToken);
    }
}

public sealed class TypeBotsResource : BaseResource
{
    internal TypeBotsResource(VZapsHttpClient http) : base(http) { }

    public Task<TResponse?> ListAsync<TResponse>(string instanceId, InstanceRequestOptions? options = null, CancellationToken cancellationToken = default) => SendAsync<TResponse>(HttpMethod.Get, $"/instances/{Escape(instanceId)}/typebots", instanceToken: options?.InstanceToken, cancellationToken: cancellationToken);

    public Task<TResponse?> CreateAsync<TResponse>(TypeBotRequest request, CancellationToken cancellationToken = default) => InstanceAsync<TResponse>(HttpMethod.Post, "/typebots", request, cancellationToken);

    public Task<TResponse?> UpdateAsync<TResponse>(TypeBotMutationRequest request, CancellationToken cancellationToken = default) => InstanceAsync<TResponse>(Patch, $"/typebots/{Escape(request.TypebotId)}", request, cancellationToken, "TypebotId");

    public Task<TResponse?> DeleteAsync<TResponse>(TypeBotMutationRequest request, CancellationToken cancellationToken = default) => InstanceAsync<TResponse>(HttpMethod.Delete, $"/typebots/{Escape(request.TypebotId)}", request, cancellationToken, "TypebotId");

    public Task<TResponse?> StartSessionAsync<TResponse>(TypeBotStartSessionRequest request, CancellationToken cancellationToken = default)
    {
        var path = string.IsNullOrWhiteSpace(request.TypebotId)
            ? "/typebots/sessions/start"
            : $"/typebots/{Escape(request.TypebotId!)}/sessions/start";
        return InstanceAsync<TResponse>(HttpMethod.Post, path, request, cancellationToken, "TypebotId");
    }

    public Task<TResponse?> ListSessionsAsync<TResponse>(string instanceId, InstanceRequestOptions? options = null, CancellationToken cancellationToken = default) => SendAsync<TResponse>(HttpMethod.Get, $"/instances/{Escape(instanceId)}/typebots/sessions", instanceToken: options?.InstanceToken, cancellationToken: cancellationToken);

    public Task<TResponse?> PauseSessionAsync<TResponse>(TypeBotSessionRequest request, CancellationToken cancellationToken = default) => InstanceAsync<TResponse>(HttpMethod.Post, $"/typebots/sessions/{Escape(request.Session)}/pause", request, cancellationToken, "Session");

    public Task<TResponse?> CloseSessionAsync<TResponse>(TypeBotSessionRequest request, CancellationToken cancellationToken = default) => InstanceAsync<TResponse>(HttpMethod.Post, $"/typebots/sessions/{Escape(request.Session)}/close", request, cancellationToken, "Session");

    private Task<TResponse?> InstanceAsync<TResponse>(HttpMethod method, string suffix, InstanceScopedRequest request, CancellationToken cancellationToken, params string[] excluded)
    {
        return SendAsync<TResponse>(method, $"/instances/{Escape(request.InstanceId)}{suffix}", BodyWithoutInstance(request, excluded), request.InstanceToken, cancellationToken: cancellationToken);
    }
}

public sealed class ChatwootResource : BaseResource
{
    internal ChatwootResource(VZapsHttpClient http) : base(http) { }

    public Task<TResponse?> GetAsync<TResponse>(string instanceId, InstanceRequestOptions? options = null, CancellationToken cancellationToken = default) => SendAsync<TResponse>(HttpMethod.Get, $"/instances/{Escape(instanceId)}/chatwoot", instanceToken: options?.InstanceToken, cancellationToken: cancellationToken);

    public Task<TResponse?> SetAsync<TResponse>(ChatwootSetRequest request, CancellationToken cancellationToken = default) => InstanceAsync<TResponse>(HttpMethod.Post, "/chatwoot", request, cancellationToken);

    public Task<TResponse?> DeleteAsync<TResponse>(string instanceId, InstanceRequestOptions? options = null, CancellationToken cancellationToken = default) => SendAsync<TResponse>(HttpMethod.Delete, $"/instances/{Escape(instanceId)}/chatwoot", instanceToken: options?.InstanceToken, cancellationToken: cancellationToken);

    public Task<TResponse?> TriggerImportAsync<TResponse>(ChatwootImportRequest request, CancellationToken cancellationToken = default) => InstanceAsync<TResponse>(HttpMethod.Post, $"/chatwoot/import/{Escape(request.What)}", request, cancellationToken, "What");

    private Task<TResponse?> InstanceAsync<TResponse>(HttpMethod method, string suffix, InstanceScopedRequest request, CancellationToken cancellationToken, params string[] excluded)
    {
        return SendAsync<TResponse>(method, $"/instances/{Escape(request.InstanceId)}{suffix}", BodyWithoutInstance(request, excluded), request.InstanceToken, cancellationToken: cancellationToken);
    }
}

public sealed class ChatsResource : BaseResource
{
    internal ChatsResource(VZapsHttpClient http) : base(http) { }

    public Task<TResponse?> ListAsync<TResponse>(ChatListRequest request, CancellationToken cancellationToken = default) => SendAsync<TResponse>(HttpMethod.Get, $"/instances/{Escape(request.InstanceId)}/chats", instanceToken: request.InstanceToken, query: new Dictionary<string, object?> { ["page"] = request.Page, ["pageSize"] = request.PageSize }, cancellationToken: cancellationToken);

    public Task<TResponse?> GetAsync<TResponse>(ChatRequest request, CancellationToken cancellationToken = default) => ChatActionAsync<TResponse>(HttpMethod.Get, request, string.Empty, cancellationToken);

    public Task<TResponse?> ArchiveAsync<TResponse>(ChatRequest request, CancellationToken cancellationToken = default) => ChatActionAsync<TResponse>(HttpMethod.Post, request, "/archive", cancellationToken);

    public Task<TResponse?> UnarchiveAsync<TResponse>(ChatRequest request, CancellationToken cancellationToken = default) => ChatActionAsync<TResponse>(HttpMethod.Post, request, "/unarchive", cancellationToken);

    public Task<TResponse?> MuteAsync<TResponse>(ChatMuteRequest request, CancellationToken cancellationToken = default) => ChatActionAsync<TResponse>(HttpMethod.Post, request, "/mute", cancellationToken);

    public Task<TResponse?> UnmuteAsync<TResponse>(ChatRequest request, CancellationToken cancellationToken = default) => ChatActionAsync<TResponse>(HttpMethod.Post, request, "/unmute", cancellationToken);

    public Task<TResponse?> PinAsync<TResponse>(ChatRequest request, CancellationToken cancellationToken = default) => ChatActionAsync<TResponse>(HttpMethod.Post, request, "/pin", cancellationToken);

    public Task<TResponse?> UnpinAsync<TResponse>(ChatRequest request, CancellationToken cancellationToken = default) => ChatActionAsync<TResponse>(HttpMethod.Post, request, "/unpin", cancellationToken);

    public Task<TResponse?> ReadAsync<TResponse>(ChatRequest request, CancellationToken cancellationToken = default) => ChatActionAsync<TResponse>(HttpMethod.Post, request, "/read", cancellationToken);

    public Task<TResponse?> UnreadAsync<TResponse>(ChatRequest request, CancellationToken cancellationToken = default) => ChatActionAsync<TResponse>(HttpMethod.Post, request, "/unread", cancellationToken);

    public Task<TResponse?> ClearAsync<TResponse>(ChatClearRequest request, CancellationToken cancellationToken = default) => ChatActionAsync<TResponse>(HttpMethod.Post, request, "/clear", cancellationToken);

    public Task<TResponse?> DeleteAsync<TResponse>(ChatDeleteRequest request, CancellationToken cancellationToken = default) => ChatActionAsync<TResponse>(HttpMethod.Delete, request, string.Empty, cancellationToken);

    public Task<TResponse?> SetExpirationAsync<TResponse>(ChatExpirationRequest request, CancellationToken cancellationToken = default) => ChatActionAsync<TResponse>(HttpMethod.Put, request, "/expiration", cancellationToken);

    private Task<TResponse?> ChatActionAsync<TResponse>(HttpMethod method, ChatRequest request, string suffix, CancellationToken cancellationToken)
    {
        return SendAsync<TResponse>(method, $"/instances/{Escape(request.InstanceId)}/chats/{Escape(request.Phone)}{suffix}", BodyWithoutInstance(request, "Phone"), request.InstanceToken, cancellationToken: cancellationToken);
    }
}
