using System;

namespace Org.BouncyCastle.Asn1.X509.SigI
{
	public sealed class SigIObjectIdentifiers
	{
		public static readonly DerObjectIdentifier IdSigI = new DerObjectIdentifier("1.3.36.8");

		public static readonly DerObjectIdentifier IdSigIKP = new DerObjectIdentifier(SigIObjectIdentifiers.IdSigI + ".2");

		public static readonly DerObjectIdentifier IdSigICP = new DerObjectIdentifier(SigIObjectIdentifiers.IdSigI + ".1");

		public static readonly DerObjectIdentifier IdSigION = new DerObjectIdentifier(SigIObjectIdentifiers.IdSigI + ".4");

		public static readonly DerObjectIdentifier IdSigIKPDirectoryService = new DerObjectIdentifier(SigIObjectIdentifiers.IdSigIKP + ".1");

		public static readonly DerObjectIdentifier IdSigIONPersonalData = new DerObjectIdentifier(SigIObjectIdentifiers.IdSigION + ".1");

		public static readonly DerObjectIdentifier IdSigICPSigConform = new DerObjectIdentifier(SigIObjectIdentifiers.IdSigICP + ".1");

		private SigIObjectIdentifiers()
		{
		}
	}
}
