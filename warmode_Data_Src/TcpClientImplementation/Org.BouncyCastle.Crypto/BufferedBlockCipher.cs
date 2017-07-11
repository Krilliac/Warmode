using Org.BouncyCastle.Crypto.Parameters;
using System;

namespace Org.BouncyCastle.Crypto
{
	public class BufferedBlockCipher : BufferedCipherBase
	{
		internal byte[] buf;

		internal int bufOff;

		internal bool forEncryption;

		internal IBlockCipher cipher;

		public override string AlgorithmName
		{
			get
			{
				return this.cipher.AlgorithmName;
			}
		}

		protected BufferedBlockCipher()
		{
		}

		public BufferedBlockCipher(IBlockCipher cipher)
		{
			if (cipher == null)
			{
				throw new ArgumentNullException("cipher");
			}
			this.cipher = cipher;
			this.buf = new byte[cipher.GetBlockSize()];
			this.bufOff = 0;
		}

		public override void Init(bool forEncryption, ICipherParameters parameters)
		{
			this.forEncryption = forEncryption;
			ParametersWithRandom parametersWithRandom = parameters as ParametersWithRandom;
			if (parametersWithRandom != null)
			{
				parameters = parametersWithRandom.Parameters;
			}
			this.Reset();
			this.cipher.Init(forEncryption, parameters);
		}

		public override int GetBlockSize()
		{
			return this.cipher.GetBlockSize();
		}

		public override int GetUpdateOutputSize(int length)
		{
			int num = length + this.bufOff;
			int num2 = num % this.buf.Length;
			return num - num2;
		}

		public override int GetOutputSize(int length)
		{
			return length + this.bufOff;
		}

		public override int ProcessByte(byte input, byte[] output, int outOff)
		{
			this.buf[this.bufOff++] = input;
			if (this.bufOff != this.buf.Length)
			{
				return 0;
			}
			if (outOff + this.buf.Length > output.Length)
			{
				throw new DataLengthException("output buffer too short");
			}
			this.bufOff = 0;
			return this.cipher.ProcessBlock(this.buf, 0, output, outOff);
		}

		public override byte[] ProcessByte(byte input)
		{
			int updateOutputSize = this.GetUpdateOutputSize(1);
			byte[] array = (updateOutputSize > 0) ? new byte[updateOutputSize] : null;
			int num = this.ProcessByte(input, array, 0);
			if (updateOutputSize > 0 && num < updateOutputSize)
			{
				byte[] array2 = new byte[num];
				Array.Copy(array, 0, array2, 0, num);
				array = array2;
			}
			return array;
		}

		public override byte[] ProcessBytes(byte[] input, int inOff, int length)
		{
			if (input == null)
			{
				throw new ArgumentNullException("input");
			}
			if (length < 1)
			{
				return null;
			}
			int updateOutputSize = this.GetUpdateOutputSize(length);
			byte[] array = (updateOutputSize > 0) ? new byte[updateOutputSize] : null;
			int num = this.ProcessBytes(input, inOff, length, array, 0);
			if (updateOutputSize > 0 && num < updateOutputSize)
			{
				byte[] array2 = new byte[num];
				Array.Copy(array, 0, array2, 0, num);
				array = array2;
			}
			return array;
		}

		public override int ProcessBytes(byte[] input, int inOff, int length, byte[] output, int outOff)
		{
			if (length >= 1)
			{
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
				if (this.bufOff == this.buf.Length)
				{
					num += this.cipher.ProcessBlock(this.buf, 0, output, outOff + num);
					this.bufOff = 0;
				}
				return num;
			}
			if (length < 0)
			{
				throw new ArgumentException("Can't have a negative input length!");
			}
			return 0;
		}

		public override byte[] DoFinal()
		{
			byte[] array = BufferedCipherBase.EmptyBuffer;
			int outputSize = this.GetOutputSize(0);
			if (outputSize > 0)
			{
				array = new byte[outputSize];
				int num = this.DoFinal(array, 0);
				if (num < array.Length)
				{
					byte[] array2 = new byte[num];
					Array.Copy(array, 0, array2, 0, num);
					array = array2;
				}
			}
			else
			{
				this.Reset();
			}
			return array;
		}

		public override byte[] DoFinal(byte[] input, int inOff, int inLen)
		{
			if (input == null)
			{
				throw new ArgumentNullException("input");
			}
			int outputSize = this.GetOutputSize(inLen);
			byte[] array = BufferedCipherBase.EmptyBuffer;
			if (outputSize > 0)
			{
				array = new byte[outputSize];
				int num = (inLen > 0) ? this.ProcessBytes(input, inOff, inLen, array, 0) : 0;
				num += this.DoFinal(array, num);
				if (num < array.Length)
				{
					byte[] array2 = new byte[num];
					Array.Copy(array, 0, array2, 0, num);
					array = array2;
				}
			}
			else
			{
				this.Reset();
			}
			return array;
		}

		public override int DoFinal(byte[] output, int outOff)
		{
			int result;
			try
			{
				if (this.bufOff != 0)
				{
					Check.DataLength(!this.cipher.IsPartialBlockOkay, "data not block size aligned");
					Check.OutputLength(output, outOff, this.bufOff, "output buffer too short for DoFinal()");
					this.cipher.ProcessBlock(this.buf, 0, this.buf, 0);
					Array.Copy(this.buf, 0, output, outOff, this.bufOff);
				}
				result = this.bufOff;
			}
			finally
			{
				this.Reset();
			}
			return result;
		}

		public override void Reset()
		{
			Array.Clear(this.buf, 0, this.buf.Length);
			this.bufOff = 0;
			this.cipher.Reset();
		}
	}
}
