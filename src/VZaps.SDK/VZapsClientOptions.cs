namespace VZaps;

public sealed class VZapsClientOptions
{
    public const string DefaultBaseUrl = "https://api.vzaps.com";
    public const string DefaultRealtimeUrl = "wss://realtime.vzaps.com";

    public string? ClientToken { get; set; }

    public string? ClientSecret { get; set; }

    public Uri BaseUrl { get; set; } = new(DefaultBaseUrl);

    public Uri RealtimeUrl { get; set; } = new(DefaultRealtimeUrl);

    public TimeSpan Timeout { get; set; } = TimeSpan.FromSeconds(30);

    public TimeSpan TokenRefreshSkew { get; set; } = TimeSpan.FromSeconds(60);

    public string? UserAgent { get; set; }

    public void Validate()
    {
        if (string.IsNullOrWhiteSpace(ClientToken))
        {
            throw new VZapsException("VZaps ClientToken is required.");
        }

        if (string.IsNullOrWhiteSpace(ClientSecret))
        {
            throw new VZapsException("VZaps ClientSecret is required.");
        }

        if (!BaseUrl.IsAbsoluteUri)
        {
            throw new VZapsException("VZaps BaseUrl must be an absolute URI.");
        }

        if (!RealtimeUrl.IsAbsoluteUri)
        {
            throw new VZapsException("VZaps RealtimeUrl must be an absolute URI.");
        }

        if (Timeout <= TimeSpan.Zero)
        {
            throw new VZapsException("VZaps Timeout must be greater than zero.");
        }

        if (TokenRefreshSkew < TimeSpan.Zero)
        {
            throw new VZapsException("VZaps TokenRefreshSkew cannot be negative.");
        }
    }
}
