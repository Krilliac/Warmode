using System;

namespace Org.BouncyCastle.Security
{
	[Obsolete("Never thrown")]
	[Serializable]
	public class NoSuchAlgorithmException : GeneralSecurityException
	{
		public NoSuchAlgorithmException()
		{
		}

		public NoSuchAlgorithmException(string message) : base(message)
		{
		}

		public NoSuchAlgorithmException(string message, Exception exception) : base(message, exception)
		{
		}
	}
}
