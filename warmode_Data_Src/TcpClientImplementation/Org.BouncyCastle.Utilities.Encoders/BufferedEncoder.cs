using System;

namespace Org.BouncyCastle.Utilities.Encoders
{
	public class BufferedEncoder
	{
		internal byte[] Buffer;

		internal int bufOff;

		internal ITranslator translator;

		public BufferedEncoder(ITranslator translator, int bufferSize)
		{
			this.translator = translator;
			if (bufferSize % translator.GetEncodedBlockSize() != 0)
			{
				throw new ArgumentException("buffer size not multiple of input block size");
			}
			this.Buffer = new byte[bufferSize];
		}

		public int ProcessByte(byte input, byte[] outBytes, int outOff)
		{
			int result = 0;
			this.Buffer[this.bufOff++] = input;
			if (this.bufOff == this.Buffer.Length)
			{
				result = this.translator.Encode(this.Buffer, 0, this.Buffer.Length, outBytes, outOff);
				this.bufOff = 0;
			}
			return result;
		}

		public int ProcessBytes(byte[] input, int inOff, int len, byte[] outBytes, int outOff)
		{
			if (len < 0)
			{
				throw new ArgumentException("Can't have a negative input length!");
			}
			int num = 0;
			int num2 = this.Buffer.Length - this.bufOff;
			if (len > num2)
			{
				Array.Copy(input, inOff, this.Buffer, this.bufOff, num2);
				num += this.translator.Encode(this.Buffer, 0, this.Buffer.Length, outBytes, outOff);
				this.bufOff = 0;
				len -= num2;
				inOff += num2;
				outOff += num;
				int num3 = len - len % this.Buffer.Length;
				num += this.translator.Encode(input, inOff, num3, outBytes, outOff);
				len -= num3;
				inOff += num3;
			}
			if (len != 0)
			{
				Array.Copy(input, inOff, this.Buffer, this.bufOff, len);
				this.bufOff += len;
			}
			return num;
		}
	}
}
