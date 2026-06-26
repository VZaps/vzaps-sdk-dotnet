using VZaps.Http;
using VZaps.Models;

namespace VZaps.Resources;

public sealed class InstancesResource : BaseResource
{
    internal InstancesResource(VZapsHttpClient http)
        : base(http)
    {
    }

    public Task<TResponse?> CreateAsync<TResponse>(InstanceCreateRequest request, CancellationToken cancellationToken = default)
    {
        return SendAsync<TResponse>(HttpMethod.Put, "/instances/create", request, cancellationToken: cancellationToken);
    }

    public Task<TResponse?> ListAsync<TResponse>(InstanceListRequest? request = null, CancellationToken cancellationToken = default)
    {
        request ??= new InstanceListRequest();
        var filter = new Dictionary<string, object?>(StringComparer.Ordinal);
        if (request.Filter is not null)
        {
            foreach (var item in request.Filter)
            {
                filter[item.Key] = item.Value;
            }
        }

        var search = request.Search;
        if (!string.IsNullOrWhiteSpace(search))
        {
            filter["query"] = search!.Trim();
        }

        return SendAsync<TResponse>(
            HttpMethod.Post,
            "/instances/list",
            new
            {
                page = request.Page ?? 1,
                size = request.Size ?? request.PageSize ?? 20,
                filter,
                sort = request.Sort,
                sortDesc = request.SortDesc,
            },
            cancellationToken: cancellationToken);
    }

    public Task<TResponse?> GetAsync<TResponse>(string instanceId, CancellationToken cancellationToken = default)
    {
        return SendAsync<TResponse>(HttpMethod.Post, "/instances/get", new { id = instanceId }, cancellationToken: cancellationToken);
    }

    public Task<TResponse?> UpdateAsync<TResponse>(string instanceId, object request, InstanceRequestOptions? options = null, CancellationToken cancellationToken = default)
    {
        return SendAsync<TResponse>(Patch, $"/instances/{Escape(instanceId)}", request, options?.InstanceToken, cancellationToken: cancellationToken);
    }

    public Task<TResponse?> RestartAsync<TResponse>(string instanceId, InstanceRequestOptions? options = null, CancellationToken cancellationToken = default)
    {
        return SendAsync<TResponse>(HttpMethod.Post, $"/instances/{Escape(instanceId)}/restart", instanceToken: options?.InstanceToken, cancellationToken: cancellationToken);
    }

    public Task<TResponse?> DeleteAsync<TResponse>(string instanceId, InstanceRequestOptions? options = null, CancellationToken cancellationToken = default)
    {
        return SendAsync<TResponse>(HttpMethod.Delete, $"/instances/{Escape(instanceId)}", instanceToken: options?.InstanceToken, cancellationToken: cancellationToken);
    }

    public Task<TResponse?> ProvisionAsync<TResponse>(InstanceCreateRequest request, CancellationToken cancellationToken = default)
    {
        return SendAsync<TResponse>(HttpMethod.Put, "/instances/provision", request, cancellationToken: cancellationToken);
    }

    public Task<TResponse?> SearchAsync<TResponse>(object request, CancellationToken cancellationToken = default)
    {
        return SendAsync<TResponse>(HttpMethod.Post, "/instances/search", request, cancellationToken: cancellationToken);
    }

    public Task<TResponse?> SubscribeAsync<TResponse>(string instanceId, object? request = null, InstanceRequestOptions? options = null, CancellationToken cancellationToken = default)
    {
        return SendAsync<TResponse>(HttpMethod.Post, $"/instances/{Escape(instanceId)}/subscribe", request, options?.InstanceToken, cancellationToken: cancellationToken);
    }

    public Task<TResponse?> ResumeSubscriptionAsync<TResponse>(string instanceId, InstanceRequestOptions? options = null, CancellationToken cancellationToken = default)
    {
        return SendAsync<TResponse>(HttpMethod.Post, $"/instances/{Escape(instanceId)}/resume-subscription", instanceToken: options?.InstanceToken, cancellationToken: cancellationToken);
    }

    public Task<TResponse?> CancelAsync<TResponse>(string instanceId, InstanceRequestOptions? options = null, CancellationToken cancellationToken = default)
    {
        return SendAsync<TResponse>(HttpMethod.Put, $"/instances/{Escape(instanceId)}/cancel", instanceToken: options?.InstanceToken, cancellationToken: cancellationToken);
    }
}
