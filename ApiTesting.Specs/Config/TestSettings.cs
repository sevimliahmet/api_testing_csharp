using System;
using Microsoft.Extensions.Configuration;

namespace ApiTesting.Specs.Config;

public sealed class TestSettings
{
    public string BaseUrl { get; set; } = "http://localhost:5052";
    public int TimeoutMs { get; set; } = 10_000;
    public int MaxResponseMs { get; set; } = 1_500;

    public int RetryCount { get; set; } = 2;
    public int RetryDelayMs { get; set; } = 200;

    public static TestSettings Load()
    {
        var cfg = new ConfigurationBuilder()
            .SetBasePath(AppContext.BaseDirectory)
            .AddJsonFile("Config/appsettings.json", optional: true)
            .AddEnvironmentVariables(prefix: "API_")
            .Build();

        var s = cfg.Get<TestSettings>() ?? new TestSettings();

        if (string.IsNullOrWhiteSpace(s.BaseUrl))
            s.BaseUrl = "http://localhost:5052";

        if (s.TimeoutMs <= 0)
            s.TimeoutMs = 10_000;

        if (s.MaxResponseMs <= 0)
            s.MaxResponseMs = 1_500;

        return s;
    }
}
