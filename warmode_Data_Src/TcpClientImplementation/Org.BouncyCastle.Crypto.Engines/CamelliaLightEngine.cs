using Org.BouncyCastle.Crypto.Parameters;
using System;

namespace Org.BouncyCastle.Crypto.Engines
{
	public class CamelliaLightEngine : IBlockCipher
	{
		private const int BLOCK_SIZE = 16;

		private bool initialised;

		private bool _keyis128;

		private uint[] subkey = new uint[96];

		private uint[] kw = new uint[8];

		private uint[] ke = new uint[12];

		private uint[] state = new uint[4];

		private static readonly uint[] SIGMA = new uint[]
		{
			2694735487u,
			1003262091u,
			3061508184u,
			1286239154u,
			3337565999u,
			3914302142u,
			1426019237u,
			4057165596u,
			283453434u,
			3731369245u,
			2958461122u,
			3018244605u
		};

		private static readonly byte[] SBOX1 = new byte[]
		{
			112,
			130,
			44,
			236,
			179,
			39,
			192,
			229,
			228,
			133,
			87,
			53,
			234,
			12,
			174,
			65,
			35,
			239,
			107,
			147,
			69,
			25,
			165,
			33,
			237,
			14,
			79,
			78,
			29,
			101,
			146,
			189,
			134,
			184,
			175,
			143,
			124,
			235,
			31,
			206,
			62,
			48,
			220,
			95,
			94,
			197,
			11,
			26,
			166,
			225,
			57,
			202,
			213,
			71,
			93,
			61,
			217,
			1,
			90,
			214,
			81,
			86,
			108,
			77,
			139,
			13,
			154,
			102,
			251,
			204,
			176,
			45,
			116,
			18,
			43,
			32,
			240,
			177,
			132,
			153,
			223,
			76,
			203,
			194,
			52,
			126,
			118,
			5,
			109,
			183,
			169,
			49,
			209,
			23,
			4,
			215,
			20,
			88,
			58,
			97,
			222,
			27,
			17,
			28,
			50,
			15,
			156,
			22,
			83,
			24,
			242,
			34,
			254,
			68,
			207,
			178,
			195,
			181,
			122,
			145,
			36,
			8,
			232,
			168,
			96,
			252,
			105,
			80,
			170,
			208,
			160,
			125,
			161,
			137,
			98,
			151,
			84,
			91,
			30,
			149,
			224,
			255,
			100,
			210,
			16,
			196,
			0,
			72,
			163,
			247,
			117,
			219,
			138,
			3,
			230,
			218,
			9,
			63,
			221,
			148,
			135,
			92,
			131,
			2,
			205,
			74,
			144,
			51,
			115,
			103,
			246,
			243,
			157,
			127,
			191,
			226,
			82,
			155,
			216,
			38,
			200,
			55,
			198,
			59,
			129,
			150,
			111,
			75,
			19,
			190,
			99,
			46,
			233,
			121,
			167,
			140,
			159,
			110,
			188,
			142,
			41,
			245,
			249,
			182,
			47,
			253,
			180,
			89,
			120,
			152,
			6,
			106,
			231,
			70,
			113,
			186,
			212,
			37,
			171,
			66,
			136,
			162,
			141,
			250,
			114,
			7,
			185,
			85,
			248,
			238,
			172,
			10,
			54,
			73,
			42,
			104,
			60,
			56,
			241,
			164,
			64,
			40,
			211,
			123,
			187,
			201,
			67,
			193,
			21,
			227,
			173,
			244,
			119,
			199,
			128,
			158
		};

		public virtual string AlgorithmName
		{
			get
			{
				return "Camellia";
			}
		}

		public virtual bool IsPartialBlockOkay
		{
			get
			{
				return false;
			}
		}

		private static uint rightRotate(uint x, int s)
		{
			return (x >> s) + (x << 32 - s);
		}

		private static uint leftRotate(uint x, int s)
		{
			return (x << s) + (x >> 32 - s);
		}

		private static void roldq(int rot, uint[] ki, int ioff, uint[] ko, int ooff)
		{
			ko[ooff] = (ki[ioff] << rot | ki[1 + ioff] >> 32 - rot);
			ko[1 + ooff] = (ki[1 + ioff] << rot | ki[2 + ioff] >> 32 - rot);
			ko[2 + ooff] = (ki[2 + ioff] << rot | ki[3 + ioff] >> 32 - rot);
			ko[3 + ooff] = (ki[3 + ioff] << rot | ki[ioff] >> 32 - rot);
			ki[ioff] = ko[ooff];
			ki[1 + ioff] = ko[1 + ooff];
			ki[2 + ioff] = ko[2 + ooff];
			ki[3 + ioff] = ko[3 + ooff];
		}

		private static void decroldq(int rot, uint[] ki, int ioff, uint[] ko, int ooff)
		{
			ko[2 + ooff] = (ki[ioff] << rot | ki[1 + ioff] >> 32 - rot);
			ko[3 + ooff] = (ki[1 + ioff] << rot | ki[2 + ioff] >> 32 - rot);
			ko[ooff] = (ki[2 + ioff] << rot | ki[3 + ioff] >> 32 - rot);
			ko[1 + ooff] = (ki[3 + ioff] << rot | ki[ioff] >> 32 - rot);
			ki[ioff] = ko[2 + ooff];
			ki[1 + ioff] = ko[3 + ooff];
			ki[2 + ioff] = ko[ooff];
			ki[3 + ioff] = ko[1 + ooff];
		}

		private static void roldqo32(int rot, uint[] ki, int ioff, uint[] ko, int ooff)
		{
			ko[ooff] = (ki[1 + ioff] << rot - 32 | ki[2 + ioff] >> 64 - rot);
			ko[1 + ooff] = (ki[2 + ioff] << rot - 32 | ki[3 + ioff] >> 64 - rot);
			ko[2 + ooff] = (ki[3 + ioff] << rot - 32 | ki[ioff] >> 64 - rot);
			ko[3 + ooff] = (ki[ioff] << rot - 32 | ki[1 + ioff] >> 64 - rot);
			ki[ioff] = ko[ooff];
			ki[1 + ioff] = ko[1 + ooff];
			ki[2 + ioff] = ko[2 + ooff];
			ki[3 + ioff] = ko[3 + ooff];
		}

		private static void decroldqo32(int rot, uint[] ki, int ioff, uint[] ko, int ooff)
		{
			ko[2 + ooff] = (ki[1 + ioff] << rot - 32 | ki[2 + ioff] >> 64 - rot);
			ko[3 + ooff] = (ki[2 + ioff] << rot - 32 | ki[3 + ioff] >> 64 - rot);
			ko[ooff] = (ki[3 + ioff] << rot - 32 | ki[ioff] >> 64 - rot);
			ko[1 + ooff] = (ki[ioff] << rot - 32 | ki[1 + ioff] >> 64 - rot);
			ki[ioff] = ko[2 + ooff];
			ki[1 + ioff] = ko[3 + ooff];
			ki[2 + ioff] = ko[ooff];
			ki[3 + ioff] = ko[1 + ooff];
		}

		private static uint bytes2uint(byte[] src, int offset)
		{
			uint num = 0u;
			for (int i = 0; i < 4; i++)
			{
				num = (num << 8) + (uint)src[i + offset];
			}
			return num;
		}

		private static void uint2bytes(uint word, byte[] dst, int offset)
		{
			for (int i = 0; i < 4; i++)
			{
				dst[3 - i + offset] = (byte)word;
				word >>= 8;
			}
		}

		private byte lRot8(byte v, int rot)
		{
			return (byte)((int)v << rot | (int)((uint)v >> 8 - rot));
		}

		private uint sbox2(int x)
		{
			return (uint)this.lRot8(CamelliaLightEngine.SBOX1[x], 1);
		}

		private uint sbox3(int x)
		{
			return (uint)this.lRot8(CamelliaLightEngine.SBOX1[x], 7);
		}

		private uint sbox4(int x)
		{
			return (uint)CamelliaLightEngine.SBOX1[(int)this.lRot8((byte)x, 1)];
		}

		private void camelliaF2(uint[] s, uint[] skey, int keyoff)
		{
			uint num = s[0] ^ skey[keyoff];
			uint num2 = this.sbox4((int)((byte)num));
			num2 |= this.sbox3((int)((byte)(num >> 8))) << 8;
			num2 |= this.sbox2((int)((byte)(num >> 16))) << 16;
			num2 |= (uint)((uint)CamelliaLightEngine.SBOX1[(int)((byte)(num >> 24))] << 24);
			uint num3 = s[1] ^ skey[1 + keyoff];
			uint num4 = (uint)CamelliaLightEngine.SBOX1[(int)((byte)num3)];
			num4 |= this.sbox4((int)((byte)(num3 >> 8))) << 8;
			num4 |= this.sbox3((int)((byte)(num3 >> 16))) << 16;
			num4 |= this.sbox2((int)((byte)(num3 >> 24))) << 24;
			num4 = CamelliaLightEngine.leftRotate(num4, 8);
			num2 ^= num4;
			num4 = (CamelliaLightEngine.leftRotate(num4, 8) ^ num2);
			num2 = (CamelliaLightEngine.rightRotate(num2, 8) ^ num4);
			s[2] ^= (CamelliaLightEngine.leftRotate(num4, 16) ^ num2);
			s[3] ^= CamelliaLightEngine.leftRotate(num2, 8);
			num = (s[2] ^ skey[2 + keyoff]);
			num2 = this.sbox4((int)((byte)num));
			num2 |= this.sbox3((int)((byte)(num >> 8))) << 8;
			num2 |= this.sbox2((int)((byte)(num >> 16))) << 16;
			num2 |= (uint)((uint)CamelliaLightEngine.SBOX1[(int)((byte)(num >> 24))] << 24);
			num3 = (s[3] ^ skey[3 + keyoff]);
			num4 = (uint)CamelliaLightEngine.SBOX1[(int)((byte)num3)];
			num4 |= this.sbox4((int)((byte)(num3 >> 8))) << 8;
			num4 |= this.sbox3((int)((byte)(num3 >> 16))) << 16;
			num4 |= this.sbox2((int)((byte)(num3 >> 24))) << 24;
			num4 = CamelliaLightEngine.leftRotate(num4, 8);
			num2 ^= num4;
			num4 = (CamelliaLightEngine.leftRotate(num4, 8) ^ num2);
			num2 = (CamelliaLightEngine.rightRotate(num2, 8) ^ num4);
			s[0] ^= (CamelliaLightEngine.leftRotate(num4, 16) ^ num2);
			s[1] ^= CamelliaLightEngine.leftRotate(num2, 8);
		}

		private void camelliaFLs(uint[] s, uint[] fkey, int keyoff)
		{
			s[1] ^= CamelliaLightEngine.leftRotate(s[0] & fkey[keyoff], 1);
			s[0] ^= (fkey[1 + keyoff] | s[1]);
			s[2] ^= (fkey[3 + keyoff] | s[3]);
			s[3] ^= CamelliaLightEngine.leftRotate(fkey[2 + keyoff] & s[2], 1);
		}

		private void setKey(bool forEncryption, byte[] key)
		{
			uint[] array = new uint[8];
			uint[] array2 = new uint[4];
			uint[] array3 = new uint[4];
			uint[] array4 = new uint[4];
			int num = key.Length;
			if (num != 16)
			{
				if (num != 24)
				{
					if (num != 32)
					{
						throw new ArgumentException("key sizes are only 16/24/32 bytes.");
					}
					array[0] = CamelliaLightEngine.bytes2uint(key, 0);
					array[1] = CamelliaLightEngine.bytes2uint(key, 4);
					array[2] = CamelliaLightEngine.bytes2uint(key, 8);
					array[3] = CamelliaLightEngine.bytes2uint(key, 12);
					array[4] = CamelliaLightEngine.bytes2uint(key, 16);
					array[5] = CamelliaLightEngine.bytes2uint(key, 20);
					array[6] = CamelliaLightEngine.bytes2uint(key, 24);
					array[7] = CamelliaLightEngine.bytes2uint(key, 28);
					this._keyis128 = false;
				}
				else
				{
					array[0] = CamelliaLightEngine.bytes2uint(key, 0);
					array[1] = CamelliaLightEngine.bytes2uint(key, 4);
					array[2] = CamelliaLightEngine.bytes2uint(key, 8);
					array[3] = CamelliaLightEngine.bytes2uint(key, 12);
					array[4] = CamelliaLightEngine.bytes2uint(key, 16);
					array[5] = CamelliaLightEngine.bytes2uint(key, 20);
					array[6] = ~array[4];
					array[7] = ~array[5];
					this._keyis128 = false;
				}
			}
			else
			{
				this._keyis128 = true;
				array[0] = CamelliaLightEngine.bytes2uint(key, 0);
				array[1] = CamelliaLightEngine.bytes2uint(key, 4);
				array[2] = CamelliaLightEngine.bytes2uint(key, 8);
				array[3] = CamelliaLightEngine.bytes2uint(key, 12);
				array[4] = (array[5] = (array[6] = (array[7] = 0u)));
			}
			for (int i = 0; i < 4; i++)
			{
				array2[i] = (array[i] ^ array[i + 4]);
			}
			this.camelliaF2(array2, CamelliaLightEngine.SIGMA, 0);
			for (int j = 0; j < 4; j++)
			{
				array2[j] ^= array[j];
			}
			this.camelliaF2(array2, CamelliaLightEngine.SIGMA, 4);
			if (this._keyis128)
			{
				if (forEncryption)
				{
					this.kw[0] = array[0];
					this.kw[1] = array[1];
					this.kw[2] = array[2];
					this.kw[3] = array[3];
					CamelliaLightEngine.roldq(15, array, 0, this.subkey, 4);
					CamelliaLightEngine.roldq(30, array, 0, this.subkey, 12);
					CamelliaLightEngine.roldq(15, array, 0, array4, 0);
					this.subkey[18] = array4[2];
					this.subkey[19] = array4[3];
					CamelliaLightEngine.roldq(17, array, 0, this.ke, 4);
					CamelliaLightEngine.roldq(17, array, 0, this.subkey, 24);
					CamelliaLightEngine.roldq(17, array, 0, this.subkey, 32);
					this.subkey[0] = array2[0];
					this.subkey[1] = array2[1];
					this.subkey[2] = array2[2];
					this.subkey[3] = array2[3];
					CamelliaLightEngine.roldq(15, array2, 0, this.subkey, 8);
					CamelliaLightEngine.roldq(15, array2, 0, this.ke, 0);
					CamelliaLightEngine.roldq(15, array2, 0, array4, 0);
					this.subkey[16] = array4[0];
					this.subkey[17] = array4[1];
					CamelliaLightEngine.roldq(15, array2, 0, this.subkey, 20);
					CamelliaLightEngine.roldqo32(34, array2, 0, this.subkey, 28);
					CamelliaLightEngine.roldq(17, array2, 0, this.kw, 4);
					return;
				}
				this.kw[4] = array[0];
				this.kw[5] = array[1];
				this.kw[6] = array[2];
				this.kw[7] = array[3];
				CamelliaLightEngine.decroldq(15, array, 0, this.subkey, 28);
				CamelliaLightEngine.decroldq(30, array, 0, this.subkey, 20);
				CamelliaLightEngine.decroldq(15, array, 0, array4, 0);
				this.subkey[16] = array4[0];
				this.subkey[17] = array4[1];
				CamelliaLightEngine.decroldq(17, array, 0, this.ke, 0);
				CamelliaLightEngine.decroldq(17, array, 0, this.subkey, 8);
				CamelliaLightEngine.decroldq(17, array, 0, this.subkey, 0);
				this.subkey[34] = array2[0];
				this.subkey[35] = array2[1];
				this.subkey[32] = array2[2];
				this.subkey[33] = array2[3];
				CamelliaLightEngine.decroldq(15, array2, 0, this.subkey, 24);
				CamelliaLightEngine.decroldq(15, array2, 0, this.ke, 4);
				CamelliaLightEngine.decroldq(15, array2, 0, array4, 0);
				this.subkey[18] = array4[2];
				this.subkey[19] = array4[3];
				CamelliaLightEngine.decroldq(15, array2, 0, this.subkey, 12);
				CamelliaLightEngine.decroldqo32(34, array2, 0, this.subkey, 4);
				CamelliaLightEngine.roldq(17, array2, 0, this.kw, 0);
				return;
			}
			else
			{
				for (int k = 0; k < 4; k++)
				{
					array3[k] = (array2[k] ^ array[k + 4]);
				}
				this.camelliaF2(array3, CamelliaLightEngine.SIGMA, 8);
				if (forEncryption)
				{
					this.kw[0] = array[0];
					this.kw[1] = array[1];
					this.kw[2] = array[2];
					this.kw[3] = array[3];
					CamelliaLightEngine.roldqo32(45, array, 0, this.subkey, 16);
					CamelliaLightEngine.roldq(15, array, 0, this.ke, 4);
					CamelliaLightEngine.roldq(17, array, 0, this.subkey, 32);
					CamelliaLightEngine.roldqo32(34, array, 0, this.subkey, 44);
					CamelliaLightEngine.roldq(15, array, 4, this.subkey, 4);
					CamelliaLightEngine.roldq(15, array, 4, this.ke, 0);
					CamelliaLightEngine.roldq(30, array, 4, this.subkey, 24);
					CamelliaLightEngine.roldqo32(34, array, 4, this.subkey, 36);
					CamelliaLightEngine.roldq(15, array2, 0, this.subkey, 8);
					CamelliaLightEngine.roldq(30, array2, 0, this.subkey, 20);
					this.ke[8] = array2[1];
					this.ke[9] = array2[2];
					this.ke[10] = array2[3];
					this.ke[11] = array2[0];
					CamelliaLightEngine.roldqo32(49, array2, 0, this.subkey, 40);
					this.subkey[0] = array3[0];
					this.subkey[1] = array3[1];
					this.subkey[2] = array3[2];
					this.subkey[3] = array3[3];
					CamelliaLightEngine.roldq(30, array3, 0, this.subkey, 12);
					CamelliaLightEngine.roldq(30, array3, 0, this.subkey, 28);
					CamelliaLightEngine.roldqo32(51, array3, 0, this.kw, 4);
					return;
				}
				this.kw[4] = array[0];
				this.kw[5] = array[1];
				this.kw[6] = array[2];
				this.kw[7] = array[3];
				CamelliaLightEngine.decroldqo32(45, array, 0, this.subkey, 28);
				CamelliaLightEngine.decroldq(15, array, 0, this.ke, 4);
				CamelliaLightEngine.decroldq(17, array, 0, this.subkey, 12);
				CamelliaLightEngine.decroldqo32(34, array, 0, this.subkey, 0);
				CamelliaLightEngine.decroldq(15, array, 4, this.subkey, 40);
				CamelliaLightEngine.decroldq(15, array, 4, this.ke, 8);
				CamelliaLightEngine.decroldq(30, array, 4, this.subkey, 20);
				CamelliaLightEngine.decroldqo32(34, array, 4, this.subkey, 8);
				CamelliaLightEngine.decroldq(15, array2, 0, this.subkey, 36);
				CamelliaLightEngine.decroldq(30, array2, 0, this.subkey, 24);
				this.ke[2] = array2[1];
				this.ke[3] = array2[2];
				this.ke[0] = array2[3];
				this.ke[1] = array2[0];
				CamelliaLightEngine.decroldqo32(49, array2, 0, this.subkey, 4);
				this.subkey[46] = array3[0];
				this.subkey[47] = array3[1];
				this.subkey[44] = array3[2];
				this.subkey[45] = array3[3];
				CamelliaLightEngine.decroldq(30, array3, 0, this.subkey, 32);
				CamelliaLightEngine.decroldq(30, array3, 0, this.subkey, 16);
				CamelliaLightEngine.roldqo32(51, array3, 0, this.kw, 0);
				return;
			}
		}

		private int processBlock128(byte[] input, int inOff, byte[] output, int outOff)
		{
			for (int i = 0; i < 4; i++)
			{
				this.state[i] = CamelliaLightEngine.bytes2uint(input, inOff + i * 4);
				this.state[i] ^= this.kw[i];
			}
			this.camelliaF2(this.state, this.subkey, 0);
			this.camelliaF2(this.state, this.subkey, 4);
			this.camelliaF2(this.state, this.subkey, 8);
			this.camelliaFLs(this.state, this.ke, 0);
			this.camelliaF2(this.state, this.subkey, 12);
			this.camelliaF2(this.state, this.subkey, 16);
			this.camelliaF2(this.state, this.subkey, 20);
			this.camelliaFLs(this.state, this.ke, 4);
			this.camelliaF2(this.state, this.subkey, 24);
			this.camelliaF2(this.state, this.subkey, 28);
			this.camelliaF2(this.state, this.subkey, 32);
			this.state[2] ^= this.kw[4];
			this.state[3] ^= this.kw[5];
			this.state[0] ^= this.kw[6];
			this.state[1] ^= this.kw[7];
			CamelliaLightEngine.uint2bytes(this.state[2], output, outOff);
			CamelliaLightEngine.uint2bytes(this.state[3], output, outOff + 4);
			CamelliaLightEngine.uint2bytes(this.state[0], output, outOff + 8);
			CamelliaLightEngine.uint2bytes(this.state[1], output, outOff + 12);
			return 16;
		}

		private int processBlock192or256(byte[] input, int inOff, byte[] output, int outOff)
		{
			for (int i = 0; i < 4; i++)
			{
				this.state[i] = CamelliaLightEngine.bytes2uint(input, inOff + i * 4);
				this.state[i] ^= this.kw[i];
			}
			this.camelliaF2(this.state, this.subkey, 0);
			this.camelliaF2(this.state, this.subkey, 4);
			this.camelliaF2(this.state, this.subkey, 8);
			this.camelliaFLs(this.state, this.ke, 0);
			this.camelliaF2(this.state, this.subkey, 12);
			this.camelliaF2(this.state, this.subkey, 16);
			this.camelliaF2(this.state, this.subkey, 20);
			this.camelliaFLs(this.state, this.ke, 4);
			this.camelliaF2(this.state, this.subkey, 24);
			this.camelliaF2(this.state, this.subkey, 28);
			this.camelliaF2(this.state, this.subkey, 32);
			this.camelliaFLs(this.state, this.ke, 8);
			this.camelliaF2(this.state, this.subkey, 36);
			this.camelliaF2(this.state, this.subkey, 40);
			this.camelliaF2(this.state, this.subkey, 44);
			this.state[2] ^= this.kw[4];
			this.state[3] ^= this.kw[5];
			this.state[0] ^= this.kw[6];
			this.state[1] ^= this.kw[7];
			CamelliaLightEngine.uint2bytes(this.state[2], output, outOff);
			CamelliaLightEngine.uint2bytes(this.state[3], output, outOff + 4);
			CamelliaLightEngine.uint2bytes(this.state[0], output, outOff + 8);
			CamelliaLightEngine.uint2bytes(this.state[1], output, outOff + 12);
			return 16;
		}

		public CamelliaLightEngine()
		{
			this.initialised = false;
		}

		public virtual int GetBlockSize()
		{
			return 16;
		}

		public virtual void Init(bool forEncryption, ICipherParameters parameters)
		{
			if (!(parameters is KeyParameter))
			{
				throw new ArgumentException("only simple KeyParameter expected.");
			}
			this.setKey(forEncryption, ((KeyParameter)parameters).GetKey());
			this.initialised = true;
		}

		public virtual int ProcessBlock(byte[] input, int inOff, byte[] output, int outOff)
		{
			if (!this.initialised)
			{
				throw new InvalidOperationException("Camellia engine not initialised");
			}
			Check.DataLength(input, inOff, 16, "input buffer too short");
			Check.OutputLength(output, outOff, 16, "output buffer too short");
			if (this._keyis128)
			{
				return this.processBlock128(input, inOff, output, outOff);
			}
			return this.processBlock192or256(input, inOff, output, outOff);
		}

		public virtual void Reset()
		{
		}
	}
}
