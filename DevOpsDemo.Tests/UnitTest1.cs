using System.Net;
using System.Text.Json;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Xunit;
using Microsoft.AspNetCore.Mvc.Testing;

namespace DevOpsDemo.Tests
{
    public class ApiTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly HttpClient _client;

        public ApiTests(WebApplicationFactory<Program> factory)
        {
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task RootEndpoint_ReturnsOk()
        {
            var response = await _client.GetAsync("/");
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadFromJsonAsync<JsonElement>();

            Assert.True(json.TryGetProperty("name", out var name));
            Assert.Equal("KORNELIA JEST SUPER", name.GetString());
        }

        [Fact]
        public async Task ProductsEndpoint_ReturnsData()
        {
            var response = await _client.GetAsync("/products");

            Assert.True(response.IsSuccessStatusCode || response.StatusCode == HttpStatusCode.Forbidden,
                "Expected 200 OK or 403 Forbidden from CoinGecko");

            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadFromJsonAsync<dynamic>();
                Assert.NotNull(json);
            }
        }
    }
}
