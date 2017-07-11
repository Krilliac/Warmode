using Org.BouncyCastle.Crypto.Parameters;
using System;

namespace Org.BouncyCastle.Crypto.Modes
{
	public class CbcBlockCipher : IBlockCipher
	{
		private byte[] IV;

		private byte[] cbcV;

		private byte[] cbcNextV;

		private int blockSize;

		private IBlockCipher cipher;

		private bool encrypting;

		public string AlgorithmName
		{
			get
			{
				return this.cipher.AlgorithmName + "/CBC";
			}
		}

		public bool IsPartialBlockOkay
		{
			get
			{
				return false;
			}
		}

		public CbcBlockCipher(IBlockCipher cipher)
		{
			this.cipher = cipher;
			this.blockSize = cipher.GetBlockSize();
			this.IV = new byte[this.blockSize];
			this.cbcV = new byte[this.blockSize];
			this.cbcNextV = new byte[this.blockSize];
		}

		public IBlockCipher GetUnderlyingCipher()
		{
			return this.cipher;
		}

		public void Init(bool forEncryption, ICipherParameters parameters)
		{
			bool flag = this.encrypting;
			this.encrypting = forEncryption;
			if (parameters is ParametersWithIV)
			{
				ParametersWithIV parametersWithIV = (ParametersWithIV)parameters;
				byte[] iV = parametersWithIV.GetIV();
				if (iV.Length != this.blockSize)
				{
					throw new ArgumentException("initialisation vector must be the same length as block size");
				}
				Array.Copy(iV, 0, this.IV, 0, iV.Length);
				parameters = parametersWithIV.Parameters;
			}
			this.Reset();
			if (parameters != null)
			{
				this.cipher.Init(this.encrypting, parameters);
				return;
			}
			if (flag != this.encrypting)
			{
				throw new ArgumentException("cannot change encrypting state without providing key.");
			}
		}

		public int GetBlockSize()
		{
			return this.cipher.GetBlockSize();
		}

		public int ProcessBlock(byte[] input, int inOff, byte[] output, int outOff)
		{
			if (!this.encrypting)
			{
				return this.DecryptBlock(input, inOff, output, outOff);
			}
			return this.EncryptBlock(input, inOff, output, outOff);
		}

		public void Reset()
		{
			Array.Copy(this.IV, 0, this.cbcV, 0, this.IV.Length);
			Array.Clear(this.cbcNextV, 0, this.cbcNextV.Length);
			this.cipher.Reset();
		}

		private int EncryptBlock(byte[] input, int inOff, byte[] outBytes, int outOff)
		{
			if (inOff + this.blockSize > input.Length)
			{
				throw new DataLengthException("input buffer too short");
			}
			for (int i = 0; i < this.blockSize; i++)
			{
				byte[] expr_28_cp_0 = this.cbcV;
				int expr_28_cp_1 = i;
				expr_28_cp_0[expr_28_cp_1] ^= input[inOff + i];
			}
			int result = this.cipher.ProcessBlock(this.cbcV, 0, outBytes, outOff);
			Array.Copy(outBytes, outOff, this.cbcV, 0, this.cbcV.Length);
			return result;
		}

		private int DecryptBlock(byte[] input, int inOff, byte[] outBytes, int outOff)
		{
			if (inOff + this.blockSize > input.Length)
			{
				throw new DataLengthException("input buffer too short");
			}
			Array.Copy(input, inOff, this.cbcNextV, 0, this.blockSize);
			int result = this.cipher.ProcessBlock(input, inOff, outBytes, outOff);
			for (int i = 0; i < this.blockSize; i++)
			{
				int expr_4B_cp_1 = outOff + i;
				outBytes[expr_4B_cp_1] ^= this.cbcV[i];
			}
			byte[] array = this.cbcV;
			this.cbcV = this.cbcNextV;
			this.cbcNextV = array;
			return result;
		}
	}
}
