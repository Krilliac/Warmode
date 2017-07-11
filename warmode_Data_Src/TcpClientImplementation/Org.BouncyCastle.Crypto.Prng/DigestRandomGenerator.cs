using System;

namespace Org.BouncyCastle.Crypto.Prng
{
	public class DigestRandomGenerator : IRandomGenerator
	{
		private const long CYCLE_COUNT = 10L;

		private long stateCounter;

		private long seedCounter;

		private IDigest digest;

		private byte[] state;

		private byte[] seed;

		public DigestRandomGenerator(IDigest digest)
		{
			this.digest = digest;
			this.seed = new byte[digest.GetDigestSize()];
			this.seedCounter = 1L;
			this.state = new byte[digest.GetDigestSize()];
			this.stateCounter = 1L;
		}

		public void AddSeedMaterial(byte[] inSeed)
		{
			lock (this)
			{
				this.DigestUpdate(inSeed);
				this.DigestUpdate(this.seed);
				this.DigestDoFinal(this.seed);
			}
		}

		public void AddSeedMaterial(long rSeed)
		{
			lock (this)
			{
				this.DigestAddCounter(rSeed);
				this.DigestUpdate(this.seed);
				this.DigestDoFinal(this.seed);
			}
		}

		public void NextBytes(byte[] bytes)
		{
			this.NextBytes(bytes, 0, bytes.Length);
		}

		public void NextBytes(byte[] bytes, int start, int len)
		{
			lock (this)
			{
				int num = 0;
				this.GenerateState();
				int num2 = start + len;
				for (int i = start; i < num2; i++)
				{
					if (num == this.state.Length)
					{
						this.GenerateState();
						num = 0;
					}
					bytes[i] = this.state[num++];
				}
			}
		}

		private void CycleSeed()
		{
			this.DigestUpdate(this.seed);
			long seedVal;
			this.seedCounter = (seedVal = this.seedCounter) + 1L;
			this.DigestAddCounter(seedVal);
			this.DigestDoFinal(this.seed);
		}

		private void GenerateState()
		{
			long seedVal;
			this.stateCounter = (seedVal = this.stateCounter) + 1L;
			this.DigestAddCounter(seedVal);
			this.DigestUpdate(this.state);
			this.DigestUpdate(this.seed);
			this.DigestDoFinal(this.state);
			if (this.stateCounter % 10L == 0L)
			{
				this.CycleSeed();
			}
		}

		private void DigestAddCounter(long seedVal)
		{
			ulong num = (ulong)seedVal;
			for (int num2 = 0; num2 != 8; num2++)
			{
				this.digest.Update((byte)num);
				num >>= 8;
			}
		}

		private void DigestUpdate(byte[] inSeed)
		{
			this.digest.BlockUpdate(inSeed, 0, inSeed.Length);
		}

		private void DigestDoFinal(byte[] result)
		{
			this.digest.DoFinal(result, 0);
		}
	}
}
