using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Math;
using Org.BouncyCastle.Security;
using System;

namespace Org.BouncyCastle.Crypto.Engines
{
	internal class RsaCoreEngine
	{
		private RsaKeyParameters key;

		private bool forEncryption;

		private int bitSize;

		public virtual void Init(bool forEncryption, ICipherParameters parameters)
		{
			if (parameters is ParametersWithRandom)
			{
				parameters = ((ParametersWithRandom)parameters).Parameters;
			}
			if (!(parameters is RsaKeyParameters))
			{
				throw new InvalidKeyException("Not an RSA key");
			}
			this.key = (RsaKeyParameters)parameters;
			this.forEncryption = forEncryption;
			this.bitSize = this.key.Modulus.BitLength;
		}

		public virtual int GetInputBlockSize()
		{
			if (this.forEncryption)
			{
				return (this.bitSize - 1) / 8;
			}
			return (this.bitSize + 7) / 8;
		}

		public virtual int GetOutputBlockSize()
		{
			if (this.forEncryption)
			{
				return (this.bitSize + 7) / 8;
			}
			return (this.bitSize - 1) / 8;
		}

		public virtual BigInteger ConvertInput(byte[] inBuf, int inOff, int inLen)
		{
			int num = (this.bitSize + 7) / 8;
			if (inLen > num)
			{
				throw new DataLengthException("input too large for RSA cipher.");
			}
			BigInteger bigInteger = new BigInteger(1, inBuf, inOff, inLen);
			if (bigInteger.CompareTo(this.key.Modulus) >= 0)
			{
				throw new DataLengthException("input too large for RSA cipher.");
			}
			return bigInteger;
		}

		public virtual byte[] ConvertOutput(BigInteger result)
		{
			byte[] array = result.ToByteArrayUnsigned();
			if (this.forEncryption)
			{
				int outputBlockSize = this.GetOutputBlockSize();
				if (array.Length < outputBlockSize)
				{
					byte[] array2 = new byte[outputBlockSize];
					array.CopyTo(array2, array2.Length - array.Length);
					array = array2;
				}
			}
			return array;
		}

		public virtual BigInteger ProcessBlock(BigInteger input)
		{
			if (this.key is RsaPrivateCrtKeyParameters)
			{
				RsaPrivateCrtKeyParameters rsaPrivateCrtKeyParameters = (RsaPrivateCrtKeyParameters)this.key;
				BigInteger p = rsaPrivateCrtKeyParameters.P;
				BigInteger q = rsaPrivateCrtKeyParameters.Q;
				BigInteger dP = rsaPrivateCrtKeyParameters.DP;
				BigInteger dQ = rsaPrivateCrtKeyParameters.DQ;
				BigInteger qInv = rsaPrivateCrtKeyParameters.QInv;
				BigInteger bigInteger = input.Remainder(p).ModPow(dP, p);
				BigInteger bigInteger2 = input.Remainder(q).ModPow(dQ, q);
				BigInteger bigInteger3 = bigInteger.Subtract(bigInteger2);
				bigInteger3 = bigInteger3.Multiply(qInv);
				bigInteger3 = bigInteger3.Mod(p);
				BigInteger bigInteger4 = bigInteger3.Multiply(q);
				return bigInteger4.Add(bigInteger2);
			}
			return input.ModPow(this.key.Exponent, this.key.Modulus);
		}
	}
}
