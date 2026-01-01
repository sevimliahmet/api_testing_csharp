using System.Text.Json;

namespace ApiTesting.Specs.Core;

public static class TestLogger
{
    private static readonly object _lock = new();

    public static void LogRequest(HttpMethod method, string path, string? payload = null)
    {
        lock (_lock)
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine($"\n→ {method.Method} {path}");
            
            if (!string.IsNullOrWhiteSpace(payload))
            {
                Console.ForegroundColor = ConsoleColor.DarkCyan;
                Console.WriteLine($"   Body: {FormatJson(payload)}");
            }
            
            Console.ResetColor();
        }
    }

    public static void LogResponse(int statusCode, string body, long elapsedMs)
    {
        lock (_lock)
        {
            var color = statusCode >= 200 && statusCode < 300 
                ? ConsoleColor.Green 
                : statusCode >= 400 
                    ? ConsoleColor.Red 
                    : ConsoleColor.Yellow;

            Console.ForegroundColor = color;
            Console.WriteLine($"← {statusCode} ({elapsedMs}ms)");
            
            if (!string.IsNullOrWhiteSpace(body))
            {
                Console.ForegroundColor = ConsoleColor.DarkGray;
                Console.WriteLine($"   Body: {FormatJson(body)}");
            }
            
            Console.ResetColor();
        }
    }

    public static void LogError(string message, Exception? ex = null)
    {
        lock (_lock)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"✗ {message}");
            
            if (ex != null)
            {
                Console.ForegroundColor = ConsoleColor.DarkRed;
                Console.WriteLine($"   {ex.Message}");
            }
            
            Console.ResetColor();
        }
    }

    private static string FormatJson(string json)
    {
        try
        {
            using var doc = JsonDocument.Parse(json);
            return JsonSerializer.Serialize(doc, new JsonSerializerOptions { WriteIndented = false });
        }
        catch
        {
            return json;
        }
    }
}
