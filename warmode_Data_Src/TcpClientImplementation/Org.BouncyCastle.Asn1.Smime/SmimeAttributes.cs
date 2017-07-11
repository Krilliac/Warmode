using Org.BouncyCastle.Asn1.Pkcs;
using System;

namespace Org.BouncyCastle.Asn1.Smime
{
	public abstract class SmimeAttributes
	{
		public static readonly DerObjectIdentifier SmimeCapabilities = PkcsObjectIdentifiers.Pkcs9AtSmimeCapabilities;

		public static readonly DerObjectIdentifier EncrypKeyPref = PkcsObjectIdentifiers.IdAAEncrypKeyPref;
	}
}
