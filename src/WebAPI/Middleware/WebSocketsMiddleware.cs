using Microsoft.AspNetCore.Http;
using System;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;
using WebAPI.WebSockets;

namespace WebApi.Middleware
{
    public class WebSocketsMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IWebSocketsManager _webSocketsManager;

        public WebSocketsMiddleware(RequestDelegate next, IWebSocketsManager webSocketsManager)
        {
            _next = next;
            _webSocketsManager = webSocketsManager;
        }
        public async Task InvokeAsync(HttpContext context)
        {
            if (context.WebSockets.IsWebSocketRequest)
            {
                WebSocket webSocket = await context.WebSockets.AcceptWebSocketAsync();

                Guid socketId = _webSocketsManager.AddSocket(webSocket);

                await Receive(webSocket, async (result, buffer) =>
                {
                    switch (result.MessageType)
                    {
                        case WebSocketMessageType.Close:
                            await _webSocketsManager.RemoveSocket(socketId);
                            break;
                        case WebSocketMessageType.Text:
                        case WebSocketMessageType.Binary:
                            await _webSocketsManager.RemoveSocketForPolicyVialtion(socketId,
                                WebSocketMessages.UnexpectedMessageType);
                            break;
                    }
                });
            }
            else
            {
                await _next(context);
            }
        }
        private async Task Receive(WebSocket socket, Action<WebSocketReceiveResult, byte[]> handleMessage)
        {
            var buffer = new byte[1024 * 4];

            while (socket.State == WebSocketState.Open)
            {
                var result = await socket.ReceiveAsync(buffer: new ArraySegment<byte>(buffer),
                                                       cancellationToken: CancellationToken.None);
                handleMessage(result, buffer);
            }
        }
    }
}
