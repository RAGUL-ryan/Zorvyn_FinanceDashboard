using System.Collections.Concurrent;
using System.Net;
using System.Text.Json;
using FinanceDashboard.Business.DTOs;

namespace FinanceDashboard.API.Middleware;

public class RateLimitingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<RateLimitingMiddleware> _logger;

    // ip -> (window start, request count)
    private static readonly ConcurrentDictionary<string, (DateTime WindowStart, int Count)> _clients = new();

    private const int    MaxRequests  = 100;
    private static readonly TimeSpan Window = TimeSpan.FromMinutes(1);

    public RateLimitingMiddleware(RequestDelegate next, ILogger<RateLimitingMiddleware> logger)
    {
        _next   = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var ip = context.Connection.RemoteIpAddress?.ToString() ?? "unknown";

        var now = DateTime.UtcNow;
        var entry = _clients.GetOrAdd(ip, _ => (now, 0));

        // Reset window if expired
        if (now - entry.WindowStart > Window)
            entry = (now, 0);

        entry = (entry.WindowStart, entry.Count + 1);
        _clients[ip] = entry;

        if (entry.Count > MaxRequests)
        {
            _logger.LogWarning("Rate limit exceeded for IP: {IP}", ip);

            context.Response.StatusCode  = (int)HttpStatusCode.TooManyRequests;
            context.Response.ContentType = "application/json";
            context.Response.Headers["Retry-After"] = "60";

            var response = ApiResponse.Fail("Rate limit exceeded. Please wait before making more requests.");
            await context.Response.WriteAsync(JsonSerializer.Serialize(response, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            }));
            return;
        }

        // Expose rate-limit headers
        context.Response.Headers["X-RateLimit-Limit"]     = MaxRequests.ToString();
        context.Response.Headers["X-RateLimit-Remaining"] = Math.Max(0, MaxRequests - entry.Count).ToString();

        await _next(context);
    }
}
