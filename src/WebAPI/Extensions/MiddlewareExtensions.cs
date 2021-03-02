using Microsoft.AspNetCore.Builder;
using WebApi.Middleware;

namespace WebAPI.Extensions
{
    public static class MiddlewareExtensions
    {
        public static IApplicationBuilder UseWebSocketsServer(this IApplicationBuilder app)
        {
            app.UseWebSockets();
            app.UseMiddleware<WebSocketsMiddleware>();
            return app;
        }
    }
}
