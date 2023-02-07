using System;
using System.IO;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;
using Application.Contracts;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using WebAPI.WebSockets;

namespace WebApi.Middleware;

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
            Guid chatId = GetChatId(context);
            Guid socketId = _webSocketsManager.AddSocket(webSocket, chatId);

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

            await _webSocketsManager.RemoveSocket(socketId);
        }
        else
        {
            await _next(context);
        }
    }

    private Guid GetChatId(HttpContext context)
    {
        Stream payload = context.Request.Body;
        string content;
        using (StreamReader sr = new(payload))
        {
            content = sr.ReadToEnd();
        }

        WebsocketConnectionMessage message
            = JsonConvert.DeserializeObject<WebsocketConnectionMessage>(content);
        return message.ChatId;
    }

    private async Task Receive(WebSocket socket, Action<WebSocketReceiveResult, byte[]> handleMessage)
    {
        byte[] buffer = new byte[1024 * 4];

        while (socket.State == WebSocketState.Open)
        {
            WebSocketReceiveResult result = await socket.ReceiveAsync(new ArraySegment<byte>(buffer),
                CancellationToken.None);
            handleMessage(result, buffer);
        }
    }
}