using Org.BouncyCastle.Math;
using System;

namespace Org.BouncyCastle.Crypto.Parameters
{
	public class DsaPrivateKeyParameters : DsaKeyParameters
	{
		private readonly BigInteger x;

		public BigInteger X
		{
			get
			{
				return this.x;
			}
		}

		public DsaPrivateKeyParameters(BigInteger x, DsaParameters parameters) : base(true, parameters)
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
			DsaPrivateKeyParameters dsaPrivateKeyParameters = obj as DsaPrivateKeyParameters;
			return dsaPrivateKeyParameters != null && this.Equals(dsaPrivateKeyParameters);
		}

		protected bool Equals(DsaPrivateKeyParameters other)
		{
			return this.x.Equals(other.x) && base.Equals(other);
		}

		public override int GetHashCode()
		{
			return this.x.GetHashCode() ^ base.GetHashCode();
		}
	}
}
