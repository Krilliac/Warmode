using System;

namespace Org.BouncyCastle.Utilities.Encoders
{
	public class HexTranslator : ITranslator
	{
		private static readonly byte[] hexTable = new byte[]
		{
			48,
			49,
			50,
			51,
			52,
			53,
			54,
			55,
			56,
			57,
			97,
			98,
			99,
			100,
			101,
			102
		};

		public int GetEncodedBlockSize()
		{
			return 2;
		}

		public int Encode(byte[] input, int inOff, int length, byte[] outBytes, int outOff)
		{
			int i = 0;
			int num = 0;
			while (i < length)
			{
				outBytes[outOff + num] = HexTranslator.hexTable[input[inOff] >> 4 & 15];
				outBytes[outOff + num + 1] = HexTranslator.hexTable[(int)(input[inOff] & 15)];
				inOff++;
				i++;
				num += 2;
			}
			return length * 2;
		}

		public int GetDecodedBlockSize()
		{
			return 1;
		}

		public int Decode(byte[] input, int inOff, int length, byte[] outBytes, int outOff)
		{
			int num = length / 2;
			for (int i = 0; i < num; i++)
			{
				byte b = input[inOff + i * 2];
				byte b2 = input[inOff + i * 2 + 1];
				if (b < 97)
				{
					outBytes[outOff] = (byte)(b - 48 << 4);
				}
				else
				{
					outBytes[outOff] = (byte)(b - 97 + 10 << 4);
				}
				if (b2 < 97)
				{
					int expr_4A_cp_1 = outOff;
					outBytes[expr_4A_cp_1] += b2 - 48;
				}
				else
				{
					int expr_67_cp_1 = outOff;
					outBytes[expr_67_cp_1] += b2 - 97 + 10;
				}
				outOff++;
			}
			return num;
		}
	}
}
