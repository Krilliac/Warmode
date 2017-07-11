using Org.BouncyCastle.Crypto.Digests;
using Org.BouncyCastle.Crypto.Macs;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Security;
using System;

namespace Org.BouncyCastle.Crypto.Generators
{
	public class Pkcs5S2ParametersGenerator : PbeParametersGenerator
	{
		private readonly IMac hMac;

		private readonly byte[] state;

		public Pkcs5S2ParametersGenerator() : this(new Sha1Digest())
		{
		}

		public Pkcs5S2ParametersGenerator(IDigest digest)
		{
			this.hMac = new HMac(digest);
			this.state = new byte[this.hMac.GetMacSize()];
		}

		private void F(byte[] S, int c, byte[] iBuf, byte[] outBytes, int outOff)
		{
			if (c == 0)
			{
				throw new ArgumentException("iteration count must be at least 1.");
			}
			if (S != null)
			{
				this.hMac.BlockUpdate(S, 0, S.Length);
			}
			this.hMac.BlockUpdate(iBuf, 0, iBuf.Length);
			this.hMac.DoFinal(this.state, 0);
			Array.Copy(this.state, 0, outBytes, outOff, this.state.Length);
			for (int i = 1; i < c; i++)
			{
				this.hMac.BlockUpdate(this.state, 0, this.state.Length);
				this.hMac.DoFinal(this.state, 0);
				for (int j = 0; j < this.state.Length; j++)
				{
					int expr_9C_cp_1 = outOff + j;
					outBytes[expr_9C_cp_1] ^= this.state[j];
				}
			}
		}

		private byte[] GenerateDerivedKey(int dkLen)
		{
			int macSize = this.hMac.GetMacSize();
			int num = (dkLen + macSize - 1) / macSize;
			byte[] array = new byte[4];
			byte[] array2 = new byte[num * macSize];
			int num2 = 0;
			ICipherParameters parameters = new KeyParameter(this.mPassword);
			this.hMac.Init(parameters);
			for (int i = 1; i <= num; i++)
			{
				int num3 = 3;
				while (true)
				{
					byte[] expr_59_cp_0 = array;
					int expr_59_cp_1 = num3;
					if ((expr_59_cp_0[expr_59_cp_1] += 1) != 0)
					{
						break;
					}
					num3--;
				}
				this.F(this.mSalt, this.mIterationCount, array, array2, num2);
				num2 += macSize;
			}
			return array2;
		}

		[Obsolete("Use version with 'algorithm' parameter")]
		public override ICipherParameters GenerateDerivedParameters(int keySize)
		{
			return this.GenerateDerivedMacParameters(keySize);
		}

		public override ICipherParameters GenerateDerivedParameters(string algorithm, int keySize)
		{
			keySize /= 8;
			byte[] keyBytes = this.GenerateDerivedKey(keySize);
			return ParameterUtilities.CreateKeyParameter(algorithm, keyBytes, 0, keySize);
		}

		[Obsolete("Use version with 'algorithm' parameter")]
		public override ICipherParameters GenerateDerivedParameters(int keySize, int ivSize)
		{
			keySize /= 8;
			ivSize /= 8;
			byte[] array = this.GenerateDerivedKey(keySize + ivSize);
			return new ParametersWithIV(new KeyParameter(array, 0, keySize), array, keySize, ivSize);
		}

		public override ICipherParameters GenerateDerivedParameters(string algorithm, int keySize, int ivSize)
		{
			keySize /= 8;
			ivSize /= 8;
			byte[] array = this.GenerateDerivedKey(keySize + ivSize);
			KeyParameter parameters = ParameterUtilities.CreateKeyParameter(algorithm, array, 0, keySize);
			return new ParametersWithIV(parameters, array, keySize, ivSize);
		}

		public override ICipherParameters GenerateDerivedMacParameters(int keySize)
		{
			keySize /= 8;
			byte[] key = this.GenerateDerivedKey(keySize);
			return new KeyParameter(key, 0, keySize);
		}
	}
}
