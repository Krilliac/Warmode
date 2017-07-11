using Org.BouncyCastle.Math;
using System;

namespace Org.BouncyCastle.Crypto.Parameters
{
	public sealed class Srp6GroupParameters
	{
		private readonly BigInteger n;

		private readonly BigInteger g;

		public BigInteger G
		{
			get
			{
				return this.g;
			}
		}

		public BigInteger N
		{
			get
			{
				return this.n;
			}
		}

		public Srp6GroupParameters(BigInteger N, BigInteger g)
		{
			this.n = N;
			this.g = g;
		}
	}
}
