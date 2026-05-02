/*
 * Author: Nikolay Dvurechensky
 * Site: https://dvurechensky.pro/
 * Gmail: dvurechenskysoft@gmail.com
 * Last Updated: 02 мая 2026 19:36:20
 * Version: 1.0.37
 */

using Microsoft.AspNetCore.Mvc.Testing;

namespace LizeriumServer.IntegrationTests;

public class MiddlewareTests: IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;

    public MiddlewareTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task Returns200OnRoot()
    {
        var client = _factory.CreateClient();
        var response = await client.GetAsync("/");
        response.EnsureSuccessStatusCode();
    }
}
