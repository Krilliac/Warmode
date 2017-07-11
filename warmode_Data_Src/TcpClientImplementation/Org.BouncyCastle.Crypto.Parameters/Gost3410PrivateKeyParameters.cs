using Org.BouncyCastle.Asn1;
using Org.BouncyCastle.Math;
using System;

namespace Org.BouncyCastle.Crypto.Parameters
{
	public class Gost3410PrivateKeyParameters : Gost3410KeyParameters
	{
		private readonly BigInteger x;

		public BigInteger X
		{
			get
			{
				return this.x;
			}
		}

		public Gost3410PrivateKeyParameters(BigInteger x, Gost3410Parameters parameters) : base(true, parameters)
		{
			if (x.SignValue < 1 || x.BitLength > 256 || x.CompareTo(base.Parameters.Q) >= 0)
			{
				throw new ArgumentException("Invalid x for GOST3410 private key", "x");
			}
			this.x = x;
		}

		public Gost3410PrivateKeyParameters(BigInteger x, DerObjectIdentifier publicKeyParamSet) : base(true, publicKeyParamSet)
		{
			if (x.SignValue < 1 || x.BitLength > 256 || x.CompareTo(base.Parameters.Q) >= 0)
			{
				throw new ArgumentException("Invalid x for GOST3410 private key", "x");
			}
			this.x = x;
		}
	}
}
