using System;

namespace BestHTTP.WebSocket.Frames
{
	public interface IWebSocketFrameWriter
	{
		WebSocketFrameTypes Type
		{
			get;
		}

		byte[] Get();
	}
}
