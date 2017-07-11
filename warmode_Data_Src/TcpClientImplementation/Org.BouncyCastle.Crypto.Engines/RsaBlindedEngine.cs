using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Math;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.Utilities;
using System;

namespace Org.BouncyCastle.Crypto.Engines
{
	public class RsaBlindedEngine : IAsymmetricBlockCipher
	{
		private readonly RsaCoreEngine core = new RsaCoreEngine();

		private RsaKeyParameters key;

		private SecureRandom random;

		public virtual string AlgorithmName
		{
			get
			{
				return "RSA";
			}
		}

		public virtual void Init(bool forEncryption, ICipherParameters param)
		{
			this.core.Init(forEncryption, param);
			if (param is ParametersWithRandom)
			{
				ParametersWithRandom parametersWithRandom = (ParametersWithRandom)param;
				this.key = (RsaKeyParameters)parametersWithRandom.Parameters;
				this.random = parametersWithRandom.Random;
				return;
			}
			this.key = (RsaKeyParameters)param;
			this.random = new SecureRandom();
		}

		public virtual int GetInputBlockSize()
		{
			return this.core.GetInputBlockSize();
		}

		public virtual int GetOutputBlockSize()
		{
			return this.core.GetOutputBlockSize();
		}

		public virtual byte[] ProcessBlock(byte[] inBuf, int inOff, int inLen)
		{
			if (this.key == null)
			{
				throw new InvalidOperationException("RSA engine not initialised");
			}
			BigInteger bigInteger = this.core.ConvertInput(inBuf, inOff, inLen);
			BigInteger result;
			if (this.key is RsaPrivateCrtKeyParameters)
			{
				RsaPrivateCrtKeyParameters rsaPrivateCrtKeyParameters = (RsaPrivateCrtKeyParameters)this.key;
				BigInteger publicExponent = rsaPrivateCrtKeyParameters.PublicExponent;
				if (publicExponent != null)
				{
					BigInteger modulus = rsaPrivateCrtKeyParameters.Modulus;
					BigInteger bigInteger2 = BigIntegers.CreateRandomInRange(BigInteger.One, modulus.Subtract(BigInteger.One), this.random);
					BigInteger input = bigInteger2.ModPow(publicExponent, modulus).Multiply(bigInteger).Mod(modulus);
					BigInteger bigInteger3 = this.core.ProcessBlock(input);
					BigInteger val = bigInteger2.ModInverse(modulus);
					result = bigInteger3.Multiply(val).Mod(modulus);
				}
				else
				{
					result = this.core.ProcessBlock(bigInteger);
				}
			}
			else
			{
				result = this.core.ProcessBlock(bigInteger);
			}
			return this.core.ConvertOutput(result);
		}
	}
}
