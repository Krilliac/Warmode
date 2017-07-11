using System;

namespace Org.BouncyCastle.Utilities.Zlib
{
	public sealed class ZStream
	{
		private const int MAX_WBITS = 15;

		private const int DEF_WBITS = 15;

		private const int Z_NO_FLUSH = 0;

		private const int Z_PARTIAL_FLUSH = 1;

		private const int Z_SYNC_FLUSH = 2;

		private const int Z_FULL_FLUSH = 3;

		private const int Z_FINISH = 4;

		private const int MAX_MEM_LEVEL = 9;

		private const int Z_OK = 0;

		private const int Z_STREAM_END = 1;

		private const int Z_NEED_DICT = 2;

		private const int Z_ERRNO = -1;

		private const int Z_STREAM_ERROR = -2;

		private const int Z_DATA_ERROR = -3;

		private const int Z_MEM_ERROR = -4;

		private const int Z_BUF_ERROR = -5;

		private const int Z_VERSION_ERROR = -6;

		public byte[] next_in;

		public int next_in_index;

		public int avail_in;

		public long total_in;

		public byte[] next_out;

		public int next_out_index;

		public int avail_out;

		public long total_out;

		public string msg;

		internal Deflate dstate;

		internal Inflate istate;

		internal int data_type;

		public long adler;

		internal Adler32 _adler = new Adler32();

		public int inflateInit()
		{
			return this.inflateInit(15);
		}

		public int inflateInit(bool nowrap)
		{
			return this.inflateInit(15, nowrap);
		}

		public int inflateInit(int w)
		{
			return this.inflateInit(w, false);
		}

		public int inflateInit(int w, bool nowrap)
		{
			this.istate = new Inflate();
			return this.istate.inflateInit(this, nowrap ? (-w) : w);
		}

		public int inflate(int f)
		{
			if (this.istate == null)
			{
				return -2;
			}
			return this.istate.inflate(this, f);
		}

		public int inflateEnd()
		{
			if (this.istate == null)
			{
				return -2;
			}
			int result = this.istate.inflateEnd(this);
			this.istate = null;
			return result;
		}

		public int inflateSync()
		{
			if (this.istate == null)
			{
				return -2;
			}
			return this.istate.inflateSync(this);
		}

		public int inflateSetDictionary(byte[] dictionary, int dictLength)
		{
			if (this.istate == null)
			{
				return -2;
			}
			return this.istate.inflateSetDictionary(this, dictionary, dictLength);
		}

		public int deflateInit(int level)
		{
			return this.deflateInit(level, 15);
		}

		public int deflateInit(int level, bool nowrap)
		{
			return this.deflateInit(level, 15, nowrap);
		}

		public int deflateInit(int level, int bits)
		{
			return this.deflateInit(level, bits, false);
		}

		public int deflateInit(int level, int bits, bool nowrap)
		{
			this.dstate = new Deflate();
			return this.dstate.deflateInit(this, level, nowrap ? (-bits) : bits);
		}

		public int deflate(int flush)
		{
			if (this.dstate == null)
			{
				return -2;
			}
			return this.dstate.deflate(this, flush);
		}

		public int deflateEnd()
		{
			if (this.dstate == null)
			{
				return -2;
			}
			int result = this.dstate.deflateEnd();
			this.dstate = null;
			return result;
		}

		public int deflateParams(int level, int strategy)
		{
			if (this.dstate == null)
			{
				return -2;
			}
			return this.dstate.deflateParams(this, level, strategy);
		}

		public int deflateSetDictionary(byte[] dictionary, int dictLength)
		{
			if (this.dstate == null)
			{
				return -2;
			}
			return this.dstate.deflateSetDictionary(this, dictionary, dictLength);
		}

		internal void flush_pending()
		{
			int pending = this.dstate.pending;
			if (pending > this.avail_out)
			{
				pending = this.avail_out;
			}
			if (pending == 0)
			{
				return;
			}
			if (this.dstate.pending_buf.Length > this.dstate.pending_out && this.next_out.Length > this.next_out_index && this.dstate.pending_buf.Length >= this.dstate.pending_out + pending)
			{
				int arg_77_0 = this.next_out.Length;
				int arg_76_0 = this.next_out_index + pending;
			}
			Array.Copy(this.dstate.pending_buf, this.dstate.pending_out, this.next_out, this.next_out_index, pending);
			this.next_out_index += pending;
			this.dstate.pending_out += pending;
			this.total_out += (long)pending;
			this.avail_out -= pending;
			this.dstate.pending -= pending;
			if (this.dstate.pending == 0)
			{
				this.dstate.pending_out = 0;
			}
		}

		internal int read_buf(byte[] buf, int start, int size)
		{
			int num = this.avail_in;
			if (num > size)
			{
				num = size;
			}
			if (num == 0)
			{
				return 0;
			}
			this.avail_in -= num;
			if (this.dstate.noheader == 0)
			{
				this.adler = this._adler.adler32(this.adler, this.next_in, this.next_in_index, num);
			}
			Array.Copy(this.next_in, this.next_in_index, buf, start, num);
			this.next_in_index += num;
			this.total_in += (long)num;
			return num;
		}

		public void free()
		{
			this.next_in = null;
			this.next_out = null;
			this.msg = null;
			this._adler = null;
		}
	}
}
