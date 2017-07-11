using Org.BouncyCastle.Crypto.Parameters;
using System;

namespace Org.BouncyCastle.Crypto.Generators
{
	public class DesEdeKeyGenerator : DesKeyGenerator
	{
		public DesEdeKeyGenerator()
		{
		}

		internal DesEdeKeyGenerator(int defaultStrength) : base(defaultStrength)
		{
		}

		protected override void engineInit(KeyGenerationParameters parameters)
		{
			this.random = parameters.Random;
			this.strength = (parameters.Strength + 7) / 8;
			if (this.strength == 0 || this.strength == 21)
			{
				this.strength = 24;
				return;
			}
			if (this.strength == 14)
			{
				this.strength = 16;
				return;
			}
			if (this.strength != 24 && this.strength != 16)
			{
				throw new ArgumentException(string.Concat(new object[]
				{
					"DESede key must be ",
					192,
					" or ",
					128,
					" bits long."
				}));
			}
		}

		protected override byte[] engineGenerateKey()
		{
			byte[] array;
			do
			{
				array = this.random.GenerateSeed(this.strength);
				DesParameters.SetOddParity(array);
			}
			while (DesEdeParameters.IsWeakKey(array, 0, array.Length));
			return array;
		}
	}
}
