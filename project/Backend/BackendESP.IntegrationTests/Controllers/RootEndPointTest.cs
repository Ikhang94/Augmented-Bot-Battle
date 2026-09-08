using System.Net;
using BackendESP.IntegrationTests.Infrastructure;
using Xunit;

namespace BackendESP.IntegrationTests.Controllers;

public class RootEndpointTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;

    public RootEndpointTests(CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task Get_Root_Returns_HelloWorld()
    {
        var response = await _client.GetAsync("/");
        var content = await response.Content.ReadAsStringAsync();

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Contains("Hello World", content);
    }

    [Fact]
    public async Task Get_Players_Returns_Success()
    {
        var response = await _client.GetAsync("/api/users/getall");

        Assert.NotEqual(HttpStatusCode.InternalServerError, response.StatusCode);
    }
}
