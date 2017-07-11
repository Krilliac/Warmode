using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Math;
using System;

namespace Org.BouncyCastle.Crypto.Engines
{
	public class RsaBlindingEngine : IAsymmetricBlockCipher
	{
		private readonly RsaCoreEngine core = new RsaCoreEngine();

		private RsaKeyParameters key;

		private BigInteger blindingFactor;

		private bool forEncryption;

		public virtual string AlgorithmName
		{
			get
			{
				return "RSA";
			}
		}

		public virtual void Init(bool forEncryption, ICipherParameters param)
		{
			RsaBlindingParameters rsaBlindingParameters;
			if (param is ParametersWithRandom)
			{
				ParametersWithRandom parametersWithRandom = (ParametersWithRandom)param;
				rsaBlindingParameters = (RsaBlindingParameters)parametersWithRandom.Parameters;
			}
			else
			{
				rsaBlindingParameters = (RsaBlindingParameters)param;
			}
			this.core.Init(forEncryption, rsaBlindingParameters.PublicKey);
			this.forEncryption = forEncryption;
			this.key = rsaBlindingParameters.PublicKey;
			this.blindingFactor = rsaBlindingParameters.BlindingFactor;
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
			BigInteger bigInteger = this.core.ConvertInput(inBuf, inOff, inLen);
			if (this.forEncryption)
			{
				bigInteger = this.BlindMessage(bigInteger);
			}
			else
			{
				bigInteger = this.UnblindMessage(bigInteger);
			}
			return this.core.ConvertOutput(bigInteger);
		}

		private BigInteger BlindMessage(BigInteger msg)
		{
			BigInteger bigInteger = this.blindingFactor;
			bigInteger = msg.Multiply(bigInteger.ModPow(this.key.Exponent, this.key.Modulus));
			return bigInteger.Mod(this.key.Modulus);
		}

		private BigInteger UnblindMessage(BigInteger blindedMsg)
		{
			BigInteger modulus = this.key.Modulus;
			BigInteger val = this.blindingFactor.ModInverse(modulus);
			BigInteger bigInteger = blindedMsg.Multiply(val);
			return bigInteger.Mod(modulus);
		}
	}
}
