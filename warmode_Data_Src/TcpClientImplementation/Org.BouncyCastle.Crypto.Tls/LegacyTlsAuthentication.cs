using System;

namespace Org.BouncyCastle.Crypto.Tls
{
	public class LegacyTlsAuthentication : TlsAuthentication
	{
		protected ICertificateVerifyer verifyer;

		protected IClientCredentialsProvider credProvider;

		public LegacyTlsAuthentication(ICertificateVerifyer verifyer, IClientCredentialsProvider prov)
		{
			this.verifyer = verifyer;
			this.credProvider = prov;
		}

		public virtual void NotifyServerCertificate(Certificate serverCertificate)
		{
			if (!this.verifyer.IsValid(serverCertificate.GetCertificateList()))
			{
				throw new TlsFatalAlert(90);
			}
		}

		public virtual TlsCredentials GetClientCredentials(CertificateRequest certificateRequest)
		{
			if (this.credProvider != null)
			{
				return this.credProvider.GetClientCredentials(certificateRequest);
			}
			return null;
		}
	}
}
