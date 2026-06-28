# VZaps .NET SDK Examples

Runnable sample projects that consume the published NuGet package (`VZaps.SDK`).

You do **not** need to clone the full SDK repository to run the console examples. Download only the project folder you need, restore packages, set environment variables, and run.

## Prerequisites

- .NET 8 SDK or later

## Option A — console sample only (recommended)

Download only [`VZaps.Examples.Console`](https://github.com/VZaps/vzaps-sdk-dotnet/tree/main/examples/VZaps.Examples.Console):

1. Open the folder on GitHub and choose **Download ZIP**, or run:

```bash
npx --yes degit VZaps/vzaps-sdk-dotnet/examples/VZaps.Examples.Console vzaps-dotnet-console
cd vzaps-dotnet-console
```

2. Set credentials:

```powershell
$env:VZAPS_CLIENT_TOKEN="your-client-token"
$env:VZAPS_CLIENT_SECRET="your-client-secret"
$env:VZAPS_INSTANCE_ID="VZ..."
$env:VZAPS_INSTANCE_TOKEN="your-instance-token"
```

3. Run one numbered flow:

```bash
dotnet run -- 07
```

| Argument | Topic |
| --- | --- |
| `01` | Auth and instance listing |
| `02` | Create instance |
| `03` | Billing subscription |
| `04` | Session and pairing |
| `05` | Webhook configuration |
| `06` | Realtime subscription |
| `07` | Send text message |
| `08` | Media and interactive messages |
| `09` | Poll, reaction, and chat actions |
| `10` | Queues |
| `11` | TypeBot and Chatwoot |

## Option B — sparse checkout

```bash
git clone --depth 1 --filter=blob:none --sparse https://github.com/VZaps/vzaps-sdk-dotnet.git
cd vzaps-sdk-dotnet
git sparse-checkout set examples/VZaps.Examples.Console
cd examples/VZaps.Examples.Console
dotnet run -- 07
```

## Option C — full repository clone

```bash
git clone https://github.com/VZaps/vzaps-sdk-dotnet.git
cd vzaps-sdk-dotnet/examples/VZaps.Examples.Console
dotnet run -- 07
```

When developing the SDK locally, replace the NuGet reference with a project reference to `src/VZaps.SDK/VZaps.SDK.csproj`.

## Additional samples

| Project | Topic |
| --- | --- |
| `VZaps.Examples.Worker` | Realtime WebSocket worker |
| `VZaps.Examples.AspNetCore` | ASP.NET Core integration sample |

## Coverage

- Auth and instance listing
- Instance creation and billing subscription checkout
- Session status, QR, and phone pairing code
- Webhook and realtime subscription
- Text, media, buttons, list, poll, reactions, presence
- Queue list/remove/purge examples
- TypeBot and Chatwoot integration examples
