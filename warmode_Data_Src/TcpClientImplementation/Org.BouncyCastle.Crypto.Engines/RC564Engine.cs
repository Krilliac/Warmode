using Org.BouncyCastle.Crypto.Parameters;
using System;

namespace Org.BouncyCastle.Crypto.Engines
{
	public class RC564Engine : IBlockCipher
	{
		private static readonly int wordSize = 64;

		private static readonly int bytesPerWord = RC564Engine.wordSize / 8;

		private int _noRounds;

		private long[] _S;

		private static readonly long P64 = -5196783011329398165L;

		private static readonly long Q64 = -7046029254386353131L;

		private bool forEncryption;

		public virtual string AlgorithmName
		{
			get
			{
				return "RC5-64";
			}
		}

		public virtual bool IsPartialBlockOkay
		{
			get
			{
				return false;
			}
		}

		public RC564Engine()
		{
			this._noRounds = 12;
		}

		public virtual int GetBlockSize()
		{
			return 2 * RC564Engine.bytesPerWord;
		}

		public virtual void Init(bool forEncryption, ICipherParameters parameters)
		{
			if (!typeof(RC5Parameters).IsInstanceOfType(parameters))
			{
				throw new ArgumentException("invalid parameter passed to RC564 init - " + parameters.GetType().ToString());
			}
			RC5Parameters rC5Parameters = (RC5Parameters)parameters;
			this.forEncryption = forEncryption;
			this._noRounds = rC5Parameters.Rounds;
			this.SetKey(rC5Parameters.GetKey());
		}

		public virtual int ProcessBlock(byte[] input, int inOff, byte[] output, int outOff)
		{
			if (!this.forEncryption)
			{
				return this.DecryptBlock(input, inOff, output, outOff);
			}
			return this.EncryptBlock(input, inOff, output, outOff);
		}

		public virtual void Reset()
		{
		}

		private void SetKey(byte[] key)
		{
			long[] array = new long[(key.Length + (RC564Engine.bytesPerWord - 1)) / RC564Engine.bytesPerWord];
			for (int num = 0; num != key.Length; num++)
			{
				array[num / RC564Engine.bytesPerWord] += (long)(key[num] & 255) << 8 * (num % RC564Engine.bytesPerWord);
			}
			this._S = new long[2 * (this._noRounds + 1)];
			this._S[0] = RC564Engine.P64;
			for (int i = 1; i < this._S.Length; i++)
			{
				this._S[i] = this._S[i - 1] + RC564Engine.Q64;
			}
			int num2;
			if (array.Length > this._S.Length)
			{
				num2 = 3 * array.Length;
			}
			else
			{
				num2 = 3 * this._S.Length;
			}
			long num3 = 0L;
			long num4 = 0L;
			int num5 = 0;
			int num6 = 0;
			for (int j = 0; j < num2; j++)
			{
				num3 = (this._S[num5] = this.RotateLeft(this._S[num5] + num3 + num4, 3L));
				num4 = (array[num6] = this.RotateLeft(array[num6] + num3 + num4, num3 + num4));
				num5 = (num5 + 1) % this._S.Length;
				num6 = (num6 + 1) % array.Length;
			}
		}

		private int EncryptBlock(byte[] input, int inOff, byte[] outBytes, int outOff)
		{
			long num = this.BytesToWord(input, inOff) + this._S[0];
			long num2 = this.BytesToWord(input, inOff + RC564Engine.bytesPerWord) + this._S[1];
			for (int i = 1; i <= this._noRounds; i++)
			{
				num = this.RotateLeft(num ^ num2, num2) + this._S[2 * i];
				num2 = this.RotateLeft(num2 ^ num, num) + this._S[2 * i + 1];
			}
			this.WordToBytes(num, outBytes, outOff);
			this.WordToBytes(num2, outBytes, outOff + RC564Engine.bytesPerWord);
			return 2 * RC564Engine.bytesPerWord;
		}

		private int DecryptBlock(byte[] input, int inOff, byte[] outBytes, int outOff)
		{
			long num = this.BytesToWord(input, inOff);
			long num2 = this.BytesToWord(input, inOff + RC564Engine.bytesPerWord);
			for (int i = this._noRounds; i >= 1; i--)
			{
				num2 = (this.RotateRight(num2 - this._S[2 * i + 1], num) ^ num);
				num = (this.RotateRight(num - this._S[2 * i], num2) ^ num2);
			}
			this.WordToBytes(num - this._S[0], outBytes, outOff);
			this.WordToBytes(num2 - this._S[1], outBytes, outOff + RC564Engine.bytesPerWord);
			return 2 * RC564Engine.bytesPerWord;
		}

		private long RotateLeft(long x, long y)
		{
			return x << (int)(y & (long)(RC564Engine.wordSize - 1)) | (long)((ulong)x >> (int)((long)RC564Engine.wordSize - (y & (long)(RC564Engine.wordSize - 1))));
		}

		private long RotateRight(long x, long y)
		{
			return (long)((ulong)x >> (int)(y & (long)(RC564Engine.wordSize - 1)) | (ulong)((ulong)x << (int)((long)RC564Engine.wordSize - (y & (long)(RC564Engine.wordSize - 1)))));
		}

		private long BytesToWord(byte[] src, int srcOff)
		{
			long num = 0L;
			for (int i = RC564Engine.bytesPerWord - 1; i >= 0; i--)
			{
				num = (num << 8) + (long)(src[i + srcOff] & 255);
			}
			return num;
		}

		private void WordToBytes(long word, byte[] dst, int dstOff)
		{
			for (int i = 0; i < RC564Engine.bytesPerWord; i++)
			{
				dst[i + dstOff] = (byte)word;
				word = (long)((ulong)word >> 8);
			}
		}
	}
}
