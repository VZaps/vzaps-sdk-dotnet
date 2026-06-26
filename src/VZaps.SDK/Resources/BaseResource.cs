using System.Text.Json;
using VZaps.Http;
using VZaps.Models;
using VZaps.Serialization;

namespace VZaps.Resources;

public abstract class BaseResource
{
    protected static readonly HttpMethod Patch = new("PATCH");

    protected BaseResource(VZapsHttpClient http)
    {
        Http = http;
    }

    internal VZapsHttpClient Http { get; }

    protected Task<TResponse?> SendAsync<TResponse>(
        HttpMethod method,
        string path,
        object? body = null,
        string? instanceToken = null,
        IDictionary<string, object?>? query = null,
        CancellationToken cancellationToken = default)
    {
        return Http.RequestAsync<TResponse>(
            method,
            path,
            new VZapsRequestOptions
            {
                Body = body,
                InstanceToken = instanceToken,
                Query = query,
            },
            cancellationToken);
    }

    protected static string Escape(string value)
    {
        return Uri.EscapeDataString(value);
    }

    protected static object? BodyWithoutInstance(InstanceScopedRequest request, params string[] excluded)
    {
        var excludedNames = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            "instance_id",
            "instance_token",
        };

        foreach (var item in excluded)
        {
            excludedNames.Add(VZapsSnakeCaseNamingPolicy.Instance.ConvertName(item));
        }

        var json = JsonSerializer.SerializeToElement(request, request.GetType(), VZapsJson.Options);
        if (json.ValueKind != JsonValueKind.Object)
        {
            return null;
        }

        var body = new Dictionary<string, JsonElement>(StringComparer.Ordinal);
        foreach (var property in json.EnumerateObject())
        {
            if (!excludedNames.Contains(property.Name) && property.Value.ValueKind != JsonValueKind.Null)
            {
                body[property.Name] = property.Value.Clone();
            }
        }

        return body.Count == 0 ? null : body;
    }
}
