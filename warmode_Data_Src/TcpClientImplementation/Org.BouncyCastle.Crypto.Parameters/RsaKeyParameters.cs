using Org.BouncyCastle.Math;
using System;

namespace Org.BouncyCastle.Crypto.Parameters
{
	public class RsaKeyParameters : AsymmetricKeyParameter
	{
		private readonly BigInteger modulus;

		private readonly BigInteger exponent;

		public BigInteger Modulus
		{
			get
			{
				return this.modulus;
			}
		}

		public BigInteger Exponent
		{
			get
			{
				return this.exponent;
			}
		}

		public RsaKeyParameters(bool isPrivate, BigInteger modulus, BigInteger exponent) : base(isPrivate)
		{
			if (modulus == null)
			{
				throw new ArgumentNullException("modulus");
			}
			if (exponent == null)
			{
				throw new ArgumentNullException("exponent");
			}
			if (modulus.SignValue <= 0)
			{
				throw new ArgumentException("Not a valid RSA modulus", "modulus");
			}
			if (exponent.SignValue <= 0)
			{
				throw new ArgumentException("Not a valid RSA exponent", "exponent");
			}
			this.modulus = modulus;
			this.exponent = exponent;
		}

		public override bool Equals(object obj)
		{
			RsaKeyParameters rsaKeyParameters = obj as RsaKeyParameters;
			return rsaKeyParameters != null && (rsaKeyParameters.IsPrivate == base.IsPrivate && rsaKeyParameters.Modulus.Equals(this.modulus)) && rsaKeyParameters.Exponent.Equals(this.exponent);
		}

		public override int GetHashCode()
		{
			return this.modulus.GetHashCode() ^ this.exponent.GetHashCode() ^ base.IsPrivate.GetHashCode();
		}
	}
}
