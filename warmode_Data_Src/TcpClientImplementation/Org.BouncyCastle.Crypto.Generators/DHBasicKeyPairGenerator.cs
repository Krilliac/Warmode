using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Math;
using System;

namespace Org.BouncyCastle.Crypto.Generators
{
	public class DHBasicKeyPairGenerator : IAsymmetricCipherKeyPairGenerator
	{
		private DHKeyGenerationParameters param;

		public virtual void Init(KeyGenerationParameters parameters)
		{
			this.param = (DHKeyGenerationParameters)parameters;
		}

		public virtual AsymmetricCipherKeyPair GenerateKeyPair()
		{
			DHKeyGeneratorHelper instance = DHKeyGeneratorHelper.Instance;
			DHParameters parameters = this.param.Parameters;
			BigInteger x = instance.CalculatePrivate(parameters, this.param.Random);
			BigInteger y = instance.CalculatePublic(parameters, x);
			return new AsymmetricCipherKeyPair(new DHPublicKeyParameters(y, parameters), new DHPrivateKeyParameters(x, parameters));
		}
	}
}
