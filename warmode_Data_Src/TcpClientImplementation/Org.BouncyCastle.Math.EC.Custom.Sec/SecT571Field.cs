using Org.BouncyCastle.Math.Raw;
using System;

namespace Org.BouncyCastle.Math.EC.Custom.Sec
{
	internal class SecT571Field
	{
		private const ulong M59 = 576460752303423487uL;

		private const ulong RM = 17256631552825064414uL;

		public static void Add(ulong[] x, ulong[] y, ulong[] z)
		{
			for (int i = 0; i < 9; i++)
			{
				z[i] = (x[i] ^ y[i]);
			}
		}

		private static void Add(ulong[] x, int xOff, ulong[] y, int yOff, ulong[] z, int zOff)
		{
			for (int i = 0; i < 9; i++)
			{
				z[zOff + i] = (x[xOff + i] ^ y[yOff + i]);
			}
		}

		private static void AddBothTo(ulong[] x, int xOff, ulong[] y, int yOff, ulong[] z, int zOff)
		{
			for (int i = 0; i < 9; i++)
			{
				z[zOff + i] ^= (x[xOff + i] ^ y[yOff + i]);
			}
		}

		public static void AddExt(ulong[] xx, ulong[] yy, ulong[] zz)
		{
			for (int i = 0; i < 18; i++)
			{
				zz[i] = (xx[i] ^ yy[i]);
			}
		}

		public static void AddOne(ulong[] x, ulong[] z)
		{
			z[0] = (x[0] ^ 1uL);
			for (int i = 1; i < 9; i++)
			{
				z[i] = x[i];
			}
		}

		public static ulong[] FromBigInteger(BigInteger x)
		{
			ulong[] array = Nat576.FromBigInteger64(x);
			SecT571Field.Reduce5(array, 0);
			return array;
		}

		public static void Multiply(ulong[] x, ulong[] y, ulong[] z)
		{
			ulong[] array = Nat576.CreateExt64();
			SecT571Field.ImplMultiply(x, y, array);
			SecT571Field.Reduce(array, z);
		}

		public static void MultiplyAddToExt(ulong[] x, ulong[] y, ulong[] zz)
		{
			ulong[] array = Nat576.CreateExt64();
			SecT571Field.ImplMultiply(x, y, array);
			SecT571Field.AddExt(zz, array, zz);
		}

		public static void Reduce(ulong[] xx, ulong[] z)
		{
			ulong num = xx[9];
			ulong num2 = xx[17];
			ulong num3 = num;
			num = (num3 ^ num2 >> 59 ^ num2 >> 57 ^ num2 >> 54 ^ num2 >> 49);
			num3 = (xx[8] ^ num2 << 5 ^ num2 << 7 ^ num2 << 10 ^ num2 << 15);
			for (int i = 16; i >= 10; i--)
			{
				num2 = xx[i];
				z[i - 8] = (num3 ^ num2 >> 59 ^ num2 >> 57 ^ num2 >> 54 ^ num2 >> 49);
				num3 = (xx[i - 9] ^ num2 << 5 ^ num2 << 7 ^ num2 << 10 ^ num2 << 15);
			}
			num2 = num;
			z[1] = (num3 ^ num2 >> 59 ^ num2 >> 57 ^ num2 >> 54 ^ num2 >> 49);
			num3 = (xx[0] ^ num2 << 5 ^ num2 << 7 ^ num2 << 10 ^ num2 << 15);
			ulong num4 = z[8];
			ulong num5 = num4 >> 59;
			z[0] = (num3 ^ num5 ^ num5 << 2 ^ num5 << 5 ^ num5 << 10);
			z[8] = (num4 & 576460752303423487uL);
		}

		public static void Reduce5(ulong[] z, int zOff)
		{
			ulong num = z[zOff + 8];
			ulong num2 = num >> 59;
			z[zOff] ^= (num2 ^ num2 << 2 ^ num2 << 5 ^ num2 << 10);
			z[zOff + 8] = (num & 576460752303423487uL);
		}

		public static void Square(ulong[] x, ulong[] z)
		{
			ulong[] array = Nat576.CreateExt64();
			SecT571Field.ImplSquare(x, array);
			SecT571Field.Reduce(array, z);
		}

		public static void SquareAddToExt(ulong[] x, ulong[] zz)
		{
			ulong[] array = Nat576.CreateExt64();
			SecT571Field.ImplSquare(x, array);
			SecT571Field.AddExt(zz, array, zz);
		}

		public static void SquareN(ulong[] x, int n, ulong[] z)
		{
			ulong[] array = Nat576.CreateExt64();
			SecT571Field.ImplSquare(x, array);
			SecT571Field.Reduce(array, z);
			while (--n > 0)
			{
				SecT571Field.ImplSquare(z, array);
				SecT571Field.Reduce(array, z);
			}
		}

		protected static void ImplMultiply(ulong[] x, ulong[] y, ulong[] zz)
		{
			ulong[] array = new ulong[144];
			Array.Copy(y, 0, array, 9, 9);
			int num = 0;
			for (int i = 7; i > 0; i--)
			{
				num += 18;
				Nat.ShiftUpBit64(9, array, num >> 1, 0uL, array, num);
				SecT571Field.Reduce5(array, num);
				SecT571Field.Add(array, 9, array, num, array, num + 9);
			}
			ulong[] array2 = new ulong[array.Length];
			Nat.ShiftUpBits64(array.Length, array, 0, 4, 0uL, array2, 0);
			uint num2 = 15u;
			for (int j = 56; j >= 0; j -= 8)
			{
				for (int k = 1; k < 9; k += 2)
				{
					uint num3 = (uint)(x[k] >> j);
					uint num4 = num3 & num2;
					uint num5 = num3 >> 4 & num2;
					SecT571Field.AddBothTo(array, (int)(9u * num4), array2, (int)(9u * num5), zz, k - 1);
				}
				Nat.ShiftUpBits64(16, zz, 0, 8, 0uL);
			}
			for (int l = 56; l >= 0; l -= 8)
			{
				for (int m = 0; m < 9; m += 2)
				{
					uint num6 = (uint)(x[m] >> l);
					uint num7 = num6 & num2;
					uint num8 = num6 >> 4 & num2;
					SecT571Field.AddBothTo(array, (int)(9u * num7), array2, (int)(9u * num8), zz, m);
				}
				if (l > 0)
				{
					Nat.ShiftUpBits64(18, zz, 0, 8, 0uL);
				}
			}
		}

		protected static void ImplMulwAcc(ulong[] xs, ulong y, ulong[] z, int zOff)
		{
			ulong[] array = new ulong[32];
			array[1] = y;
			for (int i = 2; i < 32; i += 2)
			{
				array[i] = array[i >> 1] << 1;
				array[i + 1] = (array[i] ^ y);
			}
			ulong num = 0uL;
			for (int j = 0; j < 9; j++)
			{
				ulong num2 = xs[j];
				uint num3 = (uint)num2;
				num ^= array[(int)((UIntPtr)(num3 & 31u))];
				ulong num4 = 0uL;
				int num5 = 60;
				do
				{
					num3 = (uint)(num2 >> num5);
					ulong num6 = array[(int)((UIntPtr)(num3 & 31u))];
					num ^= num6 << num5;
					num4 ^= num6 >> -num5;
				}
				while ((num5 -= 5) > 0);
				for (int k = 0; k < 4; k++)
				{
					num2 = (num2 & 17256631552825064414uL) >> 1;
					num4 ^= (num2 & y << k >> 63);
				}
				z[zOff + j] ^= num;
				num = num4;
			}
			z[zOff + 9] ^= num;
		}

		protected static void ImplSquare(ulong[] x, ulong[] zz)
		{
			for (int i = 0; i < 9; i++)
			{
				Interleave.Expand64To128(x[i], zz, i << 1);
			}
		}
	}
}
