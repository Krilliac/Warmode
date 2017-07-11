using Org.BouncyCastle.Asn1;
using Org.BouncyCastle.Asn1.X509;
using Org.BouncyCastle.Security.Certificates;
using System;

namespace Org.BouncyCastle.X509
{
	public class PrincipalUtilities
	{
		public static X509Name GetIssuerX509Principal(X509Certificate cert)
		{
			X509Name issuer;
			try
			{
				TbsCertificateStructure instance = TbsCertificateStructure.GetInstance(Asn1Object.FromByteArray(cert.GetTbsCertificate()));
				issuer = instance.Issuer;
			}
			catch (Exception e)
			{
				throw new CertificateEncodingException("Could not extract issuer", e);
			}
			return issuer;
		}

		public static X509Name GetSubjectX509Principal(X509Certificate cert)
		{
			X509Name subject;
			try
			{
				TbsCertificateStructure instance = TbsCertificateStructure.GetInstance(Asn1Object.FromByteArray(cert.GetTbsCertificate()));
				subject = instance.Subject;
			}
			catch (Exception e)
			{
				throw new CertificateEncodingException("Could not extract subject", e);
			}
			return subject;
		}

		public static X509Name GetIssuerX509Principal(X509Crl crl)
		{
			X509Name issuer;
			try
			{
				TbsCertificateList instance = TbsCertificateList.GetInstance(Asn1Object.FromByteArray(crl.GetTbsCertList()));
				issuer = instance.Issuer;
			}
			catch (Exception e)
			{
				throw new CrlException("Could not extract issuer", e);
			}
			return issuer;
		}
	}
}
