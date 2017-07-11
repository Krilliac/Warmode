using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Utilities;
using System;

namespace Org.BouncyCastle.Crypto.Digests
{
	public class SkeinDigest : IDigest, IMemoable
	{
		public const int SKEIN_256 = 256;

		public const int SKEIN_512 = 512;

		public const int SKEIN_1024 = 1024;

		private readonly SkeinEngine engine;

		public string AlgorithmName
		{
			get
			{
				return string.Concat(new object[]
				{
					"Skein-",
					this.engine.BlockSize * 8,
					"-",
					this.engine.OutputSize * 8
				});
			}
		}

		public SkeinDigest(int stateSizeBits, int digestSizeBits)
		{
			this.engine = new SkeinEngine(stateSizeBits, digestSizeBits);
			this.Init(null);
		}

		public SkeinDigest(SkeinDigest digest)
		{
			this.engine = new SkeinEngine(digest.engine);
		}

		public void Reset(IMemoable other)
		{
			SkeinDigest skeinDigest = (SkeinDigest)other;
			this.engine.Reset(skeinDigest.engine);
		}

		public IMemoable Copy()
		{
			return new SkeinDigest(this);
		}

		public int GetDigestSize()
		{
			return this.engine.OutputSize;
		}

		public int GetByteLength()
		{
			return this.engine.BlockSize;
		}

		public void Init(SkeinParameters parameters)
		{
			this.engine.Init(parameters);
		}

		public void Reset()
		{
			this.engine.Reset();
		}

		public void Update(byte inByte)
		{
			this.engine.Update(inByte);
		}

		public void BlockUpdate(byte[] inBytes, int inOff, int len)
		{
			this.engine.Update(inBytes, inOff, len);
		}

		public int DoFinal(byte[] outBytes, int outOff)
		{
			return this.engine.DoFinal(outBytes, outOff);
		}
	}
}
