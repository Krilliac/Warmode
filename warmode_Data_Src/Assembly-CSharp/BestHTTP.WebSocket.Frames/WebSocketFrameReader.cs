using BestHTTP.Extensions;
using System;
using System.Collections.Generic;
using System.IO;

namespace BestHTTP.WebSocket.Frames
{
	public sealed class WebSocketFrameReader
	{
		public bool IsFinal
		{
			get;
			private set;
		}

		public WebSocketFrameTypes Type
		{
			get;
			private set;
		}

		public bool HasMask
		{
			get;
			private set;
		}

		public ulong Length
		{
			get;
			private set;
		}

		public byte[] Mask
		{
			get;
			private set;
		}

		public byte[] Data
		{
			get;
			private set;
		}

		internal void Read(Stream stream)
		{
			byte b = (byte)stream.ReadByte();
			this.IsFinal = ((b & 128) != 0);
			this.Type = (WebSocketFrameTypes)(b & 15);
			b = (byte)stream.ReadByte();
			this.HasMask = ((b & 128) != 0);
			this.Length = (ulong)((long)(b & 127));
			if (this.Length == 126uL)
			{
				byte[] array = new byte[2];
				stream.ReadBuffer(array);
				if (BitConverter.IsLittleEndian)
				{
					Array.Reverse(array, 0, array.Length);
				}
				this.Length = (ulong)BitConverter.ToUInt16(array, 0);
			}
			else if (this.Length == 127uL)
			{
				byte[] array2 = new byte[8];
				stream.ReadBuffer(array2);
				if (BitConverter.IsLittleEndian)
				{
					Array.Reverse(array2, 0, array2.Length);
				}
				this.Length = BitConverter.ToUInt64(array2, 0);
			}
			if (this.HasMask)
			{
				this.Mask = new byte[4];
				stream.Read(this.Mask, 0, 4);
			}
			this.Data = new byte[this.Length];
			for (ulong num = 0uL; num < this.Length; num += 1uL)
			{
				this.Data[(int)(checked((IntPtr)num))] = (byte)stream.ReadByte();
				checked
				{
					if (this.HasMask)
					{
						this.Data[(int)((IntPtr)num)] = (this.Data[(int)((IntPtr)num)] ^ this.Mask[(int)((IntPtr)(num % 4uL))]);
					}
				}
			}
		}

		internal void Assemble(List<WebSocketFrameReader> fragments)
		{
			fragments.Add(this);
			ulong num = 0uL;
			for (int i = 0; i < fragments.Count; i++)
			{
				num += fragments[i].Length;
			}
			byte[] array = new byte[num];
			ulong num2 = 0uL;
			for (int j = 0; j < fragments.Count; j++)
			{
				Array.Copy(fragments[j].Data, 0, array, (int)num2, (int)fragments[j].Length);
				num2 += fragments[j].Length;
			}
			this.Type = fragments[0].Type;
			this.Length = num;
			this.Data = array;
		}
	}
}
