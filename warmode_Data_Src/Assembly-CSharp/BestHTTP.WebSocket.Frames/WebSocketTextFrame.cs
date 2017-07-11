using System;
using System.Text;

namespace BestHTTP.WebSocket.Frames
{
	public sealed class WebSocketTextFrame : WebSocketBinaryFrame
	{
		public override WebSocketFrameTypes Type
		{
			get
			{
				return WebSocketFrameTypes.Text;
			}
		}

		public WebSocketTextFrame(string text) : base(Encoding.UTF8.GetBytes(text))
		{
		}
	}
}
