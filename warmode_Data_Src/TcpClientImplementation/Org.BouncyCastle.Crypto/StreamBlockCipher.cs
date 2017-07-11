using System;

namespace Org.BouncyCastle.Crypto
{
	public class StreamBlockCipher : IStreamCipher
	{
		private readonly IBlockCipher cipher;

		private readonly byte[] oneByte = new byte[1];

		public string AlgorithmName
		{
			get
			{
				return this.cipher.AlgorithmName;
			}
		}

		public StreamBlockCipher(IBlockCipher cipher)
		{
			if (cipher == null)
			{
				throw new ArgumentNullException("cipher");
			}
			if (cipher.GetBlockSize() != 1)
			{
				throw new ArgumentException("block cipher block size != 1.", "cipher");
			}
			this.cipher = cipher;
		}

		public void Init(bool forEncryption, ICipherParameters parameters)
		{
			this.cipher.Init(forEncryption, parameters);
		}

		public byte ReturnByte(byte input)
		{
			this.oneByte[0] = input;
			this.cipher.ProcessBlock(this.oneByte, 0, this.oneByte, 0);
			return this.oneByte[0];
		}

		public void ProcessBytes(byte[] input, int inOff, int length, byte[] output, int outOff)
		{
			if (outOff + length > output.Length)
			{
				throw new DataLengthException("output buffer too small in ProcessBytes()");
			}
			for (int num = 0; num != length; num++)
			{
				this.cipher.ProcessBlock(input, inOff + num, output, outOff + num);
			}
		}

		public void Reset()
		{
			this.cipher.Reset();
		}
	}
}
