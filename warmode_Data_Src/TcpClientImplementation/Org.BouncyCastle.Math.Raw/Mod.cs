using Org.BouncyCastle.Crypto.Utilities;
using System;

namespace Org.BouncyCastle.Math.Raw
{
	internal abstract class Mod
	{
		public static void Invert(uint[] p, uint[] x, uint[] z)
		{
			int num = p.Length;
			if (Nat.IsZero(num, x))
			{
				throw new ArgumentException("cannot be 0", "x");
			}
			if (Nat.IsOne(num, x))
			{
				Array.Copy(x, 0, z, 0, num);
				return;
			}
			uint[] array = Nat.Copy(num, x);
			uint[] array2 = Nat.Create(num);
			array2[0] = 1u;
			int num2 = 0;
			if ((array[0] & 1u) == 0u)
			{
				Mod.InversionStep(p, array, num, array2, ref num2);
			}
			if (Nat.IsOne(num, array))
			{
				Mod.InversionResult(p, num2, array2, z);
				return;
			}
			uint[] array3 = Nat.Copy(num, p);
			uint[] array4 = Nat.Create(num);
			int num3 = 0;
			int num4 = num;
			while (true)
			{
				if (array[num4 - 1] != 0u || array3[num4 - 1] != 0u)
				{
					if (Nat.Gte(num, array, array3))
					{
						Nat.SubFrom(num, array3, array);
						num2 += Nat.SubFrom(num, array4, array2) - num3;
						Mod.InversionStep(p, array, num4, array2, ref num2);
						if (Nat.IsOne(num, array))
						{
							break;
						}
					}
					else
					{
						Nat.SubFrom(num, array, array3);
						num3 += Nat.SubFrom(num, array2, array4) - num2;
						Mod.InversionStep(p, array3, num4, array4, ref num3);
						if (Nat.IsOne(num, array3))
						{
							goto Block_8;
						}
					}
				}
				else
				{
					num4--;
				}
			}
			Mod.InversionResult(p, num2, array2, z);
			return;
			Block_8:
			Mod.InversionResult(p, num3, array4, z);
		}

		public static uint[] Random(uint[] p)
		{
			int num = p.Length;
			Random random = new Random();
			uint[] array = Nat.Create(num);
			uint num2 = p[num - 1];
			num2 |= num2 >> 1;
			num2 |= num2 >> 2;
			num2 |= num2 >> 4;
			num2 |= num2 >> 8;
			num2 |= num2 >> 16;
			do
			{
				byte[] array2 = new byte[num << 2];
				random.NextBytes(array2);
				Pack.BE_To_UInt32(array2, 0, array);
				array[num - 1] &= num2;
			}
			while (Nat.Gte(num, array, p));
			return array;
		}

		public static void Add(uint[] p, uint[] x, uint[] y, uint[] z)
		{
			int len = p.Length;
			uint num = Nat.Add(len, x, y, z);
			if (num != 0u)
			{
				Nat.SubFrom(len, p, z);
			}
		}

		public static void Subtract(uint[] p, uint[] x, uint[] y, uint[] z)
		{
			int len = p.Length;
			int num = Nat.Sub(len, x, y, z);
			if (num != 0)
			{
				Nat.AddTo(len, p, z);
			}
		}

		private static void InversionResult(uint[] p, int ac, uint[] a, uint[] z)
		{
			if (ac < 0)
			{
				Nat.Add(p.Length, a, p, z);
				return;
			}
			Array.Copy(a, 0, z, 0, p.Length);
		}

		private static void InversionStep(uint[] p, uint[] u, int uLen, uint[] x, ref int xc)
		{
			int len = p.Length;
			int num = 0;
			while (u[0] == 0u)
			{
				Nat.ShiftDownWord(uLen, u, 0u);
				num += 32;
			}
			int trailingZeroes = Mod.GetTrailingZeroes(u[0]);
			if (trailingZeroes > 0)
			{
				Nat.ShiftDownBits(uLen, u, trailingZeroes, 0u);
				num += trailingZeroes;
			}
			for (int i = 0; i < num; i++)
			{
				if ((x[0] & 1u) != 0u)
				{
					if (xc < 0)
					{
						xc += (int)Nat.AddTo(len, p, x);
					}
					else
					{
						xc += Nat.SubFrom(len, p, x);
					}
				}
				Nat.ShiftDownBit(len, x, (uint)xc);
			}
		}

		private static int GetTrailingZeroes(uint x)
		{
			int num = 0;
			while ((x & 1u) == 0u)
			{
				x >>= 1;
				num++;
			}
			return num;
		}
	}
}
