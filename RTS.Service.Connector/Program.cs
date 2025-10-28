using System.Net.Http.Headers;
using Microsoft.Extensions.Options;

using RTS.Service.Connector.Interfaces;
using RTS.Service.Connector.Infrastructure;
using RTS.Service.Connector.Infrastructure.Economic;
using RTS.Service.Connector.Infrastructure.Tracelink;


var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddOpenApi();

builder.Services.AddSingleton<IBackgroundTaskQueue, BackgroundTaskQueue>();
builder.Services.AddHostedService<ConnectorBackgroundWorker>();

// Tracelink configuration
builder.Services.Configure<TracelinkOptions>(builder.Configuration.GetSection(TracelinkOptions.SectionName));

builder.Services.AddHttpClient("Tracelink", (serviceProvider, client) =>
{
    var options = serviceProvider.GetRequiredService<IOptions<TracelinkOptions>>().Value;
    client.BaseAddress = new Uri(options.BaseUrl);
    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("x-access-token", options.ApiToken);
    client.Timeout = TimeSpan.FromSeconds(options.TimeoutSeconds);
});

var tracelinkOptions = builder.Configuration.GetSection("Tracelink").Get<TracelinkOptions>();

if (tracelinkOptions != null)
{
    Console.ForegroundColor = ConsoleColor.Cyan;
    Console.WriteLine($"[Tracelink] BaseUrl: {tracelinkOptions.BaseUrl}");
    Console.WriteLine($"[Tracelink] ApiToken loaded: {!string.IsNullOrWhiteSpace(tracelinkOptions.ApiToken)}");
    Console.ResetColor();
}
else
{
    Console.ForegroundColor = ConsoleColor.Cyan;
    Console.WriteLine("[Tracelink] Tracelink options not configured.");
    Console.ResetColor();
}

builder.Services.AddSingleton<ITracelinkClient, TracelinkClient>();

// Economic configuration
builder.Services.Configure<EconomicOptions>(builder.Configuration.GetSection(EconomicOptions.SectionName));

builder.Services.AddHttpClient("Economic", (serviceProvider, client) =>
{
    var options = serviceProvider.GetRequiredService<IOptions<EconomicOptions>>().Value;
    client.BaseAddress = new Uri(options.BaseUrl);
    client.DefaultRequestHeaders.Add("X-AppSecretToken", options.AppSecretToken);
    client.DefaultRequestHeaders.Add("X-AgreementGrantToken", options.AgreementGrantToken);
    client.Timeout = TimeSpan.FromSeconds(options.TimeoutSeconds);
});

var econOptions = builder.Configuration.GetSection("Economic").Get<EconomicOptions>();
if (econOptions != null)
{
    Console.ForegroundColor = ConsoleColor.Yellow;
    Console.WriteLine($"[Economic] BaseUrl: {econOptions.BaseUrl}");
    Console.WriteLine($"[Economic] Tokens loaded: {(!string.IsNullOrWhiteSpace(econOptions.AppSecretToken) && !string.IsNullOrWhiteSpace(econOptions.AgreementGrantToken))}");
    Console.ResetColor();
}
else
{
    Console.ForegroundColor = ConsoleColor.Yellow;
    Console.WriteLine("[Economic] Economic options not configured.");
    Console.ResetColor();
}

builder.Services.AddSingleton<IEconomicClient, EconomicClient>();

// Kestrel configuration to support HTTP/2
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
