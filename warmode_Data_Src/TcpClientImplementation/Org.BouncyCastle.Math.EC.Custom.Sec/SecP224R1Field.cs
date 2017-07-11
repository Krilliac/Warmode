using Org.BouncyCastle.Math.Raw;
using System;

namespace Org.BouncyCastle.Math.EC.Custom.Sec
{
	internal class SecP224R1Field
	{
		private const uint P6 = 4294967295u;

		private const uint PExt13 = 4294967295u;

		internal static readonly uint[] P = new uint[]
		{
			1u,
			0u,
			0u,
			4294967295u,
			4294967295u,
			4294967295u,
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
			0u,
			2u,
			0u,
			0u,
			4294967294u,
			4294967295u,
			4294967295u,
			4294967295u
		};

		private static readonly uint[] PExtInv = new uint[]
		{
			4294967295u,
			4294967295u,
			4294967295u,
			1u,
			0u,
			0u,
			4294967295u,
			4294967293u,
			4294967295u,
			4294967295u,
			1u
		};

		public static void Add(uint[] x, uint[] y, uint[] z)
		{
			if (Nat224.Add(x, y, z) != 0u || (z[6] == 4294967295u && Nat224.Gte(z, SecP224R1Field.P)))
			{
				SecP224R1Field.AddPInvTo(z);
			}
		}

		public static void AddExt(uint[] xx, uint[] yy, uint[] zz)
		{
			if ((Nat.Add(14, xx, yy, zz) != 0u || (zz[13] == 4294967295u && Nat.Gte(14, zz, SecP224R1Field.PExt))) && Nat.AddTo(SecP224R1Field.PExtInv.Length, SecP224R1Field.PExtInv, zz) != 0u)
			{
				Nat.IncAt(14, zz, SecP224R1Field.PExtInv.Length);
			}
		}

		public static void AddOne(uint[] x, uint[] z)
		{
			if (Nat.Inc(7, x, z) != 0u || (z[6] == 4294967295u && Nat224.Gte(z, SecP224R1Field.P)))
			{
				SecP224R1Field.AddPInvTo(z);
			}
		}

		public static uint[] FromBigInteger(BigInteger x)
		{
			uint[] array = Nat224.FromBigInteger(x);
			if (array[6] == 4294967295u && Nat224.Gte(array, SecP224R1Field.P))
			{
				Nat224.SubFrom(SecP224R1Field.P, array);
			}
			return array;
		}

		public static void Half(uint[] x, uint[] z)
		{
			if ((x[0] & 1u) == 0u)
			{
				Nat.ShiftDownBit(7, x, 0u, z);
				return;
			}
			uint c = Nat224.Add(x, SecP224R1Field.P, z);
			Nat.ShiftDownBit(7, z, c);
		}

		public static void Multiply(uint[] x, uint[] y, uint[] z)
		{
			uint[] array = Nat224.CreateExt();
			Nat224.Mul(x, y, array);
			SecP224R1Field.Reduce(array, z);
		}

		public static void MultiplyAddToExt(uint[] x, uint[] y, uint[] zz)
		{
			if ((Nat224.MulAddTo(x, y, zz) != 0u || (zz[13] == 4294967295u && Nat.Gte(14, zz, SecP224R1Field.PExt))) && Nat.AddTo(SecP224R1Field.PExtInv.Length, SecP224R1Field.PExtInv, zz) != 0u)
			{
				Nat.IncAt(14, zz, SecP224R1Field.PExtInv.Length);
			}
		}

		public static void Negate(uint[] x, uint[] z)
		{
			if (Nat224.IsZero(x))
			{
				Nat224.Zero(z);
				return;
			}
			Nat224.Sub(SecP224R1Field.P, x, z);
		}

		public static void Reduce(uint[] xx, uint[] z)
		{
			long num = (long)((ulong)xx[10]);
			long num2 = (long)((ulong)xx[11]);
			long num3 = (long)((ulong)xx[12]);
			long num4 = (long)((ulong)xx[13]);
			long num5 = (long)((ulong)xx[7] + (ulong)num2 - 1uL);
			long num6 = (long)((ulong)xx[8] + (ulong)num3);
			long num7 = (long)((ulong)xx[9] + (ulong)num4);
			long num8 = 0L;
			num8 += (long)((ulong)xx[0] - (ulong)num5);
			long num9 = (long)((ulong)((uint)num8));
			num8 >>= 32;
			num8 += (long)((ulong)xx[1] - (ulong)num6);
			z[1] = (uint)num8;
			num8 >>= 32;
			num8 += (long)((ulong)xx[2] - (ulong)num7);
			z[2] = (uint)num8;
			num8 >>= 32;
			num8 += (long)((ulong)xx[3] + (ulong)num5 - (ulong)num);
			long num10 = (long)((ulong)((uint)num8));
			num8 >>= 32;
			num8 += (long)((ulong)xx[4] + (ulong)num6 - (ulong)num2);
			z[4] = (uint)num8;
			num8 >>= 32;
			num8 += (long)((ulong)xx[5] + (ulong)num7 - (ulong)num3);
			z[5] = (uint)num8;
			num8 >>= 32;
			num8 += (long)((ulong)xx[6] + (ulong)num - (ulong)num4);
			z[6] = (uint)num8;
			num8 >>= 32;
			num8 += 1L;
			num10 += num8;
			num9 -= num8;
			z[0] = (uint)num9;
			num8 = num9 >> 32;
			if (num8 != 0L)
			{
				num8 += (long)((ulong)z[1]);
				z[1] = (uint)num8;
				num8 >>= 32;
				num8 += (long)((ulong)z[2]);
				z[2] = (uint)num8;
				num10 += num8 >> 32;
			}
			z[3] = (uint)num10;
			num8 = num10 >> 32;
			if ((num8 != 0L && Nat.IncAt(7, z, 4) != 0u) || (z[6] == 4294967295u && Nat224.Gte(z, SecP224R1Field.P)))
			{
				SecP224R1Field.AddPInvTo(z);
			}
		}

		public static void Reduce32(uint x, uint[] z)
		{
			long num = 0L;
			if (x != 0u)
			{
				long num2 = (long)((ulong)x);
				num += (long)((ulong)z[0] - (ulong)num2);
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
				num += (long)((ulong)z[3] + (ulong)num2);
				z[3] = (uint)num;
				num >>= 32;
			}
			if ((num != 0L && Nat.IncAt(7, z, 4) != 0u) || (z[6] == 4294967295u && Nat224.Gte(z, SecP224R1Field.P)))
			{
				SecP224R1Field.AddPInvTo(z);
			}
		}

		public static void Square(uint[] x, uint[] z)
		{
			uint[] array = Nat224.CreateExt();
			Nat224.Square(x, array);
			SecP224R1Field.Reduce(array, z);
		}

		public static void SquareN(uint[] x, int n, uint[] z)
		{
			uint[] array = Nat224.CreateExt();
			Nat224.Square(x, array);
			SecP224R1Field.Reduce(array, z);
			while (--n > 0)
			{
				Nat224.Square(z, array);
				SecP224R1Field.Reduce(array, z);
			}
		}

		public static void Subtract(uint[] x, uint[] y, uint[] z)
		{
			int num = Nat224.Sub(x, y, z);
			if (num != 0)
			{
				SecP224R1Field.SubPInvFrom(z);
			}
		}

		public static void SubtractExt(uint[] xx, uint[] yy, uint[] zz)
		{
			int num = Nat.Sub(14, xx, yy, zz);
			if (num != 0 && Nat.SubFrom(SecP224R1Field.PExtInv.Length, SecP224R1Field.PExtInv, zz) != 0)
			{
				Nat.DecAt(14, zz, SecP224R1Field.PExtInv.Length);
			}
		}

		public static void Twice(uint[] x, uint[] z)
		{
			if (Nat.ShiftUpBit(7, x, 0u, z) != 0u || (z[6] == 4294967295u && Nat224.Gte(z, SecP224R1Field.P)))
			{
				SecP224R1Field.AddPInvTo(z);
			}
		}

		private static void AddPInvTo(uint[] z)
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
				Nat.IncAt(7, z, 4);
			}
		}

		private static void SubPInvFrom(uint[] z)
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
				Nat.DecAt(7, z, 4);
			}
		}
	}
}
