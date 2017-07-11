using Org.BouncyCastle.Math;
using System;

namespace Org.BouncyCastle.Crypto.Parameters
{
	public class NaccacheSternKeyParameters : AsymmetricKeyParameter
	{
		private readonly BigInteger g;

		private readonly BigInteger n;

		private readonly int lowerSigmaBound;

		public BigInteger G
		{
			get
			{
				return this.g;
			}
		}

		public int LowerSigmaBound
		{
			get
			{
				return this.lowerSigmaBound;
			}
		}

		public BigInteger Modulus
		{
			get
			{
				return this.n;
			}
		}

		public NaccacheSternKeyParameters(bool privateKey, BigInteger g, BigInteger n, int lowerSigmaBound) : base(privateKey)
		{
			this.g = g;
			this.n = n;
			this.lowerSigmaBound = lowerSigmaBound;
		}
	}
}
