using System;

namespace Org.BouncyCastle.Crypto.Tls
{
	public interface IClientCredentialsProvider
	{
		TlsCredentials GetClientCredentials(CertificateRequest certificateRequest);
	}
}
