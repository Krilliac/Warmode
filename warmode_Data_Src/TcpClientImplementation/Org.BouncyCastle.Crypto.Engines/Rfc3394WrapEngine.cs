using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Utilities;
using System;

namespace Org.BouncyCastle.Crypto.Engines
{
	public class Rfc3394WrapEngine : IWrapper
	{
		private readonly IBlockCipher engine;

		private KeyParameter param;

		private bool forWrapping;

		private byte[] iv = new byte[]
		{
			166,
			166,
			166,
			166,
			166,
			166,
			166,
			166
		};

		public virtual string AlgorithmName
		{
			get
			{
				return this.engine.AlgorithmName;
			}
		}

		public Rfc3394WrapEngine(IBlockCipher engine)
		{
			this.engine = engine;
		}

		public virtual void Init(bool forWrapping, ICipherParameters parameters)
		{
			this.forWrapping = forWrapping;
			if (parameters is ParametersWithRandom)
			{
				parameters = ((ParametersWithRandom)parameters).Parameters;
			}
			if (parameters is KeyParameter)
			{
				this.param = (KeyParameter)parameters;
				return;
			}
			if (parameters is ParametersWithIV)
			{
				ParametersWithIV parametersWithIV = (ParametersWithIV)parameters;
				byte[] iV = parametersWithIV.GetIV();
				if (iV.Length != 8)
				{
					throw new ArgumentException("IV length not equal to 8", "parameters");
				}
				this.iv = iV;
				this.param = (KeyParameter)parametersWithIV.Parameters;
			}
		}

		public virtual byte[] Wrap(byte[] input, int inOff, int inLen)
		{
			if (!this.forWrapping)
			{
				throw new InvalidOperationException("not set for wrapping");
			}
			int num = inLen / 8;
			if (num * 8 != inLen)
			{
				throw new DataLengthException("wrap data must be a multiple of 8 bytes");
			}
			byte[] array = new byte[inLen + this.iv.Length];
			byte[] array2 = new byte[8 + this.iv.Length];
			Array.Copy(this.iv, 0, array, 0, this.iv.Length);
			Array.Copy(input, inOff, array, this.iv.Length, inLen);
			this.engine.Init(true, this.param);
			for (int num2 = 0; num2 != 6; num2++)
			{
				for (int i = 1; i <= num; i++)
				{
					Array.Copy(array, 0, array2, 0, this.iv.Length);
					Array.Copy(array, 8 * i, array2, this.iv.Length, 8);
					this.engine.ProcessBlock(array2, 0, array2, 0);
					int num3 = num * num2 + i;
					int num4 = 1;
					while (num3 != 0)
					{
						byte b = (byte)num3;
						byte[] expr_E8_cp_0 = array2;
						int expr_E8_cp_1 = this.iv.Length - num4;
						expr_E8_cp_0[expr_E8_cp_1] ^= b;
						num3 = (int)((uint)num3 >> 8);
						num4++;
					}
					Array.Copy(array2, 0, array, 0, 8);
					Array.Copy(array2, 8, array, 8 * i, 8);
				}
			}
			return array;
		}

		public virtual byte[] Unwrap(byte[] input, int inOff, int inLen)
		{
			if (this.forWrapping)
			{
				throw new InvalidOperationException("not set for unwrapping");
			}
			int num = inLen / 8;
			if (num * 8 != inLen)
			{
				throw new InvalidCipherTextException("unwrap data must be a multiple of 8 bytes");
			}
			byte[] array = new byte[inLen - this.iv.Length];
			byte[] array2 = new byte[this.iv.Length];
			byte[] array3 = new byte[8 + this.iv.Length];
			Array.Copy(input, inOff, array2, 0, this.iv.Length);
			Array.Copy(input, inOff + this.iv.Length, array, 0, inLen - this.iv.Length);
			this.engine.Init(false, this.param);
			num--;
			for (int i = 5; i >= 0; i--)
			{
				for (int j = num; j >= 1; j--)
				{
					Array.Copy(array2, 0, array3, 0, this.iv.Length);
					Array.Copy(array, 8 * (j - 1), array3, this.iv.Length, 8);
					int num2 = num * i + j;
					int num3 = 1;
					while (num2 != 0)
					{
						byte b = (byte)num2;
						byte[] expr_F4_cp_0 = array3;
						int expr_F4_cp_1 = this.iv.Length - num3;
						expr_F4_cp_0[expr_F4_cp_1] ^= b;
						num2 = (int)((uint)num2 >> 8);
						num3++;
					}
					this.engine.ProcessBlock(array3, 0, array3, 0);
					Array.Copy(array3, 0, array2, 0, 8);
					Array.Copy(array3, 8, array, 8 * (j - 1), 8);
				}
			}
			if (!Arrays.ConstantTimeAreEqual(array2, this.iv))
			{
				throw new InvalidCipherTextException("checksum failed");
			}
			return array;
		}
	}
}
