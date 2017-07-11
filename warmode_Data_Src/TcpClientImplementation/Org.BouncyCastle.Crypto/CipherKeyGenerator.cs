using Org.BouncyCastle.Security;
using System;

namespace Org.BouncyCastle.Crypto
{
	public class CipherKeyGenerator
	{
		protected internal SecureRandom random;

		protected internal int strength;

		private bool uninitialised = true;

		private int defaultStrength;

		public int DefaultStrength
		{
			get
			{
				return this.defaultStrength;
			}
		}

		public CipherKeyGenerator()
		{
		}

		internal CipherKeyGenerator(int defaultStrength)
		{
			if (defaultStrength < 1)
			{
				throw new ArgumentException("strength must be a positive value", "defaultStrength");
			}
			this.defaultStrength = defaultStrength;
		}

		public void Init(KeyGenerationParameters parameters)
		{
			if (parameters == null)
			{
				throw new ArgumentNullException("parameters");
			}
			this.uninitialised = false;
			this.engineInit(parameters);
		}

		protected virtual void engineInit(KeyGenerationParameters parameters)
		{
			this.random = parameters.Random;
			this.strength = (parameters.Strength + 7) / 8;
		}

		public byte[] GenerateKey()
		{
			if (this.uninitialised)
			{
				if (this.defaultStrength < 1)
				{
					throw new InvalidOperationException("Generator has not been initialised");
				}
				this.uninitialised = false;
				this.engineInit(new KeyGenerationParameters(new SecureRandom(), this.defaultStrength));
			}
			return this.engineGenerateKey();
		}

		protected virtual byte[] engineGenerateKey()
		{
			return this.random.GenerateSeed(this.strength);
		}
	}
}
