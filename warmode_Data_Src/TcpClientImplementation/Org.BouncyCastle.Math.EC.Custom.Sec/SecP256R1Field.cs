using Org.BouncyCastle.Math.Raw;
using System;

namespace Org.BouncyCastle.Math.EC.Custom.Sec
{
	internal class SecP256R1Field
	{
		internal const uint P7 = 4294967295u;

		internal const uint PExt15 = 4294967294u;

		internal static readonly uint[] P = new uint[]
		{
			4294967295u,
			4294967295u,
			4294967295u,
			0u,
			0u,
			0u,
			1u,
			4294967295u
		};

		internal static readonly uint[] PExt = new uint[]
		{
			1u,
			0u,
			0u,
			4294967294u,
			4294967295u,
			4294967295u,
			4294967294u,
			1u,
			4294967294u,
			1u,
			4294967294u,
			1u,
			1u,
			4294967294u,
			2u,
			4294967294u
		};

		public static void Add(uint[] x, uint[] y, uint[] z)
		{
			if (Nat256.Add(x, y, z) != 0u || (z[7] == 4294967295u && Nat256.Gte(z, SecP256R1Field.P)))
			{
				SecP256R1Field.AddPInvTo(z);
			}
		}

		public static void AddExt(uint[] xx, uint[] yy, uint[] zz)
		{
			if (Nat.Add(16, xx, yy, zz) != 0u || (zz[15] >= 4294967294u && Nat.Gte(16, zz, SecP256R1Field.PExt)))
			{
				Nat.SubFrom(16, SecP256R1Field.PExt, zz);
			}
		}

		public static void AddOne(uint[] x, uint[] z)
		{
			if (Nat.Inc(8, x, z) != 0u || (z[7] == 4294967295u && Nat256.Gte(z, SecP256R1Field.P)))
			{
				SecP256R1Field.AddPInvTo(z);
			}
		}

		public static uint[] FromBigInteger(BigInteger x)
		{
			uint[] array = Nat256.FromBigInteger(x);
			if (array[7] == 4294967295u && Nat256.Gte(array, SecP256R1Field.P))
			{
				Nat256.SubFrom(SecP256R1Field.P, array);
			}
			return array;
		}

		public static void Half(uint[] x, uint[] z)
		{
			if ((x[0] & 1u) == 0u)
			{
				Nat.ShiftDownBit(8, x, 0u, z);
				return;
			}
			uint c = Nat256.Add(x, SecP256R1Field.P, z);
			Nat.ShiftDownBit(8, z, c);
		}

		public static void Multiply(uint[] x, uint[] y, uint[] z)
		{
			uint[] array = Nat256.CreateExt();
			Nat256.Mul(x, y, array);
			SecP256R1Field.Reduce(array, z);
		}

		public static void MultiplyAddToExt(uint[] x, uint[] y, uint[] zz)
		{
			if (Nat256.MulAddTo(x, y, zz) != 0u || (zz[15] >= 4294967294u && Nat.Gte(16, zz, SecP256R1Field.PExt)))
			{
				Nat.SubFrom(16, SecP256R1Field.PExt, zz);
			}
		}

		public static void Negate(uint[] x, uint[] z)
		{
			if (Nat256.IsZero(x))
			{
				Nat256.Zero(z);
				return;
			}
			Nat256.Sub(SecP256R1Field.P, x, z);
		}

		public static void Reduce(uint[] xx, uint[] z)
		{
			long num = (long)((ulong)xx[8]);
			long num2 = (long)((ulong)xx[9]);
			long num3 = (long)((ulong)xx[10]);
			long num4 = (long)((ulong)xx[11]);
			long num5 = (long)((ulong)xx[12]);
			long num6 = (long)((ulong)xx[13]);
			long num7 = (long)((ulong)xx[14]);
			long num8 = (long)((ulong)xx[15]);
			num -= 6L;
			long num9 = num + num2;
			long num10 = num2 + num3;
			long num11 = num3 + num4 - num8;
			long num12 = num4 + num5;
			long num13 = num5 + num6;
			long num14 = num6 + num7;
			long num15 = num7 + num8;
			long num16 = 0L;
			num16 += (long)((ulong)xx[0] + (ulong)num9 - (ulong)num12 - (ulong)num14);
			z[0] = (uint)num16;
			num16 >>= 32;
			num16 += (long)((ulong)xx[1] + (ulong)num10 - (ulong)num13 - (ulong)num15);
			z[1] = (uint)num16;
			num16 >>= 32;
			num16 += (long)((ulong)xx[2] + (ulong)num11 - (ulong)num14);
			z[2] = (uint)num16;
			num16 >>= 32;
			num16 += (long)((ulong)xx[3] + (ulong)((ulong)num12 << 1) + (ulong)num6 - (ulong)num8 - (ulong)num9);
			z[3] = (uint)num16;
			num16 >>= 32;
			num16 += (long)((ulong)xx[4] + (ulong)((ulong)num13 << 1) + (ulong)num7 - (ulong)num10);
			z[4] = (uint)num16;
			num16 >>= 32;
			num16 += (long)((ulong)xx[5] + (ulong)((ulong)num14 << 1) - (ulong)num11);
			z[5] = (uint)num16;
			num16 >>= 32;
			num16 += (long)((ulong)xx[6] + (ulong)((ulong)num15 << 1) + (ulong)num14 - (ulong)num9);
			z[6] = (uint)num16;
			num16 >>= 32;
			num16 += (long)((ulong)xx[7] + (ulong)((ulong)num8 << 1) + (ulong)num - (ulong)num11 - (ulong)num13);
			z[7] = (uint)num16;
			num16 >>= 32;
			num16 += 6L;
			SecP256R1Field.Reduce32((uint)num16, z);
		}

		public static void Reduce32(uint x, uint[] z)
		{
			long num = 0L;
			if (x != 0u)
			{
				long num2 = (long)((ulong)x);
				num += (long)((ulong)z[0] + (ulong)num2);
				z[0] = (uint)num;
				num >>= 32;
				if (num != 0L)
				{
					num += (long)((ulong)z[1]);
					z[1] = (uint)num;
					num >>= 32;
					num += (long)((ulong)z[2]);
					z[2] = (uint)num;
					num >>= 32;
				}
				num += (long)((ulong)z[3] - (ulong)num2);
				z[3] = (uint)num;
				num >>= 32;
				if (num != 0L)
				{
					num += (long)((ulong)z[4]);
					z[4] = (uint)num;
					num >>= 32;
					num += (long)((ulong)z[5]);
					z[5] = (uint)num;
					num >>= 32;
				}
				num += (long)((ulong)z[6] - (ulong)num2);
				z[6] = (uint)num;
				num >>= 32;
				num += (long)((ulong)z[7] + (ulong)num2);
				z[7] = (uint)num;
				num >>= 32;
			}
			if (num != 0L || (z[7] == 4294967295u && Nat256.Gte(z, SecP256R1Field.P)))
			{
				SecP256R1Field.AddPInvTo(z);
			}
		}

		public static void Square(uint[] x, uint[] z)
		{
			uint[] array = Nat256.CreateExt();
			Nat256.Square(x, array);
			SecP256R1Field.Reduce(array, z);
		}

		public static void SquareN(uint[] x, int n, uint[] z)
		{
			uint[] array = Nat256.CreateExt();
			Nat256.Square(x, array);
			SecP256R1Field.Reduce(array, z);
			while (--n > 0)
			{
				Nat256.Square(z, array);
				SecP256R1Field.Reduce(array, z);
			}
		}

		public static void Subtract(uint[] x, uint[] y, uint[] z)
		{
			int num = Nat256.Sub(x, y, z);
			if (num != 0)
			{
				SecP256R1Field.SubPInvFrom(z);
			}
		}

		public static void SubtractExt(uint[] xx, uint[] yy, uint[] zz)
		{
			int num = Nat.Sub(16, xx, yy, zz);
			if (num != 0)
			{
				Nat.AddTo(16, SecP256R1Field.PExt, zz);
			}
		}

		public static void Twice(uint[] x, uint[] z)
		{
			if (Nat.ShiftUpBit(8, x, 0u, z) != 0u || (z[7] == 4294967295u && Nat256.Gte(z, SecP256R1Field.P)))
			{
				SecP256R1Field.AddPInvTo(z);
			}
		}

		private static void AddPInvTo(uint[] z)
		{
			long num = (long)((ulong)z[0] + 1uL);
			z[0] = (uint)num;
			num >>= 32;
			if (num != 0L)
			{
				num += (long)((ulong)z[1]);
				z[1] = (uint)num;
				num >>= 32;
				num += (long)((ulong)z[2]);
				z[2] = (uint)num;
				num >>= 32;
			}
			num += (long)((ulong)z[3] - 1uL);
			z[3] = (uint)num;
			num >>= 32;
			if (num != 0L)
			{
				num += (long)((ulong)z[4]);
				z[4] = (uint)num;
				num >>= 32;
				num += (long)((ulong)z[5]);
				z[5] = (uint)num;
				num >>= 32;
			}
			num += (long)((ulong)z[6] - 1uL);
			z[6] = (uint)num;
			num >>= 32;
			num += (long)((ulong)z[7] + 1uL);
			z[7] = (uint)num;
		}

		private static void SubPInvFrom(uint[] z)
		{
			long num = (long)((ulong)z[0] - 1uL);
			z[0] = (uint)num;
			num >>= 32;
			if (num != 0L)
			{
				num += (long)((ulong)z[1]);
				z[1] = (uint)num;
				num >>= 32;
				num += (long)((ulong)z[2]);
				z[2] = (uint)num;
				num >>= 32;
			}
			num += (long)((ulong)z[3] + 1uL);
			z[3] = (uint)num;
			num >>= 32;
			if (num != 0L)
			{
				num += (long)((ulong)z[4]);
				z[4] = (uint)num;
				num >>= 32;
				num += (long)((ulong)z[5]);
				z[5] = (uint)num;
				num >>= 32;
			}
			num += (long)((ulong)z[6] + 1uL);
			z[6] = (uint)num;
			num >>= 32;
			num += (long)((ulong)z[7] - 1uL);
			z[7] = (uint)num;
		}
	}
}
