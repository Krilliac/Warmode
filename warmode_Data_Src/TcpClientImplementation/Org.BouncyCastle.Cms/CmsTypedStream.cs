using Org.BouncyCastle.Asn1.Pkcs;
using Org.BouncyCastle.Asn1.Utilities;
using Org.BouncyCastle.Utilities.IO;
using System;
using System.IO;

namespace Org.BouncyCastle.Cms
{
	public class CmsTypedStream
	{
		private class FullReaderStream : FilterStream
		{
			internal FullReaderStream(Stream input) : base(input)
			{
			}

			public override int Read(byte[] buf, int off, int len)
			{
				return Streams.ReadFully(this.s, buf, off, len);
			}
		}

		private const int BufferSize = 32768;

		private readonly string _oid;

		private readonly Stream _in;

		public string ContentType
		{
			get
			{
				return this._oid;
			}
		}

		public Stream ContentStream
		{
			get
			{
				return this._in;
			}
		}

		public CmsTypedStream(Stream inStream) : this(PkcsObjectIdentifiers.Data.Id, inStream, 32768)
		{
		}

		public CmsTypedStream(string oid, Stream inStream) : this(oid, inStream, 32768)
		{
		}

		public CmsTypedStream(string oid, Stream inStream, int bufSize)
		{
			this._oid = oid;
			this._in = new CmsTypedStream.FullReaderStream(new BufferedStream(inStream, bufSize));
		}

		public void Drain()
		{
			Streams.Drain(this._in);
			this._in.Close();
		}
	}
}
