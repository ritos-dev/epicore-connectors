using RTS.Service.Connector.API.Options;


var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddOpenApi();

builder.Services.Configure<TracelinkOptions>(builder.Configuration.GetSection(TracelinkOptions.SectionName));

// Enviroment variables
builder.Configuration["Tracelink:ApiToken"] = Environment.GetEnvironmentVariable("TRACELINK_API_TOKEN");
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
