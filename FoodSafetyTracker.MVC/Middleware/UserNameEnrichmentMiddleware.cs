using Serilog.Context;

namespace FoodSafetyTracker.MVC.Middleware;

public class UserNameEnrichmentMiddleware
{
    private readonly RequestDelegate _next;

    public UserNameEnrichmentMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var username = context.User.Identity?.IsAuthenticated == true
            ? context.User.Identity.Name ?? "Anonymous"
            : "Anonymous";

        using (LogContext.PushProperty("UserName", username))
        {
            await _next(context);
        }
    }
}