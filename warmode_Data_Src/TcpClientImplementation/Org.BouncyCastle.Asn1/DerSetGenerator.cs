using System;
using System.IO;

namespace Org.BouncyCastle.Asn1
{
	public class DerSetGenerator : DerGenerator
	{
		private readonly MemoryStream _bOut = new MemoryStream();

		public DerSetGenerator(Stream outStream) : base(outStream)
		{
		}

		public DerSetGenerator(Stream outStream, int tagNo, bool isExplicit) : base(outStream, tagNo, isExplicit)
		{
		}

		public override void AddObject(Asn1Encodable obj)
		{
			new DerOutputStream(this._bOut).WriteObject(obj);
		}

		public override Stream GetRawOutputStream()
		{
			return this._bOut;
		}

		public override void Close()
		{
			base.WriteDerEncoded(49, this._bOut.ToArray());
		}
	}
}
