using System.Text.Json;
using ApiTesting.Specs.Config;
using ApiTesting.Specs.Core;
using FluentAssertions;
using Xunit;

namespace ApiTesting.Specs.Tests;

public class HealthTests
{
    private readonly TestSettings _s = TestSettings.Load();

    [Fact]
    public async Task Health_endpoint_returns_200()
    {
        var api = new ApiClient(_s.BaseUrl, _s.TimeoutMs);

        var res = await api.SendAsync(HttpMethod.Get, "/health");

        res.StatusCode.Should().Be(200);
    }

    [Fact]
    public async Task Health_endpoint_returns_ok_status()
    {
        var api = new ApiClient(_s.BaseUrl, _s.TimeoutMs);

        var res = await api.SendAsync(HttpMethod.Get, "/health");

        using var json = JsonDocument.Parse(res.Body);
        json.RootElement.GetProperty("status").GetString().Should().Be("ok");
    }

    [Fact]
    public async Task Health_endpoint_responds_fast()
    {
        var api = new ApiClient(_s.BaseUrl, _s.TimeoutMs);

        var res = await api.SendAsync(HttpMethod.Get, "/health");

        res.ElapsedMs.Should().BeLessThan(500);
    }
}
