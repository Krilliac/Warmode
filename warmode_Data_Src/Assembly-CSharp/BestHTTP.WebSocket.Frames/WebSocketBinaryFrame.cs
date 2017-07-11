using System;
using System.IO;

namespace BestHTTP.WebSocket.Frames
{
	public class WebSocketBinaryFrame : IWebSocketFrameWriter
	{
		public virtual WebSocketFrameTypes Type
		{
			get
			{
				return WebSocketFrameTypes.Binary;
			}
		}

		public bool IsFinal
		{
			get;
			protected set;
		}

		protected byte[] Data
		{
			get;
			set;
		}

		protected ulong Pos
		{
			get;
			set;
		}

		protected ulong Length
		{
			get;
			set;
		}

		public WebSocketBinaryFrame(byte[] data) : this(data, 0uL, (ulong)((data == null) ? 0L : ((long)data.Length)), true)
		{
		}

		public WebSocketBinaryFrame(byte[] data, bool isFinal) : this(data, 0uL, (ulong)((data == null) ? 0L : ((long)data.Length)), isFinal)
		{
		}

		public WebSocketBinaryFrame(byte[] data, ulong pos, ulong length, bool isFinal)
		{
			this.Data = data;
			this.Pos = pos;
			this.Length = length;
			this.IsFinal = isFinal;
		}

		public virtual byte[] Get()
		{
			if (this.Data == null)
			{
				this.Data = new byte[0];
			}
			byte[] result;
			using (MemoryStream memoryStream = new MemoryStream((int)this.Length + 9))
			{
				byte b = (!this.IsFinal) ? 0 : 128;
				memoryStream.WriteByte(b | (byte)this.Type);
				if (this.Length < 126uL)
				{
					memoryStream.WriteByte(128 | (byte)this.Length);
				}
				else if (this.Length < 65535uL)
				{
					memoryStream.WriteByte(254);
					byte[] bytes = BitConverter.GetBytes((ushort)this.Length);
					if (BitConverter.IsLittleEndian)
					{
						Array.Reverse(bytes, 0, bytes.Length);
					}
					memoryStream.Write(bytes, 0, bytes.Length);
				}
				else
				{
					memoryStream.WriteByte(255);
					byte[] bytes2 = BitConverter.GetBytes(this.Length);
					if (BitConverter.IsLittleEndian)
					{
						Array.Reverse(bytes2, 0, bytes2.Length);
					}
					memoryStream.Write(bytes2, 0, bytes2.Length);
				}
				byte[] bytes3 = BitConverter.GetBytes(this.GetHashCode());
				memoryStream.Write(bytes3, 0, bytes3.Length);
				for (ulong num = this.Pos; num < this.Pos + this.Length; num += 1uL)
				{
					memoryStream.WriteByte(checked(this.Data[(int)((IntPtr)num)] ^ bytes3[(int)((IntPtr)(unchecked(num - this.Pos) % 4uL))]));
				}
				result = memoryStream.ToArray();
			}
			return result;
		}
	}
}
