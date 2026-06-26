namespace VZaps.Models;

public class MessageSendBaseRequest : InstanceScopedRequest
{
    public string Phone { get; set; } = string.Empty;
}

public sealed class SendTextMessageRequest : MessageSendBaseRequest
{
    public string Message { get; set; } = string.Empty;
}

public sealed class SendImageMessageRequest : MessageSendBaseRequest
{
    public string Image { get; set; } = string.Empty;

    public string? Caption { get; set; }
}

public sealed class SendAudioMessageRequest : MessageSendBaseRequest
{
    public string Audio { get; set; } = string.Empty;

    public bool? Ptt { get; set; }
}

public sealed class SendDocumentMessageRequest : MessageSendBaseRequest
{
    public string Document { get; set; } = string.Empty;

    public string? FileName { get; set; }

    public string? Caption { get; set; }
}

public sealed class SendVideoMessageRequest : MessageSendBaseRequest
{
    public string Video { get; set; } = string.Empty;

    public string? Caption { get; set; }
}

public sealed class SendStickerMessageRequest : MessageSendBaseRequest
{
    public string Sticker { get; set; } = string.Empty;
}

public sealed class SendGifMessageRequest : MessageSendBaseRequest
{
    public string Gif { get; set; } = string.Empty;

    public string? Caption { get; set; }
}

public sealed class SendLocationMessageRequest : MessageSendBaseRequest
{
    public double Latitude { get; set; }

    public double Longitude { get; set; }

    public string? Name { get; set; }

    public string? Address { get; set; }
}

public sealed class SendContactMessageRequest : MessageSendBaseRequest
{
    public string? ContactName { get; set; }

    public string? ContactPhone { get; set; }
}

public sealed class MessageButton : VZapsModel
{
    public string Id { get; set; } = string.Empty;

    public string Text { get; set; } = string.Empty;
}

public sealed class SendButtonsMessageRequest : MessageSendBaseRequest
{
    public string Message { get; set; } = string.Empty;

    public IReadOnlyList<MessageButton> Buttons { get; set; } = Array.Empty<MessageButton>();

    public string? Footer { get; set; }
}

public sealed class MessageListRow : VZapsModel
{
    public string Id { get; set; } = string.Empty;

    public string Title { get; set; } = string.Empty;

    public string? Description { get; set; }
}

public sealed class MessageListSection : VZapsModel
{
    public string Title { get; set; } = string.Empty;

    public IReadOnlyList<MessageListRow> Rows { get; set; } = Array.Empty<MessageListRow>();
}

public sealed class SendListMessageRequest : MessageSendBaseRequest
{
    public string Title { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public string ButtonText { get; set; } = string.Empty;

    public IReadOnlyList<MessageListSection> Sections { get; set; } = Array.Empty<MessageListSection>();

    public string? Footer { get; set; }
}

public sealed class SendLinkMessageRequest : MessageSendBaseRequest
{
    public string Message { get; set; } = string.Empty;

    public string LinkUrl { get; set; } = string.Empty;

    public string Title { get; set; } = string.Empty;

    public string LinkDescription { get; set; } = string.Empty;

    public string? JpegThumbnail { get; set; }
}

public sealed class SendPollMessageRequest : MessageSendBaseRequest
{
    public string Name { get; set; } = string.Empty;

    public IReadOnlyList<string> Options { get; set; } = Array.Empty<string>();

    public int? SelectableOptionsCount { get; set; }

    public bool? HideParticipantNames { get; set; }

    public string? EndTime { get; set; }

    public bool? AllowAddOption { get; set; }
}

public sealed class MessagePollVoteRequest : MessageSendBaseRequest
{
    public string MessageId { get; set; } = string.Empty;

    public object? Vote { get; set; }

    public IReadOnlyList<string>? SelectedOptions { get; set; }

    public string? PollSender { get; set; }

    public bool? FromMe { get; set; }
}

public sealed class MessageReactRequest : MessageSendBaseRequest
{
    public string MessageId { get; set; } = string.Empty;

    public string Reaction { get; set; } = string.Empty;
}

public sealed class MessageReactRemoveRequest : MessageSendBaseRequest
{
    public string MessageId { get; set; } = string.Empty;
}

public sealed class MessagePresenceRequest : MessageSendBaseRequest
{
    public string State { get; set; } = string.Empty;

    public string? Media { get; set; }
}

public sealed class MessageMarkReadRequest : InstanceScopedRequest
{
    public IReadOnlyList<string> Id { get; set; } = Array.Empty<string>();

    public string Chat { get; set; } = string.Empty;

    public string? Sender { get; set; }
}

public sealed class MessageDownloadRequest : InstanceScopedRequest
{
}

public sealed class MessageEditRequest : InstanceScopedRequest
{
    public string MessageId { get; set; } = string.Empty;

    public string Message { get; set; } = string.Empty;
}

public sealed class MessageDeleteRequest : InstanceScopedRequest
{
    public string MessageId { get; set; } = string.Empty;
}
