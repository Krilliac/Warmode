using Org.BouncyCastle.Utilities.IO;
using System;
using System.IO;

namespace Org.BouncyCastle.Bcpg
{
	public class BcpgOutputStream : BaseOutputStream
	{
		private const int BufferSizePower = 16;

		private Stream outStr;

		private byte[] partialBuffer;

		private int partialBufferLength;

		private int partialPower;

		private int partialOffset;

		internal static BcpgOutputStream Wrap(Stream outStr)
		{
			if (outStr is BcpgOutputStream)
			{
				return (BcpgOutputStream)outStr;
			}
			return new BcpgOutputStream(outStr);
		}

		public BcpgOutputStream(Stream outStr)
		{
			if (outStr == null)
			{
				throw new ArgumentNullException("outStr");
			}
			this.outStr = outStr;
		}

		public BcpgOutputStream(Stream outStr, PacketTag tag)
		{
			if (outStr == null)
			{
				throw new ArgumentNullException("outStr");
			}
			this.outStr = outStr;
			this.WriteHeader(tag, true, true, 0L);
		}

		public BcpgOutputStream(Stream outStr, PacketTag tag, long length, bool oldFormat)
		{
			if (outStr == null)
			{
				throw new ArgumentNullException("outStr");
			}
			this.outStr = outStr;
			if (length > (long)((ulong)-1))
			{
				this.WriteHeader(tag, false, true, 0L);
				this.partialBufferLength = 65536;
				this.partialBuffer = new byte[this.partialBufferLength];
				this.partialPower = 16;
				this.partialOffset = 0;
				return;
			}
			this.WriteHeader(tag, oldFormat, false, length);
		}

		public BcpgOutputStream(Stream outStr, PacketTag tag, long length)
		{
			if (outStr == null)
			{
				throw new ArgumentNullException("outStr");
			}
			this.outStr = outStr;
			this.WriteHeader(tag, false, false, length);
		}

		public BcpgOutputStream(Stream outStr, PacketTag tag, byte[] buffer)
		{
			if (outStr == null)
			{
				throw new ArgumentNullException("outStr");
			}
			this.outStr = outStr;
			this.WriteHeader(tag, false, true, 0L);
			this.partialBuffer = buffer;
			uint num = (uint)this.partialBuffer.Length;
			this.partialPower = 0;
			while (num != 1u)
			{
				num >>= 1;
				this.partialPower++;
			}
			if (this.partialPower > 30)
			{
				throw new IOException("Buffer cannot be greater than 2^30 in length.");
			}
			this.partialBufferLength = 1 << this.partialPower;
			this.partialOffset = 0;
		}

		private void WriteNewPacketLength(long bodyLen)
		{
			if (bodyLen < 192L)
			{
				this.outStr.WriteByte((byte)bodyLen);
				return;
			}
			if (bodyLen <= 8383L)
			{
				bodyLen -= 192L;
				this.outStr.WriteByte((byte)((bodyLen >> 8 & 255L) + 192L));
				this.outStr.WriteByte((byte)bodyLen);
				return;
			}
			this.outStr.WriteByte(255);
			this.outStr.WriteByte((byte)(bodyLen >> 24));
			this.outStr.WriteByte((byte)(bodyLen >> 16));
			this.outStr.WriteByte((byte)(bodyLen >> 8));
			this.outStr.WriteByte((byte)bodyLen);
		}

		private void WriteHeader(PacketTag tag, bool oldPackets, bool partial, long bodyLen)
		{
			int num = 128;
			if (this.partialBuffer != null)
			{
				this.PartialFlush(true);
				this.partialBuffer = null;
			}
			if (oldPackets)
			{
				num |= (int)((int)tag << 2);
				if (partial)
				{
					this.WriteByte((byte)(num | 3));
					return;
				}
				if (bodyLen <= 255L)
				{
					this.WriteByte((byte)num);
					this.WriteByte((byte)bodyLen);
					return;
				}
				if (bodyLen <= 65535L)
				{
					this.WriteByte((byte)(num | 1));
					this.WriteByte((byte)(bodyLen >> 8));
					this.WriteByte((byte)bodyLen);
					return;
				}
				this.WriteByte((byte)(num | 2));
				this.WriteByte((byte)(bodyLen >> 24));
				this.WriteByte((byte)(bodyLen >> 16));
				this.WriteByte((byte)(bodyLen >> 8));
				this.WriteByte((byte)bodyLen);
				return;
			}
			else
			{
				num |= (int)((PacketTag)64 | tag);
				this.WriteByte((byte)num);
				if (partial)
				{
					this.partialOffset = 0;
					return;
				}
				this.WriteNewPacketLength(bodyLen);
				return;
			}
		}

		private void PartialFlush(bool isLast)
		{
			if (isLast)
			{
				this.WriteNewPacketLength((long)this.partialOffset);
				this.outStr.Write(this.partialBuffer, 0, this.partialOffset);
			}
			else
			{
				this.outStr.WriteByte((byte)(224 | this.partialPower));
				this.outStr.Write(this.partialBuffer, 0, this.partialBufferLength);
			}
			this.partialOffset = 0;
		}

		private void WritePartial(byte b)
		{
			if (this.partialOffset == this.partialBufferLength)
			{
				this.PartialFlush(false);
			}
			this.partialBuffer[this.partialOffset++] = b;
		}

		private void WritePartial(byte[] buffer, int off, int len)
		{
			if (this.partialOffset == this.partialBufferLength)
			{
				this.PartialFlush(false);
			}
			if (len <= this.partialBufferLength - this.partialOffset)
			{
				Array.Copy(buffer, off, this.partialBuffer, this.partialOffset, len);
				this.partialOffset += len;
				return;
			}
			int num = this.partialBufferLength - this.partialOffset;
			Array.Copy(buffer, off, this.partialBuffer, this.partialOffset, num);
			off += num;
			len -= num;
			this.PartialFlush(false);
			while (len > this.partialBufferLength)
			{
				Array.Copy(buffer, off, this.partialBuffer, 0, this.partialBufferLength);
				off += this.partialBufferLength;
				len -= this.partialBufferLength;
				this.PartialFlush(false);
			}
			Array.Copy(buffer, off, this.partialBuffer, 0, len);
			this.partialOffset += len;
		}

		public override void WriteByte(byte value)
		{
			if (this.partialBuffer != null)
			{
				this.WritePartial(value);
				return;
			}
			this.outStr.WriteByte(value);
		}

		public override void Write(byte[] buffer, int offset, int count)
		{
			if (this.partialBuffer != null)
			{
				this.WritePartial(buffer, offset, count);
				return;
			}
			this.outStr.Write(buffer, offset, count);
		}

		internal virtual void WriteShort(short n)
		{
			this.Write(new byte[]
			{
				(byte)(n >> 8),
				(byte)n
			});
		}

		internal virtual void WriteInt(int n)
		{
			this.Write(new byte[]
			{
				(byte)(n >> 24),
				(byte)(n >> 16),
				(byte)(n >> 8),
				(byte)n
			});
		}

		internal virtual void WriteLong(long n)
		{
			this.Write(new byte[]
			{
				(byte)(n >> 56),
				(byte)(n >> 48),
				(byte)(n >> 40),
				(byte)(n >> 32),
				(byte)(n >> 24),
				(byte)(n >> 16),
				(byte)(n >> 8),
				(byte)n
			});
		}

		public void WritePacket(ContainedPacket p)
		{
			p.Encode(this);
		}

		internal void WritePacket(PacketTag tag, byte[] body, bool oldFormat)
		{
			this.WriteHeader(tag, oldFormat, false, (long)body.Length);
			this.Write(body);
		}

		public void WriteObject(BcpgObject bcpgObject)
		{
			bcpgObject.Encode(this);
		}

		public void WriteObjects(params BcpgObject[] v)
		{
			for (int i = 0; i < v.Length; i++)
			{
				BcpgObject bcpgObject = v[i];
				bcpgObject.Encode(this);
			}
		}

		public override void Flush()
		{
			this.outStr.Flush();
		}

		public void Finish()
		{
			if (this.partialBuffer != null)
			{
				this.PartialFlush(true);
				this.partialBuffer = null;
			}
		}

		public override void Close()
		{
			this.Finish();
			this.outStr.Flush();
			this.outStr.Close();
			base.Close();
		}
	}
}
