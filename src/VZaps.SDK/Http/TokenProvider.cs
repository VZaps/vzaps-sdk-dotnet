using System.Text.Json;

namespace VZaps.Http;

internal sealed class TokenProvider
{
    private readonly VZapsHttpClient _http;
    private readonly VZapsClientOptions _options;
    private readonly SemaphoreSlim _refreshLock = new(1, 1);
    private string? _accessToken;
    private DateTimeOffset _expiresAt;

    public TokenProvider(VZapsHttpClient http, VZapsClientOptions options)
    {
        _http = http;
        _options = options;
    }

    public async Task<string> GetAccessTokenAsync(CancellationToken cancellationToken = default)
    {
        if (HasValidToken)
        {
            return _accessToken!;
        }

        return await RefreshCoreAsync(force: false, cancellationToken).ConfigureAwait(false);
    }

    public Task<string> ForceRefreshAsync(CancellationToken cancellationToken = default)
    {
        return RefreshCoreAsync(force: true, cancellationToken);
    }

    private bool HasValidToken => !string.IsNullOrEmpty(_accessToken) && _expiresAt > DateTimeOffset.UtcNow;

    private async Task<string> RefreshCoreAsync(bool force, CancellationToken cancellationToken)
    {
        await _refreshLock.WaitAsync(cancellationToken).ConfigureAwait(false);
        try
        {
            if (!force && HasValidToken)
            {
                return _accessToken!;
            }

            var response = await _http.RequestWithoutAuthAsync<JsonElement>(
                HttpMethod.Post,
                "/token",
                new
                {
                    clientToken = _options.ClientToken,
                    clientSecret = _options.ClientSecret,
                },
                cancellationToken).ConfigureAwait(false);

            var accessToken = TryGetString(response, "accessToken") ?? TryGetString(response, "access_token");
            var expiresIn = TryGetInt32(response, "expiresIn") ?? TryGetInt32(response, "expires_in") ?? 0;

            if (string.IsNullOrWhiteSpace(accessToken) || expiresIn <= 0)
            {
                throw new VZapsAuthenticationException("VZaps token response is missing accessToken or expiresIn.", System.Net.HttpStatusCode.Unauthorized);
            }

            _accessToken = accessToken!;
            _expiresAt = DateTimeOffset.UtcNow.AddSeconds(expiresIn).Subtract(_options.TokenRefreshSkew);
            return _accessToken;
        }
        finally
        {
            _refreshLock.Release();
        }
    }

    private static string? TryGetString(JsonElement element, string propertyName)
    {
        return element.ValueKind == JsonValueKind.Object
            && element.TryGetProperty(propertyName, out var property)
            && property.ValueKind == JsonValueKind.String
            ? property.GetString()
            : null;
    }

    private static int? TryGetInt32(JsonElement element, string propertyName)
    {
        if (element.ValueKind == JsonValueKind.Object
            && element.TryGetProperty(propertyName, out var property)
            && property.ValueKind == JsonValueKind.Number
            && property.TryGetInt32(out var value))
        {
            return value;
        }

        return null;
    }
}
