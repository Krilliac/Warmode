using Org.BouncyCastle.Math.Raw;
using System;

namespace Org.BouncyCastle.Math.EC.Custom.Sec
{
	internal class SecP384R1Field
	{
		private const uint P11 = 4294967295u;

		private const uint PExt23 = 4294967295u;

		internal static readonly uint[] P = new uint[]
		{
			4294967295u,
			0u,
			0u,
			4294967295u,
			4294967294u,
			4294967295u,
			4294967295u,
			4294967295u,
			4294967295u,
			4294967295u,
			4294967295u,
			4294967295u
		};

		internal static readonly uint[] PExt = new uint[]
		{
			1u,
			4294967294u,
			0u,
			2u,
			0u,
			4294967294u,
			0u,
			2u,
			1u,
			0u,
			0u,
			0u,
			4294967294u,
			1u,
			0u,
			4294967294u,
			4294967293u,
			4294967295u,
			4294967295u,
			4294967295u,
			4294967295u,
			4294967295u,
			4294967295u,
			4294967295u
		};

		private static readonly uint[] PExtInv = new uint[]
		{
			4294967295u,
			1u,
			4294967295u,
			4294967293u,
			4294967295u,
			1u,
			4294967295u,
			4294967293u,
			4294967294u,
			4294967295u,
			4294967295u,
			4294967295u,
			1u,
			4294967294u,
			4294967295u,
			1u,
			2u
		};

		public static void Add(uint[] x, uint[] y, uint[] z)
		{
			if (Nat.Add(12, x, y, z) != 0u || (z[11] == 4294967295u && Nat.Gte(12, z, SecP384R1Field.P)))
			{
				SecP384R1Field.AddPInvTo(z);
			}
		}

		public static void AddExt(uint[] xx, uint[] yy, uint[] zz)
		{
			if ((Nat.Add(24, xx, yy, zz) != 0u || (zz[23] == 4294967295u && Nat.Gte(24, zz, SecP384R1Field.PExt))) && Nat.AddTo(SecP384R1Field.PExtInv.Length, SecP384R1Field.PExtInv, zz) != 0u)
			{
				Nat.IncAt(24, zz, SecP384R1Field.PExtInv.Length);
			}
		}

		public static void AddOne(uint[] x, uint[] z)
		{
			if (Nat.Inc(12, x, z) != 0u || (z[11] == 4294967295u && Nat.Gte(12, z, SecP384R1Field.P)))
			{
				SecP384R1Field.AddPInvTo(z);
			}
		}

		public static uint[] FromBigInteger(BigInteger x)
		{
			uint[] array = Nat.FromBigInteger(384, x);
			if (array[11] == 4294967295u && Nat.Gte(12, array, SecP384R1Field.P))
			{
				Nat.SubFrom(12, SecP384R1Field.P, array);
			}
			return array;
		}

		public static void Half(uint[] x, uint[] z)
		{
			if ((x[0] & 1u) == 0u)
			{
				Nat.ShiftDownBit(12, x, 0u, z);
				return;
			}
			uint c = Nat.Add(12, x, SecP384R1Field.P, z);
			Nat.ShiftDownBit(12, z, c);
		}

		public static void Multiply(uint[] x, uint[] y, uint[] z)
		{
			uint[] array = Nat.Create(24);
			Nat384.Mul(x, y, array);
			SecP384R1Field.Reduce(array, z);
		}

		public static void Negate(uint[] x, uint[] z)
		{
			if (Nat.IsZero(12, x))
			{
				Nat.Zero(12, z);
				return;
			}
			Nat.Sub(12, SecP384R1Field.P, x, z);
		}

		public static void Reduce(uint[] xx, uint[] z)
		{
			long num = (long)((ulong)xx[16]);
			long num2 = (long)((ulong)xx[17]);
			long num3 = (long)((ulong)xx[18]);
			long num4 = (long)((ulong)xx[19]);
			long num5 = (long)((ulong)xx[20]);
			long num6 = (long)((ulong)xx[21]);
			long num7 = (long)((ulong)xx[22]);
			long num8 = (long)((ulong)xx[23]);
			long num9 = (long)((ulong)xx[12] + (ulong)num5 - 1uL);
			long num10 = (long)((ulong)xx[13] + (ulong)num7);
			long num11 = (long)((ulong)xx[14] + (ulong)num7 + (ulong)num8);
			long num12 = (long)((ulong)xx[15] + (ulong)num8);
			long num13 = num2 + num6;
			long num14 = num6 - num8;
			long num15 = num7 - num8;
			long num16 = 0L;
			num16 += (long)((ulong)xx[0] + (ulong)num9 + (ulong)num14);
			z[0] = (uint)num16;
			num16 >>= 32;
			num16 += (long)((ulong)xx[1] + (ulong)num8 - (ulong)num9 + (ulong)num10);
			z[1] = (uint)num16;
			num16 >>= 32;
			num16 += (long)((ulong)xx[2] - (ulong)num6 - (ulong)num10 + (ulong)num11);
			z[2] = (uint)num16;
			num16 >>= 32;
			num16 += (long)((ulong)xx[3] + (ulong)num9 - (ulong)num11 + (ulong)num12 + (ulong)num14);
			z[3] = (uint)num16;
			num16 >>= 32;
			num16 += (long)((ulong)xx[4] + (ulong)num + (ulong)num6 + (ulong)num9 + (ulong)num10 - (ulong)num12 + (ulong)num14);
			z[4] = (uint)num16;
			num16 >>= 32;
			num16 += (long)((ulong)xx[5] - (ulong)num + (ulong)num10 + (ulong)num11 + (ulong)num13);
			z[5] = (uint)num16;
			num16 >>= 32;
			num16 += (long)((ulong)xx[6] + (ulong)num3 - (ulong)num2 + (ulong)num11 + (ulong)num12);
			z[6] = (uint)num16;
			num16 >>= 32;
			num16 += (long)((ulong)xx[7] + (ulong)num + (ulong)num4 - (ulong)num3 + (ulong)num12);
			z[7] = (uint)num16;
			num16 >>= 32;
			num16 += (long)((ulong)xx[8] + (ulong)num + (ulong)num2 + (ulong)num5 - (ulong)num4);
			z[8] = (uint)num16;
			num16 >>= 32;
			num16 += (long)((ulong)xx[9] + (ulong)num3 - (ulong)num5 + (ulong)num13);
			z[9] = (uint)num16;
			num16 >>= 32;
			num16 += (long)((ulong)xx[10] + (ulong)num3 + (ulong)num4 - (ulong)num14 + (ulong)num15);
			z[10] = (uint)num16;
			num16 >>= 32;
			num16 += (long)((ulong)xx[11] + (ulong)num4 + (ulong)num5 - (ulong)num15);
			z[11] = (uint)num16;
			num16 >>= 32;
			num16 += 1L;
			SecP384R1Field.Reduce32((uint)num16, z);
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
				num += (long)((ulong)z[1] - (ulong)num2);
				z[1] = (uint)num;
				num >>= 32;
				if (num != 0L)
				{
					num += (long)((ulong)z[2]);
					z[2] = (uint)num;
					num >>= 32;
				}
				num += (long)((ulong)z[3] + (ulong)num2);
				z[3] = (uint)num;
				num >>= 32;
				num += (long)((ulong)z[4] + (ulong)num2);
				z[4] = (uint)num;
				num >>= 32;
			}
			if ((num != 0L && Nat.IncAt(12, z, 5) != 0u) || (z[11] == 4294967295u && Nat.Gte(12, z, SecP384R1Field.P)))
			{
				SecP384R1Field.AddPInvTo(z);
			}
		}

		public static void Square(uint[] x, uint[] z)
		{
			uint[] array = Nat.Create(24);
			Nat384.Square(x, array);
			SecP384R1Field.Reduce(array, z);
		}

		public static void SquareN(uint[] x, int n, uint[] z)
		{
			uint[] array = Nat.Create(24);
			Nat384.Square(x, array);
			SecP384R1Field.Reduce(array, z);
			while (--n > 0)
			{
				Nat384.Square(z, array);
				SecP384R1Field.Reduce(array, z);
			}
		}

		public static void Subtract(uint[] x, uint[] y, uint[] z)
		{
			int num = Nat.Sub(12, x, y, z);
			if (num != 0)
			{
				SecP384R1Field.SubPInvFrom(z);
			}
		}

		public static void SubtractExt(uint[] xx, uint[] yy, uint[] zz)
		{
			int num = Nat.Sub(24, xx, yy, zz);
			if (num != 0 && Nat.SubFrom(SecP384R1Field.PExtInv.Length, SecP384R1Field.PExtInv, zz) != 0)
			{
				Nat.DecAt(24, zz, SecP384R1Field.PExtInv.Length);
			}
		}

		public static void Twice(uint[] x, uint[] z)
		{
			if (Nat.ShiftUpBit(12, x, 0u, z) != 0u || (z[11] == 4294967295u && Nat.Gte(12, z, SecP384R1Field.P)))
			{
				SecP384R1Field.AddPInvTo(z);
			}
		}

		private static void AddPInvTo(uint[] z)
		{
			long num = (long)((ulong)z[0] + 1uL);
			z[0] = (uint)num;
			num >>= 32;
			num += (long)((ulong)z[1] - 1uL);
			z[1] = (uint)num;
			num >>= 32;
			if (num != 0L)
			{
				num += (long)((ulong)z[2]);
				z[2] = (uint)num;
				num >>= 32;
			}
			num += (long)((ulong)z[3] + 1uL);
			z[3] = (uint)num;
			num >>= 32;
			num += (long)((ulong)z[4] + 1uL);
			z[4] = (uint)num;
			num >>= 32;
			if (num != 0L)
			{
				Nat.IncAt(12, z, 5);
			}
		}

		private static void SubPInvFrom(uint[] z)
		{
			long num = (long)((ulong)z[0] - 1uL);
			z[0] = (uint)num;
			num >>= 32;
			num += (long)((ulong)z[1] + 1uL);
			z[1] = (uint)num;
			num >>= 32;
			if (num != 0L)
			{
				num += (long)((ulong)z[2]);
				z[2] = (uint)num;
				num >>= 32;
			}
			num += (long)((ulong)z[3] - 1uL);
			z[3] = (uint)num;
			num >>= 32;
			num += (long)((ulong)z[4] - 1uL);
			z[4] = (uint)num;
			num >>= 32;
			if (num != 0L)
			{
				Nat.DecAt(12, z, 5);
			}
		}
	}
}
