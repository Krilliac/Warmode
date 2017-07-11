using System;

namespace Org.BouncyCastle.Asn1.Iana
{
	public abstract class IanaObjectIdentifiers
	{
		public static readonly DerObjectIdentifier IsakmpOakley = new DerObjectIdentifier("1.3.6.1.5.5.8.1");

		public static readonly DerObjectIdentifier HmacMD5 = new DerObjectIdentifier(IanaObjectIdentifiers.IsakmpOakley + ".1");

		public static readonly DerObjectIdentifier HmacSha1 = new DerObjectIdentifier(IanaObjectIdentifiers.IsakmpOakley + ".2");

		public static readonly DerObjectIdentifier HmacTiger = new DerObjectIdentifier(IanaObjectIdentifiers.IsakmpOakley + ".3");

		public static readonly DerObjectIdentifier HmacRipeMD160 = new DerObjectIdentifier(IanaObjectIdentifiers.IsakmpOakley + ".4");
	}
}
