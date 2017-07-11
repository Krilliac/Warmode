using Org.BouncyCastle.Math;
using System;

namespace Org.BouncyCastle.Crypto.Parameters
{
	public class ElGamalPublicKeyParameters : ElGamalKeyParameters
	{
		private readonly BigInteger y;

		public BigInteger Y
		{
			get
			{
				return this.y;
			}
		}

		public ElGamalPublicKeyParameters(BigInteger y, ElGamalParameters parameters) : base(false, parameters)
		{
			if (y == null)
			{
				throw new ArgumentNullException("y");
			}
			this.y = y;
		}

		public override bool Equals(object obj)
		{
			if (obj == this)
			{
				return true;
			}
			ElGamalPublicKeyParameters elGamalPublicKeyParameters = obj as ElGamalPublicKeyParameters;
			return elGamalPublicKeyParameters != null && this.Equals(elGamalPublicKeyParameters);
		}

		protected bool Equals(ElGamalPublicKeyParameters other)
		{
			return this.y.Equals(other.y) && base.Equals(other);
		}

		public override int GetHashCode()
		{
			return this.y.GetHashCode() ^ base.GetHashCode();
		}
	}
}
