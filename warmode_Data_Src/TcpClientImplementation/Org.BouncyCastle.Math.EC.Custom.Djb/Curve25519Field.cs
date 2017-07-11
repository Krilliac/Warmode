using Org.BouncyCastle.Math.Raw;
using System;

namespace Org.BouncyCastle.Math.EC.Custom.Djb
{
	internal class Curve25519Field
	{
		private const uint P7 = 2147483647u;

		private const uint PInv = 19u;

		internal static readonly uint[] P = new uint[]
		{
			4294967277u,
			4294967295u,
			4294967295u,
			4294967295u,
			4294967295u,
			4294967295u,
			4294967295u,
			2147483647u
		};

		private static readonly uint[] PExt = new uint[]
		{
			361u,
			0u,
			0u,
			0u,
			0u,
			0u,
			0u,
			0u,
			4294967277u,
			4294967295u,
			4294967295u,
			4294967295u,
			4294967295u,
			4294967295u,
			4294967295u,
			1073741823u
		};

		public static void Add(uint[] x, uint[] y, uint[] z)
		{
			Nat256.Add(x, y, z);
			if (Nat256.Gte(z, Curve25519Field.P))
			{
				Curve25519Field.SubPFrom(z);
			}
		}

		public static void AddExt(uint[] xx, uint[] yy, uint[] zz)
		{
			Nat.Add(16, xx, yy, zz);
			if (Nat.Gte(16, zz, Curve25519Field.PExt))
			{
				Curve25519Field.SubPExtFrom(zz);
			}
		}

		public static void AddOne(uint[] x, uint[] z)
		{
			Nat.Inc(8, x, z);
			if (Nat256.Gte(z, Curve25519Field.P))
			{
				Curve25519Field.SubPFrom(z);
			}
		}

		public static uint[] FromBigInteger(BigInteger x)
		{
			uint[] array = Nat256.FromBigInteger(x);
			while (Nat256.Gte(array, Curve25519Field.P))
			{
				Nat256.SubFrom(Curve25519Field.P, array);
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
			Nat256.Add(x, Curve25519Field.P, z);
			Nat.ShiftDownBit(8, z, 0u);
		}

		public static void Multiply(uint[] x, uint[] y, uint[] z)
		{
			uint[] array = Nat256.CreateExt();
			Nat256.Mul(x, y, array);
			Curve25519Field.Reduce(array, z);
		}

		public static void MultiplyAddToExt(uint[] x, uint[] y, uint[] zz)
		{
			Nat256.MulAddTo(x, y, zz);
			if (Nat.Gte(16, zz, Curve25519Field.PExt))
			{
				Curve25519Field.SubPExtFrom(zz);
			}
		}

		public static void Negate(uint[] x, uint[] z)
		{
			if (Nat256.IsZero(x))
			{
				Nat256.Zero(z);
				return;
			}
			Nat256.Sub(Curve25519Field.P, x, z);
		}

		public static void Reduce(uint[] xx, uint[] z)
		{
			uint num = xx[7];
			Nat.ShiftUpBit(8, xx, 8, num, z, 0);
			uint num2 = Nat256.MulByWordAddTo(19u, xx, z) << 1;
			uint num3 = z[7];
			num2 += (num3 >> 31) - (num >> 31);
			num3 &= 2147483647u;
			num3 += Nat.AddWordTo(7, num2 * 19u, z);
			z[7] = num3;
			if (num3 >= 2147483647u && Nat256.Gte(z, Curve25519Field.P))
			{
				Curve25519Field.SubPFrom(z);
			}
		}

		public static void Reduce27(uint x, uint[] z)
		{
			uint num = z[7];
			uint num2 = x << 1 | num >> 31;
			num &= 2147483647u;
			num += Nat.AddWordTo(7, num2 * 19u, z);
			z[7] = num;
			if (num >= 2147483647u && Nat256.Gte(z, Curve25519Field.P))
			{
				Curve25519Field.SubPFrom(z);
			}
		}

		public static void Square(uint[] x, uint[] z)
		{
			uint[] array = Nat256.CreateExt();
			Nat256.Square(x, array);
			Curve25519Field.Reduce(array, z);
		}

		public static void SquareN(uint[] x, int n, uint[] z)
		{
			uint[] array = Nat256.CreateExt();
			Nat256.Square(x, array);
			Curve25519Field.Reduce(array, z);
			while (--n > 0)
			{
				Nat256.Square(z, array);
				Curve25519Field.Reduce(array, z);
			}
		}

		public static void Subtract(uint[] x, uint[] y, uint[] z)
		{
			int num = Nat256.Sub(x, y, z);
			if (num != 0)
			{
				Curve25519Field.AddPTo(z);
			}
		}

		public static void SubtractExt(uint[] xx, uint[] yy, uint[] zz)
		{
			int num = Nat.Sub(16, xx, yy, zz);
			if (num != 0)
			{
				Curve25519Field.AddPExtTo(zz);
			}
		}

		public static void Twice(uint[] x, uint[] z)
		{
			Nat.ShiftUpBit(8, x, 0u, z);
			if (Nat256.Gte(z, Curve25519Field.P))
			{
				Curve25519Field.SubPFrom(z);
			}
		}

		private static uint AddPTo(uint[] z)
		{
			long num = (long)((ulong)z[0] - 19uL);
			z[0] = (uint)num;
			num >>= 32;
			if (num != 0L)
			{
				num = (long)Nat.DecAt(7, z, 1);
			}
			num += (long)((ulong)z[7] + (ulong)-2147483648);
			z[7] = (uint)num;
			num >>= 32;
			return (uint)num;
		}

		private static uint AddPExtTo(uint[] zz)
		{
			long num = (long)((ulong)zz[0] + (ulong)Curve25519Field.PExt[0]);
			zz[0] = (uint)num;
			num >>= 32;
			if (num != 0L)
			{
				num = (long)((ulong)Nat.IncAt(8, zz, 1));
			}
			num += (long)((ulong)zz[8] - 19uL);
			zz[8] = (uint)num;
			num >>= 32;
			if (num != 0L)
			{
				num = (long)Nat.DecAt(15, zz, 9);
			}
			num += (long)((ulong)zz[15] + (ulong)(Curve25519Field.PExt[15] + 1u));
			zz[15] = (uint)num;
			num >>= 32;
			return (uint)num;
		}

		private static int SubPFrom(uint[] z)
		{
			long num = (long)((ulong)z[0] + 19uL);
			z[0] = (uint)num;
			num >>= 32;
			if (num != 0L)
			{
				num = (long)((ulong)Nat.IncAt(7, z, 1));
			}
			num += (long)((ulong)z[7] - (ulong)-2147483648);
			z[7] = (uint)num;
			num >>= 32;
			return (int)num;
		}

		private static int SubPExtFrom(uint[] zz)
		{
			long num = (long)((ulong)zz[0] - (ulong)Curve25519Field.PExt[0]);
			zz[0] = (uint)num;
			num >>= 32;
			if (num != 0L)
			{
				num = (long)Nat.DecAt(8, zz, 1);
			}
			num += (long)((ulong)zz[8] + 19uL);
			zz[8] = (uint)num;
			num >>= 32;
			if (num != 0L)
			{
				num = (long)((ulong)Nat.IncAt(15, zz, 9));
			}
			num += (long)((ulong)zz[15] - (ulong)(Curve25519Field.PExt[15] + 1u));
			zz[15] = (uint)num;
			num >>= 32;
			return (int)num;
		}
	}
}
