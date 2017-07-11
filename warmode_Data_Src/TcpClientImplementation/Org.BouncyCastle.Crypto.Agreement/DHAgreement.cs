using Org.BouncyCastle.Crypto.Generators;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Math;
using Org.BouncyCastle.Security;
using System;

namespace Org.BouncyCastle.Crypto.Agreement
{
	public class DHAgreement
	{
		private DHPrivateKeyParameters key;

		private DHParameters dhParams;

		private BigInteger privateValue;

		private SecureRandom random;

		public void Init(ICipherParameters parameters)
		{
			AsymmetricKeyParameter asymmetricKeyParameter;
			if (parameters is ParametersWithRandom)
			{
				ParametersWithRandom parametersWithRandom = (ParametersWithRandom)parameters;
				this.random = parametersWithRandom.Random;
				asymmetricKeyParameter = (AsymmetricKeyParameter)parametersWithRandom.Parameters;
			}
			else
			{
				this.random = new SecureRandom();
				asymmetricKeyParameter = (AsymmetricKeyParameter)parameters;
			}
			if (!(asymmetricKeyParameter is DHPrivateKeyParameters))
			{
				throw new ArgumentException("DHEngine expects DHPrivateKeyParameters");
			}
			this.key = (DHPrivateKeyParameters)asymmetricKeyParameter;
			this.dhParams = this.key.Parameters;
		}

		public BigInteger CalculateMessage()
		{
			DHKeyPairGenerator dHKeyPairGenerator = new DHKeyPairGenerator();
			dHKeyPairGenerator.Init(new DHKeyGenerationParameters(this.random, this.dhParams));
			AsymmetricCipherKeyPair asymmetricCipherKeyPair = dHKeyPairGenerator.GenerateKeyPair();
			this.privateValue = ((DHPrivateKeyParameters)asymmetricCipherKeyPair.Private).X;
			return ((DHPublicKeyParameters)asymmetricCipherKeyPair.Public).Y;
		}

		public BigInteger CalculateAgreement(DHPublicKeyParameters pub, BigInteger message)
		{
			if (pub == null)
			{
				throw new ArgumentNullException("pub");
			}
			if (message == null)
			{
				throw new ArgumentNullException("message");
			}
			if (!pub.Parameters.Equals(this.dhParams))
			{
				throw new ArgumentException("Diffie-Hellman public key has wrong parameters.");
			}
			BigInteger p = this.dhParams.P;
			return message.ModPow(this.key.X, p).Multiply(pub.Y.ModPow(this.privateValue, p)).Mod(p);
		}
	}
}
