using System;
using System.Collections.Generic;

namespace Org.BouncyCastle.Crypto.Tls
{
	public class LegacyTlsClient : DefaultTlsClient
	{
		protected ICertificateVerifyer verifyer;

		protected IClientCredentialsProvider credProvider;

		protected override List<string> HostNames
		{
			get;
			set;
		}

		public LegacyTlsClient(ICertificateVerifyer verifyer, IClientCredentialsProvider prov, List<string> hostNames)
		{
			this.verifyer = verifyer;
			this.credProvider = prov;
			this.HostNames = hostNames;
		}

		public override TlsAuthentication GetAuthentication()
		{
			return new LegacyTlsAuthentication(this.verifyer, this.credProvider);
		}
	}
}
