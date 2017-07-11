using System;

namespace Org.BouncyCastle.Asn1.IsisMtt
{
	public abstract class IsisMttObjectIdentifiers
	{
		public static readonly DerObjectIdentifier IdIsisMtt = new DerObjectIdentifier("1.3.36.8");

		public static readonly DerObjectIdentifier IdIsisMttCP = new DerObjectIdentifier(IsisMttObjectIdentifiers.IdIsisMtt + ".1");

		public static readonly DerObjectIdentifier IdIsisMttCPAccredited = new DerObjectIdentifier(IsisMttObjectIdentifiers.IdIsisMttCP + ".1");

		public static readonly DerObjectIdentifier IdIsisMttAT = new DerObjectIdentifier(IsisMttObjectIdentifiers.IdIsisMtt + ".3");

		public static readonly DerObjectIdentifier IdIsisMttATDateOfCertGen = new DerObjectIdentifier(IsisMttObjectIdentifiers.IdIsisMttAT + ".1");

		public static readonly DerObjectIdentifier IdIsisMttATProcuration = new DerObjectIdentifier(IsisMttObjectIdentifiers.IdIsisMttAT + ".2");

		public static readonly DerObjectIdentifier IdIsisMttATAdmission = new DerObjectIdentifier(IsisMttObjectIdentifiers.IdIsisMttAT + ".3");

		public static readonly DerObjectIdentifier IdIsisMttATMonetaryLimit = new DerObjectIdentifier(IsisMttObjectIdentifiers.IdIsisMttAT + ".4");

		public static readonly DerObjectIdentifier IdIsisMttATDeclarationOfMajority = new DerObjectIdentifier(IsisMttObjectIdentifiers.IdIsisMttAT + ".5");

		public static readonly DerObjectIdentifier IdIsisMttATIccsn = new DerObjectIdentifier(IsisMttObjectIdentifiers.IdIsisMttAT + ".6");

		public static readonly DerObjectIdentifier IdIsisMttATPKReference = new DerObjectIdentifier(IsisMttObjectIdentifiers.IdIsisMttAT + ".7");

		public static readonly DerObjectIdentifier IdIsisMttATRestriction = new DerObjectIdentifier(IsisMttObjectIdentifiers.IdIsisMttAT + ".8");

		public static readonly DerObjectIdentifier IdIsisMttATRetrieveIfAllowed = new DerObjectIdentifier(IsisMttObjectIdentifiers.IdIsisMttAT + ".9");

		public static readonly DerObjectIdentifier IdIsisMttATRequestedCertificate = new DerObjectIdentifier(IsisMttObjectIdentifiers.IdIsisMttAT + ".10");

		public static readonly DerObjectIdentifier IdIsisMttATNamingAuthorities = new DerObjectIdentifier(IsisMttObjectIdentifiers.IdIsisMttAT + ".11");

		public static readonly DerObjectIdentifier IdIsisMttATCertInDirSince = new DerObjectIdentifier(IsisMttObjectIdentifiers.IdIsisMttAT + ".12");

		public static readonly DerObjectIdentifier IdIsisMttATCertHash = new DerObjectIdentifier(IsisMttObjectIdentifiers.IdIsisMttAT + ".13");

		public static readonly DerObjectIdentifier IdIsisMttATNameAtBirth = new DerObjectIdentifier(IsisMttObjectIdentifiers.IdIsisMttAT + ".14");

		public static readonly DerObjectIdentifier IdIsisMttATAdditionalInformation = new DerObjectIdentifier(IsisMttObjectIdentifiers.IdIsisMttAT + ".15");

		public static readonly DerObjectIdentifier IdIsisMttATLiabilityLimitationFlag = new DerObjectIdentifier("0.2.262.1.10.12.0");
	}
}
