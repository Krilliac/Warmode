using BestHTTP.WebSocket.Frames;
using System;

namespace BestHTTP.WebSocket
{
	public delegate void OnWebSocketIncompleteFrameDelegate(WebSocket webSocket, WebSocketFrameReader frame);
}
