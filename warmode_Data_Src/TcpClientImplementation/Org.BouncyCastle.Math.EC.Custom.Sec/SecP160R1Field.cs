using Org.BouncyCastle.Math.Raw;
using System;

namespace Org.BouncyCastle.Math.EC.Custom.Sec
{
	internal class SecP160R1Field
	{
		private const uint P4 = 4294967295u;

		private const uint PExt9 = 4294967295u;

		private const uint PInv = 2147483649u;

		internal static readonly uint[] P = new uint[]
		{
			2147483647u,
			4294967295u,
			4294967295u,
			4294967295u,
			4294967295u
		};

		internal static readonly uint[] PExt = new uint[]
		{
			1u,
			1073741825u,
			0u,
			0u,
			0u,
			4294967294u,
			4294967294u,
			4294967295u,
			4294967295u,
			4294967295u
		};

		private static readonly uint[] PExtInv = new uint[]
		{
			4294967295u,
			3221225470u,
			4294967295u,
			4294967295u,
			4294967295u,
			1u,
			1u
		};

		public static void Add(uint[] x, uint[] y, uint[] z)
		{
			if (Nat160.Add(x, y, z) != 0u || (z[4] == 4294967295u && Nat160.Gte(z, SecP160R1Field.P)))
			{
				Nat.AddWordTo(5, 2147483649u, z);
			}
		}

		public static void AddExt(uint[] xx, uint[] yy, uint[] zz)
		{
			if ((Nat.Add(10, xx, yy, zz) != 0u || (zz[9] == 4294967295u && Nat.Gte(10, zz, SecP160R1Field.PExt))) && Nat.AddTo(SecP160R1Field.PExtInv.Length, SecP160R1Field.PExtInv, zz) != 0u)
			{
				Nat.IncAt(10, zz, SecP160R1Field.PExtInv.Length);
			}
		}

		public static void AddOne(uint[] x, uint[] z)
		{
			if (Nat.Inc(5, x, z) != 0u || (z[4] == 4294967295u && Nat160.Gte(z, SecP160R1Field.P)))
			{
				Nat.AddWordTo(5, 2147483649u, z);
			}
		}

		public static uint[] FromBigInteger(BigInteger x)
		{
			uint[] array = Nat160.FromBigInteger(x);
			if (array[4] == 4294967295u && Nat160.Gte(array, SecP160R1Field.P))
			{
				Nat160.SubFrom(SecP160R1Field.P, array);
			}
			return array;
		}

		public static void Half(uint[] x, uint[] z)
		{
			if ((x[0] & 1u) == 0u)
			{
				Nat.ShiftDownBit(5, x, 0u, z);
				return;
			}
			uint c = Nat160.Add(x, SecP160R1Field.P, z);
			Nat.ShiftDownBit(5, z, c);
		}

		public static void Multiply(uint[] x, uint[] y, uint[] z)
		{
			uint[] array = Nat160.CreateExt();
			Nat160.Mul(x, y, array);
			SecP160R1Field.Reduce(array, z);
		}

		public static void MultiplyAddToExt(uint[] x, uint[] y, uint[] zz)
		{
			if ((Nat160.MulAddTo(x, y, zz) != 0u || (zz[9] == 4294967295u && Nat.Gte(10, zz, SecP160R1Field.PExt))) && Nat.AddTo(SecP160R1Field.PExtInv.Length, SecP160R1Field.PExtInv, zz) != 0u)
			{
				Nat.IncAt(10, zz, SecP160R1Field.PExtInv.Length);
			}
		}

		public static void Negate(uint[] x, uint[] z)
		{
			if (Nat160.IsZero(x))
			{
				Nat160.Zero(z);
				return;
			}
			Nat160.Sub(SecP160R1Field.P, x, z);
		}

		public static void Reduce(uint[] xx, uint[] z)
		{
			ulong num = (ulong)xx[5];
			ulong num2 = (ulong)xx[6];
			ulong num3 = (ulong)xx[7];
			ulong num4 = (ulong)xx[8];
			ulong num5 = (ulong)xx[9];
			ulong num6 = 0uL;
			num6 += (ulong)xx[0] + num + (num << 31);
			z[0] = (uint)num6;
			num6 >>= 32;
			num6 += (ulong)xx[1] + num2 + (num2 << 31);
			z[1] = (uint)num6;
			num6 >>= 32;
			num6 += (ulong)xx[2] + num3 + (num3 << 31);
			z[2] = (uint)num6;
			num6 >>= 32;
			num6 += (ulong)xx[3] + num4 + (num4 << 31);
			z[3] = (uint)num6;
			num6 >>= 32;
			num6 += (ulong)xx[4] + num5 + (num5 << 31);
			z[4] = (uint)num6;
			num6 >>= 32;
			SecP160R1Field.Reduce32((uint)num6, z);
		}

		public static void Reduce32(uint x, uint[] z)
		{
			if ((x != 0u && Nat160.MulWordsAdd(2147483649u, x, z, 0) != 0u) || (z[4] == 4294967295u && Nat160.Gte(z, SecP160R1Field.P)))
			{
				Nat.AddWordTo(5, 2147483649u, z);
			}
		}

		public static void Square(uint[] x, uint[] z)
		{
			uint[] array = Nat160.CreateExt();
			Nat160.Square(x, array);
			SecP160R1Field.Reduce(array, z);
		}

		public static void SquareN(uint[] x, int n, uint[] z)
		{
			uint[] array = Nat160.CreateExt();
			Nat160.Square(x, array);
			SecP160R1Field.Reduce(array, z);
			while (--n > 0)
			{
				Nat160.Square(z, array);
				SecP160R1Field.Reduce(array, z);
			}
		}

		public static void Subtract(uint[] x, uint[] y, uint[] z)
		{
			int num = Nat160.Sub(x, y, z);
			if (num != 0)
			{
				Nat.SubWordFrom(5, 2147483649u, z);
			}
		}

		public static void SubtractExt(uint[] xx, uint[] yy, uint[] zz)
		{
			int num = Nat.Sub(10, xx, yy, zz);
			if (num != 0 && Nat.SubFrom(SecP160R1Field.PExtInv.Length, SecP160R1Field.PExtInv, zz) != 0)
			{
				Nat.DecAt(10, zz, SecP160R1Field.PExtInv.Length);
			}
		}

		public static void Twice(uint[] x, uint[] z)
		{
			if (Nat.ShiftUpBit(5, x, 0u, z) != 0u || (z[4] == 4294967295u && Nat160.Gte(z, SecP160R1Field.P)))
			{
				Nat.AddWordTo(5, 2147483649u, z);
			}
		}
	}
}
