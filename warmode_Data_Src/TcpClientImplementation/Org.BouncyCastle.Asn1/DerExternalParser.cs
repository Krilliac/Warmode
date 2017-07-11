using System;

namespace Org.BouncyCastle.Asn1
{
	public class DerExternalParser : Asn1Encodable
	{
		private readonly Asn1StreamParser _parser;

		public DerExternalParser(Asn1StreamParser parser)
		{
			this._parser = parser;
		}

		public IAsn1Convertible ReadObject()
		{
			return this._parser.ReadObject();
		}

		public override Asn1Object ToAsn1Object()
		{
			return new DerExternal(this._parser.ReadVector());
		}
	}
}
