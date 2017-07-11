using System;
using System.IO;

namespace Org.BouncyCastle.Utilities.IO
{
	public abstract class BaseInputStream : Stream
	{
		private bool closed;

		public sealed override bool CanRead
		{
			get
			{
				return !this.closed;
			}
		}

		public sealed override bool CanSeek
		{
			get
			{
				return false;
			}
		}

		public sealed override bool CanWrite
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

		public override void Close()
		{
			this.closed = true;
		}

		public sealed override void Flush()
		{
		}

		public override int Read(byte[] buffer, int offset, int count)
		{
			int i = offset;
			try
			{
				int num = offset + count;
				while (i < num)
				{
					int num2 = this.ReadByte();
					if (num2 == -1)
					{
						break;
					}
					buffer[i++] = (byte)num2;
				}
			}
			catch (IOException)
			{
				if (i == offset)
				{
					throw;
				}
			}
			return i - offset;
		}

		public sealed override long Seek(long offset, SeekOrigin origin)
		{
			throw new NotSupportedException();
		}

		public sealed override void SetLength(long value)
		{
			throw new NotSupportedException();
		}

		public sealed override void Write(byte[] buffer, int offset, int count)
		{
			throw new NotSupportedException();
		}
	}
}
