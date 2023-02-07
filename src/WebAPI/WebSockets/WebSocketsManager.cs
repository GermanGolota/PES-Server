using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Application.Contracts;
using Core.Extensions;
using Newtonsoft.Json;

namespace WebAPI.WebSockets;

public class WebSocketsManager : IWebSocketsManager
{
    private ConcurrentDictionary<Guid, ChatWebSocket> WebSockets { get; } = new();

    public Guid AddSocket(WebSocket socket, Guid chatId)
    {
        Guid guid = Guid.NewGuid();
        ChatWebSocket chatSocket = new()
        {
            Client = socket,
            ChatId = chatId
        };
        WebSockets.TryAddWithRetries(guid, chatSocket, 10);
        return guid;
    }

    public async Task RemoveSocket(Guid socketId)
    {
        if (WebSockets.ContainsKey(socketId))
        {
            ChatWebSocket webSocket = WebSockets[socketId];
            if (WebSocketIsOpen(webSocket))
                await webSocket.Client.CloseAsync(WebSocketCloseStatus.NormalClosure,
                    WebSocketMessages.ConnectionClosingRequestSucceeded,
                    CancellationToken.None);

            WebSockets.TryRemoveWithRetries(socketId, 10);
        }
    }

    public async Task RemoveSocketForPolicyVialtion(Guid socketId, string closureReason)
    {
        if (WebSockets.ContainsKey(socketId))
        {
            ChatWebSocket webSocket = WebSockets[socketId];
            if (WebSocketIsOpen(webSocket))
                await webSocket.Client.CloseAsync(WebSocketCloseStatus.PolicyViolation,
                    closureReason, CancellationToken.None);
            WebSockets.TryRemoveWithRetries(socketId, 10);
        }
    }

    public async Task SendTextMessageToSocketsConnectedToChat(Guid chatId, string textMessage)
    {
        List<ChatWebSocket> webSockets = WebSockets.Values.Where(x => x.ChatId.Equals(chatId)).ToList();

        ArraySegment<byte> message = CreateMessageFrom(textMessage);
        List<Task> sendTasks = new();
        foreach (ChatWebSocket webSocket in webSockets) sendTasks.Add(SendMessage(message, webSocket.Client));

        await Task.WhenAll(sendTasks);
    }

    public async Task SendJsonToSocketsConnectedToChat<T>(Guid chatId, T jsonObject)
    {
        string message = JsonConvert.SerializeObject(jsonObject);
        await SendTextMessageToSocketsConnectedToChat(chatId, message);
    }

    private static bool WebSocketIsOpen(ChatWebSocket webSocket)
    {
        return webSocket.Client.State.Equals(WebSocketState.CloseReceived)
               || webSocket.Client.State.Equals(WebSocketState.Open);
    }

    private ArraySegment<byte> CreateMessageFrom(string textMessage)
    {
        byte[] buffer = Encoding.ASCII.GetBytes(textMessage);
        ArraySegment<byte> message = new(buffer);
        return message;
    }

    public Task SendMessage(ArraySegment<byte> message, WebSocket socket)
    {
        return socket.SendAsync(message, WebSocketMessageType.Text,
            true, CancellationToken.None);
    }
}