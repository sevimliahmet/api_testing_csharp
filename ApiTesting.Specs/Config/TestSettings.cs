using System;
using Microsoft.Extensions.Configuration;

namespace ApiTesting.Specs.Config;

public sealed class TestSettings
{
    public string BaseUrl { get; set; } = "http://localhost:5052";
    public int TimeoutMs { get; set; } = 10_000;
    public int MaxResponseMs { get; set; } = 1_500;

    // Şimdilik kullanılmıyor ama ileride retry eklemek istersem hazır dursun diye bıraktım
    public int RetryCount { get; set; } = 2;
    public int RetryDelayMs { get; set; } = 200;

    public static TestSettings Load()
    {
        // Not: appsettings.json dosyasını default ayarlar için kullanıyorum.
        // İstersem aynı ayarları environment variable ile override edebiliyorum.
        // Örn: API_BaseUrl, API_TimeoutMs, API_MaxResponseMs

        var cfg = new ConfigurationBuilder()
            .SetBasePath(AppContext.BaseDirectory)
            .AddJsonFile("Config/appsettings.json", optional: true, reloadOnChange: false)
            .AddEnvironmentVariables(prefix: "API_")
            .Build();

        var s = cfg.Get<TestSettings>() ?? new TestSettings();

        // Boş/saçma değer gelirse default’a düşsün
        if (string.IsNullOrWhiteSpace(s.BaseUrl))
            s.BaseUrl = "http://127.0.0.1:5055";

        if (s.TimeoutMs <= 0)
            s.TimeoutMs = 10_000;

        if (s.MaxResponseMs <= 0)
            s.MaxResponseMs = 1_500;

        return s;
    }
}
