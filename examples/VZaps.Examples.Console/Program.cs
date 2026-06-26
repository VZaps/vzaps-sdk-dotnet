using System.Text.Json;
using VZaps;
using VZaps.Models;

var env = ExampleEnvironment.Load();
if (!env.HasCredentials)
{
    Console.WriteLine("Set VZAPS_CLIENT_TOKEN and VZAPS_CLIENT_SECRET to run the VZaps examples.");
    return;
}

using var client = new VZapsClient(new VZapsClientOptions
{
    ClientToken = env.ClientToken,
    ClientSecret = env.ClientSecret,
});

var example = args.FirstOrDefault() ?? "01";
switch (example)
{
    case "01":
        Console.WriteLine("01_AuthAndListInstances");
        Console.WriteLine(await client.Auth.GetAccessTokenAsync());
        Console.WriteLine(await client.Instances.ListAsync<string>(new InstanceListRequest { PageSize = 10 }));
        break;
    case "02":
        Console.WriteLine("02_CreateInstance");
        Console.WriteLine(await client.Instances.CreateAsync<string>(new InstanceCreateRequest { Name = "dotnet-sdk-example" }));
        break;
    case "03":
        RequireInstance(env);
        Console.WriteLine("03_InstanceSubscription");
        Console.WriteLine(await client.Instances.SubscribeAsync<string>(env.InstanceId!, new { plan = "direct" }, new InstanceRequestOptions { InstanceToken = env.InstanceToken }));
        break;
    case "04":
        RequireInstance(env);
        Console.WriteLine("04_SessionAndPairing");
        Console.WriteLine(await client.Sessions.StatusAsync<string>(env.InstanceId!, new InstanceRequestOptions { InstanceToken = env.InstanceToken }));
        break;
    case "05":
        RequireInstance(env);
        Console.WriteLine("05_ConfigureWebhook");
        Console.WriteLine(await client.Webhooks.SetAsync<string>(new WebhookSetRequest { InstanceId = env.InstanceId!, InstanceToken = env.InstanceToken!, WebhookURL = "https://example.com/webhook", Events = new[] { "Message" } }));
        break;
    case "06":
        RequireInstance(env);
        Console.WriteLine("06_RealtimeSubscribe");
        await using (var subscription = await client.Events.SubscribeAsync(new VZapsEventSubscribeRequest { InstanceId = env.InstanceId!, InstanceToken = env.InstanceToken!, Events = new[] { VZapsEventType.Message } }))
        {
            subscription.On(VZapsEventType.Message, evt => Console.WriteLine(JsonSerializer.Serialize(evt)));
            await subscription.WaitForCloseAsync();
        }
        break;
    case "07":
        RequireInstance(env);
        Console.WriteLine("07_SendTextMessage");
        Console.WriteLine(await client.Messages.SendTextAsync<string>(new SendTextMessageRequest { InstanceId = env.InstanceId!, InstanceToken = env.InstanceToken!, Phone = env.Phone ?? "5511999999999", Message = "Hello from VZaps .NET SDK" }));
        break;
    case "08":
        RequireInstance(env);
        Console.WriteLine("08_SendMediaAndInteractive");
        Console.WriteLine(await client.Messages.SendImageAsync<string>(new SendImageMessageRequest { InstanceId = env.InstanceId!, InstanceToken = env.InstanceToken!, Phone = env.Phone ?? "5511999999999", Image = "https://example.com/image.png", Caption = "Image from .NET" }));
        break;
    case "09":
        RequireInstance(env);
        Console.WriteLine("09_SendPollReactionAndChatActions");
        Console.WriteLine(await client.Messages.SendPollAsync<string>(new SendPollMessageRequest { InstanceId = env.InstanceId!, InstanceToken = env.InstanceToken!, Phone = env.Phone ?? "5511999999999", Name = "Choose", Options = new[] { "A", "B" } }));
        break;
    case "10":
        RequireInstance(env);
        Console.WriteLine("10_Queues");
        Console.WriteLine(await client.Queues.ListMessagesAsync<string>(new QueueRequest { InstanceId = env.InstanceId!, InstanceToken = env.InstanceToken! }));
        break;
    case "11":
        RequireInstance(env);
        Console.WriteLine("11_TypeBotAndChatwoot");
        Console.WriteLine(await client.TypeBots.ListAsync<string>(env.InstanceId!, new InstanceRequestOptions { InstanceToken = env.InstanceToken }));
        Console.WriteLine(await client.Chatwoot.GetAsync<string>(env.InstanceId!, new InstanceRequestOptions { InstanceToken = env.InstanceToken }));
        break;
    default:
        Console.WriteLine("Use an example number from 01 to 11.");
        break;
}

static void RequireInstance(ExampleEnvironment env)
{
    if (!env.HasInstance)
    {
        throw new InvalidOperationException("Set VZAPS_INSTANCE_ID and VZAPS_INSTANCE_TOKEN to run this example.");
    }
}

internal sealed record ExampleEnvironment(string? ClientToken, string? ClientSecret, string? InstanceId, string? InstanceToken, string? Phone)
{
    public bool HasCredentials => !string.IsNullOrWhiteSpace(ClientToken) && !string.IsNullOrWhiteSpace(ClientSecret);

    public bool HasInstance => HasCredentials && !string.IsNullOrWhiteSpace(InstanceId) && !string.IsNullOrWhiteSpace(InstanceToken);

    public static ExampleEnvironment Load() => new(
        Environment.GetEnvironmentVariable("VZAPS_CLIENT_TOKEN"),
        Environment.GetEnvironmentVariable("VZAPS_CLIENT_SECRET"),
        Environment.GetEnvironmentVariable("VZAPS_INSTANCE_ID"),
        Environment.GetEnvironmentVariable("VZAPS_INSTANCE_TOKEN"),
        Environment.GetEnvironmentVariable("VZAPS_PHONE"));
}
