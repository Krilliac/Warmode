using System;

namespace Org.BouncyCastle.Asn1.Misc
{
	public abstract class MiscObjectIdentifiers
	{
		public static readonly DerObjectIdentifier Netscape = new DerObjectIdentifier("2.16.840.1.113730.1");

		public static readonly DerObjectIdentifier NetscapeCertType = MiscObjectIdentifiers.Netscape.Branch("1");

		public static readonly DerObjectIdentifier NetscapeBaseUrl = MiscObjectIdentifiers.Netscape.Branch("2");

		public static readonly DerObjectIdentifier NetscapeRevocationUrl = MiscObjectIdentifiers.Netscape.Branch("3");

		public static readonly DerObjectIdentifier NetscapeCARevocationUrl = MiscObjectIdentifiers.Netscape.Branch("4");

		public static readonly DerObjectIdentifier NetscapeRenewalUrl = MiscObjectIdentifiers.Netscape.Branch("7");

		public static readonly DerObjectIdentifier NetscapeCAPolicyUrl = MiscObjectIdentifiers.Netscape.Branch("8");

		public static readonly DerObjectIdentifier NetscapeSslServerName = MiscObjectIdentifiers.Netscape.Branch("12");

		public static readonly DerObjectIdentifier NetscapeCertComment = MiscObjectIdentifiers.Netscape.Branch("13");

		public static readonly DerObjectIdentifier Verisign = new DerObjectIdentifier("2.16.840.1.113733.1");

		public static readonly DerObjectIdentifier VerisignCzagExtension = MiscObjectIdentifiers.Verisign.Branch("6.3");

		public static readonly DerObjectIdentifier VerisignPrivate_6_9 = MiscObjectIdentifiers.Verisign.Branch("6.9");

		public static readonly DerObjectIdentifier VerisignOnSiteJurisdictionHash = MiscObjectIdentifiers.Verisign.Branch("6.11");

		public static readonly DerObjectIdentifier VerisignBitString_6_13 = MiscObjectIdentifiers.Verisign.Branch("6.13");

		public static readonly DerObjectIdentifier VerisignDnbDunsNumber = MiscObjectIdentifiers.Verisign.Branch("6.15");

		public static readonly DerObjectIdentifier VerisignIssStrongCrypto = MiscObjectIdentifiers.Verisign.Branch("8.1");

		public static readonly string Novell = "2.16.840.1.113719";

		public static readonly DerObjectIdentifier NovellSecurityAttribs = new DerObjectIdentifier(MiscObjectIdentifiers.Novell + ".1.9.4.1");

		public static readonly string Entrust = "1.2.840.113533.7";

		public static readonly DerObjectIdentifier EntrustVersionExtension = new DerObjectIdentifier(MiscObjectIdentifiers.Entrust + ".65.0");
	}
}
