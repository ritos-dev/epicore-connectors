var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddOpenApi();

// Enviroment variables
builder.Configuration["Tracelink:ApiToken"] = Environment.GetEnvironmentVariable("TRACELINK_API_TOKEN");
builder.Configuration["Economic:AgreementGrantToken"] = Environment.GetEnvironmentVariable("ECONOMIC_AGREEMENT_TOKEN");
builder.Configuration["Economic:AppSecretToken"] = Environment.GetEnvironmentVariable("ECONOMIC_APP_SECRET_TOKEN");

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

await app.RunAsync();
