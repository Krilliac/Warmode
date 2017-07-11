using Org.BouncyCastle.Crypto.Modes;
using Org.BouncyCastle.Crypto.Paddings;
using System;

namespace Org.BouncyCastle.Crypto.Macs
{
	public class CbcBlockCipherMac : IMac
	{
		private byte[] buf;

		private int bufOff;

		private IBlockCipher cipher;

		private IBlockCipherPadding padding;

		private int macSize;

		public string AlgorithmName
		{
			get
			{
				return this.cipher.AlgorithmName;
			}
		}

		public CbcBlockCipherMac(IBlockCipher cipher) : this(cipher, cipher.GetBlockSize() * 8 / 2, null)
		{
		}

		public CbcBlockCipherMac(IBlockCipher cipher, IBlockCipherPadding padding) : this(cipher, cipher.GetBlockSize() * 8 / 2, padding)
		{
		}

		public CbcBlockCipherMac(IBlockCipher cipher, int macSizeInBits) : this(cipher, macSizeInBits, null)
		{
		}

		public CbcBlockCipherMac(IBlockCipher cipher, int macSizeInBits, IBlockCipherPadding padding)
		{
			if (macSizeInBits % 8 != 0)
			{
				throw new ArgumentException("MAC size must be multiple of 8");
			}
			this.cipher = new CbcBlockCipher(cipher);
			this.padding = padding;
			this.macSize = macSizeInBits / 8;
			this.buf = new byte[cipher.GetBlockSize()];
			this.bufOff = 0;
		}

		public void Init(ICipherParameters parameters)
		{
			this.Reset();
			this.cipher.Init(true, parameters);
		}

		public int GetMacSize()
		{
			return this.macSize;
		}

		public void Update(byte input)
		{
			if (this.bufOff == this.buf.Length)
			{
				this.cipher.ProcessBlock(this.buf, 0, this.buf, 0);
				this.bufOff = 0;
			}
			this.buf[this.bufOff++] = input;
		}

		public void BlockUpdate(byte[] input, int inOff, int len)
		{
			if (len < 0)
			{
				throw new ArgumentException("Can't have a negative input length!");
			}
			int blockSize = this.cipher.GetBlockSize();
			int num = blockSize - this.bufOff;
			if (len > num)
			{
				Array.Copy(input, inOff, this.buf, this.bufOff, num);
				this.cipher.ProcessBlock(this.buf, 0, this.buf, 0);
				this.bufOff = 0;
				len -= num;
				inOff += num;
				while (len > blockSize)
				{
					this.cipher.ProcessBlock(input, inOff, this.buf, 0);
					len -= blockSize;
					inOff += blockSize;
				}
			}
			Array.Copy(input, inOff, this.buf, this.bufOff, len);
			this.bufOff += len;
		}

		public int DoFinal(byte[] output, int outOff)
		{
			int blockSize = this.cipher.GetBlockSize();
			if (this.padding == null)
			{
				while (this.bufOff < blockSize)
				{
					this.buf[this.bufOff++] = 0;
				}
			}
			else
			{
				if (this.bufOff == blockSize)
				{
					this.cipher.ProcessBlock(this.buf, 0, this.buf, 0);
					this.bufOff = 0;
				}
				this.padding.AddPadding(this.buf, this.bufOff);
			}
			this.cipher.ProcessBlock(this.buf, 0, this.buf, 0);
			Array.Copy(this.buf, 0, output, outOff, this.macSize);
			this.Reset();
			return this.macSize;
		}

		public void Reset()
		{
			Array.Clear(this.buf, 0, this.buf.Length);
			this.bufOff = 0;
			this.cipher.Reset();
		}
	}
}
