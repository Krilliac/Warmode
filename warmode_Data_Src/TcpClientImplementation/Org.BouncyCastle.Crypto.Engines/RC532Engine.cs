using Org.BouncyCastle.Crypto.Parameters;
using System;

namespace Org.BouncyCastle.Crypto.Engines
{
	public class RC532Engine : IBlockCipher
	{
		private int _noRounds;

		private int[] _S;

		private static readonly int P32 = -1209970333;

		private static readonly int Q32 = -1640531527;

		private bool forEncryption;

		public virtual string AlgorithmName
		{
			get
			{
				return "RC5-32";
			}
		}

		public virtual bool IsPartialBlockOkay
		{
			get
			{
				return false;
			}
		}

		public RC532Engine()
		{
			this._noRounds = 12;
		}

		public virtual int GetBlockSize()
		{
			return 8;
		}

		public virtual void Init(bool forEncryption, ICipherParameters parameters)
		{
			if (typeof(RC5Parameters).IsInstanceOfType(parameters))
			{
				RC5Parameters rC5Parameters = (RC5Parameters)parameters;
				this._noRounds = rC5Parameters.Rounds;
				this.SetKey(rC5Parameters.GetKey());
			}
			else
			{
				if (!typeof(KeyParameter).IsInstanceOfType(parameters))
				{
					throw new ArgumentException("invalid parameter passed to RC532 init - " + parameters.GetType().ToString());
				}
				KeyParameter keyParameter = (KeyParameter)parameters;
				this.SetKey(keyParameter.GetKey());
			}
			this.forEncryption = forEncryption;
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
			int[] array = new int[(key.Length + 3) / 4];
			for (int num = 0; num != key.Length; num++)
			{
				array[num / 4] += (int)(key[num] & 255) << 8 * (num % 4);
			}
			this._S = new int[2 * (this._noRounds + 1)];
			this._S[0] = RC532Engine.P32;
			for (int i = 1; i < this._S.Length; i++)
			{
				this._S[i] = this._S[i - 1] + RC532Engine.Q32;
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
			int num3 = 0;
			int num4 = 0;
			int num5 = 0;
			int num6 = 0;
			for (int j = 0; j < num2; j++)
			{
				num3 = (this._S[num5] = this.RotateLeft(this._S[num5] + num3 + num4, 3));
				num4 = (array[num6] = this.RotateLeft(array[num6] + num3 + num4, num3 + num4));
				num5 = (num5 + 1) % this._S.Length;
				num6 = (num6 + 1) % array.Length;
			}
		}

		private int EncryptBlock(byte[] input, int inOff, byte[] outBytes, int outOff)
		{
			int num = this.BytesToWord(input, inOff) + this._S[0];
			int num2 = this.BytesToWord(input, inOff + 4) + this._S[1];
			for (int i = 1; i <= this._noRounds; i++)
			{
				num = this.RotateLeft(num ^ num2, num2) + this._S[2 * i];
				num2 = this.RotateLeft(num2 ^ num, num) + this._S[2 * i + 1];
			}
			this.WordToBytes(num, outBytes, outOff);
			this.WordToBytes(num2, outBytes, outOff + 4);
			return 8;
		}

		private int DecryptBlock(byte[] input, int inOff, byte[] outBytes, int outOff)
		{
			int num = this.BytesToWord(input, inOff);
			int num2 = this.BytesToWord(input, inOff + 4);
			for (int i = this._noRounds; i >= 1; i--)
			{
				num2 = (this.RotateRight(num2 - this._S[2 * i + 1], num) ^ num);
				num = (this.RotateRight(num - this._S[2 * i], num2) ^ num2);
			}
			this.WordToBytes(num - this._S[0], outBytes, outOff);
			this.WordToBytes(num2 - this._S[1], outBytes, outOff + 4);
			return 8;
		}

		private int RotateLeft(int x, int y)
		{
			return x << y | (int)((uint)x >> 32 - (y & 31));
		}

		private int RotateRight(int x, int y)
		{
			return (int)((uint)x >> y | (uint)((uint)x << 32 - (y & 31)));
		}

		private int BytesToWord(byte[] src, int srcOff)
		{
			return (int)(src[srcOff] & 255) | (int)(src[srcOff + 1] & 255) << 8 | (int)(src[srcOff + 2] & 255) << 16 | (int)(src[srcOff + 3] & 255) << 24;
		}

		private void WordToBytes(int word, byte[] dst, int dstOff)
		{
			dst[dstOff] = (byte)word;
			dst[dstOff + 1] = (byte)(word >> 8);
			dst[dstOff + 2] = (byte)(word >> 16);
			dst[dstOff + 3] = (byte)(word >> 24);
		}
	}
}
