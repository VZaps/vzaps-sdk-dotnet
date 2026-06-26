using System.Net;
using System.Net.Http.Headers;
using System.Reflection;
using System.Text;
using System.Text.Json;
using VZaps.Serialization;

namespace VZaps.Http;

public sealed class VZapsHttpClient : IDisposable
{
    private const int MaxErrorBodyLength = 4096;
    private readonly VZapsClientOptions _options;
    private readonly TokenProvider _tokenProvider;

    public VZapsHttpClient(VZapsClientOptions options, HttpClient httpClient)
    {
        _options = options;
        InnerClient = httpClient;
        if (InnerClient.BaseAddress is null)
        {
            InnerClient.BaseAddress = NormalizeBaseUri(options.BaseUrl);
        }

        InnerClient.Timeout = options.Timeout;
        _tokenProvider = new TokenProvider(this, options);
    }

    internal HttpClient InnerClient { get; }

    public Task<string> GetAccessTokenAsync(CancellationToken cancellationToken = default)
    {
        return _tokenProvider.GetAccessTokenAsync(cancellationToken);
    }

    public async Task<TResponse?> RequestAsync<TResponse>(
        HttpMethod method,
        string path,
        VZapsRequestOptions? options = null,
        CancellationToken cancellationToken = default)
    {
        options ??= new VZapsRequestOptions();
        using var response = await SendAsync(method, path, options, forceRefresh: false, cancellationToken).ConfigureAwait(false);

        if (response.StatusCode == HttpStatusCode.Unauthorized && options.Authenticate)
        {
            await _tokenProvider.ForceRefreshAsync(cancellationToken).ConfigureAwait(false);
            using var retryResponse = await SendAsync(method, path, options, forceRefresh: true, cancellationToken).ConfigureAwait(false);
            return await ReadResponseAsync<TResponse>(retryResponse, cancellationToken).ConfigureAwait(false);
        }

        return await ReadResponseAsync<TResponse>(response, cancellationToken).ConfigureAwait(false);
    }

    internal Task<TResponse?> RequestWithoutAuthAsync<TResponse>(
        HttpMethod method,
        string path,
        object? body,
        CancellationToken cancellationToken)
    {
        return RequestAsync<TResponse>(
            method,
            path,
            new VZapsRequestOptions { Authenticate = false, Body = body },
            cancellationToken);
    }

    private async Task<HttpResponseMessage> SendAsync(
        HttpMethod method,
        string path,
        VZapsRequestOptions options,
        bool forceRefresh,
        CancellationToken cancellationToken)
    {
        using var request = new HttpRequestMessage(method, BuildUri(path, options.Query));
        request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        request.Headers.TryAddWithoutValidation("User-Agent", BuildUserAgent(_options.UserAgent));

        if (options.Authenticate)
        {
            var token = forceRefresh
                ? await _tokenProvider.ForceRefreshAsync(cancellationToken).ConfigureAwait(false)
                : await _tokenProvider.GetAccessTokenAsync(cancellationToken).ConfigureAwait(false);
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            request.Headers.TryAddWithoutValidation("X-Client-Token", _options.ClientToken);
        }

        if (!string.IsNullOrWhiteSpace(options.InstanceToken))
        {
            request.Headers.TryAddWithoutValidation("X-Instance-Token", options.InstanceToken);
        }

        if (options.Headers is not null)
        {
            foreach (var header in options.Headers)
            {
                if (header.Value is not null)
                {
                    request.Headers.TryAddWithoutValidation(header.Key, header.Value);
                }
            }
        }

        if (options.Body is not null)
        {
            var json = JsonSerializer.Serialize(options.Body, VZapsJson.Options);
            request.Content = new StringContent(json, Encoding.UTF8, "application/json");
        }

        try
        {
            return await InnerClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, cancellationToken).ConfigureAwait(false);
        }
        catch (TaskCanceledException ex) when (!cancellationToken.IsCancellationRequested)
        {
            throw new VZapsTimeoutException("The VZaps request timed out.", ex);
        }
    }

    private async Task<TResponse?> ReadResponseAsync<TResponse>(HttpResponseMessage response, CancellationToken cancellationToken)
    {
        var content = response.Content is null
            ? string.Empty
            : await response.Content.ReadAsStringAsync().ConfigureAwait(false);

        if (!response.IsSuccessStatusCode)
        {
            throw CreateApiException(response, content);
        }

        if (typeof(TResponse) == typeof(string))
        {
            return (TResponse?)(object?)content;
        }

        if (string.IsNullOrWhiteSpace(content))
        {
            return default;
        }

        return JsonSerializer.Deserialize<TResponse>(content, VZapsJson.Options);
    }

    private VZapsApiException CreateApiException(HttpResponseMessage response, string responseBody)
    {
        var safeBody = Truncate(responseBody);
        var error = ReadError(responseBody);
        var requestId = TryGetHeader(response, "X-Request-Id");
        var message = error.Message ?? response.ReasonPhrase ?? "VZaps request failed.";

        return response.StatusCode switch
        {
            HttpStatusCode.Unauthorized or HttpStatusCode.Forbidden => new VZapsAuthenticationException(message, response.StatusCode, error.Code, error.Details, requestId, safeBody),
            (HttpStatusCode)429 => new VZapsRateLimitException(message, response.StatusCode, error.Code, error.Details, requestId, safeBody),
            _ => new VZapsApiException(message, response.StatusCode, error.Code, error.Details, requestId, safeBody),
        };
    }

    private Uri BuildUri(string path, IDictionary<string, object?>? query)
    {
        var baseUri = InnerClient.BaseAddress ?? NormalizeBaseUri(_options.BaseUrl);
        var cleanPath = path.StartsWith("/", StringComparison.Ordinal) ? path.Substring(1) : path;
        var uri = new Uri(baseUri, cleanPath);

        if (query is null || query.Count == 0)
        {
            return uri;
        }

        var builder = new UriBuilder(uri);
        var parts = new List<string>();
        if (!string.IsNullOrWhiteSpace(builder.Query))
        {
            parts.Add(builder.Query.TrimStart('?'));
        }

        foreach (var item in query)
        {
            if (item.Value is null)
            {
                continue;
            }

            var key = VZapsSnakeCaseNamingPolicy.Instance.ConvertName(item.Key);
            parts.Add(Uri.EscapeDataString(key) + "=" + Uri.EscapeDataString(Convert.ToString(item.Value, System.Globalization.CultureInfo.InvariantCulture) ?? string.Empty));
        }

        builder.Query = string.Join("&", parts);
        return builder.Uri;
    }

    private static Uri NormalizeBaseUri(Uri uri)
    {
        var value = uri.ToString();
        if (!value.EndsWith("/", StringComparison.Ordinal))
        {
            value += "/";
        }

        return new Uri(value, UriKind.Absolute);
    }

    private static string BuildUserAgent(string? configured)
    {
        if (!string.IsNullOrWhiteSpace(configured))
        {
            return configured!;
        }

        var version = typeof(VZapsClient).Assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>()?.InformationalVersion
            ?? typeof(VZapsClient).Assembly.GetName().Version?.ToString()
            ?? "0.1.0";
        return $"VZaps.SDK/{version} (.NET)";
    }

    private static (string? Message, string? Code, string? Details) ReadError(string responseBody)
    {
        if (string.IsNullOrWhiteSpace(responseBody))
        {
            return (null, null, null);
        }

        try
        {
            using var document = JsonDocument.Parse(responseBody);
            var root = document.RootElement;
            return (
                TryGetString(root, "message") ?? TryGetString(root, "error"),
                TryGetString(root, "code") ?? TryGetString(root, "error_code"),
                TryGetString(root, "details"));
        }
        catch (JsonException)
        {
            return (responseBody, null, null);
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

    private static string? TryGetHeader(HttpResponseMessage response, string name)
    {
        return response.Headers.TryGetValues(name, out var values) ? values.FirstOrDefault() : null;
    }

    private static string? Truncate(string value)
    {
        if (string.IsNullOrEmpty(value))
        {
            return null;
        }

        return value.Length <= MaxErrorBodyLength ? value : value.Substring(0, MaxErrorBodyLength);
    }

    public void Dispose()
    {
        _tokenProvider.GetType();
    }
}
