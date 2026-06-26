using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace VZaps;

public static class VZapsServiceCollectionExtensions
{
    public static IServiceCollection AddVZapsClient(this IServiceCollection services, Action<VZapsClientOptions> configure)
    {
        if (services is null)
        {
            throw new ArgumentNullException(nameof(services));
        }

        if (configure is null)
        {
            throw new ArgumentNullException(nameof(configure));
        }

        services.AddOptions<VZapsClientOptions>().Configure(configure);
        services.AddHttpClient("VZaps", (serviceProvider, httpClient) =>
        {
            var options = serviceProvider.GetRequiredService<IOptions<VZapsClientOptions>>().Value;
            options.Validate();
            httpClient.Timeout = options.Timeout;
        });

        services.AddTransient(serviceProvider =>
        {
            var options = serviceProvider.GetRequiredService<IOptions<VZapsClientOptions>>().Value;
            var httpClient = serviceProvider.GetRequiredService<IHttpClientFactory>().CreateClient("VZaps");
            return new VZapsClient(options, httpClient);
        });

        return services;
    }
}
