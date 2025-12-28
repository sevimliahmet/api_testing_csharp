using System.Diagnostics;
using System.Net.Http.Headers;
using System.Text;

namespace ApiTesting.Specs.Core;

public class ApiClient
{
    private readonly HttpClient _http;

    public ApiClient(string baseUrl, int timeoutMs)
    {
        _http = new HttpClient
        {
            BaseAddress = new Uri(baseUrl),
            Timeout = TimeSpan.FromMilliseconds(timeoutMs)
        };

        _http.DefaultRequestHeaders.Accept.Add(
            new MediaTypeWithQualityHeaderValue("application/json")
        );
    }

    public async Task<ResponseResult> SendAsync(HttpMethod method, string path, string? jsonPayload = null)
    {
        using var req = new HttpRequestMessage(method, path);

        if (jsonPayload != null)
            req.Content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");

        var sw = Stopwatch.StartNew();
        using var resp = await _http.SendAsync(req);
        sw.Stop();

        var body = await resp.Content.ReadAsStringAsync();

        return new ResponseResult((int)resp.StatusCode, body, sw.ElapsedMilliseconds);
    }
}
