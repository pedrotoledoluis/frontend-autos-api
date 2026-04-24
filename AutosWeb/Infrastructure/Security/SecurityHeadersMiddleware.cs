namespace AutosWeb.Infrastructure.Security;

/// <summary>
/// Agrega headers de seguridad estándar a todas las respuestas HTTP.
/// Cubre los Security Hotspots más comunes reportados por SonarQube
/// (<c>S5122</c>, <c>S5332</c>, <c>S5739</c>) y buenas prácticas de
/// OWASP Secure Headers.
/// </summary>
public sealed class SecurityHeadersMiddleware
{
    private readonly RequestDelegate _next;

    public SecurityHeadersMiddleware(RequestDelegate next) => _next = next;

    public Task InvokeAsync(HttpContext context)
    {
        var headers = context.Response.Headers;

        headers["X-Content-Type-Options"] = "nosniff";
        headers["X-Frame-Options"] = "DENY";
        headers["Referrer-Policy"] = "no-referrer";
        headers["Permissions-Policy"] = "camera=(), microphone=(), geolocation=(), payment=()";
        headers["Cross-Origin-Opener-Policy"] = "same-origin";
        headers["Cross-Origin-Resource-Policy"] = "same-origin";

        // CSP: sólo permite los CDN que la app carga vía SRI y los propios
        // estáticos. Sin `unsafe-inline` ni `unsafe-eval` → bloquea XSS inline.
        headers["Content-Security-Policy"] =
            "default-src 'self'; " +
            "script-src 'self' https://code.jquery.com https://cdn.jsdelivr.net; " +
            "style-src 'self' https://cdn.jsdelivr.net; " +
            "img-src 'self' data:; " +
            "font-src 'self' https://cdn.jsdelivr.net; " +
            "connect-src 'self'; " +
            "frame-ancestors 'none'; " +
            "base-uri 'self'; " +
            "form-action 'self'";

        return _next(context);
    }
}

public static class SecurityHeadersMiddlewareExtensions
{
    public static IApplicationBuilder UseSecurityHeaders(this IApplicationBuilder app)
        => app.UseMiddleware<SecurityHeadersMiddleware>();
}
