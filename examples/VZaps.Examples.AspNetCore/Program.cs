using System.Text.Json;
using VZaps;
using VZaps.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddVZapsClient(options =>
{
    options.ClientToken = builder.Configuration["VZaps:ClientToken"] ?? Environment.GetEnvironmentVariable("VZAPS_CLIENT_TOKEN");
    options.ClientSecret = builder.Configuration["VZaps:ClientSecret"] ?? Environment.GetEnvironmentVariable("VZAPS_CLIENT_SECRET");
});

var app = builder.Build();

app.MapGet("/instances", async (VZapsClient vzaps, CancellationToken cancellationToken) =>
{
    var result = await vzaps.Instances.ListAsync<JsonElement>(new InstanceListRequest { PageSize = 20 }, cancellationToken);
    return Results.Json(result);
});

app.MapPost("/instances/{instanceId}/messages/text", async (
    VZapsClient vzaps,
    string instanceId,
    SendTextDto input,
    CancellationToken cancellationToken) =>
{
    var result = await vzaps.Messages.SendTextAsync<JsonElement>(new SendTextMessageRequest
    {
        InstanceId = instanceId,
        InstanceToken = input.InstanceToken,
        Phone = input.Phone,
        Message = input.Message,
    }, cancellationToken);

    return Results.Json(result);
});

app.Run();

internal sealed record SendTextDto(string InstanceToken, string Phone, string Message);
