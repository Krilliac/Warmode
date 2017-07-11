using System;

namespace Org.BouncyCastle.Crypto.Parameters
{
	public class MqvPrivateParameters : ICipherParameters
	{
		private readonly ECPrivateKeyParameters staticPrivateKey;

		private readonly ECPrivateKeyParameters ephemeralPrivateKey;

		private readonly ECPublicKeyParameters ephemeralPublicKey;

		public ECPrivateKeyParameters StaticPrivateKey
		{
			get
			{
				return this.staticPrivateKey;
			}
		}

		public ECPrivateKeyParameters EphemeralPrivateKey
		{
			get
			{
				return this.ephemeralPrivateKey;
			}
		}

		public ECPublicKeyParameters EphemeralPublicKey
		{
			get
			{
				return this.ephemeralPublicKey;
			}
		}

		public MqvPrivateParameters(ECPrivateKeyParameters staticPrivateKey, ECPrivateKeyParameters ephemeralPrivateKey) : this(staticPrivateKey, ephemeralPrivateKey, null)
		{
		}

		public MqvPrivateParameters(ECPrivateKeyParameters staticPrivateKey, ECPrivateKeyParameters ephemeralPrivateKey, ECPublicKeyParameters ephemeralPublicKey)
		{
			this.staticPrivateKey = staticPrivateKey;
			this.ephemeralPrivateKey = ephemeralPrivateKey;
			this.ephemeralPublicKey = ephemeralPublicKey;
		}
	}
}
