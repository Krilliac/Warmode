using Org.BouncyCastle.Crypto.Parameters;
using System;

namespace Org.BouncyCastle.Crypto.Generators
{
	public class DesKeyGenerator : CipherKeyGenerator
	{
		public DesKeyGenerator()
		{
		}

		internal DesKeyGenerator(int defaultStrength) : base(defaultStrength)
		{
		}

		protected override void engineInit(KeyGenerationParameters parameters)
		{
			base.engineInit(parameters);
			if (this.strength == 0 || this.strength == 7)
			{
				this.strength = 8;
				return;
			}
			if (this.strength != 8)
			{
				throw new ArgumentException("DES key must be " + 64 + " bits long.");
			}
		}

		protected override byte[] engineGenerateKey()
		{
			byte[] array;
			do
			{
				array = this.random.GenerateSeed(8);
				DesParameters.SetOddParity(array);
			}
			while (DesParameters.IsWeakKey(array, 0));
			return array;
		}
	}
}
