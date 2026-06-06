/*
 * Author: Nikolay Dvurechensky
 * Site: https://dvurechensky.pro/
 * Gmail: dvurechenskysoft@gmail.com
 * Last Updated: 06 июня 2026 09:06:57
 * Version: 1.0.71
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
