using Org.BouncyCastle.Asn1;
using Org.BouncyCastle.Asn1.X509;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.Security.Certificates;
using System;

namespace Org.BouncyCastle.X509.Extension
{
	public class AuthorityKeyIdentifierStructure : AuthorityKeyIdentifier
	{
		public AuthorityKeyIdentifierStructure(Asn1OctetString encodedValue) : base((Asn1Sequence)X509ExtensionUtilities.FromExtensionValue(encodedValue))
		{
		}

		private static Asn1Sequence FromCertificate(X509Certificate certificate)
		{
			Asn1Sequence result;
			try
			{
				GeneralName name = new GeneralName(PrincipalUtilities.GetIssuerX509Principal(certificate));
				if (certificate.Version == 3)
				{
					Asn1OctetString extensionValue = certificate.GetExtensionValue(X509Extensions.SubjectKeyIdentifier);
					if (extensionValue != null)
					{
						Asn1OctetString asn1OctetString = (Asn1OctetString)X509ExtensionUtilities.FromExtensionValue(extensionValue);
						result = (Asn1Sequence)new AuthorityKeyIdentifier(asn1OctetString.GetOctets(), new GeneralNames(name), certificate.SerialNumber).ToAsn1Object();
						return result;
					}
				}
				SubjectPublicKeyInfo spki = SubjectPublicKeyInfoFactory.CreateSubjectPublicKeyInfo(certificate.GetPublicKey());
				result = (Asn1Sequence)new AuthorityKeyIdentifier(spki, new GeneralNames(name), certificate.SerialNumber).ToAsn1Object();
			}
			catch (Exception exception)
			{
				throw new CertificateParsingException("Exception extracting certificate details", exception);
			}
			return result;
		}

		private static Asn1Sequence FromKey(AsymmetricKeyParameter pubKey)
		{
			Asn1Sequence result;
			try
			{
				SubjectPublicKeyInfo spki = SubjectPublicKeyInfoFactory.CreateSubjectPublicKeyInfo(pubKey);
				result = (Asn1Sequence)new AuthorityKeyIdentifier(spki).ToAsn1Object();
			}
			catch (Exception arg)
			{
				throw new InvalidKeyException("can't process key: " + arg);
			}
			return result;
		}

		public AuthorityKeyIdentifierStructure(X509Certificate certificate) : base(AuthorityKeyIdentifierStructure.FromCertificate(certificate))
		{
		}

		public AuthorityKeyIdentifierStructure(AsymmetricKeyParameter pubKey) : base(AuthorityKeyIdentifierStructure.FromKey(pubKey))
		{
		}
	}
}
