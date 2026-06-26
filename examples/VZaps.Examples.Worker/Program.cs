using VZaps;
using VZaps.Examples.Worker;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddVZapsClient(options =>
{
    options.ClientToken = builder.Configuration["VZaps:ClientToken"] ?? Environment.GetEnvironmentVariable("VZAPS_CLIENT_TOKEN");
    options.ClientSecret = builder.Configuration["VZaps:ClientSecret"] ?? Environment.GetEnvironmentVariable("VZAPS_CLIENT_SECRET");
});
builder.Services.AddHostedService<Worker>();

var host = builder.Build();
host.Run();
