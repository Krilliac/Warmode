using Org.BouncyCastle.Security;
using System;
using System.IO;

namespace Org.BouncyCastle.Crypto.Tls
{
	[Obsolete("Use 'TlsClientProtocol' instead")]
	public class TlsProtocolHandler : TlsClientProtocol
	{
		public TlsProtocolHandler(Stream stream, SecureRandom secureRandom) : base(stream, stream, secureRandom)
		{
		}

		public TlsProtocolHandler(Stream input, Stream output, SecureRandom secureRandom) : base(input, output, secureRandom)
		{
		}
	}
}
