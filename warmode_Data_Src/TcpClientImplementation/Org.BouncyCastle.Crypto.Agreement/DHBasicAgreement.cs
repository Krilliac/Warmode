using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Math;
using System;

namespace Org.BouncyCastle.Crypto.Agreement
{
	public class DHBasicAgreement : IBasicAgreement
	{
		private DHPrivateKeyParameters key;

		private DHParameters dhParams;

		public virtual void Init(ICipherParameters parameters)
		{
			if (parameters is ParametersWithRandom)
			{
				parameters = ((ParametersWithRandom)parameters).Parameters;
			}
			if (!(parameters is DHPrivateKeyParameters))
			{
				throw new ArgumentException("DHEngine expects DHPrivateKeyParameters");
			}
			this.key = (DHPrivateKeyParameters)parameters;
			this.dhParams = this.key.Parameters;
		}

		public virtual int GetFieldSize()
		{
			return (this.key.Parameters.P.BitLength + 7) / 8;
		}

		public virtual BigInteger CalculateAgreement(ICipherParameters pubKey)
		{
			if (this.key == null)
			{
				throw new InvalidOperationException("Agreement algorithm not initialised");
			}
			DHPublicKeyParameters dHPublicKeyParameters = (DHPublicKeyParameters)pubKey;
			if (!dHPublicKeyParameters.Parameters.Equals(this.dhParams))
			{
				throw new ArgumentException("Diffie-Hellman public key has wrong parameters.");
			}
			return dHPublicKeyParameters.Y.ModPow(this.key.X, this.dhParams.P);
		}
	}
}
