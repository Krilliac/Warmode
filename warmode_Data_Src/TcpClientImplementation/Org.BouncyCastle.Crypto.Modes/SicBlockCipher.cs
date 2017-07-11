using Org.BouncyCastle.Crypto.Parameters;
using System;

namespace Org.BouncyCastle.Crypto.Modes
{
	public class SicBlockCipher : IBlockCipher
	{
		private readonly IBlockCipher cipher;

		private readonly int blockSize;

		private readonly byte[] IV;

		private readonly byte[] counter;

		private readonly byte[] counterOut;

		public string AlgorithmName
		{
			get
			{
				return this.cipher.AlgorithmName + "/SIC";
			}
		}

		public bool IsPartialBlockOkay
		{
			get
			{
				return true;
			}
		}

		public SicBlockCipher(IBlockCipher cipher)
		{
			this.cipher = cipher;
			this.blockSize = cipher.GetBlockSize();
			this.IV = new byte[this.blockSize];
			this.counter = new byte[this.blockSize];
			this.counterOut = new byte[this.blockSize];
		}

		public IBlockCipher GetUnderlyingCipher()
		{
			return this.cipher;
		}

		public void Init(bool forEncryption, ICipherParameters parameters)
		{
			if (!(parameters is ParametersWithIV))
			{
				throw new ArgumentException("SIC mode requires ParametersWithIV", "parameters");
			}
			ParametersWithIV parametersWithIV = (ParametersWithIV)parameters;
			byte[] iV = parametersWithIV.GetIV();
			Array.Copy(iV, 0, this.IV, 0, this.IV.Length);
			this.Reset();
			if (parametersWithIV.Parameters != null)
			{
				this.cipher.Init(true, parametersWithIV.Parameters);
				return;
			}
		}

		public int GetBlockSize()
		{
			return this.cipher.GetBlockSize();
		}

		public int ProcessBlock(byte[] input, int inOff, byte[] output, int outOff)
		{
			this.cipher.ProcessBlock(this.counter, 0, this.counterOut, 0);
			for (int i = 0; i < this.counterOut.Length; i++)
			{
				output[outOff + i] = (this.counterOut[i] ^ input[inOff + i]);
			}
			int num = this.counter.Length;
			while (--num >= 0)
			{
				byte[] expr_5F_cp_0 = this.counter;
				int expr_5F_cp_1 = num;
				if ((expr_5F_cp_0[expr_5F_cp_1] += 1) != 0)
				{
					break;
				}
			}
			return this.counter.Length;
		}

		public void Reset()
		{
			Array.Copy(this.IV, 0, this.counter, 0, this.counter.Length);
			this.cipher.Reset();
		}
	}
}
