using System.Text.Json;
using System.Text.Json.Serialization;

namespace VZaps.Models;

public class VZapsModel
{
    [JsonExtensionData]
    public IDictionary<string, JsonElement>? AdditionalData { get; set; }
}

public class InstanceScopedRequest : VZapsModel
{
    public string InstanceId { get; set; } = string.Empty;

    public string InstanceToken { get; set; } = string.Empty;
}

public sealed class InstanceRequestOptions
{
    public string? InstanceToken { get; set; }
}

public sealed class InstanceCreateRequest : VZapsModel
{
    public string Name { get; set; } = string.Empty;

    public string? Webhook { get; set; }

    public object? EventsSubscribe { get; set; }
}

public sealed class InstanceListRequest : VZapsModel
{
    public int? Page { get; set; }

    public int? Size { get; set; }

    public int? PageSize { get; set; }

    public IDictionary<string, object?>? Filter { get; set; }

    public string? Search { get; set; }

    public string? Sort { get; set; }

    public bool? SortDesc { get; set; }
}

public sealed class InstanceGetRequest : VZapsModel
{
    public string Id { get; set; } = string.Empty;
}

public sealed class ContactAddRequest : InstanceScopedRequest
{
    public string Phone { get; set; } = string.Empty;

    public string FirstName { get; set; } = string.Empty;

    public string FullName { get; set; } = string.Empty;
}

public sealed class UserPhonesRequest : InstanceScopedRequest
{
    public string? Phone { get; set; }
}

public sealed class UserAvatarRequest : InstanceScopedRequest
{
    public string? Phone { get; set; }
}

public class PagedInstanceRequest : InstanceScopedRequest
{
    public int? Page { get; set; }

    public int? PageSize { get; set; }
}
