using Org.BouncyCastle.Math.Raw;
using System;

namespace Org.BouncyCastle.Math.EC.Custom.Sec
{
	internal class SecP160R2Field
	{
		private const uint P4 = 4294967295u;

		private const uint PExt9 = 4294967295u;

		private const uint PInv33 = 21389u;

		internal static readonly uint[] P = new uint[]
		{
			4294945907u,
			4294967294u,
			4294967295u,
			4294967295u,
			4294967295u
		};

		internal static readonly uint[] PExt = new uint[]
		{
			457489321u,
			42778u,
			1u,
			0u,
			0u,
			4294924518u,
			4294967293u,
			4294967295u,
			4294967295u,
			4294967295u
		};

		private static readonly uint[] PExtInv = new uint[]
		{
			3837477975u,
			4294924517u,
			4294967294u,
			4294967295u,
			4294967295u,
			42777u,
			2u
		};

		public static void Add(uint[] x, uint[] y, uint[] z)
		{
			if (Nat160.Add(x, y, z) != 0u || (z[4] == 4294967295u && Nat160.Gte(z, SecP160R2Field.P)))
			{
				Nat.Add33To(5, 21389u, z);
			}
		}

		public static void AddExt(uint[] xx, uint[] yy, uint[] zz)
		{
			if ((Nat.Add(10, xx, yy, zz) != 0u || (zz[9] == 4294967295u && Nat.Gte(10, zz, SecP160R2Field.PExt))) && Nat.AddTo(SecP160R2Field.PExtInv.Length, SecP160R2Field.PExtInv, zz) != 0u)
			{
				Nat.IncAt(10, zz, SecP160R2Field.PExtInv.Length);
			}
		}

		public static void AddOne(uint[] x, uint[] z)
		{
			if (Nat.Inc(5, x, z) != 0u || (z[4] == 4294967295u && Nat160.Gte(z, SecP160R2Field.P)))
			{
				Nat.Add33To(5, 21389u, z);
			}
		}

		public static uint[] FromBigInteger(BigInteger x)
		{
			uint[] array = Nat160.FromBigInteger(x);
			if (array[4] == 4294967295u && Nat160.Gte(array, SecP160R2Field.P))
			{
				Nat160.SubFrom(SecP160R2Field.P, array);
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
			uint c = Nat160.Add(x, SecP160R2Field.P, z);
			Nat.ShiftDownBit(5, z, c);
		}

		public static void Multiply(uint[] x, uint[] y, uint[] z)
		{
			uint[] array = Nat160.CreateExt();
			Nat160.Mul(x, y, array);
			SecP160R2Field.Reduce(array, z);
		}

		public static void MultiplyAddToExt(uint[] x, uint[] y, uint[] zz)
		{
			if ((Nat160.MulAddTo(x, y, zz) != 0u || (zz[9] == 4294967295u && Nat.Gte(10, zz, SecP160R2Field.PExt))) && Nat.AddTo(SecP160R2Field.PExtInv.Length, SecP160R2Field.PExtInv, zz) != 0u)
			{
				Nat.IncAt(10, zz, SecP160R2Field.PExtInv.Length);
			}
		}

		public static void Negate(uint[] x, uint[] z)
		{
			if (Nat160.IsZero(x))
			{
				Nat160.Zero(z);
				return;
			}
			Nat160.Sub(SecP160R2Field.P, x, z);
		}

		public static void Reduce(uint[] xx, uint[] z)
		{
			ulong y = Nat160.Mul33Add(21389u, xx, 5, xx, 0, z, 0);
			if (Nat160.Mul33DWordAdd(21389u, y, z, 0) != 0u || (z[4] == 4294967295u && Nat160.Gte(z, SecP160R2Field.P)))
			{
				Nat.Add33To(5, 21389u, z);
			}
		}

		public static void Reduce32(uint x, uint[] z)
		{
			if ((x != 0u && Nat160.Mul33WordAdd(21389u, x, z, 0) != 0u) || (z[4] == 4294967295u && Nat160.Gte(z, SecP160R2Field.P)))
			{
				Nat.Add33To(5, 21389u, z);
			}
		}

		public static void Square(uint[] x, uint[] z)
		{
			uint[] array = Nat160.CreateExt();
			Nat160.Square(x, array);
			SecP160R2Field.Reduce(array, z);
		}

		public static void SquareN(uint[] x, int n, uint[] z)
		{
			uint[] array = Nat160.CreateExt();
			Nat160.Square(x, array);
			SecP160R2Field.Reduce(array, z);
			while (--n > 0)
			{
				Nat160.Square(z, array);
				SecP160R2Field.Reduce(array, z);
			}
		}

		public static void Subtract(uint[] x, uint[] y, uint[] z)
		{
			int num = Nat160.Sub(x, y, z);
			if (num != 0)
			{
				Nat.Sub33From(5, 21389u, z);
			}
		}

		public static void SubtractExt(uint[] xx, uint[] yy, uint[] zz)
		{
			int num = Nat.Sub(10, xx, yy, zz);
			if (num != 0 && Nat.SubFrom(SecP160R2Field.PExtInv.Length, SecP160R2Field.PExtInv, zz) != 0)
			{
				Nat.DecAt(10, zz, SecP160R2Field.PExtInv.Length);
			}
		}

		public static void Twice(uint[] x, uint[] z)
		{
			if (Nat.ShiftUpBit(5, x, 0u, z) != 0u || (z[4] == 4294967295u && Nat160.Gte(z, SecP160R2Field.P)))
			{
				Nat.Add33To(5, 21389u, z);
			}
		}
	}
}
