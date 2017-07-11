using System;

namespace Org.BouncyCastle.Crypto.Parameters
{
	public class KdfParameters : IDerivationParameters
	{
		private byte[] iv;

		private byte[] shared;

		public KdfParameters(byte[] shared, byte[] iv)
		{
			this.shared = shared;
			this.iv = iv;
		}

		public byte[] GetSharedSecret()
		{
			return this.shared;
		}

		public byte[] GetIV()
		{
			return this.iv;
		}
	}
}
