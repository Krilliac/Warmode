using System;

namespace BestHTTP.WebSocket.Frames
{
	public sealed class WebSocketContinuationFrame : WebSocketBinaryFrame
	{
		public override WebSocketFrameTypes Type
		{
			get
			{
				return WebSocketFrameTypes.Continuation;
			}
		}

		public WebSocketContinuationFrame(byte[] data, bool isFinal) : base(data, 0uL, (ulong)((long)data.Length), isFinal)
		{
		}

		public WebSocketContinuationFrame(byte[] data, ulong pos, ulong length, bool isFinal) : base(data, pos, length, isFinal)
		{
		}
	}
}
