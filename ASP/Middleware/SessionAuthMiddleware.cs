using ASP.DATA;
using ASP.DATA.Entity;
using System.Security.Claims;

namespace ASP.Middleware
{
    public class SessionAuthMiddleware
    {
        private readonly RequestDelegate _next;

        public SessionAuthMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context, ILogger<SessionAuthMiddleware> logger, DataContext data_context)
        {
            String? user_id = context.Session.GetString("authUserId");

            if (user_id is not null)
            {
                try
                {
                    User? user = data_context.Users.Find(Guid.Parse(user_id));

                    if (user is not null)
                    {
                        context.Items.Add("authUser", user);

                        Claim[] claims = new Claim[]
                        {
                            new Claim(ClaimTypes.Sid, user_id),
                            new Claim(ClaimTypes.Name, user.RealName),
                            new Claim(ClaimTypes.NameIdentifier, user.Login),
                            new Claim(ClaimTypes.UserData, user.Avatar ?? String.Empty)
                        };

                        var principal = new ClaimsPrincipal(new ClaimsIdentity(claims, nameof(SessionAuthMiddleware)));

                        context.User = principal;
                    }
                }
                catch (Exception ex) { logger.LogWarning(ex, "SessionAuthMiddleware"); }
            }

            logger.LogInformation("SessionsAuthMiddleware works");

            await _next(context);
        }
    }

    public static class SessionAuthMiddlewareExeption
    {
        public static IApplicationBuilder UseSesssionAuth(this IApplicationBuilder app) { return app.UseMiddleware<SessionAuthMiddleware>(); }
    }
}
