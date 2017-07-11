using Org.BouncyCastle.Crypto.Parameters;
using System;

namespace Org.BouncyCastle.Crypto.Tls
{
	public interface TlsSrpGroupVerifier
	{
		bool Accept(Srp6GroupParameters group);
	}
}
