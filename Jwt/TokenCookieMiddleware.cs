namespace Mailroom.Jwt;

public class TokenCookieMiddleware
{
    private readonly RequestDelegate _next;

    public TokenCookieMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task Invoke(HttpContext context)
    {
        if (!context.Request.Headers.ContainsKey("Authorization"))
        {
            context.Request.Cookies.TryGetValue("AuthToken", out var token);
            if (!string.IsNullOrEmpty(token))
            {
                context.Request.Headers.Append("Authorization", $"Bearer {token}");
            }
        }

        await _next(context);
    }
}