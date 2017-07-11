using System;

namespace Org.BouncyCastle.Asn1.Microsoft
{
	public abstract class MicrosoftObjectIdentifiers
	{
		public static readonly DerObjectIdentifier Microsoft = new DerObjectIdentifier("1.3.6.1.4.1.311");

		public static readonly DerObjectIdentifier MicrosoftCertTemplateV1 = MicrosoftObjectIdentifiers.Microsoft.Branch("20.2");

		public static readonly DerObjectIdentifier MicrosoftCAVersion = MicrosoftObjectIdentifiers.Microsoft.Branch("21.1");

		public static readonly DerObjectIdentifier MicrosoftPrevCACertHash = MicrosoftObjectIdentifiers.Microsoft.Branch("21.2");

		public static readonly DerObjectIdentifier MicrosoftCrlNextPublish = MicrosoftObjectIdentifiers.Microsoft.Branch("21.4");

		public static readonly DerObjectIdentifier MicrosoftCertTemplateV2 = MicrosoftObjectIdentifiers.Microsoft.Branch("21.7");

		public static readonly DerObjectIdentifier MicrosoftAppPolicies = MicrosoftObjectIdentifiers.Microsoft.Branch("21.10");
	}
}
