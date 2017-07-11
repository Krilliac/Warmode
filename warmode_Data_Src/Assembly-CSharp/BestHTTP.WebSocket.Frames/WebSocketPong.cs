using System;

namespace BestHTTP.WebSocket.Frames
{
	public sealed class WebSocketPong : WebSocketBinaryFrame
	{
		public override WebSocketFrameTypes Type
		{
			get
			{
				return WebSocketFrameTypes.Pong;
			}
		}

		public WebSocketPong(WebSocketFrameReader ping) : base(ping.Data)
		{
		}
	}
}
