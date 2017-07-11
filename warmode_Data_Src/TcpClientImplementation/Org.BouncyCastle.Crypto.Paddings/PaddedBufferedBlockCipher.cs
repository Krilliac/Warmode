using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Security;
using System;

namespace Org.BouncyCastle.Crypto.Paddings
{
	public class PaddedBufferedBlockCipher : BufferedBlockCipher
	{
		private readonly IBlockCipherPadding padding;

		public PaddedBufferedBlockCipher(IBlockCipher cipher, IBlockCipherPadding padding)
		{
			this.cipher = cipher;
			this.padding = padding;
			this.buf = new byte[cipher.GetBlockSize()];
			this.bufOff = 0;
		}

		public PaddedBufferedBlockCipher(IBlockCipher cipher) : this(cipher, new Pkcs7Padding())
		{
		}

		public override void Init(bool forEncryption, ICipherParameters parameters)
		{
			this.forEncryption = forEncryption;
			SecureRandom random = null;
			if (parameters is ParametersWithRandom)
			{
				ParametersWithRandom parametersWithRandom = (ParametersWithRandom)parameters;
				random = parametersWithRandom.Random;
				parameters = parametersWithRandom.Parameters;
			}
			this.Reset();
			this.padding.Init(random);
			this.cipher.Init(forEncryption, parameters);
		}

		public override int GetOutputSize(int length)
		{
			int num = length + this.bufOff;
			int num2 = num % this.buf.Length;
			if (num2 != 0)
			{
				return num - num2 + this.buf.Length;
			}
			if (this.forEncryption)
			{
				return num + this.buf.Length;
			}
			return num;
		}

		public override int GetUpdateOutputSize(int length)
		{
			int num = length + this.bufOff;
			int num2 = num % this.buf.Length;
			if (num2 == 0)
			{
				return num - this.buf.Length;
			}
			return num - num2;
		}

		public override int ProcessByte(byte input, byte[] output, int outOff)
		{
			int result = 0;
			if (this.bufOff == this.buf.Length)
			{
				result = this.cipher.ProcessBlock(this.buf, 0, output, outOff);
				this.bufOff = 0;
			}
			this.buf[this.bufOff++] = input;
			return result;
		}

		public override int ProcessBytes(byte[] input, int inOff, int length, byte[] output, int outOff)
		{
			if (length < 0)
			{
				throw new ArgumentException("Can't have a negative input length!");
			}
			int blockSize = this.GetBlockSize();
			int updateOutputSize = this.GetUpdateOutputSize(length);
			if (updateOutputSize > 0)
			{
				Check.OutputLength(output, outOff, updateOutputSize, "output buffer too short");
			}
			int num = 0;
			int num2 = this.buf.Length - this.bufOff;
			if (length > num2)
			{
				Array.Copy(input, inOff, this.buf, this.bufOff, num2);
				num += this.cipher.ProcessBlock(this.buf, 0, output, outOff);
				this.bufOff = 0;
				length -= num2;
				inOff += num2;
				while (length > this.buf.Length)
				{
					num += this.cipher.ProcessBlock(input, inOff, output, outOff + num);
					length -= blockSize;
					inOff += blockSize;
				}
			}
			Array.Copy(input, inOff, this.buf, this.bufOff, length);
			this.bufOff += length;
			return num;
		}

		public override int DoFinal(byte[] output, int outOff)
		{
			int blockSize = this.cipher.GetBlockSize();
			int num = 0;
			if (this.forEncryption)
			{
				if (this.bufOff == blockSize)
				{
					if (outOff + 2 * blockSize > output.Length)
					{
						this.Reset();
						throw new OutputLengthException("output buffer too short");
					}
					num = this.cipher.ProcessBlock(this.buf, 0, output, outOff);
					this.bufOff = 0;
				}
				this.padding.AddPadding(this.buf, this.bufOff);
				num += this.cipher.ProcessBlock(this.buf, 0, output, outOff + num);
				this.Reset();
			}
			else
			{
				if (this.bufOff != blockSize)
				{
					this.Reset();
					throw new DataLengthException("last block incomplete in decryption");
				}
				num = this.cipher.ProcessBlock(this.buf, 0, this.buf, 0);
				this.bufOff = 0;
				try
				{
					num -= this.padding.PadCount(this.buf);
					Array.Copy(this.buf, 0, output, outOff, num);
				}
				finally
				{
					this.Reset();
				}
			}
			return num;
		}
	}
}
