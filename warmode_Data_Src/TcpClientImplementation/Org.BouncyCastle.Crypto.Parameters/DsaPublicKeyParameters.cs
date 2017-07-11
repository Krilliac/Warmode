using Org.BouncyCastle.Math;
using System;

namespace Org.BouncyCastle.Crypto.Parameters
{
	public class DsaPublicKeyParameters : DsaKeyParameters
	{
		private readonly BigInteger y;

		public BigInteger Y
		{
			get
			{
				return this.y;
			}
		}

		public DsaPublicKeyParameters(BigInteger y, DsaParameters parameters) : base(false, parameters)
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
			DsaPublicKeyParameters dsaPublicKeyParameters = obj as DsaPublicKeyParameters;
			return dsaPublicKeyParameters != null && this.Equals(dsaPublicKeyParameters);
		}

		protected bool Equals(DsaPublicKeyParameters other)
		{
			return this.y.Equals(other.y) && base.Equals(other);
		}

		public override int GetHashCode()
		{
			return this.y.GetHashCode() ^ base.GetHashCode();
		}
	}
}
