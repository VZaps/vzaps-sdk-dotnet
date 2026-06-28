# VZaps .NET SDK

[![CI](https://github.com/VZaps/vzaps-sdk-dotnet/actions/workflows/ci.yml/badge.svg?branch=main)](https://github.com/VZaps/vzaps-sdk-dotnet/actions/workflows/ci.yml) [![SDK Documentation](https://img.shields.io/badge/SDK-Documentation-blue)](https://docs.vzaps.com/en/sdk/dotnet/installation) [![license](https://img.shields.io/badge/license-MIT-blue.svg)](./LICENSE)
[![NuGet](https://img.shields.io/nuget/v/VZaps.SDK.svg?logo=nuget&logoColor=white)](https://www.nuget.org/packages/VZaps.SDK/)

Official .NET client for the [VZaps public API](https://docs.vzaps.com). Send WhatsApp messages, manage instances, configure webhooks, and subscribe to realtime events with a resource-oriented, async-first interface.

Targets **`netstandard2.0`** and **`net8.0`**. .NET 8 or newer is recommended for new applications.

---

## Table of contents

- [Features](#features)
- [Requirements](#requirements)
- [Installation](#installation)
- [Quick start](#quick-start)
- [Authentication](#authentication)
- [Configuration](#configuration)
- [Resources](#resources)
- [Instance tokens](#instance-tokens)
- [Webhooks](#webhooks)
- [Realtime events](#realtime-events)
- [Error handling](#error-handling)
- [.NET](#net)
- [Documentation](#documentation)

---

## Features

- **Automatic JWT handling** — exchanges `ClientToken` + `ClientSecret` for a bearer token and refreshes it before expiry.
- **Resource-oriented API** — `Instances`, `Messages`, `Webhooks`, `Contacts`, `Groups`, and `Events` mirror the public HTTP contract.
- **Async-first** — all operations accept `CancellationToken`.
- **Realtime WebSocket client** — subscribe to instance events with reconnect, resume (`LastEventId`), and server-side ack.
- **Instance token support** — pass `InstanceToken` on each instance-scoped request.
- **Typed request models** — exported C# models for common public API payloads.
- **Extensible transport** — inject a custom `HttpClient` for tests, proxies, or tracing.

---

## Requirements

| Runtime | Minimum version |
| --- | --- |
| .NET | `netstandard2.0` (library) / .NET 8+ recommended |

The SDK uses `HttpClient` by default. No extra HTTP dependency is required.

---

## Installation

```bash
dotnet add package VZaps.SDK
```

---

## Quick start

Create credentials in the [VZaps dashboard](https://docs.vzaps.com) (`ClientToken` and `ClientSecret`), then send a text message:

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
    InstanceId = "VZKB8AU4S4CWY1SLXX4I5WJGRZQMDDFTV6",
    InstanceToken = Environment.GetEnvironmentVariable("VZAPS_INSTANCE_TOKEN"),
    Phone = "5511999999999",
    Message = "Hello from VZaps",
});
```

---

## Authentication

VZaps uses a two-step model:

1. **Account credentials** — `ClientToken` and `ClientSecret` identify your integration. The SDK calls `POST /token` and caches the JWT.
2. **Instance token** — instance-scoped routes also require `X-Instance-Token`. Pass it on each instance-scoped request (see [Instance tokens](#instance-tokens)).

Every authenticated HTTP request sends:

| Header | Value |
| --- | --- |
| `Authorization` | `Bearer <jwt>` |
| `X-Client-Token` | Your client token |
| `X-Instance-Token` | Instance token, on instance-scoped requests |

You rarely need to call `Auth.GetAccessTokenAsync` directly — resources attach the token for you. Use it when integrating with custom HTTP logic:

```csharp
var token = await client.Auth.GetAccessTokenAsync(cancellationToken);
```

---

## Configuration

The SDK connects to the VZaps production platform automatically:

| Service | Endpoint |
| --- | --- |
| REST API | `https://api.vzaps.com` |
| Realtime WebSocket | `wss://realtime.vzaps.com/events/ws` |

Pass options to `new VZapsClient(options)`:

| Option | Type | Default | Description |
| --- | --- | --- | --- |
| `ClientToken` | `string` | — | **Required.** Public client token from the dashboard. |
| `ClientSecret` | `string` | — | **Required.** Client secret used to obtain JWTs. |
| `BaseUrl` | `Uri` | `https://api.vzaps.com` | REST API base URL. |
| `RealtimeUrl` | `Uri` | `wss://realtime.vzaps.com` | Realtime WebSocket base URL. |
| `Timeout` | `TimeSpan` | `30s` | HTTP request timeout. |
| `TokenRefreshSkew` | `TimeSpan` | `60s` | Refresh JWT this long before expiry. |
| `UserAgent` | `string` | — | Optional `User-Agent` header on HTTP requests. |

No host configuration is required — install the package, pass your credentials, and the client targets the production API and realtime service.

Pass a custom `HttpClient` as the second constructor argument when you need proxy, TLS, or test handlers:

```csharp
using var client = new VZapsClient(options, httpClient);
```

---

## Resources

The client exposes namespaced resources. Generic response types (`TResponse`) let you align with your own models or the [OpenAPI schema](https://docs.vzaps.com/api-reference).

All resource methods accept a `CancellationToken`.

### `client.Instances`

| Method | HTTP | Description |
| --- | --- | --- |
| `CreateAsync<TResponse>(request)` | `PUT /instances/create` | Create a WhatsApp instance. |
| `ListAsync<TResponse>(request?)` | `POST /instances/list` | List instances (pagination, search, sort). |
| `GetAsync<TResponse>(instanceId)` | `POST /instances/get` | Get instance details. |
| `UpdateAsync<TResponse>(instanceId, request, options?)` | `PATCH /instances/:id` | Update instance settings. |
| `RestartAsync<TResponse>(instanceId, options?)` | `POST /instances/:id/restart` | Restart instance runtime. |

### `client.Messages`

`client.Messages` wraps the public WhatsApp send and chat endpoints. The most common calls are shown below; the SDK also exposes the other public message operations documented in the API reference, including media, interactive messages, reactions, polls, downloads, edits, deletes, presence, and read receipts.

```csharp
await client.Messages.SendTextAsync<object>(new SendTextMessageRequest
{
    InstanceId = "VZ...",
    InstanceToken = "instance-token",
    Phone = "5511999999999",
    Message = "Hello",
});

await client.Messages.SendImageAsync<object>(new SendImageMessageRequest
{
    InstanceId = "VZ...",
    InstanceToken = "instance-token",
    Phone = "5511999999999",
    Image = "https://example.com/photo.jpg",
    Caption = "Check this out",
});
```

Available send helpers include `SendTextAsync`, `SendImageAsync`, `SendAudioAsync`, `SendDocumentAsync`, `SendVideoAsync`, `SendStickerAsync`, `SendGifAsync`, `SendLocationAsync`, `SendContactAsync`, `SendButtonsAsync`, `SendListAsync`, `SendLinkAsync`, and `SendPollAsync`. See the API documentation for complete payload examples.

### `client.Webhooks`

| Method | HTTP | Description |
| --- | --- | --- |
| `GetAsync<TResponse>(instanceId, options?)` | `GET /instances/:id/webhook` | Read current webhook configuration. |
| `SetAsync<TResponse>(request)` | `POST /instances/:id/webhook` | Configure webhook URL and subscribed events. |

### `client.Contacts`

| Method | HTTP | Description |
| --- | --- | --- |
| `ListAsync<TResponse>(instanceId, options?)` | `GET /instances/:id/contact/list` | List contacts for the instance. |
| `AddAsync<TResponse>(request)` | `POST /instances/:id/contact/add` | Add a contact. |

### `client.Groups`

| Method | HTTP | Description |
| --- | --- | --- |
| `ListAsync<TResponse>(request)` | `GET /instances/:id/group/list` | List groups (paginated). |
| `GetAsync<TResponse>(request)` | `GET /instances/:id/group/info` | Get group metadata by `GroupId`. |

Other public namespaces are available as first-class resources too: `Sessions`, `Users`, `Queues`, `TypeBots`, `Chatwoot`, and `Chats`.

### `client.RequestAsync<TResponse>(method, path, options?)`

Escape hatch for advanced calls or newly released endpoints:

```csharp
var instance = await client.RequestAsync<object>(
    HttpMethod.Post,
    "/instances/get",
    new VZaps.Http.VZapsRequestOptions
    {
        Body = new { id = "VZ..." },
    });
```

---

## Instance tokens

Instance-scoped routes require the instance token in addition to account credentials. Pass it on each request that targets an instance:

```csharp
await client.Messages.SendTextAsync<object>(new SendTextMessageRequest
{
    InstanceId = "VZ...",
    InstanceToken = "instance-token",
    Phone = "5511999999999",
    Message = "Hello",
});
```

Or pass `InstanceRequestOptions` on resource methods that accept it:

```csharp
await client.Instances.RestartAsync<object>(
    "VZ...",
    new InstanceRequestOptions { InstanceToken = "instance-token" });
```

---

## Webhooks

Configure HTTP callbacks for instance events (same payload shape as realtime `data`, delivered to your URL):

```csharp
await client.Webhooks.SetAsync<object>(new WebhookSetRequest
{
    InstanceId = "VZ...",
    InstanceToken = "instance-token",
    WebhookURL = "https://example.com/webhooks/vzaps",
    Events = new[] { "Message", "Connected", "Disconnected" },
});
```

Common event types: `Message`, `ReadReceipt`, `Connected`, `Disconnected`, `Presence`, `ChatPresence`, `HistorySync`, `GroupParticipantsAdd`, `GroupParticipantsRemove`, or `All`.

Event payloads (webhook and realtime) use **snake_case**, matching the platform. Incoming media events include `media_url` inside `data` when platform storage is available.

---

## Realtime events

Subscribe to the same events over WebSocket at **`wss://realtime.vzaps.com`**. This is the recommended path for in-app notifications, bots, and dashboards that need low-latency delivery without exposing a public webhook URL.

### Subscribe

```csharp
await using var subscription = await client.Events.SubscribeAsync(
    new VZapsEventSubscribeRequest
    {
        InstanceId = "VZ...",
        InstanceToken = "instance-token",
        Events = new[] { VZapsEventType.Message, VZapsEventType.Connected, VZapsEventType.Disconnected },
        Reconnect = true,
        LastEventId = "evt_previous_id", // optional resume after disconnect
    },
    cancellationToken);

subscription.On(VZapsEventType.Message, evt =>
{
    Console.WriteLine(evt.Data);
});

subscription.OnError(error => Task.CompletedTask);

await subscription.WaitForCloseAsync(cancellationToken);
```

### Event envelope

Each WebSocket message keeps the platform shape (`snake_case`):

```json
{
  "id": "evt_…",
  "type": "Message",
  "instance_id": "VZ…",
  "created_at": "2026-06-23T22:57:17.000Z",
  "data": {
    "type": "Message",
    "event": { },
    "media_url": "https://…"
  }
}
```

- **`data`** — same payload as webhook delivery (`snake_case`).
- **`media_url`** — present on incoming media messages when platform storage is available.

### Delivery and ack

Delivery is **at-least-once**. After your handler runs, the SDK sends an ack automatically on the WebSocket connection. Use `LastEventId` when reconnecting if you need to reduce gaps. Deduplicate on `evt.Id` in your application if you process events idempotently.

### Subscribe options

| Option | Type | Default | Description |
| --- | --- | --- | --- |
| `InstanceId` | `string` | — | **Required.** Instance to watch. |
| `Events` | `VZapsEventType[]` | all subscribed | Comma-filtered event types. |
| `InstanceToken` | `string` | — | **Required.** Instance token for authorization. |
| `Reconnect` | `bool` | `true` | Reconnect after socket close. |
| `MaxRetries` | `int` | unlimited | Max reconnect attempts. |
| `RetryDelay` | `TimeSpan` | exponential backoff | Delay between reconnects. |
| `LastEventId` | `string` | — | Resume cursor after disconnect. |

### Handler registration

| Event name | When it fires |
| --- | --- |
| `OnError` | Handler or transport error. |
| `On(VZapsEventType.Message, …)`, `On(VZapsEventType.Connected, …)` | Matching realtime event type. |
| `On(VZapsEventType.All, …)` | Every event type. |

---

## Error handling

The SDK throws typed exceptions you can catch and branch on:

| Class | When |
| --- | --- |
| `VZapsException` | Base class for SDK failures. |
| `VZapsApiException` | Base for HTTP API errors; includes `StatusCode`, `ErrorCode`, and `Details`. |
| `VZapsAuthenticationException` | Invalid `ClientToken` / `ClientSecret` (401/403). |
| `VZapsRateLimitException` | Rate limited (429). |
| `VZapsTimeoutException` | Request exceeded `Timeout`. |
| `VZapsRealtimeException` | Realtime handler or transport failures. |

```csharp
try
{
    await client.Messages.SendTextAsync<object>(request, cancellationToken);
}
catch (VZapsAuthenticationException)
{
    Console.WriteLine("Check client credentials");
}
catch (VZapsTimeoutException)
{
    Console.WriteLine("Request timed out");
}
catch (VZapsRateLimitException)
{
    Console.WriteLine("Rate limited");
}
catch (VZapsApiException ex)
{
    Console.WriteLine(ex.StatusCode, ex.Message, ex.Details);
}
```

Exception fields include `StatusCode`, `ErrorCode`, `Details`, `RequestId`, and a truncated `ResponseBody` when available.

---

## .NET

The package uses **PascalCase** for C# models and **camelCase JSON serialization** on HTTP request bodies and API responses. **Realtime and webhook event payloads stay in snake_case** so both delivery channels match the platform.

Resources accept a generic `TResponse` when you want strongly typed API responses:

```csharp
var page = await client.Instances.ListAsync<object>(new InstanceListRequest
{
    Page = 1,
    Size = 20,
    Search = "support",
});
```

Use `RequestAsync<TResponse>` when you need full control over method, path, and body.

---

## Documentation

- [VZaps docs](https://docs.vzaps.com)
- [API reference (OpenAPI)](https://docs.vzaps.com/api-reference)
- [Postman collections](https://docs.vzaps.com/postman/)
- [Report an issue](https://github.com/VZaps/vzaps-sdk-dotnet/issues)

---

## License

MIT © VZaps
