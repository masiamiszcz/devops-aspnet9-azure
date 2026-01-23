using Serilog;


var builder = WebApplication.CreateBuilder(args);

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .Enrich.FromLogContext()
    .Enrich.WithEnvironmentName()
    .Enrich.WithThreadId()
    .WriteTo.Console(
        outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj} {Properties}{NewLine}{Exception}"
    )
    .CreateLogger();

builder.Host.UseSerilog();


// Add services
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHttpClient();

var app = builder.Build();
app.UseSerilogRequestLogging(options =>
{
    options.MessageTemplate =
        "HTTP {RequestMethod} {RequestPath} responded {StatusCode} in {Elapsed:0.0000} ms";
});


// Configure pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// ROOT endpoint
app.MapGet("/", (ILogger<Program> logger) =>
{
    logger.LogInformation("Root endpoint '/' called");

    return Results.Ok(new
    {
        name = "KORNELIA JEST SUPER",
        status = "Running",
        timestamp = DateTime.UtcNow
    });
});


// PRODUCTS endpoint (external API)
app.MapGet("/products", async (HttpClient http, ILogger<Program> logger) =>
{
    logger.LogInformation("Products endpoint '/products' called");

    var url =
        "https://api.coingecko.com/api/v3/coins/markets?vs_currency=usd";

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

app.MapGet("/health", () =>
{
    Log.Information("Health check endpoint called");

    return Results.Ok(new
    {
        status = "Healthy",
        timestamp = DateTime.UtcNow
    });
});


app.Run();
// To make Program class accessible for integration tests
public partial class Program { }