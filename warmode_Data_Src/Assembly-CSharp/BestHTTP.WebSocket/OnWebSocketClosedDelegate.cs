using System;

namespace BestHTTP.WebSocket
{
	public delegate void OnWebSocketClosedDelegate(WebSocket webSocket, ushort code, string message);
}
