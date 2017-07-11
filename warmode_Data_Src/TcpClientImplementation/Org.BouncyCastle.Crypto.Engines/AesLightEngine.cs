using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Crypto.Utilities;
using System;

namespace Org.BouncyCastle.Crypto.Engines
{
	public class AesLightEngine : IBlockCipher
	{
		private const uint m1 = 2155905152u;

		private const uint m2 = 2139062143u;

		private const uint m3 = 27u;

		private const int BLOCK_SIZE = 16;

		private static readonly byte[] S = new byte[]
		{
			99,
			124,
			119,
			123,
			242,
			107,
			111,
			197,
			48,
			1,
			103,
			43,
			254,
			215,
			171,
			118,
			202,
			130,
			201,
			125,
			250,
			89,
			71,
			240,
			173,
			212,
			162,
			175,
			156,
			164,
			114,
			192,
			183,
			253,
			147,
			38,
			54,
			63,
			247,
			204,
			52,
			165,
			229,
			241,
			113,
			216,
			49,
			21,
			4,
			199,
			35,
			195,
			24,
			150,
			5,
			154,
			7,
			18,
			128,
			226,
			235,
			39,
			178,
			117,
			9,
			131,
			44,
			26,
			27,
			110,
			90,
			160,
			82,
			59,
			214,
			179,
			41,
			227,
			47,
			132,
			83,
			209,
			0,
			237,
			32,
			252,
			177,
			91,
			106,
			203,
			190,
			57,
			74,
			76,
			88,
			207,
			208,
			239,
			170,
			251,
			67,
			77,
			51,
			133,
			69,
			249,
			2,
			127,
			80,
			60,
			159,
			168,
			81,
			163,
			64,
			143,
			146,
			157,
			56,
			245,
			188,
			182,
			218,
			33,
			16,
			255,
			243,
			210,
			205,
			12,
			19,
			236,
			95,
			151,
			68,
			23,
			196,
			167,
			126,
			61,
			100,
			93,
			25,
			115,
			96,
			129,
			79,
			220,
			34,
			42,
			144,
			136,
			70,
			238,
			184,
			20,
			222,
			94,
			11,
			219,
			224,
			50,
			58,
			10,
			73,
			6,
			36,
			92,
			194,
			211,
			172,
			98,
			145,
			149,
			228,
			121,
			231,
			200,
			55,
			109,
			141,
			213,
			78,
			169,
			108,
			86,
			244,
			234,
			101,
			122,
			174,
			8,
			186,
			120,
			37,
			46,
			28,
			166,
			180,
			198,
			232,
			221,
			116,
			31,
			75,
			189,
			139,
			138,
			112,
			62,
			181,
			102,
			72,
			3,
			246,
			14,
			97,
			53,
			87,
			185,
			134,
			193,
			29,
			158,
			225,
			248,
			152,
			17,
			105,
			217,
			142,
			148,
			155,
			30,
			135,
			233,
			206,
			85,
			40,
			223,
			140,
			161,
			137,
			13,
			191,
			230,
			66,
			104,
			65,
			153,
			45,
			15,
			176,
			84,
			187,
			22
		};

		private static readonly byte[] Si = new byte[]
		{
			82,
			9,
			106,
			213,
			48,
			54,
			165,
			56,
			191,
			64,
			163,
			158,
			129,
			243,
			215,
			251,
			124,
			227,
			57,
			130,
			155,
			47,
			255,
			135,
			52,
			142,
			67,
			68,
			196,
			222,
			233,
			203,
			84,
			123,
			148,
			50,
			166,
			194,
			35,
			61,
			238,
			76,
			149,
			11,
			66,
			250,
			195,
			78,
			8,
			46,
			161,
			102,
			40,
			217,
			36,
			178,
			118,
			91,
			162,
			73,
			109,
			139,
			209,
			37,
			114,
			248,
			246,
			100,
			134,
			104,
			152,
			22,
			212,
			164,
			92,
			204,
			93,
			101,
			182,
			146,
			108,
			112,
			72,
			80,
			253,
			237,
			185,
			218,
			94,
			21,
			70,
			87,
			167,
			141,
			157,
			132,
			144,
			216,
			171,
			0,
			140,
			188,
			211,
			10,
			247,
			228,
			88,
			5,
			184,
			179,
			69,
			6,
			208,
			44,
			30,
			143,
			202,
			63,
			15,
			2,
			193,
			175,
			189,
			3,
			1,
			19,
			138,
			107,
			58,
			145,
			17,
			65,
			79,
			103,
			220,
			234,
			151,
			242,
			207,
			206,
			240,
			180,
			230,
			115,
			150,
			172,
			116,
			34,
			231,
			173,
			53,
			133,
			226,
			249,
			55,
			232,
			28,
			117,
			223,
			110,
			71,
			241,
			26,
			113,
			29,
			41,
			197,
			137,
			111,
			183,
			98,
			14,
			170,
			24,
			190,
			27,
			252,
			86,
			62,
			75,
			198,
			210,
			121,
			32,
			154,
			219,
			192,
			254,
			120,
			205,
			90,
			244,
			31,
			221,
			168,
			51,
			136,
			7,
			199,
			49,
			177,
			18,
			16,
			89,
			39,
			128,
			236,
			95,
			96,
			81,
			127,
			169,
			25,
			181,
			74,
			13,
			45,
			229,
			122,
			159,
			147,
			201,
			156,
			239,
			160,
			224,
			59,
			77,
			174,
			42,
			245,
			176,
			200,
			235,
			187,
			60,
			131,
			83,
			153,
			97,
			23,
			43,
			4,
			126,
			186,
			119,
			214,
			38,
			225,
			105,
			20,
			99,
			85,
			33,
			12,
			125
		};

		private static readonly byte[] rcon = new byte[]
		{
			1,
			2,
			4,
			8,
			16,
			32,
			64,
			128,
			27,
			54,
			108,
			216,
			171,
			77,
			154,
			47,
			94,
			188,
			99,
			198,
			151,
			53,
			106,
			212,
			179,
			125,
			250,
			239,
			197,
			145
		};

		private int ROUNDS;

		private uint[][] WorkingKey;

		private uint C0;

		private uint C1;

		private uint C2;

		private uint C3;

		private bool forEncryption;

		public virtual string AlgorithmName
		{
			get
			{
				return "AES";
			}
		}

		public virtual bool IsPartialBlockOkay
		{
			get
			{
				return false;
			}
		}

		private static uint Shift(uint r, int shift)
		{
			return r >> shift | r << 32 - shift;
		}

		private static uint FFmulX(uint x)
		{
			return (x & 2139062143u) << 1 ^ ((x & 2155905152u) >> 7) * 27u;
		}

		private static uint Mcol(uint x)
		{
			uint num = AesLightEngine.FFmulX(x);
			return num ^ AesLightEngine.Shift(x ^ num, 8) ^ AesLightEngine.Shift(x, 16) ^ AesLightEngine.Shift(x, 24);
		}

		private static uint Inv_Mcol(uint x)
		{
			uint num = AesLightEngine.FFmulX(x);
			uint num2 = AesLightEngine.FFmulX(num);
			uint num3 = AesLightEngine.FFmulX(num2);
			uint num4 = x ^ num3;
			return num ^ num2 ^ num3 ^ AesLightEngine.Shift(num ^ num4, 8) ^ AesLightEngine.Shift(num2 ^ num4, 16) ^ AesLightEngine.Shift(num4, 24);
		}

		private static uint SubWord(uint x)
		{
			return (uint)((int)AesLightEngine.S[(int)((UIntPtr)(x & 255u))] | (int)AesLightEngine.S[(int)((UIntPtr)(x >> 8 & 255u))] << 8 | (int)AesLightEngine.S[(int)((UIntPtr)(x >> 16 & 255u))] << 16 | (int)AesLightEngine.S[(int)((UIntPtr)(x >> 24 & 255u))] << 24);
		}

		private uint[][] GenerateWorkingKey(byte[] key, bool forEncryption)
		{
			int num = key.Length / 4;
			if (num != 4 && num != 6 && num != 8)
			{
				throw new ArgumentException("Key length not 128/192/256 bits.");
			}
			this.ROUNDS = num + 6;
			uint[][] array = new uint[this.ROUNDS + 1][];
			for (int i = 0; i <= this.ROUNDS; i++)
			{
				array[i] = new uint[4];
			}
			int num2 = 0;
			int j = 0;
			while (j < key.Length)
			{
				array[num2 >> 2][num2 & 3] = Pack.LE_To_UInt32(key, j);
				j += 4;
				num2++;
			}
			int num3 = this.ROUNDS + 1 << 2;
			for (int k = num; k < num3; k++)
			{
				uint num4 = array[k - 1 >> 2][k - 1 & 3];
				if (k % num == 0)
				{
					num4 = (AesLightEngine.SubWord(AesLightEngine.Shift(num4, 8)) ^ (uint)AesLightEngine.rcon[k / num - 1]);
				}
				else if (num > 6 && k % num == 4)
				{
					num4 = AesLightEngine.SubWord(num4);
				}
				array[k >> 2][k & 3] = (array[k - num >> 2][k - num & 3] ^ num4);
			}
			if (!forEncryption)
			{
				for (int l = 1; l < this.ROUNDS; l++)
				{
					uint[] array2 = array[l];
					for (int m = 0; m < 4; m++)
					{
						array2[m] = AesLightEngine.Inv_Mcol(array2[m]);
					}
				}
			}
			return array;
		}

		public virtual void Init(bool forEncryption, ICipherParameters parameters)
		{
			KeyParameter keyParameter = parameters as KeyParameter;
			if (keyParameter == null)
			{
				throw new ArgumentException("invalid parameter passed to AES init - " + parameters.GetType().Name);
			}
			this.WorkingKey = this.GenerateWorkingKey(keyParameter.GetKey(), forEncryption);
			this.forEncryption = forEncryption;
		}

		public virtual int GetBlockSize()
		{
			return 16;
		}

		public virtual int ProcessBlock(byte[] input, int inOff, byte[] output, int outOff)
		{
			if (this.WorkingKey == null)
			{
				throw new InvalidOperationException("AES engine not initialised");
			}
			Check.DataLength(input, inOff, 16, "input buffer too short");
			Check.OutputLength(output, outOff, 16, "output buffer too short");
			this.UnPackBlock(input, inOff);
			if (this.forEncryption)
			{
				this.EncryptBlock(this.WorkingKey);
			}
			else
			{
				this.DecryptBlock(this.WorkingKey);
			}
			this.PackBlock(output, outOff);
			return 16;
		}

		public virtual void Reset()
		{
		}

		private void UnPackBlock(byte[] bytes, int off)
		{
			this.C0 = Pack.LE_To_UInt32(bytes, off);
			this.C1 = Pack.LE_To_UInt32(bytes, off + 4);
			this.C2 = Pack.LE_To_UInt32(bytes, off + 8);
			this.C3 = Pack.LE_To_UInt32(bytes, off + 12);
		}

		private void PackBlock(byte[] bytes, int off)
		{
			Pack.UInt32_To_LE(this.C0, bytes, off);
			Pack.UInt32_To_LE(this.C1, bytes, off + 4);
			Pack.UInt32_To_LE(this.C2, bytes, off + 8);
			Pack.UInt32_To_LE(this.C3, bytes, off + 12);
		}

		private void EncryptBlock(uint[][] KW)
		{
			uint[] array = KW[0];
			uint num = this.C0 ^ array[0];
			uint num2 = this.C1 ^ array[1];
			uint num3 = this.C2 ^ array[2];
			uint num4 = this.C3 ^ array[3];
			int i = 1;
			uint num5;
			uint num6;
			uint num7;
			while (i < this.ROUNDS - 1)
			{
				array = KW[i++];
				num5 = (AesLightEngine.Mcol((uint)((int)AesLightEngine.S[(int)((UIntPtr)(num & 255u))] ^ (int)AesLightEngine.S[(int)((UIntPtr)(num2 >> 8 & 255u))] << 8 ^ (int)AesLightEngine.S[(int)((UIntPtr)(num3 >> 16 & 255u))] << 16 ^ (int)AesLightEngine.S[(int)((UIntPtr)(num4 >> 24 & 255u))] << 24)) ^ array[0]);
				num6 = (AesLightEngine.Mcol((uint)((int)AesLightEngine.S[(int)((UIntPtr)(num2 & 255u))] ^ (int)AesLightEngine.S[(int)((UIntPtr)(num3 >> 8 & 255u))] << 8 ^ (int)AesLightEngine.S[(int)((UIntPtr)(num4 >> 16 & 255u))] << 16 ^ (int)AesLightEngine.S[(int)((UIntPtr)(num >> 24 & 255u))] << 24)) ^ array[1]);
				num7 = (AesLightEngine.Mcol((uint)((int)AesLightEngine.S[(int)((UIntPtr)(num3 & 255u))] ^ (int)AesLightEngine.S[(int)((UIntPtr)(num4 >> 8 & 255u))] << 8 ^ (int)AesLightEngine.S[(int)((UIntPtr)(num >> 16 & 255u))] << 16 ^ (int)AesLightEngine.S[(int)((UIntPtr)(num2 >> 24 & 255u))] << 24)) ^ array[2]);
				num4 = (AesLightEngine.Mcol((uint)((int)AesLightEngine.S[(int)((UIntPtr)(num4 & 255u))] ^ (int)AesLightEngine.S[(int)((UIntPtr)(num >> 8 & 255u))] << 8 ^ (int)AesLightEngine.S[(int)((UIntPtr)(num2 >> 16 & 255u))] << 16 ^ (int)AesLightEngine.S[(int)((UIntPtr)(num3 >> 24 & 255u))] << 24)) ^ array[3]);
				array = KW[i++];
				num = (AesLightEngine.Mcol((uint)((int)AesLightEngine.S[(int)((UIntPtr)(num5 & 255u))] ^ (int)AesLightEngine.S[(int)((UIntPtr)(num6 >> 8 & 255u))] << 8 ^ (int)AesLightEngine.S[(int)((UIntPtr)(num7 >> 16 & 255u))] << 16 ^ (int)AesLightEngine.S[(int)((UIntPtr)(num4 >> 24 & 255u))] << 24)) ^ array[0]);
				num2 = (AesLightEngine.Mcol((uint)((int)AesLightEngine.S[(int)((UIntPtr)(num6 & 255u))] ^ (int)AesLightEngine.S[(int)((UIntPtr)(num7 >> 8 & 255u))] << 8 ^ (int)AesLightEngine.S[(int)((UIntPtr)(num4 >> 16 & 255u))] << 16 ^ (int)AesLightEngine.S[(int)((UIntPtr)(num5 >> 24 & 255u))] << 24)) ^ array[1]);
				num3 = (AesLightEngine.Mcol((uint)((int)AesLightEngine.S[(int)((UIntPtr)(num7 & 255u))] ^ (int)AesLightEngine.S[(int)((UIntPtr)(num4 >> 8 & 255u))] << 8 ^ (int)AesLightEngine.S[(int)((UIntPtr)(num5 >> 16 & 255u))] << 16 ^ (int)AesLightEngine.S[(int)((UIntPtr)(num6 >> 24 & 255u))] << 24)) ^ array[2]);
				num4 = (AesLightEngine.Mcol((uint)((int)AesLightEngine.S[(int)((UIntPtr)(num4 & 255u))] ^ (int)AesLightEngine.S[(int)((UIntPtr)(num5 >> 8 & 255u))] << 8 ^ (int)AesLightEngine.S[(int)((UIntPtr)(num6 >> 16 & 255u))] << 16 ^ (int)AesLightEngine.S[(int)((UIntPtr)(num7 >> 24 & 255u))] << 24)) ^ array[3]);
			}
			array = KW[i++];
			num5 = (AesLightEngine.Mcol((uint)((int)AesLightEngine.S[(int)((UIntPtr)(num & 255u))] ^ (int)AesLightEngine.S[(int)((UIntPtr)(num2 >> 8 & 255u))] << 8 ^ (int)AesLightEngine.S[(int)((UIntPtr)(num3 >> 16 & 255u))] << 16 ^ (int)AesLightEngine.S[(int)((UIntPtr)(num4 >> 24 & 255u))] << 24)) ^ array[0]);
			num6 = (AesLightEngine.Mcol((uint)((int)AesLightEngine.S[(int)((UIntPtr)(num2 & 255u))] ^ (int)AesLightEngine.S[(int)((UIntPtr)(num3 >> 8 & 255u))] << 8 ^ (int)AesLightEngine.S[(int)((UIntPtr)(num4 >> 16 & 255u))] << 16 ^ (int)AesLightEngine.S[(int)((UIntPtr)(num >> 24 & 255u))] << 24)) ^ array[1]);
			num7 = (AesLightEngine.Mcol((uint)((int)AesLightEngine.S[(int)((UIntPtr)(num3 & 255u))] ^ (int)AesLightEngine.S[(int)((UIntPtr)(num4 >> 8 & 255u))] << 8 ^ (int)AesLightEngine.S[(int)((UIntPtr)(num >> 16 & 255u))] << 16 ^ (int)AesLightEngine.S[(int)((UIntPtr)(num2 >> 24 & 255u))] << 24)) ^ array[2]);
			num4 = (AesLightEngine.Mcol((uint)((int)AesLightEngine.S[(int)((UIntPtr)(num4 & 255u))] ^ (int)AesLightEngine.S[(int)((UIntPtr)(num >> 8 & 255u))] << 8 ^ (int)AesLightEngine.S[(int)((UIntPtr)(num2 >> 16 & 255u))] << 16 ^ (int)AesLightEngine.S[(int)((UIntPtr)(num3 >> 24 & 255u))] << 24)) ^ array[3]);
			array = KW[i];
			this.C0 = (uint)((int)AesLightEngine.S[(int)((UIntPtr)(num5 & 255u))] ^ (int)AesLightEngine.S[(int)((UIntPtr)(num6 >> 8 & 255u))] << 8 ^ (int)AesLightEngine.S[(int)((UIntPtr)(num7 >> 16 & 255u))] << 16 ^ (int)AesLightEngine.S[(int)((UIntPtr)(num4 >> 24 & 255u))] << 24 ^ (int)array[0]);
			this.C1 = (uint)((int)AesLightEngine.S[(int)((UIntPtr)(num6 & 255u))] ^ (int)AesLightEngine.S[(int)((UIntPtr)(num7 >> 8 & 255u))] << 8 ^ (int)AesLightEngine.S[(int)((UIntPtr)(num4 >> 16 & 255u))] << 16 ^ (int)AesLightEngine.S[(int)((UIntPtr)(num5 >> 24 & 255u))] << 24 ^ (int)array[1]);
			this.C2 = (uint)((int)AesLightEngine.S[(int)((UIntPtr)(num7 & 255u))] ^ (int)AesLightEngine.S[(int)((UIntPtr)(num4 >> 8 & 255u))] << 8 ^ (int)AesLightEngine.S[(int)((UIntPtr)(num5 >> 16 & 255u))] << 16 ^ (int)AesLightEngine.S[(int)((UIntPtr)(num6 >> 24 & 255u))] << 24 ^ (int)array[2]);
			this.C3 = (uint)((int)AesLightEngine.S[(int)((UIntPtr)(num4 & 255u))] ^ (int)AesLightEngine.S[(int)((UIntPtr)(num5 >> 8 & 255u))] << 8 ^ (int)AesLightEngine.S[(int)((UIntPtr)(num6 >> 16 & 255u))] << 16 ^ (int)AesLightEngine.S[(int)((UIntPtr)(num7 >> 24 & 255u))] << 24 ^ (int)array[3]);
		}

		private void DecryptBlock(uint[][] KW)
		{
			uint[] array = KW[this.ROUNDS];
			uint num = this.C0 ^ array[0];
			uint num2 = this.C1 ^ array[1];
			uint num3 = this.C2 ^ array[2];
			uint num4 = this.C3 ^ array[3];
			int i = this.ROUNDS - 1;
			uint num5;
			uint num6;
			uint num7;
			while (i > 1)
			{
				array = KW[i--];
				num5 = (AesLightEngine.Inv_Mcol((uint)((int)AesLightEngine.Si[(int)((UIntPtr)(num & 255u))] ^ (int)AesLightEngine.Si[(int)((UIntPtr)(num4 >> 8 & 255u))] << 8 ^ (int)AesLightEngine.Si[(int)((UIntPtr)(num3 >> 16 & 255u))] << 16 ^ (int)AesLightEngine.Si[(int)((UIntPtr)(num2 >> 24 & 255u))] << 24)) ^ array[0]);
				num6 = (AesLightEngine.Inv_Mcol((uint)((int)AesLightEngine.Si[(int)((UIntPtr)(num2 & 255u))] ^ (int)AesLightEngine.Si[(int)((UIntPtr)(num >> 8 & 255u))] << 8 ^ (int)AesLightEngine.Si[(int)((UIntPtr)(num4 >> 16 & 255u))] << 16 ^ (int)AesLightEngine.Si[(int)((UIntPtr)(num3 >> 24 & 255u))] << 24)) ^ array[1]);
				num7 = (AesLightEngine.Inv_Mcol((uint)((int)AesLightEngine.Si[(int)((UIntPtr)(num3 & 255u))] ^ (int)AesLightEngine.Si[(int)((UIntPtr)(num2 >> 8 & 255u))] << 8 ^ (int)AesLightEngine.Si[(int)((UIntPtr)(num >> 16 & 255u))] << 16 ^ (int)AesLightEngine.Si[(int)((UIntPtr)(num4 >> 24 & 255u))] << 24)) ^ array[2]);
				num4 = (AesLightEngine.Inv_Mcol((uint)((int)AesLightEngine.Si[(int)((UIntPtr)(num4 & 255u))] ^ (int)AesLightEngine.Si[(int)((UIntPtr)(num3 >> 8 & 255u))] << 8 ^ (int)AesLightEngine.Si[(int)((UIntPtr)(num2 >> 16 & 255u))] << 16 ^ (int)AesLightEngine.Si[(int)((UIntPtr)(num >> 24 & 255u))] << 24)) ^ array[3]);
				array = KW[i--];
				num = (AesLightEngine.Inv_Mcol((uint)((int)AesLightEngine.Si[(int)((UIntPtr)(num5 & 255u))] ^ (int)AesLightEngine.Si[(int)((UIntPtr)(num4 >> 8 & 255u))] << 8 ^ (int)AesLightEngine.Si[(int)((UIntPtr)(num7 >> 16 & 255u))] << 16 ^ (int)AesLightEngine.Si[(int)((UIntPtr)(num6 >> 24 & 255u))] << 24)) ^ array[0]);
				num2 = (AesLightEngine.Inv_Mcol((uint)((int)AesLightEngine.Si[(int)((UIntPtr)(num6 & 255u))] ^ (int)AesLightEngine.Si[(int)((UIntPtr)(num5 >> 8 & 255u))] << 8 ^ (int)AesLightEngine.Si[(int)((UIntPtr)(num4 >> 16 & 255u))] << 16 ^ (int)AesLightEngine.Si[(int)((UIntPtr)(num7 >> 24 & 255u))] << 24)) ^ array[1]);
				num3 = (AesLightEngine.Inv_Mcol((uint)((int)AesLightEngine.Si[(int)((UIntPtr)(num7 & 255u))] ^ (int)AesLightEngine.Si[(int)((UIntPtr)(num6 >> 8 & 255u))] << 8 ^ (int)AesLightEngine.Si[(int)((UIntPtr)(num5 >> 16 & 255u))] << 16 ^ (int)AesLightEngine.Si[(int)((UIntPtr)(num4 >> 24 & 255u))] << 24)) ^ array[2]);
				num4 = (AesLightEngine.Inv_Mcol((uint)((int)AesLightEngine.Si[(int)((UIntPtr)(num4 & 255u))] ^ (int)AesLightEngine.Si[(int)((UIntPtr)(num7 >> 8 & 255u))] << 8 ^ (int)AesLightEngine.Si[(int)((UIntPtr)(num6 >> 16 & 255u))] << 16 ^ (int)AesLightEngine.Si[(int)((UIntPtr)(num5 >> 24 & 255u))] << 24)) ^ array[3]);
			}
			array = KW[1];
			num5 = (AesLightEngine.Inv_Mcol((uint)((int)AesLightEngine.Si[(int)((UIntPtr)(num & 255u))] ^ (int)AesLightEngine.Si[(int)((UIntPtr)(num4 >> 8 & 255u))] << 8 ^ (int)AesLightEngine.Si[(int)((UIntPtr)(num3 >> 16 & 255u))] << 16 ^ (int)AesLightEngine.Si[(int)((UIntPtr)(num2 >> 24 & 255u))] << 24)) ^ array[0]);
			num6 = (AesLightEngine.Inv_Mcol((uint)((int)AesLightEngine.Si[(int)((UIntPtr)(num2 & 255u))] ^ (int)AesLightEngine.Si[(int)((UIntPtr)(num >> 8 & 255u))] << 8 ^ (int)AesLightEngine.Si[(int)((UIntPtr)(num4 >> 16 & 255u))] << 16 ^ (int)AesLightEngine.Si[(int)((UIntPtr)(num3 >> 24 & 255u))] << 24)) ^ array[1]);
			num7 = (AesLightEngine.Inv_Mcol((uint)((int)AesLightEngine.Si[(int)((UIntPtr)(num3 & 255u))] ^ (int)AesLightEngine.Si[(int)((UIntPtr)(num2 >> 8 & 255u))] << 8 ^ (int)AesLightEngine.Si[(int)((UIntPtr)(num >> 16 & 255u))] << 16 ^ (int)AesLightEngine.Si[(int)((UIntPtr)(num4 >> 24 & 255u))] << 24)) ^ array[2]);
			num4 = (AesLightEngine.Inv_Mcol((uint)((int)AesLightEngine.Si[(int)((UIntPtr)(num4 & 255u))] ^ (int)AesLightEngine.Si[(int)((UIntPtr)(num3 >> 8 & 255u))] << 8 ^ (int)AesLightEngine.Si[(int)((UIntPtr)(num2 >> 16 & 255u))] << 16 ^ (int)AesLightEngine.Si[(int)((UIntPtr)(num >> 24 & 255u))] << 24)) ^ array[3]);
			array = KW[0];
			this.C0 = (uint)((int)AesLightEngine.Si[(int)((UIntPtr)(num5 & 255u))] ^ (int)AesLightEngine.Si[(int)((UIntPtr)(num4 >> 8 & 255u))] << 8 ^ (int)AesLightEngine.Si[(int)((UIntPtr)(num7 >> 16 & 255u))] << 16 ^ (int)AesLightEngine.Si[(int)((UIntPtr)(num6 >> 24 & 255u))] << 24 ^ (int)array[0]);
			this.C1 = (uint)((int)AesLightEngine.Si[(int)((UIntPtr)(num6 & 255u))] ^ (int)AesLightEngine.Si[(int)((UIntPtr)(num5 >> 8 & 255u))] << 8 ^ (int)AesLightEngine.Si[(int)((UIntPtr)(num4 >> 16 & 255u))] << 16 ^ (int)AesLightEngine.Si[(int)((UIntPtr)(num7 >> 24 & 255u))] << 24 ^ (int)array[1]);
			this.C2 = (uint)((int)AesLightEngine.Si[(int)((UIntPtr)(num7 & 255u))] ^ (int)AesLightEngine.Si[(int)((UIntPtr)(num6 >> 8 & 255u))] << 8 ^ (int)AesLightEngine.Si[(int)((UIntPtr)(num5 >> 16 & 255u))] << 16 ^ (int)AesLightEngine.Si[(int)((UIntPtr)(num4 >> 24 & 255u))] << 24 ^ (int)array[2]);
			this.C3 = (uint)((int)AesLightEngine.Si[(int)((UIntPtr)(num4 & 255u))] ^ (int)AesLightEngine.Si[(int)((UIntPtr)(num7 >> 8 & 255u))] << 8 ^ (int)AesLightEngine.Si[(int)((UIntPtr)(num6 >> 16 & 255u))] << 16 ^ (int)AesLightEngine.Si[(int)((UIntPtr)(num5 >> 24 & 255u))] << 24 ^ (int)array[3]);
		}
	}
}
