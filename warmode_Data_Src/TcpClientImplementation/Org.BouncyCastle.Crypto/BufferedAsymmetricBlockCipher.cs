using System;

namespace Org.BouncyCastle.Crypto
{
	public class BufferedAsymmetricBlockCipher : BufferedCipherBase
	{
		private readonly IAsymmetricBlockCipher cipher;

		private byte[] buffer;

		private int bufOff;

		public override string AlgorithmName
		{
			get
			{
				return this.cipher.AlgorithmName;
			}
		}

		public BufferedAsymmetricBlockCipher(IAsymmetricBlockCipher cipher)
		{
			this.cipher = cipher;
		}

		internal int GetBufferPosition()
		{
			return this.bufOff;
		}

		public override int GetBlockSize()
		{
			return this.cipher.GetInputBlockSize();
		}

		public override int GetOutputSize(int length)
		{
			return this.cipher.GetOutputBlockSize();
		}

		public override int GetUpdateOutputSize(int length)
		{
			return 0;
		}

		public override void Init(bool forEncryption, ICipherParameters parameters)
		{
			this.Reset();
			this.cipher.Init(forEncryption, parameters);
			this.buffer = new byte[this.cipher.GetInputBlockSize() + (forEncryption ? 1 : 0)];
			this.bufOff = 0;
		}

		public override byte[] ProcessByte(byte input)
		{
			if (this.bufOff >= this.buffer.Length)
			{
				throw new DataLengthException("attempt to process message to long for cipher");
			}
			this.buffer[this.bufOff++] = input;
			return null;
		}

		public override byte[] ProcessBytes(byte[] input, int inOff, int length)
		{
			if (length < 1)
			{
				return null;
			}
			if (input == null)
			{
				throw new ArgumentNullException("input");
			}
			if (this.bufOff + length > this.buffer.Length)
			{
				throw new DataLengthException("attempt to process message to long for cipher");
			}
			Array.Copy(input, inOff, this.buffer, this.bufOff, length);
			this.bufOff += length;
			return null;
		}

		public override byte[] DoFinal()
		{
			byte[] result = (this.bufOff > 0) ? this.cipher.ProcessBlock(this.buffer, 0, this.bufOff) : BufferedCipherBase.EmptyBuffer;
			this.Reset();
			return result;
		}

		public override byte[] DoFinal(byte[] input, int inOff, int length)
		{
			this.ProcessBytes(input, inOff, length);
			return this.DoFinal();
		}

		public override void Reset()
		{
			if (this.buffer != null)
			{
				Array.Clear(this.buffer, 0, this.buffer.Length);
				this.bufOff = 0;
			}
		}
	}
}
