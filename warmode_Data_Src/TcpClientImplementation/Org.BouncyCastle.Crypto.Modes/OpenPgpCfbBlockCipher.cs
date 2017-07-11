using Org.BouncyCastle.Crypto.Parameters;
using System;

namespace Org.BouncyCastle.Crypto.Modes
{
	public class OpenPgpCfbBlockCipher : IBlockCipher
	{
		private byte[] IV;

		private byte[] FR;

		private byte[] FRE;

		private readonly IBlockCipher cipher;

		private readonly int blockSize;

		private int count;

		private bool forEncryption;

		public string AlgorithmName
		{
			get
			{
				return this.cipher.AlgorithmName + "/OpenPGPCFB";
			}
		}

		public bool IsPartialBlockOkay
		{
			get
			{
				return true;
			}
		}

		public OpenPgpCfbBlockCipher(IBlockCipher cipher)
		{
			this.cipher = cipher;
			this.blockSize = cipher.GetBlockSize();
			this.IV = new byte[this.blockSize];
			this.FR = new byte[this.blockSize];
			this.FRE = new byte[this.blockSize];
		}

		public IBlockCipher GetUnderlyingCipher()
		{
			return this.cipher;
		}

		public int GetBlockSize()
		{
			return this.cipher.GetBlockSize();
		}

		public int ProcessBlock(byte[] input, int inOff, byte[] output, int outOff)
		{
			if (!this.forEncryption)
			{
				return this.DecryptBlock(input, inOff, output, outOff);
			}
			return this.EncryptBlock(input, inOff, output, outOff);
		}

		public void Reset()
		{
			this.count = 0;
			Array.Copy(this.IV, 0, this.FR, 0, this.FR.Length);
			this.cipher.Reset();
		}

		public void Init(bool forEncryption, ICipherParameters parameters)
		{
			this.forEncryption = forEncryption;
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
			this.cipher.Init(true, parameters);
		}

		private byte EncryptByte(byte data, int blockOff)
		{
			return this.FRE[blockOff] ^ data;
		}

		private int EncryptBlock(byte[] input, int inOff, byte[] outBytes, int outOff)
		{
			if (inOff + this.blockSize > input.Length)
			{
				throw new DataLengthException("input buffer too short");
			}
			if (outOff + this.blockSize > outBytes.Length)
			{
				throw new DataLengthException("output buffer too short");
			}
			if (this.count > this.blockSize)
			{
				this.FR[this.blockSize - 2] = (outBytes[outOff] = this.EncryptByte(input[inOff], this.blockSize - 2));
				this.FR[this.blockSize - 1] = (outBytes[outOff + 1] = this.EncryptByte(input[inOff + 1], this.blockSize - 1));
				this.cipher.ProcessBlock(this.FR, 0, this.FRE, 0);
				for (int i = 2; i < this.blockSize; i++)
				{
					this.FR[i - 2] = (outBytes[outOff + i] = this.EncryptByte(input[inOff + i], i - 2));
				}
			}
			else if (this.count == 0)
			{
				this.cipher.ProcessBlock(this.FR, 0, this.FRE, 0);
				for (int j = 0; j < this.blockSize; j++)
				{
					this.FR[j] = (outBytes[outOff + j] = this.EncryptByte(input[inOff + j], j));
				}
				this.count += this.blockSize;
			}
			else if (this.count == this.blockSize)
			{
				this.cipher.ProcessBlock(this.FR, 0, this.FRE, 0);
				outBytes[outOff] = this.EncryptByte(input[inOff], 0);
				outBytes[outOff + 1] = this.EncryptByte(input[inOff + 1], 1);
				Array.Copy(this.FR, 2, this.FR, 0, this.blockSize - 2);
				Array.Copy(outBytes, outOff, this.FR, this.blockSize - 2, 2);
				this.cipher.ProcessBlock(this.FR, 0, this.FRE, 0);
				for (int k = 2; k < this.blockSize; k++)
				{
					this.FR[k - 2] = (outBytes[outOff + k] = this.EncryptByte(input[inOff + k], k - 2));
				}
				this.count += this.blockSize;
			}
			return this.blockSize;
		}

		private int DecryptBlock(byte[] input, int inOff, byte[] outBytes, int outOff)
		{
			if (inOff + this.blockSize > input.Length)
			{
				throw new DataLengthException("input buffer too short");
			}
			if (outOff + this.blockSize > outBytes.Length)
			{
				throw new DataLengthException("output buffer too short");
			}
			if (this.count > this.blockSize)
			{
				byte b = input[inOff];
				this.FR[this.blockSize - 2] = b;
				outBytes[outOff] = this.EncryptByte(b, this.blockSize - 2);
				b = input[inOff + 1];
				this.FR[this.blockSize - 1] = b;
				outBytes[outOff + 1] = this.EncryptByte(b, this.blockSize - 1);
				this.cipher.ProcessBlock(this.FR, 0, this.FRE, 0);
				for (int i = 2; i < this.blockSize; i++)
				{
					b = input[inOff + i];
					this.FR[i - 2] = b;
					outBytes[outOff + i] = this.EncryptByte(b, i - 2);
				}
			}
			else if (this.count == 0)
			{
				this.cipher.ProcessBlock(this.FR, 0, this.FRE, 0);
				for (int j = 0; j < this.blockSize; j++)
				{
					this.FR[j] = input[inOff + j];
					outBytes[j] = this.EncryptByte(input[inOff + j], j);
				}
				this.count += this.blockSize;
			}
			else if (this.count == this.blockSize)
			{
				this.cipher.ProcessBlock(this.FR, 0, this.FRE, 0);
				byte b2 = input[inOff];
				byte b3 = input[inOff + 1];
				outBytes[outOff] = this.EncryptByte(b2, 0);
				outBytes[outOff + 1] = this.EncryptByte(b3, 1);
				Array.Copy(this.FR, 2, this.FR, 0, this.blockSize - 2);
				this.FR[this.blockSize - 2] = b2;
				this.FR[this.blockSize - 1] = b3;
				this.cipher.ProcessBlock(this.FR, 0, this.FRE, 0);
				for (int k = 2; k < this.blockSize; k++)
				{
					byte b4 = input[inOff + k];
					this.FR[k - 2] = b4;
					outBytes[outOff + k] = this.EncryptByte(b4, k - 2);
				}
				this.count += this.blockSize;
			}
			return this.blockSize;
		}
	}
}
