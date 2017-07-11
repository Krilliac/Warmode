using Org.BouncyCastle.Math;
using Org.BouncyCastle.Security;
using System;

namespace Org.BouncyCastle.Crypto.Parameters
{
	public class RsaKeyGenerationParameters : KeyGenerationParameters
	{
		private readonly BigInteger publicExponent;

		private readonly int certainty;

		public BigInteger PublicExponent
		{
			get
			{
				return this.publicExponent;
			}
		}

		public int Certainty
		{
			get
			{
				return this.certainty;
			}
		}

		public RsaKeyGenerationParameters(BigInteger publicExponent, SecureRandom random, int strength, int certainty) : base(random, strength)
		{
			this.publicExponent = publicExponent;
			this.certainty = certainty;
		}

		public override bool Equals(object obj)
		{
			RsaKeyGenerationParameters rsaKeyGenerationParameters = obj as RsaKeyGenerationParameters;
			return rsaKeyGenerationParameters != null && this.certainty == rsaKeyGenerationParameters.certainty && this.publicExponent.Equals(rsaKeyGenerationParameters.publicExponent);
		}

		public override int GetHashCode()
		{
			return this.certainty.GetHashCode() ^ this.publicExponent.GetHashCode();
		}
	}
}
