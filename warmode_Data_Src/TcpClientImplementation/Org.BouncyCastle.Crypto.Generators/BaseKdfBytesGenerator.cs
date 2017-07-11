using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Crypto.Utilities;
using System;

namespace Org.BouncyCastle.Crypto.Generators
{
	public class BaseKdfBytesGenerator : IDerivationFunction
	{
		private int counterStart;

		private IDigest digest;

		private byte[] shared;

		private byte[] iv;

		public virtual IDigest Digest
		{
			get
			{
				return this.digest;
			}
		}

		public BaseKdfBytesGenerator(int counterStart, IDigest digest)
		{
			this.counterStart = counterStart;
			this.digest = digest;
		}

		public virtual void Init(IDerivationParameters parameters)
		{
			if (parameters is KdfParameters)
			{
				KdfParameters kdfParameters = (KdfParameters)parameters;
				this.shared = kdfParameters.GetSharedSecret();
				this.iv = kdfParameters.GetIV();
				return;
			}
			if (parameters is Iso18033KdfParameters)
			{
				Iso18033KdfParameters iso18033KdfParameters = (Iso18033KdfParameters)parameters;
				this.shared = iso18033KdfParameters.GetSeed();
				this.iv = null;
				return;
			}
			throw new ArgumentException("KDF parameters required for KDF Generator");
		}

		public virtual int GenerateBytes(byte[] output, int outOff, int length)
		{
			if (output.Length - length < outOff)
			{
				throw new DataLengthException("output buffer too small");
			}
			long num = (long)length;
			int digestSize = this.digest.GetDigestSize();
			if (num > 8589934591L)
			{
				throw new ArgumentException("Output length too large");
			}
			int num2 = (int)((num + (long)digestSize - 1L) / (long)digestSize);
			byte[] array = new byte[this.digest.GetDigestSize()];
			byte[] array2 = new byte[4];
			Pack.UInt32_To_BE((uint)this.counterStart, array2, 0);
			uint num3 = (uint)(this.counterStart & -256);
			for (int i = 0; i < num2; i++)
			{
				this.digest.BlockUpdate(this.shared, 0, this.shared.Length);
				this.digest.BlockUpdate(array2, 0, 4);
				if (this.iv != null)
				{
					this.digest.BlockUpdate(this.iv, 0, this.iv.Length);
				}
				this.digest.DoFinal(array, 0);
				if (length > digestSize)
				{
					Array.Copy(array, 0, output, outOff, digestSize);
					outOff += digestSize;
					length -= digestSize;
				}
				else
				{
					Array.Copy(array, 0, output, outOff, length);
				}
				byte[] expr_107_cp_0 = array2;
				int expr_107_cp_1 = 3;
				if ((expr_107_cp_0[expr_107_cp_1] += 1) == 0)
				{
					num3 += 256u;
					Pack.UInt32_To_BE(num3, array2, 0);
				}
			}
			this.digest.Reset();
			return (int)num;
		}
	}
}
