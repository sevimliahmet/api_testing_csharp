using Microsoft.Extensions.Configuration;

namespace ApiTesting.Specs.Config;

public class TestSettings
{
    public string BaseUrl { get; init; } = "";
    public int TimeoutMs { get; init; }
    public long MaxResponseMs { get; init; }

    public static TestSettings Load()
    {
        var cfg = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("Config/appsettings.json", optional: false)
            .Build();

        return cfg.Get<TestSettings>()!;
    }
}
