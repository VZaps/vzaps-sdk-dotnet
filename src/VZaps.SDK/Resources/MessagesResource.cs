using VZaps.Http;
using VZaps.Models;

namespace VZaps.Resources;

public sealed class MessagesResource : BaseResource
{
    internal MessagesResource(VZapsHttpClient http)
        : base(http)
    {
    }

    public Task<TResponse?> SendTextAsync<TResponse>(SendTextMessageRequest request, CancellationToken cancellationToken = default) => SendMessageAsync<TResponse>(HttpMethod.Post, "/chat/send/text", request, cancellationToken);

    public Task<TResponse?> SendImageAsync<TResponse>(SendImageMessageRequest request, CancellationToken cancellationToken = default) => SendMessageAsync<TResponse>(HttpMethod.Post, "/chat/send/image", request, cancellationToken);

    public Task<TResponse?> SendAudioAsync<TResponse>(SendAudioMessageRequest request, CancellationToken cancellationToken = default) => SendMessageAsync<TResponse>(HttpMethod.Post, "/chat/send/audio", request, cancellationToken);

    public Task<TResponse?> SendDocumentAsync<TResponse>(SendDocumentMessageRequest request, CancellationToken cancellationToken = default) => SendMessageAsync<TResponse>(HttpMethod.Post, "/chat/send/document", request, cancellationToken);

    public Task<TResponse?> SendVideoAsync<TResponse>(SendVideoMessageRequest request, CancellationToken cancellationToken = default) => SendMessageAsync<TResponse>(HttpMethod.Post, "/chat/send/video", request, cancellationToken);

    public Task<TResponse?> SendStickerAsync<TResponse>(SendStickerMessageRequest request, CancellationToken cancellationToken = default) => SendMessageAsync<TResponse>(HttpMethod.Post, "/chat/send/sticker", request, cancellationToken);

    public Task<TResponse?> SendGifAsync<TResponse>(SendGifMessageRequest request, CancellationToken cancellationToken = default) => SendMessageAsync<TResponse>(HttpMethod.Post, "/chat/send/gif", request, cancellationToken);

    public Task<TResponse?> SendLocationAsync<TResponse>(SendLocationMessageRequest request, CancellationToken cancellationToken = default) => SendMessageAsync<TResponse>(HttpMethod.Post, "/chat/send/location", request, cancellationToken);

    public Task<TResponse?> SendContactAsync<TResponse>(SendContactMessageRequest request, CancellationToken cancellationToken = default) => SendMessageAsync<TResponse>(HttpMethod.Post, "/chat/send/contact", request, cancellationToken);

    public Task<TResponse?> SendButtonsAsync<TResponse>(SendButtonsMessageRequest request, CancellationToken cancellationToken = default) => SendMessageAsync<TResponse>(HttpMethod.Post, "/chat/send/buttons", request, cancellationToken);

    public Task<TResponse?> SendListAsync<TResponse>(SendListMessageRequest request, CancellationToken cancellationToken = default) => SendMessageAsync<TResponse>(HttpMethod.Post, "/chat/send/list", request, cancellationToken);

    public Task<TResponse?> SendLinkAsync<TResponse>(SendLinkMessageRequest request, CancellationToken cancellationToken = default) => SendMessageAsync<TResponse>(HttpMethod.Post, "/chat/send/link", request, cancellationToken);

    public Task<TResponse?> SendPollAsync<TResponse>(SendPollMessageRequest request, CancellationToken cancellationToken = default) => SendMessageAsync<TResponse>(HttpMethod.Post, "/chat/send/poll", request, cancellationToken);

    public Task<TResponse?> PollVoteAsync<TResponse>(MessagePollVoteRequest request, CancellationToken cancellationToken = default) => SendMessageAsync<TResponse>(HttpMethod.Post, "/chat/poll/vote", request, cancellationToken);

    public Task<TResponse?> ReactAsync<TResponse>(MessageReactRequest request, CancellationToken cancellationToken = default) => SendMessageAsync<TResponse>(HttpMethod.Post, "/chat/react", request, cancellationToken);

    public Task<TResponse?> RemoveReactionAsync<TResponse>(MessageReactRemoveRequest request, CancellationToken cancellationToken = default) => SendMessageAsync<TResponse>(HttpMethod.Delete, "/chat/react", request, cancellationToken);

    public Task<TResponse?> PresenceAsync<TResponse>(MessagePresenceRequest request, CancellationToken cancellationToken = default) => SendMessageAsync<TResponse>(HttpMethod.Post, "/chat/presence", request, cancellationToken);

    public Task<TResponse?> MarkReadAsync<TResponse>(MessageMarkReadRequest request, CancellationToken cancellationToken = default) => SendMessageAsync<TResponse>(HttpMethod.Post, "/chat/markread", request, cancellationToken);

    public Task<TResponse?> DownloadImageAsync<TResponse>(MessageDownloadRequest request, CancellationToken cancellationToken = default) => SendMessageAsync<TResponse>(HttpMethod.Post, "/chat/downloadimage", request, cancellationToken);

    public Task<TResponse?> DownloadVideoAsync<TResponse>(MessageDownloadRequest request, CancellationToken cancellationToken = default) => SendMessageAsync<TResponse>(HttpMethod.Post, "/chat/downloadvideo", request, cancellationToken);

    public Task<TResponse?> DownloadAudioAsync<TResponse>(MessageDownloadRequest request, CancellationToken cancellationToken = default) => SendMessageAsync<TResponse>(HttpMethod.Post, "/chat/downloadaudio", request, cancellationToken);

    public Task<TResponse?> DownloadDocumentAsync<TResponse>(MessageDownloadRequest request, CancellationToken cancellationToken = default) => SendMessageAsync<TResponse>(HttpMethod.Post, "/chat/downloaddocument", request, cancellationToken);

    public Task<TResponse?> EditAsync<TResponse>(MessageEditRequest request, CancellationToken cancellationToken = default)
    {
        return SendMessageAsync<TResponse>(Patch, $"/chat/messages/{Escape(request.MessageId)}", request, cancellationToken, "MessageId");
    }

    public Task<TResponse?> DeleteAsync<TResponse>(MessageDeleteRequest request, CancellationToken cancellationToken = default)
    {
        return SendMessageAsync<TResponse>(HttpMethod.Delete, $"/chat/messages/{Escape(request.MessageId)}", request, cancellationToken, "MessageId");
    }

    public Task<TResponse?> SendAsync<TResponse>(string instanceId, string path, object body, string? instanceToken = null, CancellationToken cancellationToken = default)
    {
        return SendAsync<TResponse>(HttpMethod.Post, $"/instances/{Escape(instanceId)}/chat/{path.TrimStart('/')}", body, instanceToken, cancellationToken: cancellationToken);
    }

    private Task<TResponse?> SendMessageAsync<TResponse>(HttpMethod method, string suffix, InstanceScopedRequest request, CancellationToken cancellationToken, params string[] excluded)
    {
        return SendAsync<TResponse>(method, $"/instances/{Escape(request.InstanceId)}{suffix}", BodyWithoutInstance(request, excluded), request.InstanceToken, cancellationToken: cancellationToken);
    }
}
