using System;

namespace Org.BouncyCastle.Crypto.Digests
{
	public class ShortenedDigest : IDigest
	{
		private IDigest baseDigest;

		private int length;

		public string AlgorithmName
		{
			get
			{
				return string.Concat(new object[]
				{
					this.baseDigest.AlgorithmName,
					"(",
					this.length * 8,
					")"
				});
			}
		}

		public ShortenedDigest(IDigest baseDigest, int length)
		{
			if (baseDigest == null)
			{
				throw new ArgumentNullException("baseDigest");
			}
			if (length > baseDigest.GetDigestSize())
			{
				throw new ArgumentException("baseDigest output not large enough to support length");
			}
			this.baseDigest = baseDigest;
			this.length = length;
		}

		public int GetDigestSize()
		{
			return this.length;
		}

		public void Update(byte input)
		{
			this.baseDigest.Update(input);
		}

		public void BlockUpdate(byte[] input, int inOff, int length)
		{
			this.baseDigest.BlockUpdate(input, inOff, length);
		}

		public int DoFinal(byte[] output, int outOff)
		{
			byte[] array = new byte[this.baseDigest.GetDigestSize()];
			this.baseDigest.DoFinal(array, 0);
			Array.Copy(array, 0, output, outOff, this.length);
			return this.length;
		}

		public void Reset()
		{
			this.baseDigest.Reset();
		}

		public int GetByteLength()
		{
			return this.baseDigest.GetByteLength();
		}
	}
}
