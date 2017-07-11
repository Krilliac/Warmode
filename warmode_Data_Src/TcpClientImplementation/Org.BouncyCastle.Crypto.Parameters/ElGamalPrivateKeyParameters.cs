using Org.BouncyCastle.Math;
using System;

namespace Org.BouncyCastle.Crypto.Parameters
{
	public class ElGamalPrivateKeyParameters : ElGamalKeyParameters
	{
		private readonly BigInteger x;

		public BigInteger X
		{
			get
			{
				return this.x;
			}
		}

		public ElGamalPrivateKeyParameters(BigInteger x, ElGamalParameters parameters) : base(true, parameters)
		{
			if (x == null)
			{
				throw new ArgumentNullException("x");
			}
			this.x = x;
		}

		public override bool Equals(object obj)
		{
			if (obj == this)
			{
				return true;
			}
			ElGamalPrivateKeyParameters elGamalPrivateKeyParameters = obj as ElGamalPrivateKeyParameters;
			return elGamalPrivateKeyParameters != null && this.Equals(elGamalPrivateKeyParameters);
		}

		protected bool Equals(ElGamalPrivateKeyParameters other)
		{
			return other.x.Equals(this.x) && base.Equals(other);
		}

		public override int GetHashCode()
		{
			return this.x.GetHashCode() ^ base.GetHashCode();
		}
	}
}
