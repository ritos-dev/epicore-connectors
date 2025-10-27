using System.Net.Http.Headers;
using Microsoft.Extensions.Options;

using RTS.Service.Connector.Interfaces;
using RTS.Service.Connector.Application.Contracts;
using RTS.Service.Connector.Infrastructure.Tracelink;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddOpenApi();

// Tracelink configuration
builder.Services.Configure<TracelinkOptions>(builder.Configuration.GetSection(TracelinkOptions.SectionName));

builder.Services.AddHttpClient("Tracelink", (serviceProvider, client) =>
{
    var options = serviceProvider.GetRequiredService<IOptions<TracelinkOptions>>().Value;
    client.BaseAddress = new Uri(options.BaseUrl);
    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("x-access-token", options.ApiToken);
    client.Timeout = TimeSpan.FromSeconds(options.TimeoutSeconds);
});

builder.Services.AddSingleton<IBackgroundTaskQueue, BackgroundTaskQueue>();
builder.Services.AddHostedService<TracelinkBackgroundWorker>();
builder.Services.AddSingleton<ITracelinkClient, TracelinkClient>();

var traceLinkOptions = builder.Configuration
    .GetSection("TraceLink")
    .Get<TracelinkOptions>();

if (traceLinkOptions != null)
{
    Console.WriteLine($"[TraceLink] BaseUrl: {traceLinkOptions.BaseUrl}");
    Console.WriteLine($"[TraceLink] ApiToken loaded: {!string.IsNullOrWhiteSpace(traceLinkOptions.ApiToken)}");
}
else
{
    Console.WriteLine("[TraceLink] TraceLink options not configured.");
}

// Economic configuration
builder.Configuration["Economic:AgreementGrantToken"] = Environment.GetEnvironmentVariable("ECONOMIC_AGREEMENT_TOKEN");
builder.Configuration["Economic:AppSecretToken"] = Environment.GetEnvironmentVariable("ECONOMIC_APP_SECRET_TOKEN");

builder.WebHost.ConfigureKestrel(options =>
{
    options.ConfigureEndpointDefaults(endpoint =>
    {
        endpoint.Protocols = Microsoft.AspNetCore.Server.Kestrel.Core.HttpProtocols.Http1AndHttp2;
    });
});

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}

app.UseAuthorization();

app.MapControllers();

await app.RunAsync();
