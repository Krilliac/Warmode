using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Math;
using Org.BouncyCastle.Math.EC;
using System;

namespace Org.BouncyCastle.Crypto.Agreement
{
	public class ECDHCBasicAgreement : IBasicAgreement
	{
		private ECPrivateKeyParameters key;

		public virtual void Init(ICipherParameters parameters)
		{
			if (parameters is ParametersWithRandom)
			{
				parameters = ((ParametersWithRandom)parameters).Parameters;
			}
			this.key = (ECPrivateKeyParameters)parameters;
		}

		public virtual int GetFieldSize()
		{
			return (this.key.Parameters.Curve.FieldSize + 7) / 8;
		}

		public virtual BigInteger CalculateAgreement(ICipherParameters pubKey)
		{
			ECPublicKeyParameters eCPublicKeyParameters = (ECPublicKeyParameters)pubKey;
			ECDomainParameters parameters = eCPublicKeyParameters.Parameters;
			BigInteger b = parameters.H.Multiply(this.key.D).Mod(parameters.N);
			ECPoint eCPoint = eCPublicKeyParameters.Q.Multiply(b).Normalize();
			if (eCPoint.IsInfinity)
			{
				throw new InvalidOperationException("Infinity is not a valid agreement value for ECDHC");
			}
			return eCPoint.AffineXCoord.ToBigInteger();
		}
	}
}
