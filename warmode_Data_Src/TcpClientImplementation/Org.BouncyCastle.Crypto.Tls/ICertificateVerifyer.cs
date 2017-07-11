using Org.BouncyCastle.Asn1.X509;
using System;

namespace Org.BouncyCastle.Crypto.Tls
{
	public interface ICertificateVerifyer
	{
		bool IsValid(X509CertificateStructure[] certs);
	}
}
