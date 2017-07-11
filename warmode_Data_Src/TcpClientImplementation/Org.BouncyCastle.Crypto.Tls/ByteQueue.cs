using System;

namespace Org.BouncyCastle.Crypto.Tls
{
	public class ByteQueue
	{
		private const int DefaultCapacity = 1024;

		private byte[] databuf;

		private int skipped;

		private int available;

		public int Available
		{
			get
			{
				return this.available;
			}
		}

		public static int NextTwoPow(int i)
		{
			i |= i >> 1;
			i |= i >> 2;
			i |= i >> 4;
			i |= i >> 8;
			i |= i >> 16;
			return i + 1;
		}

		public ByteQueue() : this(1024)
		{
		}

		public ByteQueue(int capacity)
		{
			this.databuf = new byte[capacity];
		}

		public void Read(byte[] buf, int offset, int len, int skip)
		{
			if (buf.Length - offset < len)
			{
				throw new ArgumentException(string.Concat(new object[]
				{
					"Buffer size of ",
					buf.Length,
					" is too small for a read of ",
					len,
					" bytes"
				}));
			}
			if (this.available - skip < len)
			{
				throw new InvalidOperationException("Not enough data to read");
			}
			Array.Copy(this.databuf, this.skipped + skip, buf, offset, len);
		}

		public void AddData(byte[] data, int offset, int len)
		{
			if (this.skipped + this.available + len > this.databuf.Length)
			{
				int num = ByteQueue.NextTwoPow(this.available + len);
				if (num > this.databuf.Length)
				{
					byte[] destinationArray = new byte[num];
					Array.Copy(this.databuf, this.skipped, destinationArray, 0, this.available);
					this.databuf = destinationArray;
				}
				else
				{
					Array.Copy(this.databuf, this.skipped, this.databuf, 0, this.available);
				}
				this.skipped = 0;
			}
			Array.Copy(data, offset, this.databuf, this.skipped + this.available, len);
			this.available += len;
		}

		public void RemoveData(int i)
		{
			if (i > this.available)
			{
				throw new InvalidOperationException(string.Concat(new object[]
				{
					"Cannot remove ",
					i,
					" bytes, only got ",
					this.available
				}));
			}
			this.available -= i;
			this.skipped += i;
		}

		public void RemoveData(byte[] buf, int off, int len, int skip)
		{
			this.Read(buf, off, len, skip);
			this.RemoveData(skip + len);
		}

		public byte[] RemoveData(int len, int skip)
		{
			byte[] array = new byte[len];
			this.RemoveData(array, 0, len, skip);
			return array;
		}
	}
}
