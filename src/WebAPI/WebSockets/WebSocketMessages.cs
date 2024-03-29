﻿namespace WebAPI.WebSockets;

public class WebSocketMessages
{
    public static string UnexpectedMessageType { get; } = "You are not supposed to send this message type";

    public static string ConnectionClosingRequestSucceeded { get; } = "Connection closed as requested";
}