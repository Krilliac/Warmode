using Org.BouncyCastle.Math;
using Org.BouncyCastle.Security;
using System;

namespace Org.BouncyCastle.Crypto.Signers
{
	public class RandomDsaKCalculator : IDsaKCalculator
	{
		private BigInteger q;

		private SecureRandom random;

		public virtual bool IsDeterministic
		{
			get
			{
				return false;
			}
		}

		public virtual void Init(BigInteger n, SecureRandom random)
		{
			this.q = n;
			this.random = random;
		}

		public virtual void Init(BigInteger n, BigInteger d, byte[] message)
		{
			throw new InvalidOperationException("Operation not supported");
		}

		public virtual BigInteger NextK()
		{
			int bitLength = this.q.BitLength;
			BigInteger bigInteger;
			do
			{
				bigInteger = new BigInteger(bitLength, this.random);
			}
			while (bigInteger.SignValue < 1 || bigInteger.CompareTo(this.q) >= 0);
			return bigInteger;
		}
	}
}
