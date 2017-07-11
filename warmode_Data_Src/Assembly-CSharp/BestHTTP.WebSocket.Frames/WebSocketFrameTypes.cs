using System;

namespace BestHTTP.WebSocket.Frames
{
	public enum WebSocketFrameTypes : byte
	{
		Continuation,
		Text,
		Binary,
		ConnectionClose = 8,
		Ping,
		Pong
	}
}
