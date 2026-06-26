# VZaps .NET SDK

[![CI](https://github.com/VZaps/vzaps-sdk-dotnet/actions/workflows/ci.yml/badge.svg?branch=main)](https://github.com/VZaps/vzaps-sdk-dotnet/actions/workflows/ci.yml)
[![NuGet](https://img.shields.io/nuget/v/VZaps.SDK.svg?logo=nuget&logoColor=white)](https://www.nuget.org/packages/VZaps.SDK/)

Official .NET SDK for the VZaps public API.

## Installation

```bash
dotnet add package VZaps.SDK
```

The package targets `netstandard2.0` and `net8.0`. .NET 8 or newer is recommended for new applications.

## Quick Start

```csharp
using VZaps;
using VZaps.Models;

using var client = new VZapsClient(new VZapsClientOptions
{
    ClientToken = Environment.GetEnvironmentVariable("VZAPS_CLIENT_TOKEN"),
    ClientSecret = Environment.GetEnvironmentVariable("VZAPS_CLIENT_SECRET"),
});

await client.Messages.SendTextAsync<object>(new SendTextMessageRequest
{
    InstanceId = "VZ...",
    InstanceToken = "instance-token",
    Phone = "5511999999999",
    Message = "Hello from VZaps .NET SDK",
});
```

## ASP.NET Core / Worker DI

```csharp
builder.Services.AddVZapsClient(options =>
{
    options.ClientToken = builder.Configuration["VZaps:ClientToken"];
    options.ClientSecret = builder.Configuration["VZaps:ClientSecret"];
});
```

Then inject `VZapsClient` into controllers, minimal API handlers, hosted services, or workers.

## Authentication

The SDK exchanges `ClientToken` and `ClientSecret` for a JWT through `POST /token`. Tokens are cached and refreshed before expiration using a default 60 second skew. Instance-scoped calls send `X-Instance-Token` from the request model or `InstanceRequestOptions`.

## Resources

The main client exposes:

- `Auth`
- `Instances`
- `Sessions`
- `Messages`
- `Webhooks`
- `Contacts`
- `Groups`
- `Users`
- `Queues`
- `TypeBots`
- `Chatwoot`
- `Chats`
- `Events`

All operations are async-first and accept `CancellationToken`.

## Generic Requests

For newly released API fields, use the escape hatch:

```csharp
var response = await client.RequestAsync<object>(
    HttpMethod.Post,
    "/instances/VZ.../chat/send/text",
    new VZaps.Http.VZapsRequestOptions
    {
        InstanceToken = "instance-token",
        Body = new { phone = "5511999999999", message = "Hello" },
    });
```

## Realtime

```csharp
await using var subscription = await client.Events.SubscribeAsync(
    new VZapsEventSubscribeRequest
    {
        InstanceId = "VZ...",
        InstanceToken = "instance-token",
        Events = new[] { VZapsEventType.Message, VZapsEventType.Connected },
        Reconnect = true,
        MaxRetries = 10,
    },
    cancellationToken);

subscription.On(VZapsEventType.Message, evt =>
{
    Console.WriteLine(evt.Id);
});

await subscription.WaitForCloseAsync(cancellationToken);
```

Realtime delivery is at-least-once. Deduplicate by `evt.Id` when handlers have side effects.

## Errors

The SDK maps non-2xx responses to typed exceptions:

- `VZapsAuthenticationException` for 401/403.
- `VZapsRateLimitException` for 429.
- `VZapsApiException` for other API errors.
- `VZapsTimeoutException` for SDK-side request timeouts.
- `VZapsRealtimeException` for realtime failures.

Exception fields include `StatusCode`, `ErrorCode`, `Details`, `RequestId`, and a truncated `ResponseBody`.

## Examples

Examples live under `examples/`:

- `VZaps.Examples.Console`: numbered flows from auth to TypeBot/Chatwoot.
- `VZaps.Examples.AspNetCore`: minimal API using `AddVZapsClient`.
- `VZaps.Examples.Worker`: hosted service with realtime subscription.

Environment variables:

- `VZAPS_CLIENT_TOKEN`
- `VZAPS_CLIENT_SECRET`
- `VZAPS_INSTANCE_ID`
- `VZAPS_INSTANCE_TOKEN`
- `VZAPS_PHONE`

## Documentation

Official documentation: [docs.vzaps.com](https://docs.vzaps.com)
