using RTS.Service.Connector.Infrastructure.Tracelink;

using System.Net.Http.Headers;
using Microsoft.Extensions.Options;


var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddOpenApi();

// Tracelink configuration
builder.Services.Configure<TracelinkOptions>( builder.Configuration.GetSection(TracelinkOptions.SectionName ));

builder.Services.AddHttpClient("Tracelink", (serviceProvider, client) =>
{
    var options = serviceProvider.GetRequiredService<IOptions<TracelinkOptions>>().Value;
    client.BaseAddress = new Uri(options.BaseUrl);
    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", options.ApiToken);
    client.Timeout = TimeSpan.FromSeconds(options.TimeoutSeconds);
});

var traceLinkOptions = builder.Configuration
    .GetSection("TraceLink")
    .Get<TracelinkOptions>();

Console.WriteLine($"[TraceLink] BaseUrl: {traceLinkOptions.BaseUrl}");
Console.WriteLine($"[TraceLink] ApiToken loaded: {!string.IsNullOrWhiteSpace(traceLinkOptions.ApiToken)}");

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
