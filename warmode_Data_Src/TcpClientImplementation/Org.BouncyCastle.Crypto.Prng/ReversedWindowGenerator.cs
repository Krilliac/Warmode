using System;

namespace Org.BouncyCastle.Crypto.Prng
{
	public class ReversedWindowGenerator : IRandomGenerator
	{
		private readonly IRandomGenerator generator;

		private byte[] window;

		private int windowCount;

		public ReversedWindowGenerator(IRandomGenerator generator, int windowSize)
		{
			if (generator == null)
			{
				throw new ArgumentNullException("generator");
			}
			if (windowSize < 2)
			{
				throw new ArgumentException("Window size must be at least 2", "windowSize");
			}
			this.generator = generator;
			this.window = new byte[windowSize];
		}

		public virtual void AddSeedMaterial(byte[] seed)
		{
			lock (this)
			{
				this.windowCount = 0;
				this.generator.AddSeedMaterial(seed);
			}
		}

		public virtual void AddSeedMaterial(long seed)
		{
			lock (this)
			{
				this.windowCount = 0;
				this.generator.AddSeedMaterial(seed);
			}
		}

		public virtual void NextBytes(byte[] bytes)
		{
			this.doNextBytes(bytes, 0, bytes.Length);
		}

		public virtual void NextBytes(byte[] bytes, int start, int len)
		{
			this.doNextBytes(bytes, start, len);
		}

		private void doNextBytes(byte[] bytes, int start, int len)
		{
			lock (this)
			{
				int i = 0;
				while (i < len)
				{
					if (this.windowCount < 1)
					{
						this.generator.NextBytes(this.window, 0, this.window.Length);
						this.windowCount = this.window.Length;
					}
					bytes[start + i++] = this.window[--this.windowCount];
				}
			}
		}
	}
}
