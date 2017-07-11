using System;
using System.IO;

namespace Org.BouncyCastle.Utilities.Zlib
{
	public class ZInputStream : Stream
	{
		private const int BufferSize = 512;

		protected ZStream z;

		protected int flushLevel;

		protected byte[] buf;

		protected byte[] buf1;

		protected bool compress;

		protected Stream input;

		protected bool closed;

		private bool nomoreinput;

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

		public virtual int FlushMode
		{
			get
			{
				return this.flushLevel;
			}
			set
			{
				this.flushLevel = value;
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

		public virtual long TotalIn
		{
			get
			{
				return this.z.total_in;
			}
		}

		public virtual long TotalOut
		{
			get
			{
				return this.z.total_out;
			}
		}

		public ZInputStream(Stream input) : this(input, false)
		{
		}

		public ZInputStream(Stream input, bool nowrap)
		{
			this.z = new ZStream();
			this.buf = new byte[512];
			this.buf1 = new byte[1];
			base..ctor();
			this.input = input;
			this.z.inflateInit(nowrap);
			this.compress = false;
			this.z.next_in = this.buf;
			this.z.next_in_index = 0;
			this.z.avail_in = 0;
		}

		public ZInputStream(Stream input, int level)
		{
			this.z = new ZStream();
			this.buf = new byte[512];
			this.buf1 = new byte[1];
			base..ctor();
			this.input = input;
			this.z.deflateInit(level);
			this.compress = true;
			this.z.next_in = this.buf;
			this.z.next_in_index = 0;
			this.z.avail_in = 0;
		}

		public override void Close()
		{
			if (!this.closed)
			{
				this.closed = true;
				this.input.Close();
			}
		}

		public sealed override void Flush()
		{
		}

		public override int Read(byte[] b, int off, int len)
		{
			if (len == 0)
			{
				return 0;
			}
			this.z.next_out = b;
			this.z.next_out_index = off;
			this.z.avail_out = len;
			while (true)
			{
				if (this.z.avail_in == 0 && !this.nomoreinput)
				{
					this.z.next_in_index = 0;
					this.z.avail_in = this.input.Read(this.buf, 0, this.buf.Length);
					if (this.z.avail_in <= 0)
					{
						this.z.avail_in = 0;
						this.nomoreinput = true;
					}
				}
				int num = this.compress ? this.z.deflate(this.flushLevel) : this.z.inflate(this.flushLevel);
				if (this.nomoreinput && num == -5)
				{
					break;
				}
				if (num != 0 && num != 1)
				{
					goto Block_9;
				}
				if ((this.nomoreinput || num == 1) && this.z.avail_out == len)
				{
					return 0;
				}
				if (this.z.avail_out != len || num != 0)
				{
					goto IL_132;
				}
			}
			return 0;
			Block_9:
			throw new IOException((this.compress ? "de" : "in") + "flating: " + this.z.msg);
			IL_132:
			return len - this.z.avail_out;
		}

		public override int ReadByte()
		{
			if (this.Read(this.buf1, 0, 1) <= 0)
			{
				return -1;
			}
			return (int)this.buf1[0];
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
