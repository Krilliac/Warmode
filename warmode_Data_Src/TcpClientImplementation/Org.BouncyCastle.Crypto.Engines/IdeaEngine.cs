using Org.BouncyCastle.Crypto.Parameters;
using System;

namespace Org.BouncyCastle.Crypto.Engines
{
	public class IdeaEngine : IBlockCipher
	{
		private const int BLOCK_SIZE = 8;

		private int[] workingKey;

		private static readonly int MASK = 65535;

		private static readonly int BASE = 65537;

		public virtual string AlgorithmName
		{
			get
			{
				return "IDEA";
			}
		}

		public virtual bool IsPartialBlockOkay
		{
			get
			{
				return false;
			}
		}

		public virtual void Init(bool forEncryption, ICipherParameters parameters)
		{
			if (!(parameters is KeyParameter))
			{
				throw new ArgumentException("invalid parameter passed to IDEA init - " + parameters.GetType().ToString());
			}
			this.workingKey = this.GenerateWorkingKey(forEncryption, ((KeyParameter)parameters).GetKey());
		}

		public virtual int GetBlockSize()
		{
			return 8;
		}

		public virtual int ProcessBlock(byte[] input, int inOff, byte[] output, int outOff)
		{
			if (this.workingKey == null)
			{
				throw new InvalidOperationException("IDEA engine not initialised");
			}
			Check.DataLength(input, inOff, 8, "input buffer too short");
			Check.OutputLength(output, outOff, 8, "output buffer too short");
			this.IdeaFunc(this.workingKey, input, inOff, output, outOff);
			return 8;
		}

		public virtual void Reset()
		{
		}

		private int BytesToWord(byte[] input, int inOff)
		{
			return ((int)input[inOff] << 8 & 65280) + (int)(input[inOff + 1] & 255);
		}

		private void WordToBytes(int word, byte[] outBytes, int outOff)
		{
			outBytes[outOff] = (byte)((uint)word >> 8);
			outBytes[outOff + 1] = (byte)word;
		}

		private int Mul(int x, int y)
		{
			if (x == 0)
			{
				x = IdeaEngine.BASE - y;
			}
			else if (y == 0)
			{
				x = IdeaEngine.BASE - x;
			}
			else
			{
				int num = x * y;
				y = (num & IdeaEngine.MASK);
				x = (int)((uint)num >> 16);
				x = y - x + ((y < x) ? 1 : 0);
			}
			return x & IdeaEngine.MASK;
		}

		private void IdeaFunc(int[] workingKey, byte[] input, int inOff, byte[] outBytes, int outOff)
		{
			int num = 0;
			int num2 = this.BytesToWord(input, inOff);
			int num3 = this.BytesToWord(input, inOff + 2);
			int num4 = this.BytesToWord(input, inOff + 4);
			int num5 = this.BytesToWord(input, inOff + 6);
			for (int i = 0; i < 8; i++)
			{
				num2 = this.Mul(num2, workingKey[num++]);
				num3 += workingKey[num++];
				num3 &= IdeaEngine.MASK;
				num4 += workingKey[num++];
				num4 &= IdeaEngine.MASK;
				num5 = this.Mul(num5, workingKey[num++]);
				int num6 = num3;
				int num7 = num4;
				num4 ^= num2;
				num3 ^= num5;
				num4 = this.Mul(num4, workingKey[num++]);
				num3 += num4;
				num3 &= IdeaEngine.MASK;
				num3 = this.Mul(num3, workingKey[num++]);
				num4 += num3;
				num4 &= IdeaEngine.MASK;
				num2 ^= num3;
				num5 ^= num4;
				num3 ^= num7;
				num4 ^= num6;
			}
			this.WordToBytes(this.Mul(num2, workingKey[num++]), outBytes, outOff);
			this.WordToBytes(num4 + workingKey[num++], outBytes, outOff + 2);
			this.WordToBytes(num3 + workingKey[num++], outBytes, outOff + 4);
			this.WordToBytes(this.Mul(num5, workingKey[num]), outBytes, outOff + 6);
		}

		private int[] ExpandKey(byte[] uKey)
		{
			int[] array = new int[52];
			if (uKey.Length < 16)
			{
				byte[] array2 = new byte[16];
				Array.Copy(uKey, 0, array2, array2.Length - uKey.Length, uKey.Length);
				uKey = array2;
			}
			for (int i = 0; i < 8; i++)
			{
				array[i] = this.BytesToWord(uKey, i * 2);
			}
			for (int j = 8; j < 52; j++)
			{
				if ((j & 7) < 6)
				{
					array[j] = (((array[j - 7] & 127) << 9 | array[j - 6] >> 7) & IdeaEngine.MASK);
				}
				else if ((j & 7) == 6)
				{
					array[j] = (((array[j - 7] & 127) << 9 | array[j - 14] >> 7) & IdeaEngine.MASK);
				}
				else
				{
					array[j] = (((array[j - 15] & 127) << 9 | array[j - 14] >> 7) & IdeaEngine.MASK);
				}
			}
			return array;
		}

		private int MulInv(int x)
		{
			if (x < 2)
			{
				return x;
			}
			int num = 1;
			int num2 = IdeaEngine.BASE / x;
			int num3 = IdeaEngine.BASE % x;
			while (num3 != 1)
			{
				int num4 = x / num3;
				x %= num3;
				num = (num + num2 * num4 & IdeaEngine.MASK);
				if (x == 1)
				{
					return num;
				}
				num4 = num3 / x;
				num3 %= x;
				num2 = (num2 + num * num4 & IdeaEngine.MASK);
			}
			return 1 - num2 & IdeaEngine.MASK;
		}

		private int AddInv(int x)
		{
			return -x & IdeaEngine.MASK;
		}

		private int[] InvertKey(int[] inKey)
		{
			int num = 52;
			int[] array = new int[52];
			int num2 = 0;
			int num3 = this.MulInv(inKey[num2++]);
			int num4 = this.AddInv(inKey[num2++]);
			int num5 = this.AddInv(inKey[num2++]);
			int num6 = this.MulInv(inKey[num2++]);
			array[--num] = num6;
			array[--num] = num5;
			array[--num] = num4;
			array[--num] = num3;
			for (int i = 1; i < 8; i++)
			{
				num3 = inKey[num2++];
				num4 = inKey[num2++];
				array[--num] = num4;
				array[--num] = num3;
				num3 = this.MulInv(inKey[num2++]);
				num4 = this.AddInv(inKey[num2++]);
				num5 = this.AddInv(inKey[num2++]);
				num6 = this.MulInv(inKey[num2++]);
				array[--num] = num6;
				array[--num] = num4;
				array[--num] = num5;
				array[--num] = num3;
			}
			num3 = inKey[num2++];
			num4 = inKey[num2++];
			array[--num] = num4;
			array[--num] = num3;
			num3 = this.MulInv(inKey[num2++]);
			num4 = this.AddInv(inKey[num2++]);
			num5 = this.AddInv(inKey[num2++]);
			num6 = this.MulInv(inKey[num2]);
			array[--num] = num6;
			array[--num] = num5;
			array[--num] = num4;
			array[num - 1] = num3;
			return array;
		}

		private int[] GenerateWorkingKey(bool forEncryption, byte[] userKey)
		{
			if (forEncryption)
			{
				return this.ExpandKey(userKey);
			}
			return this.InvertKey(this.ExpandKey(userKey));
		}
	}
}
