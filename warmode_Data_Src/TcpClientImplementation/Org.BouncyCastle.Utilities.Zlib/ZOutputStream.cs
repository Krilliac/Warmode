using System;
using System.IO;

namespace Org.BouncyCastle.Utilities.Zlib
{
	public class ZOutputStream : Stream
	{
		private const int BufferSize = 512;

		protected ZStream z;

		protected int flushLevel;

		protected byte[] buf;

		protected byte[] buf1;

		protected bool compress;

		protected Stream output;

		protected bool closed;

		public sealed override bool CanRead
		{
			get
			{
				return false;
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
				return !this.closed;
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

		public ZOutputStream(Stream output) : this(output, null)
		{
		}

		public ZOutputStream(Stream output, ZStream z)
		{
			this.buf = new byte[512];
			this.buf1 = new byte[1];
			base..ctor();
			if (z == null)
			{
				z = new ZStream();
				z.inflateInit();
			}
			this.output = output;
			this.z = z;
			this.compress = false;
		}

		public ZOutputStream(Stream output, int level) : this(output, level, false)
		{
		}

		public ZOutputStream(Stream output, int level, bool nowrap)
		{
			this.buf = new byte[512];
			this.buf1 = new byte[1];
			base..ctor();
			this.output = output;
			this.z = new ZStream();
			this.z.deflateInit(level, nowrap);
			this.compress = true;
		}

		public override void Close()
		{
			if (this.closed)
			{
				return;
			}
			try
			{
				this.Finish();
			}
			catch (IOException)
			{
			}
			finally
			{
				this.closed = true;
				this.End();
				this.output.Close();
				this.output = null;
			}
		}

		public virtual void End()
		{
			if (this.z == null)
			{
				return;
			}
			if (this.compress)
			{
				this.z.deflateEnd();
			}
			else
			{
				this.z.inflateEnd();
			}
			this.z.free();
			this.z = null;
		}

		public virtual void Finish()
		{
			while (true)
			{
				this.z.next_out = this.buf;
				this.z.next_out_index = 0;
				this.z.avail_out = this.buf.Length;
				int num = this.compress ? this.z.deflate(4) : this.z.inflate(4);
				if (num != 1 && num != 0)
				{
					break;
				}
				int num2 = this.buf.Length - this.z.avail_out;
				if (num2 > 0)
				{
					this.output.Write(this.buf, 0, num2);
				}
				if (this.z.avail_in <= 0 && this.z.avail_out != 0)
				{
					goto Block_6;
				}
			}
			throw new IOException((this.compress ? "de" : "in") + "flating: " + this.z.msg);
			Block_6:
			this.Flush();
		}

		public override void Flush()
		{
			this.output.Flush();
		}

		public sealed override int Read(byte[] buffer, int offset, int count)
		{
			throw new NotSupportedException();
		}

		public sealed override long Seek(long offset, SeekOrigin origin)
		{
			throw new NotSupportedException();
		}

		public sealed override void SetLength(long value)
		{
			throw new NotSupportedException();
		}

		public override void Write(byte[] b, int off, int len)
		{
			if (len == 0)
			{
				return;
			}
			this.z.next_in = b;
			this.z.next_in_index = off;
			this.z.avail_in = len;
			while (true)
			{
				this.z.next_out = this.buf;
				this.z.next_out_index = 0;
				this.z.avail_out = this.buf.Length;
				int num = this.compress ? this.z.deflate(this.flushLevel) : this.z.inflate(this.flushLevel);
				if (num != 0)
				{
					break;
				}
				this.output.Write(this.buf, 0, this.buf.Length - this.z.avail_out);
				if (this.z.avail_in <= 0 && this.z.avail_out != 0)
				{
					return;
				}
			}
			throw new IOException((this.compress ? "de" : "in") + "flating: " + this.z.msg);
		}

		public override void WriteByte(byte b)
		{
			this.buf1[0] = b;
			this.Write(this.buf1, 0, 1);
		}
	}
}
