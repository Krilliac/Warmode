using System;

namespace Org.BouncyCastle.Crypto.Tls
{
	public class TlsException : Exception
	{
		public TlsException()
		{
		}

		public TlsException(string message) : base(message)
		{
		}

		public TlsException(string message, Exception exception) : base(message, exception)
		{
		}
	}
}
