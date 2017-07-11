using System;

namespace Org.BouncyCastle.Crypto
{
	public class AsymmetricCipherKeyPair
	{
		private readonly AsymmetricKeyParameter publicParameter;

		private readonly AsymmetricKeyParameter privateParameter;

		public AsymmetricKeyParameter Public
		{
			get
			{
				return this.publicParameter;
			}
		}

		public AsymmetricKeyParameter Private
		{
			get
			{
				return this.privateParameter;
			}
		}

		public AsymmetricCipherKeyPair(AsymmetricKeyParameter publicParameter, AsymmetricKeyParameter privateParameter)
		{
			if (publicParameter.IsPrivate)
			{
				throw new ArgumentException("Expected a public key", "publicParameter");
			}
			if (!privateParameter.IsPrivate)
			{
				throw new ArgumentException("Expected a private key", "privateParameter");
			}
			this.publicParameter = publicParameter;
			this.privateParameter = privateParameter;
		}
	}
}
