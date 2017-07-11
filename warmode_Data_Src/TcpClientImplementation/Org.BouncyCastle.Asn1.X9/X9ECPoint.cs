using Org.BouncyCastle.Math.EC;
using System;

namespace Org.BouncyCastle.Asn1.X9
{
	public class X9ECPoint : Asn1Encodable
	{
		private readonly ECPoint p;

		public ECPoint Point
		{
			get
			{
				return this.p;
			}
		}

		public X9ECPoint(ECPoint p)
		{
			this.p = p.Normalize();
		}

		public X9ECPoint(ECCurve c, Asn1OctetString s)
		{
			this.p = c.DecodePoint(s.GetOctets());
		}

		public override Asn1Object ToAsn1Object()
		{
			return new DerOctetString(this.p.GetEncoded());
		}
	}
}
