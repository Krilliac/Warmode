using System;

namespace Org.BouncyCastle.Asn1.Ocsp
{
	public abstract class OcspObjectIdentifiers
	{
		internal const string PkixOcspId = "1.3.6.1.5.5.7.48.1";

		public static readonly DerObjectIdentifier PkixOcsp = new DerObjectIdentifier("1.3.6.1.5.5.7.48.1");

		public static readonly DerObjectIdentifier PkixOcspBasic = new DerObjectIdentifier("1.3.6.1.5.5.7.48.1.1");

		public static readonly DerObjectIdentifier PkixOcspNonce = new DerObjectIdentifier(OcspObjectIdentifiers.PkixOcsp + ".2");

		public static readonly DerObjectIdentifier PkixOcspCrl = new DerObjectIdentifier(OcspObjectIdentifiers.PkixOcsp + ".3");

		public static readonly DerObjectIdentifier PkixOcspResponse = new DerObjectIdentifier(OcspObjectIdentifiers.PkixOcsp + ".4");

		public static readonly DerObjectIdentifier PkixOcspNocheck = new DerObjectIdentifier(OcspObjectIdentifiers.PkixOcsp + ".5");

		public static readonly DerObjectIdentifier PkixOcspArchiveCutoff = new DerObjectIdentifier(OcspObjectIdentifiers.PkixOcsp + ".6");

		public static readonly DerObjectIdentifier PkixOcspServiceLocator = new DerObjectIdentifier(OcspObjectIdentifiers.PkixOcsp + ".7");
	}
}
