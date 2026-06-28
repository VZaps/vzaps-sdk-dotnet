# VZaps .NET SDK Examples

Runnable sample projects that reference the SDK source from this repository.

## Prerequisites

- [.NET SDK](https://dotnet.microsoft.com/download) 8.0 or later

## Option A — console sample only

Download the console project **and** the SDK source (project reference):

```bash
git clone --depth 1 --filter=blob:none --sparse https://github.com/VZaps/vzaps-sdk-dotnet.git
cd vzaps-sdk-dotnet
git sparse-checkout set examples/VZaps.Examples.Console src/VZaps.SDK Directory.Build.props Directory.Packages.props
cd examples/VZaps.Examples.Console
```

Or create a standalone app from NuGet (no clone):

```bash
dotnet new console -f net8.0 -n vzaps-dotnet-console
cd vzaps-dotnet-console
dotnet add package VZaps.SDK
```

Set credentials and run (after copying example `Program.cs` flows from the repo if using NuGet):

```powershell
$env:VZAPS_CLIENT_TOKEN="your-client-token"
$env:VZAPS_CLIENT_SECRET="your-client-secret"
$env:VZAPS_INSTANCE_ID="VZ..."
$env:VZAPS_INSTANCE_TOKEN="your-instance-token"
```

Run one numbered flow:

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

## Option B — sparse checkout (console only)

```bash
git clone --depth 1 --filter=blob:none --sparse https://github.com/VZaps/vzaps-sdk-dotnet.git
cd vzaps-sdk-dotnet
git sparse-checkout set examples/VZaps.Examples.Console src/VZaps.SDK Directory.Build.props Directory.Packages.props
cd examples/VZaps.Examples.Console
dotnet run -- 07
```

## Option C — full repository clone

```bash
git clone https://github.com/VZaps/vzaps-sdk-dotnet.git
cd vzaps-sdk-dotnet/examples/VZaps.Examples.Console
dotnet run -- 07
```

Examples in this repository use a **project reference** to `src/VZaps.SDK`. Standalone apps can install **`VZaps.SDK`** from NuGet instead.

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
