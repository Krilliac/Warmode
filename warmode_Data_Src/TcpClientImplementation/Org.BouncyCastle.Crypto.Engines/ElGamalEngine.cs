using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Math;
using Org.BouncyCastle.Security;
using System;

namespace Org.BouncyCastle.Crypto.Engines
{
	public class ElGamalEngine : IAsymmetricBlockCipher
	{
		private ElGamalKeyParameters key;

		private SecureRandom random;

		private bool forEncryption;

		private int bitSize;

		public virtual string AlgorithmName
		{
			get
			{
				return "ElGamal";
			}
		}

		public virtual void Init(bool forEncryption, ICipherParameters parameters)
		{
			if (parameters is ParametersWithRandom)
			{
				ParametersWithRandom parametersWithRandom = (ParametersWithRandom)parameters;
				this.key = (ElGamalKeyParameters)parametersWithRandom.Parameters;
				this.random = parametersWithRandom.Random;
			}
			else
			{
				this.key = (ElGamalKeyParameters)parameters;
				this.random = new SecureRandom();
			}
			this.forEncryption = forEncryption;
			this.bitSize = this.key.Parameters.P.BitLength;
			if (forEncryption)
			{
				if (!(this.key is ElGamalPublicKeyParameters))
				{
					throw new ArgumentException("ElGamalPublicKeyParameters are required for encryption.");
				}
			}
			else if (!(this.key is ElGamalPrivateKeyParameters))
			{
				throw new ArgumentException("ElGamalPrivateKeyParameters are required for decryption.");
			}
		}

		public virtual int GetInputBlockSize()
		{
			if (this.forEncryption)
			{
				return (this.bitSize - 1) / 8;
			}
			return 2 * ((this.bitSize + 7) / 8);
		}

		public virtual int GetOutputBlockSize()
		{
			if (this.forEncryption)
			{
				return 2 * ((this.bitSize + 7) / 8);
			}
			return (this.bitSize - 1) / 8;
		}

		public virtual byte[] ProcessBlock(byte[] input, int inOff, int length)
		{
			if (this.key == null)
			{
				throw new InvalidOperationException("ElGamal engine not initialised");
			}
			int num = this.forEncryption ? ((this.bitSize - 1 + 7) / 8) : this.GetInputBlockSize();
			if (length > num)
			{
				throw new DataLengthException("input too large for ElGamal cipher.\n");
			}
			BigInteger p = this.key.Parameters.P;
			byte[] array;
			if (this.key is ElGamalPrivateKeyParameters)
			{
				int num2 = length / 2;
				BigInteger bigInteger = new BigInteger(1, input, inOff, num2);
				BigInteger val = new BigInteger(1, input, inOff + num2, num2);
				ElGamalPrivateKeyParameters elGamalPrivateKeyParameters = (ElGamalPrivateKeyParameters)this.key;
				BigInteger bigInteger2 = bigInteger.ModPow(p.Subtract(BigInteger.One).Subtract(elGamalPrivateKeyParameters.X), p).Multiply(val).Mod(p);
				array = bigInteger2.ToByteArrayUnsigned();
			}
			else
			{
				BigInteger bigInteger3 = new BigInteger(1, input, inOff, length);
				if (bigInteger3.BitLength >= p.BitLength)
				{
					throw new DataLengthException("input too large for ElGamal cipher.\n");
				}
				ElGamalPublicKeyParameters elGamalPublicKeyParameters = (ElGamalPublicKeyParameters)this.key;
				BigInteger value = p.Subtract(BigInteger.Two);
				BigInteger bigInteger4;
				do
				{
					bigInteger4 = new BigInteger(p.BitLength, this.random);
				}
				while (bigInteger4.SignValue == 0 || bigInteger4.CompareTo(value) > 0);
				BigInteger g = this.key.Parameters.G;
				BigInteger bigInteger5 = g.ModPow(bigInteger4, p);
				BigInteger bigInteger6 = bigInteger3.Multiply(elGamalPublicKeyParameters.Y.ModPow(bigInteger4, p)).Mod(p);
				array = new byte[this.GetOutputBlockSize()];
				byte[] array2 = bigInteger5.ToByteArrayUnsigned();
				byte[] array3 = bigInteger6.ToByteArrayUnsigned();
				array2.CopyTo(array, array.Length / 2 - array2.Length);
				array3.CopyTo(array, array.Length - array3.Length);
			}
			return array;
		}
	}
}
