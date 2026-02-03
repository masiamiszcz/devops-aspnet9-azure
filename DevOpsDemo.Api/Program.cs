using Serilog;
using Microsoft.ApplicationInsights.Extensibility;

var builder = WebApplication.CreateBuilder(args);

// ===== SERILOG + APPLICATION INSIGHTS =====
builder.Host.UseSerilog((context, services, configuration) =>
{
    configuration
        .MinimumLevel.Information()
        .Enrich.FromLogContext()
        .Enrich.WithEnvironmentName()
        .Enrich.WithThreadId()
        .WriteTo.Console(
            outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj} {Properties}{NewLine}{Exception}"
        )
        .WriteTo.ApplicationInsights(
            services.GetRequiredService<TelemetryConfiguration>(),
            TelemetryConverter.Traces // logi jako traces
        );
});

// ===== ADD SERVICES =====
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHttpClient();
builder.Services.AddApplicationInsightsTelemetry(); // <- AI SDK

var app = builder.Build();

// ===== REQUEST LOGGING =====
app.UseSerilogRequestLogging(options =>
{
    options.MessageTemplate =
        "HTTP {RequestMethod} {RequestPath} responded {StatusCode} in {Elapsed:0.0000} ms";
});

// ===== SWAGGER =====
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// ===== ENDPOINTS =====

// ROOT endpoint
app.MapGet("/", (ILogger<Program> logger) =>
{
    logger.LogInformation("Root endpoint '/' called");

    return Results.Ok(new
    {
        name = "TEST123",
        status = "Running",
        timestamp = DateTime.UtcNow
    });
});

// PRODUCTS endpoint (external API)
app.MapGet("/products", async (HttpClient http, ILogger<Program> logger) =>
{
    logger.LogInformation("Products endpoint '/products' called");

    var url =
        "https://api.coingecko.com/api/v3/coins/markets?vs_currency=usd&ids=bitcoin";

    var request = new HttpRequestMessage(HttpMethod.Get, url);
    request.Headers.Add("User-Agent", "DevOpsDemoApp/1.0");

    try
    {
        var response = await http.SendAsync(request);
        response.EnsureSuccessStatusCode();

        logger.LogInformation("Successfully fetched products from CoinGecko");

        var json = await response.Content.ReadAsStringAsync();
        return Results.Content(json, "application/json");
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "Error while fetching products from CoinGecko");
        return Results.Problem("External API error");
    }
});

// HELLO endpoint
app.MapGet("/hello", () =>
{
    Log.Information("Hello endpoint called");

    return Results.Ok(new
    {
        message = "hello n",
        timestamp = DateTime.UtcNow
    });
});

// HEALTH endpoint
app.MapGet("/health", () =>
{
    Log.Information("Health check endpoint called");

    return Results.Ok(new
    {
        status = "Healthy",
        timestamp = DateTime.UtcNow
    });
});

// ===== VERSION endpoint (opcjonalnie, do automatycznego wersjonowania) =====
app.MapGet("/version", () =>
{
    var commitSha = Environment.GetEnvironmentVariable("GIT_SHA") ?? "unknown";
    return Results.Ok(new
    {
        version = commitSha,
        timestamp = DateTime.UtcNow
    });
});

app.Run();

// To make Program class accessible for integration tests
public partial class Program { }
