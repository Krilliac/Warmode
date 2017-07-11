using Org.BouncyCastle.Utilities.IO;
using System;
using System.IO;

namespace Org.BouncyCastle.Asn1
{
	public class BerOctetStringGenerator : BerGenerator
	{
		private class BufferedBerOctetStream : BaseOutputStream
		{
			private byte[] _buf;

			private int _off;

			private readonly BerOctetStringGenerator _gen;

			private readonly DerOutputStream _derOut;

			internal BufferedBerOctetStream(BerOctetStringGenerator gen, byte[] buf)
			{
				this._gen = gen;
				this._buf = buf;
				this._off = 0;
				this._derOut = new DerOutputStream(this._gen.Out);
			}

			public override void WriteByte(byte b)
			{
				this._buf[this._off++] = b;
				if (this._off == this._buf.Length)
				{
					DerOctetString.Encode(this._derOut, this._buf, 0, this._off);
					this._off = 0;
				}
			}

			public override void Write(byte[] buf, int offset, int len)
			{
				while (len > 0)
				{
					int num = Math.Min(len, this._buf.Length - this._off);
					if (num == this._buf.Length)
					{
						DerOctetString.Encode(this._derOut, buf, offset, num);
					}
					else
					{
						Array.Copy(buf, offset, this._buf, this._off, num);
						this._off += num;
						if (this._off < this._buf.Length)
						{
							return;
						}
						DerOctetString.Encode(this._derOut, this._buf, 0, this._off);
						this._off = 0;
					}
					offset += num;
					len -= num;
				}
			}

			public override void Close()
			{
				if (this._off != 0)
				{
					DerOctetString.Encode(this._derOut, this._buf, 0, this._off);
				}
				this._gen.WriteBerEnd();
				base.Close();
			}
		}

		public BerOctetStringGenerator(Stream outStream) : base(outStream)
		{
			base.WriteBerHeader(36);
		}

		public BerOctetStringGenerator(Stream outStream, int tagNo, bool isExplicit) : base(outStream, tagNo, isExplicit)
		{
			base.WriteBerHeader(36);
		}

		public Stream GetOctetOutputStream()
		{
			return this.GetOctetOutputStream(new byte[1000]);
		}

		public Stream GetOctetOutputStream(int bufSize)
		{
			if (bufSize >= 1)
			{
				return this.GetOctetOutputStream(new byte[bufSize]);
			}
			return this.GetOctetOutputStream();
		}

		public Stream GetOctetOutputStream(byte[] buf)
		{
			return new BerOctetStringGenerator.BufferedBerOctetStream(this, buf);
		}
	}
}
