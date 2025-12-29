namespace Recipe_Webpage.Middleware;

public class ChinaIpDetectionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ChinaIpDetectionMiddleware> _logger;
    
    // China IP ranges (simplified - major ranges)
    // In production, you'd use a proper GeoIP database like MaxMind
    private static readonly string[] ChinaIpPrefixes = new[]
    {
        "1.0.", "1.1.", "1.2.", "1.3.", "1.4.", "1.5.", "1.6.", "1.7.", "1.8.",
        "14.", "27.", "36.", "39.", "42.", "49.", "58.", "59.", "60.", "61.",
        "101.", "103.", "106.", "110.", "111.", "112.", "113.", "114.", "115.",
        "116.", "117.", "118.", "119.", "120.", "121.", "122.", "123.", "124.",
        "125.", "139.", "140.", "144.", "150.", "153.", "157.", "159.", "163.",
        "166.", "167.", "171.", "175.", "180.", "182.", "183.", "202.", "203.",
        "210.", "211.", "218.", "219.", "220.", "221.", "222.", "223."
    };

    public ChinaIpDetectionMiddleware(RequestDelegate next, ILogger<ChinaIpDetectionMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var culture = "en";
        var shouldSetCookie = false;
        
        // Check query string override first (for manual language selection)
        if (context.Request.Query.ContainsKey("lang"))
        {
            var lang = context.Request.Query["lang"].ToString().ToLower();
            if (lang == "zh" || lang == "cn" || lang == "chinese")
            {
                culture = "zh";
            }
            else
            {
                culture = "en";
            }
            shouldSetCookie = true; // Remember the manual selection
        }
        // Check cookie for language preference
        else if (context.Request.Cookies.ContainsKey("lang"))
        {
            var lang = context.Request.Cookies["lang"]?.ToLower();
            if (lang == "zh")
            {
                culture = "zh";
            }
        }
        // Check IP address
        else
        {
            var ipAddress = GetClientIpAddress(context);
            if (!string.IsNullOrEmpty(ipAddress) && IsChineseIp(ipAddress))
            {
                culture = "zh";
                _logger.LogInformation("Chinese IP detected: {IpAddress}", ipAddress);
            }
        }
        
        // Set cookie if language was manually selected
        if (shouldSetCookie)
        {
            context.Response.Cookies.Append("lang", culture, new CookieOptions
            {
                Expires = DateTimeOffset.UtcNow.AddYears(1),
                IsEssential = true,
                SameSite = SameSiteMode.Lax
            });
        }
        
        // Store culture in HttpContext for use by LocalizationService
        context.Items["Culture"] = culture;
        
        // Set response header for debugging
        context.Response.Headers["X-Culture"] = culture;
        
        await _next(context);
    }

    private string? GetClientIpAddress(HttpContext context)
    {
        // Check for forwarded IP (when behind proxy/load balancer)
        var forwardedFor = context.Request.Headers["X-Forwarded-For"].FirstOrDefault();
        if (!string.IsNullOrEmpty(forwardedFor))
        {
            // Take the first IP in the chain
            return forwardedFor.Split(',')[0].Trim();
        }
        
        // Check CF-Connecting-IP for Cloudflare
        var cfIp = context.Request.Headers["CF-Connecting-IP"].FirstOrDefault();
        if (!string.IsNullOrEmpty(cfIp))
        {
            return cfIp;
        }
        
        // Fall back to connection remote IP
        return context.Connection.RemoteIpAddress?.ToString();
    }

    private bool IsChineseIp(string ipAddress)
    {
        // Skip localhost/private IPs
        if (ipAddress.StartsWith("127.") || 
            ipAddress.StartsWith("10.") || 
            ipAddress.StartsWith("192.168.") ||
            ipAddress.StartsWith("172.16.") ||
            ipAddress == "::1")
        {
            return false;
        }
        
        foreach (var prefix in ChinaIpPrefixes)
        {
            if (ipAddress.StartsWith(prefix))
            {
                return true;
            }
        }
        
        return false;
    }
}

public static class ChinaIpDetectionMiddlewareExtensions
{
    public static IApplicationBuilder UseChinaIpDetection(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<ChinaIpDetectionMiddleware>();
    }
}
