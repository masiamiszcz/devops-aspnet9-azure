var builder = WebApplication.CreateBuilder(args);

// Add services
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHttpClient();

var app = builder.Build();

// Configure pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// ROOT endpoint
app.MapGet("/", () => Results.Ok(new
{
    name = "DevOps ASP.NET 9 Demo API",
    status = "Running",
    timestamp = DateTime.UtcNow
}));

// PRODUCTS endpoint (external API)
app.MapGet("/products", async (HttpClient http) =>
{
    var url =
        "https://api.coingecko.com/api/v3/coins/markets?vs_currency=usd";

    // Dodaj User-Agent
    var request = new HttpRequestMessage(HttpMethod.Get, url);
    request.Headers.Add("User-Agent", "DevOpsDemoApp/1.0");

    var response = await http.SendAsync(request);
    response.EnsureSuccessStatusCode();

    var json = await response.Content.ReadAsStringAsync();
    return Results.Content(json, "application/json");
});


app.Run();
