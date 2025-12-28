using System.Text.Json;
using ApiTesting.Specs.Config;
using ApiTesting.Specs.Core;
using FluentAssertions;
using Xunit;

namespace ApiTesting.Specs.Tests;

public class PostsTests
{
    private readonly TestSettings _s = TestSettings.Load();

    [Fact]
    public async Task GET_post_should_return_200_and_expected_body_and_fast()
    {
        var api = new ApiClient(_s.BaseUrl, _s.TimeoutMs);

        var result = await api.SendAsync(HttpMethod.Get, "/posts/1");

        result.StatusCode.Should().Be(200);
        result.ElapsedMs.Should().BeLessThanOrEqualTo(_s.MaxResponseMs);

        using var json = JsonDocument.Parse(result.Body);
        json.RootElement.GetProperty("id").GetInt32().Should().Be(1);
    }

    [Fact]
    public async Task POST_should_return_201_and_expected_title()
    {
        var api = new ApiClient(_s.BaseUrl, _s.TimeoutMs);

        var payload = """
        { "title": "hello", "body": "from C#", "userId": 1 }
        """;

        var result = await api.SendAsync(HttpMethod.Post, "/posts", payload);

        result.StatusCode.Should().Be(201);

        using var json = JsonDocument.Parse(result.Body);
        json.RootElement.GetProperty("title").GetString().Should().Be("hello");
    }
}
