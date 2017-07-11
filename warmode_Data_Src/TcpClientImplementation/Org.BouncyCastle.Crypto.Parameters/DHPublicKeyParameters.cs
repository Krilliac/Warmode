using Org.BouncyCastle.Asn1;
using Org.BouncyCastle.Math;
using System;

namespace Org.BouncyCastle.Crypto.Parameters
{
	public class DHPublicKeyParameters : DHKeyParameters
	{
		private readonly BigInteger y;

		public BigInteger Y
		{
			get
			{
				return this.y;
			}
		}

		public DHPublicKeyParameters(BigInteger y, DHParameters parameters) : base(false, parameters)
		{
			if (y == null)
			{
				throw new ArgumentNullException("y");
			}
			this.y = y;
		}

		public DHPublicKeyParameters(BigInteger y, DHParameters parameters, DerObjectIdentifier algorithmOid) : base(false, parameters, algorithmOid)
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
			DHPublicKeyParameters dHPublicKeyParameters = obj as DHPublicKeyParameters;
			return dHPublicKeyParameters != null && this.Equals(dHPublicKeyParameters);
		}

		protected bool Equals(DHPublicKeyParameters other)
		{
			return this.y.Equals(other.y) && base.Equals(other);
		}

		public override int GetHashCode()
		{
			return this.y.GetHashCode() ^ base.GetHashCode();
		}
	}
}
