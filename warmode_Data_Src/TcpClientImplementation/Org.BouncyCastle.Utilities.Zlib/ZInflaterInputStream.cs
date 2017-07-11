using System;
using System.IO;

namespace Org.BouncyCastle.Utilities.Zlib
{
	[Obsolete("Use 'ZInputStream' instead")]
	public class ZInflaterInputStream : Stream
	{
		private const int BUFSIZE = 4192;

		protected ZStream z = new ZStream();

		protected int flushLevel;

		protected byte[] buf = new byte[4192];

		private byte[] buf1 = new byte[1];

		protected Stream inp;

		private bool nomoreinput;

		public override bool CanRead
		{
			get
			{
				return true;
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
				return false;
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

		public ZInflaterInputStream(Stream inp) : this(inp, false)
		{
		}

		public ZInflaterInputStream(Stream inp, bool nowrap)
		{
			this.inp = inp;
			this.z.inflateInit(nowrap);
			this.z.next_in = this.buf;
			this.z.next_in_index = 0;
			this.z.avail_in = 0;
		}

		public override void Write(byte[] b, int off, int len)
		{
		}

		public override long Seek(long offset, SeekOrigin origin)
		{
			return 0L;
		}

		public override void SetLength(long value)
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
					this.z.avail_in = this.inp.Read(this.buf, 0, 4192);
					if (this.z.avail_in <= 0)
					{
						this.z.avail_in = 0;
						this.nomoreinput = true;
					}
				}
				int num = this.z.inflate(this.flushLevel);
				if (this.nomoreinput && num == -5)
				{
					break;
				}
				if (num != 0 && num != 1)
				{
					goto Block_8;
				}
				if ((this.nomoreinput || num == 1) && this.z.avail_out == len)
				{
					return 0;
				}
				if (this.z.avail_out != len || num != 0)
				{
					goto IL_100;
				}
			}
			return 0;
			Block_8:
			throw new IOException("inflating: " + this.z.msg);
			IL_100:
			return len - this.z.avail_out;
		}

		public override void Flush()
		{
			this.inp.Flush();
		}

		public override void WriteByte(byte b)
		{
		}

		public override void Close()
		{
			this.inp.Close();
		}

		public override int ReadByte()
		{
			if (this.Read(this.buf1, 0, 1) <= 0)
			{
				return -1;
			}
			return (int)(this.buf1[0] & 255);
		}
	}
}
