using Microsoft.AspNetCore.Http;
using InternshipManagement.API.Services;

namespace InternshipManagement.API.Middleware;

public class ActivityLoggingMiddleware
{
    private readonly RequestDelegate _next;

    public ActivityLoggingMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context, ActivityLogService logService)
    {
        var originalPath = context.Request.Path.Value?.ToLowerInvariant();

        // فقط سجل الأفعال المهمة
        if (originalPath?.Contains("/api/") == true)
        {
            var userId = context.User.FindFirst("nameid")?.Value;
            if (Guid.TryParse(userId, out var parsedUserId))
            {
                string action = DetermineAction(context.Request);

                if (!string.IsNullOrEmpty(action))
                {
                    var ipAddress = context.Connection.RemoteIpAddress?.ToString();
                    await logService.LogActivityAsync(parsedUserId, action, ipAddress: ipAddress);
                }
            }
        }

        await _next(context);
    }

    private string DetermineAction(HttpRequest request)
    {
        var path = request.Path.Value?.ToLowerInvariant();
        var method = request.Method;

        // مثال: "/api/auth/login" + "POST" => "User Login"
        if (path?.Contains("/api/auth/login") == true && method == "POST")
            return "User Login";
        if (path?.Contains("/api/auth/logout") == true && method == "POST")
            return "User Logout";
        if (path?.Contains("/api/student/internships") == true && method == "POST")
            return "Create Internship Request";
        if (path?.Contains("/api/sitesupervisor/internships/status") == true && method == "PUT")
            return "Approve/Reject Internship";
        if (path?.Contains("/api/student/reports") == true && method == "POST")
            return "Submit Weekly Report";
        if (path?.Contains("/api/messages") == true && method == "POST")
            return "Send Message";
        // أضف المزيد من الحالات حسب الحاجة

        return string.Empty; // لا تقم بالتسجيل إذا لم يكن الحدث محددًا
    }
}