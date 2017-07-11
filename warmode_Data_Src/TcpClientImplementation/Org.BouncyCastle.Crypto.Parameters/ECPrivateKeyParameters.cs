using Org.BouncyCastle.Asn1;
using Org.BouncyCastle.Math;
using System;

namespace Org.BouncyCastle.Crypto.Parameters
{
	public class ECPrivateKeyParameters : ECKeyParameters
	{
		private readonly BigInteger d;

		public BigInteger D
		{
			get
			{
				return this.d;
			}
		}

		public ECPrivateKeyParameters(BigInteger d, ECDomainParameters parameters) : this("EC", d, parameters)
		{
		}

		[Obsolete("Use version with explicit 'algorithm' parameter")]
		public ECPrivateKeyParameters(BigInteger d, DerObjectIdentifier publicKeyParamSet) : base("ECGOST3410", true, publicKeyParamSet)
		{
			if (d == null)
			{
				throw new ArgumentNullException("d");
			}
			this.d = d;
		}

		public ECPrivateKeyParameters(string algorithm, BigInteger d, ECDomainParameters parameters) : base(algorithm, true, parameters)
		{
			if (d == null)
			{
				throw new ArgumentNullException("d");
			}
			this.d = d;
		}

		public ECPrivateKeyParameters(string algorithm, BigInteger d, DerObjectIdentifier publicKeyParamSet) : base(algorithm, true, publicKeyParamSet)
		{
			if (d == null)
			{
				throw new ArgumentNullException("d");
			}
			this.d = d;
		}

		public override bool Equals(object obj)
		{
			if (obj == this)
			{
				return true;
			}
			ECPrivateKeyParameters eCPrivateKeyParameters = obj as ECPrivateKeyParameters;
			return eCPrivateKeyParameters != null && this.Equals(eCPrivateKeyParameters);
		}

		protected bool Equals(ECPrivateKeyParameters other)
		{
			return this.d.Equals(other.d) && base.Equals(other);
		}

		public override int GetHashCode()
		{
			return this.d.GetHashCode() ^ base.GetHashCode();
		}
	}
}
