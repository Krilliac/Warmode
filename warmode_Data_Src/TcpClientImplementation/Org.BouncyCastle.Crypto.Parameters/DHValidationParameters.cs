using Org.BouncyCastle.Utilities;
using System;

namespace Org.BouncyCastle.Crypto.Parameters
{
	public class DHValidationParameters
	{
		private readonly byte[] seed;

		private readonly int counter;

		public int Counter
		{
			get
			{
				return this.counter;
			}
		}

		public DHValidationParameters(byte[] seed, int counter)
		{
			if (seed == null)
			{
				throw new ArgumentNullException("seed");
			}
			this.seed = (byte[])seed.Clone();
			this.counter = counter;
		}

		public byte[] GetSeed()
		{
			return (byte[])this.seed.Clone();
		}

		public override bool Equals(object obj)
		{
			if (obj == this)
			{
				return true;
			}
			DHValidationParameters dHValidationParameters = obj as DHValidationParameters;
			return dHValidationParameters != null && this.Equals(dHValidationParameters);
		}

		protected bool Equals(DHValidationParameters other)
		{
			return this.counter == other.counter && Arrays.AreEqual(this.seed, other.seed);
		}

		public override int GetHashCode()
		{
			return this.counter.GetHashCode() ^ Arrays.GetHashCode(this.seed);
		}
	}
}
