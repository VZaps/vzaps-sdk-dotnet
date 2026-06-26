using VZaps.Models;

namespace VZaps.Examples.Worker;

public class Worker : BackgroundService
{
    private readonly ILogger<Worker> _logger;
    private readonly global::VZaps.VZapsClient _client;

    public Worker(ILogger<Worker> logger, global::VZaps.VZapsClient client)
    {
        _logger = logger;
        _client = client;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var instanceId = Environment.GetEnvironmentVariable("VZAPS_INSTANCE_ID");
        var instanceToken = Environment.GetEnvironmentVariable("VZAPS_INSTANCE_TOKEN");
        if (string.IsNullOrWhiteSpace(instanceId) || string.IsNullOrWhiteSpace(instanceToken))
        {
            _logger.LogInformation("Set VZAPS_INSTANCE_ID and VZAPS_INSTANCE_TOKEN to run realtime worker example.");
            return;
        }

        await using var subscription = await _client.Events.SubscribeAsync(new VZapsEventSubscribeRequest
        {
            InstanceId = instanceId,
            InstanceToken = instanceToken,
            Events = new[] { VZapsEventType.Message, VZapsEventType.Connected },
        }, stoppingToken);

        subscription.On(VZapsEventType.Message, evt =>
        {
            _logger.LogInformation("Received VZaps event {EventId} of type {EventType}", evt.Id, evt.Type);
        });

        await subscription.WaitForCloseAsync(stoppingToken);
    }
}
