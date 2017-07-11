using Org.BouncyCastle.Asn1.X509;
using System;

namespace Org.BouncyCastle.Crypto.Tls
{
	public class AlwaysValidVerifyer : ICertificateVerifyer
	{
		public bool IsValid(X509CertificateStructure[] certs)
		{
			return true;
		}
	}
}
