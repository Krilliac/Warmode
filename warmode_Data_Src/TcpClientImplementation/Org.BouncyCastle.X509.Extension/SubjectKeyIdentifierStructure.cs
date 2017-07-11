using Org.BouncyCastle.Asn1;
using Org.BouncyCastle.Asn1.X509;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Security.Certificates;
using System;

namespace Org.BouncyCastle.X509.Extension
{
	public class SubjectKeyIdentifierStructure : SubjectKeyIdentifier
	{
		public SubjectKeyIdentifierStructure(Asn1OctetString encodedValue) : base((Asn1OctetString)X509ExtensionUtilities.FromExtensionValue(encodedValue))
		{
		}

		private static Asn1OctetString FromPublicKey(AsymmetricKeyParameter pubKey)
		{
			Asn1OctetString result;
			try
			{
				SubjectPublicKeyInfo spki = SubjectPublicKeyInfoFactory.CreateSubjectPublicKeyInfo(pubKey);
				result = (Asn1OctetString)new SubjectKeyIdentifier(spki).ToAsn1Object();
			}
			catch (Exception ex)
			{
				throw new CertificateParsingException("Exception extracting certificate details: " + ex.ToString());
			}
			return result;
		}

		public SubjectKeyIdentifierStructure(AsymmetricKeyParameter pubKey) : base(SubjectKeyIdentifierStructure.FromPublicKey(pubKey))
		{
		}
	}
}
