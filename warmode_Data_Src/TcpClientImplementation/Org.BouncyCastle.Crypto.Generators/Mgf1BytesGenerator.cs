using Org.BouncyCastle.Crypto.Parameters;
using System;

namespace Org.BouncyCastle.Crypto.Generators
{
	public class Mgf1BytesGenerator : IDerivationFunction
	{
		private IDigest digest;

		private byte[] seed;

		private int hLen;

		public IDigest Digest
		{
			get
			{
				return this.digest;
			}
		}

		public Mgf1BytesGenerator(IDigest digest)
		{
			this.digest = digest;
			this.hLen = digest.GetDigestSize();
		}

		public void Init(IDerivationParameters parameters)
		{
			if (!typeof(MgfParameters).IsInstanceOfType(parameters))
			{
				throw new ArgumentException("MGF parameters required for MGF1Generator");
			}
			MgfParameters mgfParameters = (MgfParameters)parameters;
			this.seed = mgfParameters.GetSeed();
		}

		private void ItoOSP(int i, byte[] sp)
		{
			sp[0] = (byte)((uint)i >> 24);
			sp[1] = (byte)((uint)i >> 16);
			sp[2] = (byte)((uint)i >> 8);
			sp[3] = (byte)i;
		}

		public int GenerateBytes(byte[] output, int outOff, int length)
		{
			if (output.Length - length < outOff)
			{
				throw new DataLengthException("output buffer too small");
			}
			byte[] array = new byte[this.hLen];
			byte[] array2 = new byte[4];
			int num = 0;
			this.digest.Reset();
			if (length > this.hLen)
			{
				do
				{
					this.ItoOSP(num, array2);
					this.digest.BlockUpdate(this.seed, 0, this.seed.Length);
					this.digest.BlockUpdate(array2, 0, array2.Length);
					this.digest.DoFinal(array, 0);
					Array.Copy(array, 0, output, outOff + num * this.hLen, this.hLen);
				}
				while (++num < length / this.hLen);
			}
			if (num * this.hLen < length)
			{
				this.ItoOSP(num, array2);
				this.digest.BlockUpdate(this.seed, 0, this.seed.Length);
				this.digest.BlockUpdate(array2, 0, array2.Length);
				this.digest.DoFinal(array, 0);
				Array.Copy(array, 0, output, outOff + num * this.hLen, length - num * this.hLen);
			}
			return length;
		}
	}
}
