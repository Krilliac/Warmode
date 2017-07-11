using System;

namespace Org.BouncyCastle.Crypto.Parameters
{
	public class MqvPublicParameters : ICipherParameters
	{
		private readonly ECPublicKeyParameters staticPublicKey;

		private readonly ECPublicKeyParameters ephemeralPublicKey;

		public ECPublicKeyParameters StaticPublicKey
		{
			get
			{
				return this.staticPublicKey;
			}
		}

		public ECPublicKeyParameters EphemeralPublicKey
		{
			get
			{
				return this.ephemeralPublicKey;
			}
		}

		public MqvPublicParameters(ECPublicKeyParameters staticPublicKey, ECPublicKeyParameters ephemeralPublicKey)
		{
			this.staticPublicKey = staticPublicKey;
			this.ephemeralPublicKey = ephemeralPublicKey;
		}
	}
}
