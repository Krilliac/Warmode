using Org.BouncyCastle.Asn1;
using Org.BouncyCastle.Math;
using System;

namespace Org.BouncyCastle.Crypto.Parameters
{
	public class DHPrivateKeyParameters : DHKeyParameters
	{
		private readonly BigInteger x;

		public BigInteger X
		{
			get
			{
				return this.x;
			}
		}

		public DHPrivateKeyParameters(BigInteger x, DHParameters parameters) : base(true, parameters)
		{
			this.x = x;
		}

		public DHPrivateKeyParameters(BigInteger x, DHParameters parameters, DerObjectIdentifier algorithmOid) : base(true, parameters, algorithmOid)
		{
			this.x = x;
		}

		public override bool Equals(object obj)
		{
			if (obj == this)
			{
				return true;
			}
			DHPrivateKeyParameters dHPrivateKeyParameters = obj as DHPrivateKeyParameters;
			return dHPrivateKeyParameters != null && this.Equals(dHPrivateKeyParameters);
		}

		protected bool Equals(DHPrivateKeyParameters other)
		{
			return this.x.Equals(other.x) && base.Equals(other);
		}

		public override int GetHashCode()
		{
			return this.x.GetHashCode() ^ base.GetHashCode();
		}
	}
}
