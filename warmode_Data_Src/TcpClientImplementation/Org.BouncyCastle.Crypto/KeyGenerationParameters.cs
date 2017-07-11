using Org.BouncyCastle.Security;
using System;

namespace Org.BouncyCastle.Crypto
{
	public class KeyGenerationParameters
	{
		private SecureRandom random;

		private int strength;

		public SecureRandom Random
		{
			get
			{
				return this.random;
			}
		}

		public int Strength
		{
			get
			{
				return this.strength;
			}
		}

		public KeyGenerationParameters(SecureRandom random, int strength)
		{
			if (random == null)
			{
				throw new ArgumentNullException("random");
			}
			if (strength < 1)
			{
				throw new ArgumentException("strength must be a positive value", "strength");
			}
			this.random = random;
			this.strength = strength;
		}
	}
}
