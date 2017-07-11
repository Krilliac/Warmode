using Org.BouncyCastle.Utilities.Zlib;
using System;
using System.IO;

namespace Org.BouncyCastle.Crypto.Tls
{
	public class TlsDeflateCompression : TlsCompression
	{
		protected class DeflateOutputStream : ZOutputStream
		{
			public DeflateOutputStream(Stream output, ZStream z, bool compress) : base(output, z)
			{
				this.compress = compress;
				this.FlushMode = 2;
			}

			public override void Flush()
			{
			}
		}

		public const int LEVEL_NONE = 0;

		public const int LEVEL_FASTEST = 1;

		public const int LEVEL_SMALLEST = 9;

		public const int LEVEL_DEFAULT = -1;

		protected readonly ZStream zIn;

		protected readonly ZStream zOut;

		public TlsDeflateCompression() : this(-1)
		{
		}

		public TlsDeflateCompression(int level)
		{
			this.zIn = new ZStream();
			this.zIn.inflateInit();
			this.zOut = new ZStream();
			this.zOut.deflateInit(level);
		}

		public virtual Stream Compress(Stream output)
		{
			return new TlsDeflateCompression.DeflateOutputStream(output, this.zOut, true);
		}

		public virtual Stream Decompress(Stream output)
		{
			return new TlsDeflateCompression.DeflateOutputStream(output, this.zIn, false);
		}
	}
}
