using Org.BouncyCastle.Crypto.Parameters;
using System;

namespace Org.BouncyCastle.Crypto.Modes
{
	public class OfbBlockCipher : IBlockCipher
	{
		private byte[] IV;

		private byte[] ofbV;

		private byte[] ofbOutV;

		private readonly int blockSize;

		private readonly IBlockCipher cipher;

		public string AlgorithmName
		{
			get
			{
				return this.cipher.AlgorithmName + "/OFB" + this.blockSize * 8;
			}
		}

		public bool IsPartialBlockOkay
		{
			get
			{
				return true;
			}
		}

		public OfbBlockCipher(IBlockCipher cipher, int blockSize)
		{
			this.cipher = cipher;
			this.blockSize = blockSize / 8;
			this.IV = new byte[cipher.GetBlockSize()];
			this.ofbV = new byte[cipher.GetBlockSize()];
			this.ofbOutV = new byte[cipher.GetBlockSize()];
		}

		public IBlockCipher GetUnderlyingCipher()
		{
			return this.cipher;
		}

		public void Init(bool forEncryption, ICipherParameters parameters)
		{
			if (parameters is ParametersWithIV)
			{
				ParametersWithIV parametersWithIV = (ParametersWithIV)parameters;
				byte[] iV = parametersWithIV.GetIV();
				if (iV.Length < this.IV.Length)
				{
					Array.Copy(iV, 0, this.IV, this.IV.Length - iV.Length, iV.Length);
					for (int i = 0; i < this.IV.Length - iV.Length; i++)
					{
						this.IV[i] = 0;
					}
				}
				else
				{
					Array.Copy(iV, 0, this.IV, 0, this.IV.Length);
				}
				parameters = parametersWithIV.Parameters;
			}
			this.Reset();
			if (parameters != null)
			{
				this.cipher.Init(true, parameters);
			}
		}

		public int GetBlockSize()
		{
			return this.blockSize;
		}

		public int ProcessBlock(byte[] input, int inOff, byte[] output, int outOff)
		{
			if (inOff + this.blockSize > input.Length)
			{
				throw new DataLengthException("input buffer too short");
			}
			if (outOff + this.blockSize > output.Length)
			{
				throw new DataLengthException("output buffer too short");
			}
			this.cipher.ProcessBlock(this.ofbV, 0, this.ofbOutV, 0);
			for (int i = 0; i < this.blockSize; i++)
			{
				output[outOff + i] = (this.ofbOutV[i] ^ input[inOff + i]);
			}
			Array.Copy(this.ofbV, this.blockSize, this.ofbV, 0, this.ofbV.Length - this.blockSize);
			Array.Copy(this.ofbOutV, 0, this.ofbV, this.ofbV.Length - this.blockSize, this.blockSize);
			return this.blockSize;
		}

		public void Reset()
		{
			Array.Copy(this.IV, 0, this.ofbV, 0, this.IV.Length);
			this.cipher.Reset();
		}
	}
}
