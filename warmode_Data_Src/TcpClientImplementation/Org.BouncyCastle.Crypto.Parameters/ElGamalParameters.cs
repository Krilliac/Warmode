using Org.BouncyCastle.Math;
using System;

namespace Org.BouncyCastle.Crypto.Parameters
{
	public class ElGamalParameters : ICipherParameters
	{
		private readonly BigInteger p;

		private readonly BigInteger g;

		private readonly int l;

		public BigInteger P
		{
			get
			{
				return this.p;
			}
		}

		public BigInteger G
		{
			get
			{
				return this.g;
			}
		}

		public int L
		{
			get
			{
				return this.l;
			}
		}

		public ElGamalParameters(BigInteger p, BigInteger g) : this(p, g, 0)
		{
		}

		public ElGamalParameters(BigInteger p, BigInteger g, int l)
		{
			if (p == null)
			{
				throw new ArgumentNullException("p");
			}
			if (g == null)
			{
				throw new ArgumentNullException("g");
			}
			this.p = p;
			this.g = g;
			this.l = l;
		}

		public override bool Equals(object obj)
		{
			if (obj == this)
			{
				return true;
			}
			ElGamalParameters elGamalParameters = obj as ElGamalParameters;
			return elGamalParameters != null && this.Equals(elGamalParameters);
		}

		protected bool Equals(ElGamalParameters other)
		{
			return this.p.Equals(other.p) && this.g.Equals(other.g) && this.l == other.l;
		}

		public override int GetHashCode()
		{
			return this.p.GetHashCode() ^ this.g.GetHashCode() ^ this.l;
		}
	}
}
