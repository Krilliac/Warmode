using Org.BouncyCastle.Crypto.Utilities;
using System;

namespace Org.BouncyCastle.Crypto.Modes.Gcm
{
	internal abstract class GcmUtilities
	{
		private const uint E1 = 3774873600u;

		private const ulong E1L = 16212958658533785600uL;

		private static readonly uint[] LOOKUP = GcmUtilities.GenerateLookup();

		private static uint[] GenerateLookup()
		{
			uint[] array = new uint[256];
			for (int i = 0; i < 256; i++)
			{
				uint num = 0u;
				for (int j = 7; j >= 0; j--)
				{
					if ((i & 1 << j) != 0)
					{
						num ^= 3774873600u >> 7 - j;
					}
				}
				array[i] = num;
			}
			return array;
		}

		internal static byte[] OneAsBytes()
		{
			byte[] array = new byte[16];
			array[0] = 128;
			return array;
		}

		internal static uint[] OneAsUints()
		{
			uint[] array = new uint[4];
			array[0] = 2147483648u;
			return array;
		}

		internal static byte[] AsBytes(uint[] x)
		{
			return Pack.UInt32_To_BE(x);
		}

		internal static void AsBytes(uint[] x, byte[] z)
		{
			Pack.UInt32_To_BE(x, z, 0);
		}

		internal static uint[] AsUints(byte[] bs)
		{
			uint[] array = new uint[4];
			Pack.BE_To_UInt32(bs, 0, array);
			return array;
		}

		internal static void AsUints(byte[] bs, uint[] output)
		{
			Pack.BE_To_UInt32(bs, 0, output);
		}

		internal static void Multiply(byte[] x, byte[] y)
		{
			uint[] x2 = GcmUtilities.AsUints(x);
			uint[] y2 = GcmUtilities.AsUints(y);
			GcmUtilities.Multiply(x2, y2);
			GcmUtilities.AsBytes(x2, x);
		}

		internal static void Multiply(uint[] x, uint[] y)
		{
			uint num = x[0];
			uint num2 = x[1];
			uint num3 = x[2];
			uint num4 = x[3];
			uint num5 = 0u;
			uint num6 = 0u;
			uint num7 = 0u;
			uint num8 = 0u;
			for (int i = 0; i < 4; i++)
			{
				int num9 = (int)y[i];
				for (int j = 0; j < 32; j++)
				{
					uint num10 = (uint)(num9 >> 31);
					num9 <<= 1;
					num5 ^= (num & num10);
					num6 ^= (num2 & num10);
					num7 ^= (num3 & num10);
					num8 ^= (num4 & num10);
					uint num11 = (uint)((int)((int)num4 << 31) >> 8);
					num4 = (num4 >> 1 | num3 << 31);
					num3 = (num3 >> 1 | num2 << 31);
					num2 = (num2 >> 1 | num << 31);
					num = (num >> 1 ^ (num11 & 3774873600u));
				}
			}
			x[0] = num5;
			x[1] = num6;
			x[2] = num7;
			x[3] = num8;
		}

		internal static void Multiply(ulong[] x, ulong[] y)
		{
			ulong num = x[0];
			ulong num2 = x[1];
			ulong num3 = 0uL;
			ulong num4 = 0uL;
			for (int i = 0; i < 2; i++)
			{
				long num5 = (long)y[i];
				for (int j = 0; j < 64; j++)
				{
					ulong num6 = (ulong)(num5 >> 63);
					num5 <<= 1;
					num3 ^= (num & num6);
					num4 ^= (num2 & num6);
					ulong num7 = num2 << 63 >> 8;
					num2 = (num2 >> 1 | num << 63);
					num = (num >> 1 ^ (num7 & 16212958658533785600uL));
				}
			}
			x[0] = num3;
			x[1] = num4;
		}

		internal static void MultiplyP(uint[] x)
		{
			uint num = (uint)((int)GcmUtilities.ShiftRight(x) >> 8);
			x[0] ^= (num & 3774873600u);
		}

		internal static void MultiplyP(uint[] x, uint[] z)
		{
			uint num = (uint)((int)GcmUtilities.ShiftRight(x, z) >> 8);
			z[0] ^= (num & 3774873600u);
		}

		internal static void MultiplyP8(uint[] x)
		{
			uint num = GcmUtilities.ShiftRightN(x, 8);
			x[0] ^= GcmUtilities.LOOKUP[(int)((UIntPtr)(num >> 24))];
		}

		internal static void MultiplyP8(uint[] x, uint[] y)
		{
			uint num = GcmUtilities.ShiftRightN(x, 8, y);
			y[0] ^= GcmUtilities.LOOKUP[(int)((UIntPtr)(num >> 24))];
		}

		internal static uint ShiftRight(uint[] x)
		{
			uint num = x[0];
			x[0] = num >> 1;
			uint num2 = num << 31;
			num = x[1];
			x[1] = (num >> 1 | num2);
			num2 = num << 31;
			num = x[2];
			x[2] = (num >> 1 | num2);
			num2 = num << 31;
			num = x[3];
			x[3] = (num >> 1 | num2);
			return num << 31;
		}

		internal static uint ShiftRight(uint[] x, uint[] z)
		{
			uint num = x[0];
			z[0] = num >> 1;
			uint num2 = num << 31;
			num = x[1];
			z[1] = (num >> 1 | num2);
			num2 = num << 31;
			num = x[2];
			z[2] = (num >> 1 | num2);
			num2 = num << 31;
			num = x[3];
			z[3] = (num >> 1 | num2);
			return num << 31;
		}

		internal static uint ShiftRightN(uint[] x, int n)
		{
			uint num = x[0];
			int num2 = 32 - n;
			x[0] = num >> n;
			uint num3 = num << num2;
			num = x[1];
			x[1] = (num >> n | num3);
			num3 = num << num2;
			num = x[2];
			x[2] = (num >> n | num3);
			num3 = num << num2;
			num = x[3];
			x[3] = (num >> n | num3);
			return num << num2;
		}

		internal static uint ShiftRightN(uint[] x, int n, uint[] z)
		{
			uint num = x[0];
			int num2 = 32 - n;
			z[0] = num >> n;
			uint num3 = num << num2;
			num = x[1];
			z[1] = (num >> n | num3);
			num3 = num << num2;
			num = x[2];
			z[2] = (num >> n | num3);
			num3 = num << num2;
			num = x[3];
			z[3] = (num >> n | num3);
			return num << num2;
		}

		internal static void Xor(byte[] x, byte[] y)
		{
			int num = 0;
			do
			{
				int expr_09_cp_1 = num;
				x[expr_09_cp_1] ^= y[num];
				num++;
				int expr_24_cp_1 = num;
				x[expr_24_cp_1] ^= y[num];
				num++;
				int expr_3F_cp_1 = num;
				x[expr_3F_cp_1] ^= y[num];
				num++;
				int expr_5A_cp_1 = num;
				x[expr_5A_cp_1] ^= y[num];
				num++;
			}
			while (num < 16);
		}

		internal static void Xor(byte[] x, byte[] y, int yOff, int yLen)
		{
			while (--yLen >= 0)
			{
				int expr_09_cp_1 = yLen;
				x[expr_09_cp_1] ^= y[yOff + yLen];
			}
		}

		internal static void Xor(byte[] x, byte[] y, byte[] z)
		{
			int num = 0;
			do
			{
				z[num] = (x[num] ^ y[num]);
				num++;
				z[num] = (x[num] ^ y[num]);
				num++;
				z[num] = (x[num] ^ y[num]);
				num++;
				z[num] = (x[num] ^ y[num]);
				num++;
			}
			while (num < 16);
		}

		internal static void Xor(uint[] x, uint[] y)
		{
			x[0] ^= y[0];
			x[1] ^= y[1];
			x[2] ^= y[2];
			x[3] ^= y[3];
		}

		internal static void Xor(uint[] x, uint[] y, uint[] z)
		{
			z[0] = (x[0] ^ y[0]);
			z[1] = (x[1] ^ y[1]);
			z[2] = (x[2] ^ y[2]);
			z[3] = (x[3] ^ y[3]);
		}
	}
}
