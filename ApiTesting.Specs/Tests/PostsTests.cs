using System.Text.Json;
using ApiTesting.Specs.Config;
using ApiTesting.Specs.Core;
using ApiTesting.Specs.Helpers;
using FluentAssertions;
using Xunit;

namespace ApiTesting.Specs.Tests;

public class PostsTests : IAsyncLifetime
{
    private readonly TestSettings _s = TestSettings.Load();
    private ApiClient? _api;

    public async Task InitializeAsync()
    {
        _api = new ApiClient(_s.BaseUrl, _s.TimeoutMs, _s.RetryCount, _s.RetryDelayMs);
        
        // Verify API is reachable
        try
        {
            var health = await _api.SendAsync(HttpMethod.Get, "/health");
            if (health.StatusCode != 200)
                throw new InvalidOperationException($"API health check failed with status {health.StatusCode}");
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException("API is not reachable. Make sure the API is running.", ex);
        }
    }

    public Task DisposeAsync()
    {
        // Cleanup if needed
        return Task.CompletedTask;
    }

    [Fact]
    public async Task GET_valid_post_returns_200_fast()
    {
        var res = await _api!.SendAsync(HttpMethod.Get, "/posts/1");

        res.StatusCode.Should().Be(200);
        res.ElapsedMs.Should().BeLessThanOrEqualTo(_s.MaxResponseMs);

        using var json = JsonDocument.Parse(res.Body);
        json.RootElement.GetProperty("id").GetInt32().Should().Be(1);
    }

    [Fact]
    public async Task GET_invalid_post_returns_404()
    {
        var res = await _api!.SendAsync(HttpMethod.Get, "/posts/999");

        res.StatusCode.Should().Be(404);
    }

    [Fact]
    public async Task POST_creates_post()
    {
        var payload = PostBuilder.New()
            .WithTitle("hello")
            .WithBody("from test")
            .WithUserId(1)
            .BuildJson();

        var res = await _api!.SendAsync(HttpMethod.Post, "/posts", payload);

        res.StatusCode.Should().Be(201);

        using var json = JsonDocument.Parse(res.Body);
        json.RootElement.GetProperty("title").GetString().Should().Be("hello");
    }

    [Fact]
    public async Task GET_zero_id_returns_400()
    {
        var res = await _api!.SendAsync(HttpMethod.Get, "/posts/0");

        res.StatusCode.Should().Be(400);
    }

    [Fact]
    public async Task GET_negative_id_returns_400()
    {
        var res = await _api!.SendAsync(HttpMethod.Get, "/posts/-5");

        res.StatusCode.Should().Be(400);
    }

    [Fact]
    public async Task POST_with_empty_title_creates_post()
    {
        var payload = PostBuilder.New()
            .WithTitle("")
            .WithBody("body only")
            .BuildJson();

        var res = await _api!.SendAsync(HttpMethod.Post, "/posts", payload);

        res.StatusCode.Should().Be(201);
    }

    [Fact]
    public async Task POST_with_special_characters_in_title()
    {
        var payload = PostBuilder.New()
            .WithTitle("Special: <>&\"'")
            .WithBody("Testing special chars")
            .BuildJson();

        var res = await _api!.SendAsync(HttpMethod.Post, "/posts", payload);

        res.StatusCode.Should().Be(201);
        
        using var json = JsonDocument.Parse(res.Body);
        json.RootElement.GetProperty("title").GetString().Should().Contain("Special");
    }

    [Fact]
    public async Task POST_with_large_content()
    {
        var largeBody = new string('x', 5000);
        var payload = PostBuilder.New()
            .WithTitle("Large content test")
            .WithBody(largeBody)
            .BuildJson();

        var res = await _api!.SendAsync(HttpMethod.Post, "/posts", payload);

        res.StatusCode.Should().Be(201);
    }

    [Theory]
    [InlineData(1)]
    [InlineData(100)]
    [InlineData(999)]
    public async Task GET_various_user_ids(int userId)
    {
        var payload = PostBuilder.New()
            .WithUserId(userId)
            .BuildJson();

        var res = await _api!.SendAsync(HttpMethod.Post, "/posts", payload);

        res.StatusCode.Should().Be(201);
        
        using var json = JsonDocument.Parse(res.Body);
        json.RootElement.GetProperty("userId").GetInt32().Should().Be(userId);
    }

    // Negative Test Scenarios
    
    [Fact]
    public async Task POST_with_invalid_json_returns_400()
    {
        var invalidJson = "{ this is not valid json }";

        var res = await _api!.SendAsync(HttpMethod.Post, "/posts", invalidJson);

        res.StatusCode.Should().Be(400);
    }

    [Fact]
    public async Task POST_with_malformed_json_returns_400()
    {
        var malformedJson = """{ "title": "test", "body": "test", "userId": }""";

        var res = await _api!.SendAsync(HttpMethod.Post, "/posts", malformedJson);

        res.StatusCode.Should().Be(400);
    }

    [Fact]
    public async Task POST_with_unclosed_brackets_returns_400()
    {
        var unclosedJson = """{ "title": "test", "body": "test" """;

        var res = await _api!.SendAsync(HttpMethod.Post, "/posts", unclosedJson);

        res.StatusCode.Should().Be(400);
    }

    [Fact]
    public async Task POST_with_extra_commas_returns_400()
    {
        var extraCommasJson = """{ "title": "test",, "body": "test", "userId": 1 }""";

        var res = await _api!.SendAsync(HttpMethod.Post, "/posts", extraCommasJson);

        res.StatusCode.Should().Be(400);
    }

    [Fact]
    public async Task POST_with_missing_title_field()
    {
        var missingTitle = """{ "body": "test body", "userId": 1 }""";

        var res = await _api!.SendAsync(HttpMethod.Post, "/posts", missingTitle);

        // Depending on API implementation, could be 400 or 201 with null/empty title
        res.StatusCode.Should().BeOneOf(201, 400);
    }

    [Fact]
    public async Task POST_with_missing_body_field()
    {
        var missingBody = """{ "title": "test title", "userId": 1 }""";

        var res = await _api!.SendAsync(HttpMethod.Post, "/posts", missingBody);

        res.StatusCode.Should().BeOneOf(201, 400);
    }

    [Fact]
    public async Task POST_with_missing_userId_field()
    {
        var missingUserId = """{ "title": "test title", "body": "test body" }""";

        var res = await _api!.SendAsync(HttpMethod.Post, "/posts", missingUserId);

        res.StatusCode.Should().BeOneOf(201, 400);
    }

    [Fact]
    public async Task POST_with_empty_body_returns_400()
    {
        var res = await _api!.SendAsync(HttpMethod.Post, "/posts", "");

        res.StatusCode.Should().Be(400);
    }

    [Fact]
    public async Task POST_with_text_plain_content_type_returns_415()
    {
        var payload = """{ "title": "test", "body": "test", "userId": 1 }""";

        var res = await _api!.SendAsync(HttpMethod.Post, "/posts", payload, "text/plain");

        res.StatusCode.Should().Be(415); // Unsupported Media Type
    }

    [Fact]
    public async Task POST_with_xml_content_type_returns_415()
    {
        var payload = """{ "title": "test", "body": "test", "userId": 1 }""";

        var res = await _api!.SendAsync(HttpMethod.Post, "/posts", payload, "application/xml");

        res.StatusCode.Should().Be(415);
    }

    [Fact]
    public async Task POST_with_null_title_in_json()
    {
        var nullTitle = """{ "title": null, "body": "test body", "userId": 1 }""";

        var res = await _api!.SendAsync(HttpMethod.Post, "/posts", nullTitle);

        res.StatusCode.Should().BeOneOf(201, 400);
    }

    [Fact]
    public async Task POST_with_null_body_in_json()
    {
        var nullBody = """{ "title": "test title", "body": null, "userId": 1 }""";

        var res = await _api!.SendAsync(HttpMethod.Post, "/posts", nullBody);

        res.StatusCode.Should().BeOneOf(201, 400);
    }

    [Fact]
    public async Task POST_with_invalid_userId_type_returns_400()
    {
        var invalidUserId = """{ "title": "test", "body": "test", "userId": "not_a_number" }""";

        var res = await _api!.SendAsync(HttpMethod.Post, "/posts", invalidUserId);

        res.StatusCode.Should().Be(400);
    }

    [Fact]
    public async Task POST_with_negative_userId()
    {
        var negativeUserId = """{ "title": "test", "body": "test", "userId": -1 }""";

        var res = await _api!.SendAsync(HttpMethod.Post, "/posts", negativeUserId);

        res.StatusCode.Should().BeOneOf(201, 400);
    }
}
