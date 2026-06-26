namespace VZaps.Http;

public sealed class VZapsRequestOptions
{
    public object? Body { get; set; }

    public IDictionary<string, object?>? Query { get; set; }

    public IDictionary<string, string?>? Headers { get; set; }

    public string? InstanceToken { get; set; }

    public bool Authenticate { get; set; } = true;
}
