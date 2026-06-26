using VZaps.Http;

namespace VZaps.Resources;

public sealed class AuthResource : BaseResource
{
    internal AuthResource(VZapsHttpClient http)
        : base(http)
    {
    }

    public Task<string> GetAccessTokenAsync(CancellationToken cancellationToken = default)
    {
        return Http.GetAccessTokenAsync(cancellationToken);
    }
}
