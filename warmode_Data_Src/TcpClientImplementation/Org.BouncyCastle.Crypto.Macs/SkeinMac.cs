using Org.BouncyCastle.Crypto.Digests;
using Org.BouncyCastle.Crypto.Parameters;
using System;

namespace Org.BouncyCastle.Crypto.Macs
{
	public class SkeinMac : IMac
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
					"Skein-MAC-",
					this.engine.BlockSize * 8,
					"-",
					this.engine.OutputSize * 8
				});
			}
		}

		public SkeinMac(int stateSizeBits, int digestSizeBits)
		{
			this.engine = new SkeinEngine(stateSizeBits, digestSizeBits);
		}

		public SkeinMac(SkeinMac mac)
		{
			this.engine = new SkeinEngine(mac.engine);
		}

		public void Init(ICipherParameters parameters)
		{
			SkeinParameters skeinParameters;
			if (parameters is SkeinParameters)
			{
				skeinParameters = (SkeinParameters)parameters;
			}
			else
			{
				if (!(parameters is KeyParameter))
				{
					throw new ArgumentException("Invalid parameter passed to Skein MAC init - " + parameters.GetType().Name);
				}
				skeinParameters = new SkeinParameters.Builder().SetKey(((KeyParameter)parameters).GetKey()).Build();
			}
			if (skeinParameters.GetKey() == null)
			{
				throw new ArgumentException("Skein MAC requires a key parameter.");
			}
			this.engine.Init(skeinParameters);
		}

		public int GetMacSize()
		{
			return this.engine.OutputSize;
		}

		public void Reset()
		{
			this.engine.Reset();
		}

		public void Update(byte inByte)
		{
			this.engine.Update(inByte);
		}

		public void BlockUpdate(byte[] input, int inOff, int len)
		{
			this.engine.Update(input, inOff, len);
		}

		public int DoFinal(byte[] output, int outOff)
		{
			return this.engine.DoFinal(output, outOff);
		}
	}
}
