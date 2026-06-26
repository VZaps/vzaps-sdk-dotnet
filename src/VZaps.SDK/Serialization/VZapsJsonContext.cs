using System.Text.Json.Serialization;
using VZaps.Models;

namespace VZaps.Serialization;

[JsonSerializable(typeof(InstanceCreateRequest))]
[JsonSerializable(typeof(InstanceListRequest))]
[JsonSerializable(typeof(SendTextMessageRequest))]
[JsonSerializable(typeof(SendImageMessageRequest))]
[JsonSerializable(typeof(SendAudioMessageRequest))]
[JsonSerializable(typeof(SendDocumentMessageRequest))]
[JsonSerializable(typeof(SendVideoMessageRequest))]
[JsonSerializable(typeof(SendPollMessageRequest))]
[JsonSerializable(typeof(WebhookSetRequest))]
[JsonSerializable(typeof(QueueRequest))]
[JsonSerializable(typeof(TypeBotRequest))]
[JsonSerializable(typeof(ChatwootSetRequest))]
[JsonSerializable(typeof(GroupMutationRequest))]
[JsonSerializable(typeof(VZapsEvent))]
internal partial class VZapsJsonContext : JsonSerializerContext
{
}
