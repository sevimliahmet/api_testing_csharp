using System.Diagnostics;
using System.Net.Http.Headers;
using System.Text;

namespace ApiTesting.Specs.Core;

public class ApiClient
{
    private readonly HttpClient _http;
    private readonly int _retryCount;
    private readonly int _retryDelayMs;

    public ApiClient(string baseUrl, int timeoutMs, int retryCount = 0, int retryDelayMs = 200)
    {
        _http = new HttpClient
        {
            BaseAddress = new Uri(baseUrl),
            Timeout = TimeSpan.FromMilliseconds(timeoutMs)
        };

        _http.DefaultRequestHeaders.Accept.Add(
            new MediaTypeWithQualityHeaderValue("application/json")
        );

        _retryCount = retryCount;
        _retryDelayMs = retryDelayMs;
    }

    public async Task<ResponseResult> SendAsync(HttpMethod method, string path, string? jsonPayload = null)
    {
        Exception? lastException = null;
        
        TestLogger.LogRequest(method, path, jsonPayload);
        
        for (int attempt = 0; attempt <= _retryCount; attempt++)
        {
            try
            {
                using var req = new HttpRequestMessage(method, path);

                if (jsonPayload != null)
                    req.Content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");

                var sw = Stopwatch.StartNew();
                using var resp = await _http.SendAsync(req);
                sw.Stop();

                var body = await resp.Content.ReadAsStringAsync();
                
                var result = new ResponseResult((int)resp.StatusCode, body, sw.ElapsedMilliseconds);
                TestLogger.LogResponse(result.StatusCode, result.Body, result.ElapsedMs);

                return result;
            }
            catch (HttpRequestException ex) when (attempt < _retryCount)
            {
                lastException = ex;
                TestLogger.LogError($"Attempt {attempt + 1} failed, retrying...", ex);
                await Task.Delay(_retryDelayMs);
            }
            catch (TaskCanceledException ex) when (attempt < _retryCount)
            {
                lastException = ex;
                TestLogger.LogError($"Attempt {attempt + 1} timed out, retrying...", ex);
                await Task.Delay(_retryDelayMs);
            }
        }

        var finalException = new InvalidOperationException($"Request failed after {_retryCount + 1} attempts", lastException);
        TestLogger.LogError("All retry attempts failed", finalException);
        throw finalException;
    }

    public async Task<ResponseResult> SendAsync(HttpMethod method, string path, string? payload, string contentType)
    {
        Exception? lastException = null;
        
        TestLogger.LogRequest(method, path, payload);
        
        for (int attempt = 0; attempt <= _retryCount; attempt++)
        {
            try
            {
                using var req = new HttpRequestMessage(method, path);

                if (payload != null)
                    req.Content = new StringContent(payload, Encoding.UTF8, contentType);

                var sw = Stopwatch.StartNew();
                using var resp = await _http.SendAsync(req);
                sw.Stop();

                var body = await resp.Content.ReadAsStringAsync();
                
                var result = new ResponseResult((int)resp.StatusCode, body, sw.ElapsedMilliseconds);
                TestLogger.LogResponse(result.StatusCode, result.Body, result.ElapsedMs);

                return result;
            }
            catch (HttpRequestException ex) when (attempt < _retryCount)
            {
                lastException = ex;
                TestLogger.LogError($"Attempt {attempt + 1} failed, retrying...", ex);
                await Task.Delay(_retryDelayMs);
            }
            catch (TaskCanceledException ex) when (attempt < _retryCount)
            {
                lastException = ex;
                TestLogger.LogError($"Attempt {attempt + 1} timed out, retrying...", ex);
                await Task.Delay(_retryDelayMs);
            }
        }

        var finalException = new InvalidOperationException($"Request failed after {_retryCount + 1} attempts", lastException);
        TestLogger.LogError("All retry attempts failed", finalException);
        throw finalException;
    }
}
