using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Crypto.Utilities;
using System;

namespace Org.BouncyCastle.Crypto.Engines
{
	public class DesEngine : IBlockCipher
	{
		internal const int BLOCK_SIZE = 8;

		private int[] workingKey;

		private static readonly short[] bytebit = new short[]
		{
			128,
			64,
			32,
			16,
			8,
			4,
			2,
			1
		};

		private static readonly int[] bigbyte = new int[]
		{
			8388608,
			4194304,
			2097152,
			1048576,
			524288,
			262144,
			131072,
			65536,
			32768,
			16384,
			8192,
			4096,
			2048,
			1024,
			512,
			256,
			128,
			64,
			32,
			16,
			8,
			4,
			2,
			1
		};

		private static readonly byte[] pc1 = new byte[]
		{
			56,
			48,
			40,
			32,
			24,
			16,
			8,
			0,
			57,
			49,
			41,
			33,
			25,
			17,
			9,
			1,
			58,
			50,
			42,
			34,
			26,
			18,
			10,
			2,
			59,
			51,
			43,
			35,
			62,
			54,
			46,
			38,
			30,
			22,
			14,
			6,
			61,
			53,
			45,
			37,
			29,
			21,
			13,
			5,
			60,
			52,
			44,
			36,
			28,
			20,
			12,
			4,
			27,
			19,
			11,
			3
		};

		private static readonly byte[] totrot = new byte[]
		{
			1,
			2,
			4,
			6,
			8,
			10,
			12,
			14,
			15,
			17,
			19,
			21,
			23,
			25,
			27,
			28
		};

		private static readonly byte[] pc2 = new byte[]
		{
			13,
			16,
			10,
			23,
			0,
			4,
			2,
			27,
			14,
			5,
			20,
			9,
			22,
			18,
			11,
			3,
			25,
			7,
			15,
			6,
			26,
			19,
			12,
			1,
			40,
			51,
			30,
			36,
			46,
			54,
			29,
			39,
			50,
			44,
			32,
			47,
			43,
			48,
			38,
			55,
			33,
			52,
			45,
			41,
			49,
			35,
			28,
			31
		};

		private static readonly uint[] SP1 = new uint[]
		{
			16843776u,
			0u,
			65536u,
			16843780u,
			16842756u,
			66564u,
			4u,
			65536u,
			1024u,
			16843776u,
			16843780u,
			1024u,
			16778244u,
			16842756u,
			16777216u,
			4u,
			1028u,
			16778240u,
			16778240u,
			66560u,
			66560u,
			16842752u,
			16842752u,
			16778244u,
			65540u,
			16777220u,
			16777220u,
			65540u,
			0u,
			1028u,
			66564u,
			16777216u,
			65536u,
			16843780u,
			4u,
			16842752u,
			16843776u,
			16777216u,
			16777216u,
			1024u,
			16842756u,
			65536u,
			66560u,
			16777220u,
			1024u,
			4u,
			16778244u,
			66564u,
			16843780u,
			65540u,
			16842752u,
			16778244u,
			16777220u,
			1028u,
			66564u,
			16843776u,
			1028u,
			16778240u,
			16778240u,
			0u,
			65540u,
			66560u,
			0u,
			16842756u
		};

		private static readonly uint[] SP2 = new uint[]
		{
			2148565024u,
			2147516416u,
			32768u,
			1081376u,
			1048576u,
			32u,
			2148532256u,
			2147516448u,
			2147483680u,
			2148565024u,
			2148564992u,
			2147483648u,
			2147516416u,
			1048576u,
			32u,
			2148532256u,
			1081344u,
			1048608u,
			2147516448u,
			0u,
			2147483648u,
			32768u,
			1081376u,
			2148532224u,
			1048608u,
			2147483680u,
			0u,
			1081344u,
			32800u,
			2148564992u,
			2148532224u,
			32800u,
			0u,
			1081376u,
			2148532256u,
			1048576u,
			2147516448u,
			2148532224u,
			2148564992u,
			32768u,
			2148532224u,
			2147516416u,
			32u,
			2148565024u,
			1081376u,
			32u,
			32768u,
			2147483648u,
			32800u,
			2148564992u,
			1048576u,
			2147483680u,
			1048608u,
			2147516448u,
			2147483680u,
			1048608u,
			1081344u,
			0u,
			2147516416u,
			32800u,
			2147483648u,
			2148532256u,
			2148565024u,
			1081344u
		};

		private static readonly uint[] SP3 = new uint[]
		{
			520u,
			134349312u,
			0u,
			134348808u,
			134218240u,
			0u,
			131592u,
			134218240u,
			131080u,
			134217736u,
			134217736u,
			131072u,
			134349320u,
			131080u,
			134348800u,
			520u,
			134217728u,
			8u,
			134349312u,
			512u,
			131584u,
			134348800u,
			134348808u,
			131592u,
			134218248u,
			131584u,
			131072u,
			134218248u,
			8u,
			134349320u,
			512u,
			134217728u,
			134349312u,
			134217728u,
			131080u,
			520u,
			131072u,
			134349312u,
			134218240u,
			0u,
			512u,
			131080u,
			134349320u,
			134218240u,
			134217736u,
			512u,
			0u,
			134348808u,
			134218248u,
			131072u,
			134217728u,
			134349320u,
			8u,
			131592u,
			131584u,
			134217736u,
			134348800u,
			134218248u,
			520u,
			134348800u,
			131592u,
			8u,
			134348808u,
			131584u
		};

		private static readonly uint[] SP4 = new uint[]
		{
			8396801u,
			8321u,
			8321u,
			128u,
			8396928u,
			8388737u,
			8388609u,
			8193u,
			0u,
			8396800u,
			8396800u,
			8396929u,
			129u,
			0u,
			8388736u,
			8388609u,
			1u,
			8192u,
			8388608u,
			8396801u,
			128u,
			8388608u,
			8193u,
			8320u,
			8388737u,
			1u,
			8320u,
			8388736u,
			8192u,
			8396928u,
			8396929u,
			129u,
			8388736u,
			8388609u,
			8396800u,
			8396929u,
			129u,
			0u,
			0u,
			8396800u,
			8320u,
			8388736u,
			8388737u,
			1u,
			8396801u,
			8321u,
			8321u,
			128u,
			8396929u,
			129u,
			1u,
			8192u,
			8388609u,
			8193u,
			8396928u,
			8388737u,
			8193u,
			8320u,
			8388608u,
			8396801u,
			128u,
			8388608u,
			8192u,
			8396928u
		};

		private static readonly uint[] SP5 = new uint[]
		{
			256u,
			34078976u,
			34078720u,
			1107296512u,
			524288u,
			256u,
			1073741824u,
			34078720u,
			1074266368u,
			524288u,
			33554688u,
			1074266368u,
			1107296512u,
			1107820544u,
			524544u,
			1073741824u,
			33554432u,
			1074266112u,
			1074266112u,
			0u,
			1073742080u,
			1107820800u,
			1107820800u,
			33554688u,
			1107820544u,
			1073742080u,
			0u,
			1107296256u,
			34078976u,
			33554432u,
			1107296256u,
			524544u,
			524288u,
			1107296512u,
			256u,
			33554432u,
			1073741824u,
			34078720u,
			1107296512u,
			1074266368u,
			33554688u,
			1073741824u,
			1107820544u,
			34078976u,
			1074266368u,
			256u,
			33554432u,
			1107820544u,
			1107820800u,
			524544u,
			1107296256u,
			1107820800u,
			34078720u,
			0u,
			1074266112u,
			1107296256u,
			524544u,
			33554688u,
			1073742080u,
			524288u,
			0u,
			1074266112u,
			34078976u,
			1073742080u
		};

		private static readonly uint[] SP6 = new uint[]
		{
			536870928u,
			541065216u,
			16384u,
			541081616u,
			541065216u,
			16u,
			541081616u,
			4194304u,
			536887296u,
			4210704u,
			4194304u,
			536870928u,
			4194320u,
			536887296u,
			536870912u,
			16400u,
			0u,
			4194320u,
			536887312u,
			16384u,
			4210688u,
			536887312u,
			16u,
			541065232u,
			541065232u,
			0u,
			4210704u,
			541081600u,
			16400u,
			4210688u,
			541081600u,
			536870912u,
			536887296u,
			16u,
			541065232u,
			4210688u,
			541081616u,
			4194304u,
			16400u,
			536870928u,
			4194304u,
			536887296u,
			536870912u,
			16400u,
			536870928u,
			541081616u,
			4210688u,
			541065216u,
			4210704u,
			541081600u,
			0u,
			541065232u,
			16u,
			16384u,
			541065216u,
			4210704u,
			16384u,
			4194320u,
			536887312u,
			0u,
			541081600u,
			536870912u,
			4194320u,
			536887312u
		};

		private static readonly uint[] SP7 = new uint[]
		{
			2097152u,
			69206018u,
			67110914u,
			0u,
			2048u,
			67110914u,
			2099202u,
			69208064u,
			69208066u,
			2097152u,
			0u,
			67108866u,
			2u,
			67108864u,
			69206018u,
			2050u,
			67110912u,
			2099202u,
			2097154u,
			67110912u,
			67108866u,
			69206016u,
			69208064u,
			2097154u,
			69206016u,
			2048u,
			2050u,
			69208066u,
			2099200u,
			2u,
			67108864u,
			2099200u,
			67108864u,
			2099200u,
			2097152u,
			67110914u,
			67110914u,
			69206018u,
			69206018u,
			2u,
			2097154u,
			67108864u,
			67110912u,
			2097152u,
			69208064u,
			2050u,
			2099202u,
			69208064u,
			2050u,
			67108866u,
			69208066u,
			69206016u,
			2099200u,
			0u,
			2u,
			69208066u,
			0u,
			2099202u,
			69206016u,
			2048u,
			67108866u,
			67110912u,
			2048u,
			2097154u
		};

		private static readonly uint[] SP8 = new uint[]
		{
			268439616u,
			4096u,
			262144u,
			268701760u,
			268435456u,
			268439616u,
			64u,
			268435456u,
			262208u,
			268697600u,
			268701760u,
			266240u,
			268701696u,
			266304u,
			4096u,
			64u,
			268697600u,
			268435520u,
			268439552u,
			4160u,
			266240u,
			262208u,
			268697664u,
			268701696u,
			4160u,
			0u,
			0u,
			268697664u,
			268435520u,
			268439552u,
			266304u,
			262144u,
			266304u,
			262144u,
			268701696u,
			4096u,
			64u,
			268697664u,
			4096u,
			266304u,
			268439552u,
			64u,
			268435520u,
			268697600u,
			268697664u,
			268435456u,
			262144u,
			268439616u,
			0u,
			268701760u,
			262208u,
			268435520u,
			268697600u,
			268439552u,
			268439616u,
			0u,
			268701760u,
			266240u,
			266240u,
			4160u,
			4160u,
			262208u,
			268435456u,
			268701696u
		};

		public virtual string AlgorithmName
		{
			get
			{
				return "DES";
			}
		}

		public virtual bool IsPartialBlockOkay
		{
			get
			{
				return false;
			}
		}

		public virtual int[] GetWorkingKey()
		{
			return this.workingKey;
		}

		public virtual void Init(bool forEncryption, ICipherParameters parameters)
		{
			if (!(parameters is KeyParameter))
			{
				throw new ArgumentException("invalid parameter passed to DES init - " + parameters.GetType().ToString());
			}
			this.workingKey = DesEngine.GenerateWorkingKey(forEncryption, ((KeyParameter)parameters).GetKey());
		}

		public virtual int GetBlockSize()
		{
			return 8;
		}

		public virtual int ProcessBlock(byte[] input, int inOff, byte[] output, int outOff)
		{
			if (this.workingKey == null)
			{
				throw new InvalidOperationException("DES engine not initialised");
			}
			Check.DataLength(input, inOff, 8, "input buffer too short");
			Check.OutputLength(output, outOff, 8, "output buffer too short");
			DesEngine.DesFunc(this.workingKey, input, inOff, output, outOff);
			return 8;
		}

		public virtual void Reset()
		{
		}

		protected static int[] GenerateWorkingKey(bool encrypting, byte[] key)
		{
			int[] array = new int[32];
			bool[] array2 = new bool[56];
			bool[] array3 = new bool[56];
			for (int i = 0; i < 56; i++)
			{
				int num = (int)DesEngine.pc1[i];
				array2[i] = (((short)key[(int)((UIntPtr)((uint)num >> 3))] & DesEngine.bytebit[num & 7]) != 0);
			}
			for (int j = 0; j < 16; j++)
			{
				int num2;
				if (encrypting)
				{
					num2 = j << 1;
				}
				else
				{
					num2 = 15 - j << 1;
				}
				int num3 = num2 + 1;
				array[num2] = (array[num3] = 0);
				for (int k = 0; k < 28; k++)
				{
					int num4 = k + (int)DesEngine.totrot[j];
					if (num4 < 28)
					{
						array3[k] = array2[num4];
					}
					else
					{
						array3[k] = array2[num4 - 28];
					}
				}
				for (int l = 28; l < 56; l++)
				{
					int num4 = l + (int)DesEngine.totrot[j];
					if (num4 < 56)
					{
						array3[l] = array2[num4];
					}
					else
					{
						array3[l] = array2[num4 - 28];
					}
				}
				for (int m = 0; m < 24; m++)
				{
					if (array3[(int)DesEngine.pc2[m]])
					{
						array[num2] |= DesEngine.bigbyte[m];
					}
					if (array3[(int)DesEngine.pc2[m + 24]])
					{
						array[num3] |= DesEngine.bigbyte[m];
					}
				}
			}
			for (int num5 = 0; num5 != 32; num5 += 2)
			{
				int num6 = array[num5];
				int num7 = array[num5 + 1];
				array[num5] = ((num6 & 16515072) << 6 | (num6 & 4032) << 10 | (int)((uint)(num7 & 16515072) >> 10) | (int)((uint)(num7 & 4032) >> 6));
				array[num5 + 1] = ((num6 & 258048) << 12 | (num6 & 63) << 16 | (int)((uint)(num7 & 258048) >> 4) | (num7 & 63));
			}
			return array;
		}

		internal static void DesFunc(int[] wKey, byte[] input, int inOff, byte[] outBytes, int outOff)
		{
			uint num = Pack.BE_To_UInt32(input, inOff);
			uint num2 = Pack.BE_To_UInt32(input, inOff + 4);
			uint num3 = (num >> 4 ^ num2) & 252645135u;
			num2 ^= num3;
			num ^= num3 << 4;
			num3 = ((num >> 16 ^ num2) & 65535u);
			num2 ^= num3;
			num ^= num3 << 16;
			num3 = ((num2 >> 2 ^ num) & 858993459u);
			num ^= num3;
			num2 ^= num3 << 2;
			num3 = ((num2 >> 8 ^ num) & 16711935u);
			num ^= num3;
			num2 ^= num3 << 8;
			num2 = (num2 << 1 | num2 >> 31);
			num3 = ((num ^ num2) & 2863311530u);
			num ^= num3;
			num2 ^= num3;
			num = (num << 1 | num >> 31);
			for (int i = 0; i < 8; i++)
			{
				num3 = (num2 << 28 | num2 >> 4);
				num3 ^= (uint)wKey[i * 4];
				uint num4 = DesEngine.SP7[(int)((UIntPtr)(num3 & 63u))];
				num4 |= DesEngine.SP5[(int)((UIntPtr)(num3 >> 8 & 63u))];
				num4 |= DesEngine.SP3[(int)((UIntPtr)(num3 >> 16 & 63u))];
				num4 |= DesEngine.SP1[(int)((UIntPtr)(num3 >> 24 & 63u))];
				num3 = (num2 ^ (uint)wKey[i * 4 + 1]);
				num4 |= DesEngine.SP8[(int)((UIntPtr)(num3 & 63u))];
				num4 |= DesEngine.SP6[(int)((UIntPtr)(num3 >> 8 & 63u))];
				num4 |= DesEngine.SP4[(int)((UIntPtr)(num3 >> 16 & 63u))];
				num4 |= DesEngine.SP2[(int)((UIntPtr)(num3 >> 24 & 63u))];
				num ^= num4;
				num3 = (num << 28 | num >> 4);
				num3 ^= (uint)wKey[i * 4 + 2];
				num4 = DesEngine.SP7[(int)((UIntPtr)(num3 & 63u))];
				num4 |= DesEngine.SP5[(int)((UIntPtr)(num3 >> 8 & 63u))];
				num4 |= DesEngine.SP3[(int)((UIntPtr)(num3 >> 16 & 63u))];
				num4 |= DesEngine.SP1[(int)((UIntPtr)(num3 >> 24 & 63u))];
				num3 = (num ^ (uint)wKey[i * 4 + 3]);
				num4 |= DesEngine.SP8[(int)((UIntPtr)(num3 & 63u))];
				num4 |= DesEngine.SP6[(int)((UIntPtr)(num3 >> 8 & 63u))];
				num4 |= DesEngine.SP4[(int)((UIntPtr)(num3 >> 16 & 63u))];
				num4 |= DesEngine.SP2[(int)((UIntPtr)(num3 >> 24 & 63u))];
				num2 ^= num4;
			}
			num2 = (num2 << 31 | num2 >> 1);
			num3 = ((num ^ num2) & 2863311530u);
			num ^= num3;
			num2 ^= num3;
			num = (num << 31 | num >> 1);
			num3 = ((num >> 8 ^ num2) & 16711935u);
			num2 ^= num3;
			num ^= num3 << 8;
			num3 = ((num >> 2 ^ num2) & 858993459u);
			num2 ^= num3;
			num ^= num3 << 2;
			num3 = ((num2 >> 16 ^ num) & 65535u);
			num ^= num3;
			num2 ^= num3 << 16;
			num3 = ((num2 >> 4 ^ num) & 252645135u);
			num ^= num3;
			num2 ^= num3 << 4;
			Pack.UInt32_To_BE(num2, outBytes, outOff);
			Pack.UInt32_To_BE(num, outBytes, outOff + 4);
		}
	}
}
