using Org.BouncyCastle.Security;
using System;

namespace Org.BouncyCastle.Crypto.Tls
{
	internal class TlsClientContextImpl : AbstractTlsContext, TlsClientContext, TlsContext
	{
		public override bool IsServer
		{
			get
			{
				return false;
			}
		}

		internal TlsClientContextImpl(SecureRandom secureRandom, SecurityParameters securityParameters) : base(secureRandom, securityParameters)
		{
		}
	}
}
