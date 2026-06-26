using VZaps.Http;
using VZaps.Models;
using VZaps.Realtime;

namespace VZaps.Resources;

public sealed class EventsResource : BaseResource
{
    private readonly VZapsClientOptions _options;

    internal EventsResource(VZapsHttpClient http, VZapsClientOptions options)
        : base(http)
    {
        _options = options;
    }

    public async Task<VZapsEventSubscription> SubscribeAsync(VZapsEventSubscribeRequest request, CancellationToken cancellationToken = default)
    {
        if (request is null)
        {
            throw new ArgumentNullException(nameof(request));
        }

        if (string.IsNullOrWhiteSpace(request.InstanceId))
        {
            throw new VZapsRealtimeException("InstanceId is required for realtime subscriptions.");
        }

        if (string.IsNullOrWhiteSpace(request.InstanceToken))
        {
            throw new VZapsRealtimeException("InstanceToken is required for realtime subscriptions.");
        }

        var accessToken = await Http.GetAccessTokenAsync(cancellationToken).ConfigureAwait(false);
        var subscription = new VZapsEventSubscription(_options, request, accessToken);
        subscription.Start(cancellationToken);
        return subscription;
    }
}
