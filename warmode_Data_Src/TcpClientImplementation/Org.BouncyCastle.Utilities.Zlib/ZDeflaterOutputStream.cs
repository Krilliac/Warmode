using System;
using System.IO;

namespace Org.BouncyCastle.Utilities.Zlib
{
	[Obsolete("Use 'ZOutputStream' instead")]
	public class ZDeflaterOutputStream : Stream
	{
		private const int BUFSIZE = 4192;

		protected ZStream z = new ZStream();

		protected int flushLevel;

		protected byte[] buf = new byte[4192];

		private byte[] buf1 = new byte[1];

		protected Stream outp;

		public override bool CanRead
		{
			get
			{
				return false;
			}
		}

		public override bool CanSeek
		{
			get
			{
				return false;
			}
		}

		public override bool CanWrite
		{
			get
			{
				return true;
			}
		}

		public override long Length
		{
			get
			{
				return 0L;
			}
		}

		public override long Position
		{
			get
			{
				return 0L;
			}
			set
			{
			}
		}

		public ZDeflaterOutputStream(Stream outp) : this(outp, 6, false)
		{
		}

		public ZDeflaterOutputStream(Stream outp, int level) : this(outp, level, false)
		{
		}

		public ZDeflaterOutputStream(Stream outp, int level, bool nowrap)
		{
			this.outp = outp;
			this.z.deflateInit(level, nowrap);
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
				this.z.avail_out = 4192;
				int num = this.z.deflate(this.flushLevel);
				if (num != 0)
				{
					break;
				}
				if (this.z.avail_out < 4192)
				{
					this.outp.Write(this.buf, 0, 4192 - this.z.avail_out);
				}
				if (this.z.avail_in <= 0 && this.z.avail_out != 0)
				{
					return;
				}
			}
			throw new IOException("deflating: " + this.z.msg);
		}

		public override long Seek(long offset, SeekOrigin origin)
		{
			return 0L;
		}

		public override void SetLength(long value)
		{
		}

		public override int Read(byte[] buffer, int offset, int count)
		{
			return 0;
		}

		public override void Flush()
		{
			this.outp.Flush();
		}

		public override void WriteByte(byte b)
		{
			this.buf1[0] = b;
			this.Write(this.buf1, 0, 1);
		}

		public void Finish()
		{
			while (true)
			{
				this.z.next_out = this.buf;
				this.z.next_out_index = 0;
				this.z.avail_out = 4192;
				int num = this.z.deflate(4);
				if (num != 1 && num != 0)
				{
					break;
				}
				if (4192 - this.z.avail_out > 0)
				{
					this.outp.Write(this.buf, 0, 4192 - this.z.avail_out);
				}
				if (this.z.avail_in <= 0 && this.z.avail_out != 0)
				{
					goto Block_4;
				}
			}
			throw new IOException("deflating: " + this.z.msg);
			Block_4:
			this.Flush();
		}

		public void End()
		{
			if (this.z == null)
			{
				return;
			}
			this.z.deflateEnd();
			this.z.free();
			this.z = null;
		}

		public override void Close()
		{
			try
			{
				this.Finish();
			}
			catch (IOException)
			{
			}
			finally
			{
				this.End();
				this.outp.Close();
				this.outp = null;
			}
		}
	}
}
