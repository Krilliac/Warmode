using System;
using System.IO;

namespace Org.BouncyCastle.Crypto.IO
{
	public class CipherStream : Stream
	{
		internal Stream stream;

		internal IBufferedCipher inCipher;

		internal IBufferedCipher outCipher;

		private byte[] mInBuf;

		private int mInPos;

		private bool inStreamEnded;

		public IBufferedCipher ReadCipher
		{
			get
			{
				return this.inCipher;
			}
		}

		public IBufferedCipher WriteCipher
		{
			get
			{
				return this.outCipher;
			}
		}

		public override bool CanRead
		{
			get
			{
				return this.stream.CanRead && this.inCipher != null;
			}
		}

		public override bool CanWrite
		{
			get
			{
				return this.stream.CanWrite && this.outCipher != null;
			}
		}

		public override bool CanSeek
		{
			get
			{
				return false;
			}
		}

		public sealed override long Length
		{
			get
			{
				throw new NotSupportedException();
			}
		}

		public sealed override long Position
		{
			get
			{
				throw new NotSupportedException();
			}
			set
			{
				throw new NotSupportedException();
			}
		}

		public CipherStream(Stream stream, IBufferedCipher readCipher, IBufferedCipher writeCipher)
		{
			this.stream = stream;
			if (readCipher != null)
			{
				this.inCipher = readCipher;
				this.mInBuf = null;
			}
			if (writeCipher != null)
			{
				this.outCipher = writeCipher;
			}
		}

		public override int ReadByte()
		{
			if (this.inCipher == null)
			{
				return this.stream.ReadByte();
			}
			if ((this.mInBuf == null || this.mInPos >= this.mInBuf.Length) && !this.FillInBuf())
			{
				return -1;
			}
			return (int)this.mInBuf[this.mInPos++];
		}

		public override int Read(byte[] buffer, int offset, int count)
		{
			if (this.inCipher == null)
			{
				return this.stream.Read(buffer, offset, count);
			}
			int num = 0;
			while (num < count && ((this.mInBuf != null && this.mInPos < this.mInBuf.Length) || this.FillInBuf()))
			{
				int num2 = Math.Min(count - num, this.mInBuf.Length - this.mInPos);
				Array.Copy(this.mInBuf, this.mInPos, buffer, offset + num, num2);
				this.mInPos += num2;
				num += num2;
			}
			return num;
		}

		private bool FillInBuf()
		{
			if (this.inStreamEnded)
			{
				return false;
			}
			this.mInPos = 0;
			do
			{
				this.mInBuf = this.ReadAndProcessBlock();
			}
			while (!this.inStreamEnded && this.mInBuf == null);
			return this.mInBuf != null;
		}

		private byte[] ReadAndProcessBlock()
		{
			int blockSize = this.inCipher.GetBlockSize();
			int num = (blockSize == 0) ? 256 : blockSize;
			byte[] array = new byte[num];
			int num2 = 0;
			while (true)
			{
				int num3 = this.stream.Read(array, num2, array.Length - num2);
				if (num3 < 1)
				{
					break;
				}
				num2 += num3;
				if (num2 >= array.Length)
				{
					goto IL_4E;
				}
			}
			this.inStreamEnded = true;
			IL_4E:
			byte[] array2 = this.inStreamEnded ? this.inCipher.DoFinal(array, 0, num2) : this.inCipher.ProcessBytes(array);
			if (array2 != null && array2.Length == 0)
			{
				array2 = null;
			}
			return array2;
		}

		public override void Write(byte[] buffer, int offset, int count)
		{
			if (this.outCipher == null)
			{
				this.stream.Write(buffer, offset, count);
				return;
			}
			byte[] array = this.outCipher.ProcessBytes(buffer, offset, count);
			if (array != null)
			{
				this.stream.Write(array, 0, array.Length);
			}
		}

		public override void WriteByte(byte b)
		{
			if (this.outCipher == null)
			{
				this.stream.WriteByte(b);
				return;
			}
			byte[] array = this.outCipher.ProcessByte(b);
			if (array != null)
			{
				this.stream.Write(array, 0, array.Length);
			}
		}

		public override void Close()
		{
			if (this.outCipher != null)
			{
				byte[] array = this.outCipher.DoFinal();
				this.stream.Write(array, 0, array.Length);
				this.stream.Flush();
			}
			this.stream.Close();
		}

		public override void Flush()
		{
			this.stream.Flush();
		}

		public sealed override long Seek(long offset, SeekOrigin origin)
		{
			throw new NotSupportedException();
		}

		public sealed override void SetLength(long length)
		{
			throw new NotSupportedException();
		}
	}
}
