using Org.BouncyCastle.Math.Raw;
using System;

namespace Org.BouncyCastle.Math.EC.Custom.Sec
{
	internal class SecP224K1Field
	{
		private const uint P6 = 4294967295u;

		private const uint PExt13 = 4294967295u;

		private const uint PInv33 = 6803u;

		internal static readonly uint[] P = new uint[]
		{
			4294960493u,
			4294967294u,
			4294967295u,
			4294967295u,
			4294967295u,
			4294967295u,
			4294967295u
		};

		internal static readonly uint[] PExt = new uint[]
		{
			46280809u,
			13606u,
			1u,
			0u,
			0u,
			0u,
			0u,
			4294953690u,
			4294967293u,
			4294967295u,
			4294967295u,
			4294967295u,
			4294967295u,
			4294967295u
		};

		private static readonly uint[] PExtInv = new uint[]
		{
			4248686487u,
			4294953689u,
			4294967294u,
			4294967295u,
			4294967295u,
			4294967295u,
			4294967295u,
			13605u,
			2u
		};

		public static void Add(uint[] x, uint[] y, uint[] z)
		{
			if (Nat224.Add(x, y, z) != 0u || (z[6] == 4294967295u && Nat224.Gte(z, SecP224K1Field.P)))
			{
				Nat.Add33To(7, 6803u, z);
			}
		}

		public static void AddExt(uint[] xx, uint[] yy, uint[] zz)
		{
			if ((Nat.Add(14, xx, yy, zz) != 0u || (zz[13] == 4294967295u && Nat.Gte(14, zz, SecP224K1Field.PExt))) && Nat.AddTo(SecP224K1Field.PExtInv.Length, SecP224K1Field.PExtInv, zz) != 0u)
			{
				Nat.IncAt(14, zz, SecP224K1Field.PExtInv.Length);
			}
		}

		public static void AddOne(uint[] x, uint[] z)
		{
			if (Nat.Inc(7, x, z) != 0u || (z[6] == 4294967295u && Nat224.Gte(z, SecP224K1Field.P)))
			{
				Nat.Add33To(7, 6803u, z);
			}
		}

		public static uint[] FromBigInteger(BigInteger x)
		{
			uint[] array = Nat224.FromBigInteger(x);
			if (array[6] == 4294967295u && Nat224.Gte(array, SecP224K1Field.P))
			{
				Nat224.SubFrom(SecP224K1Field.P, array);
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
			uint c = Nat224.Add(x, SecP224K1Field.P, z);
			Nat.ShiftDownBit(7, z, c);
		}

		public static void Multiply(uint[] x, uint[] y, uint[] z)
		{
			uint[] array = Nat224.CreateExt();
			Nat224.Mul(x, y, array);
			SecP224K1Field.Reduce(array, z);
		}

		public static void MultiplyAddToExt(uint[] x, uint[] y, uint[] zz)
		{
			if ((Nat224.MulAddTo(x, y, zz) != 0u || (zz[13] == 4294967295u && Nat.Gte(14, zz, SecP224K1Field.PExt))) && Nat.AddTo(SecP224K1Field.PExtInv.Length, SecP224K1Field.PExtInv, zz) != 0u)
			{
				Nat.IncAt(14, zz, SecP224K1Field.PExtInv.Length);
			}
		}

		public static void Negate(uint[] x, uint[] z)
		{
			if (Nat224.IsZero(x))
			{
				Nat224.Zero(z);
				return;
			}
			Nat224.Sub(SecP224K1Field.P, x, z);
		}

		public static void Reduce(uint[] xx, uint[] z)
		{
			ulong y = Nat224.Mul33Add(6803u, xx, 7, xx, 0, z, 0);
			if (Nat224.Mul33DWordAdd(6803u, y, z, 0) != 0u || (z[6] == 4294967295u && Nat224.Gte(z, SecP224K1Field.P)))
			{
				Nat.Add33To(7, 6803u, z);
			}
		}

		public static void Reduce32(uint x, uint[] z)
		{
			if ((x != 0u && Nat224.Mul33WordAdd(6803u, x, z, 0) != 0u) || (z[6] == 4294967295u && Nat224.Gte(z, SecP224K1Field.P)))
			{
				Nat.Add33To(7, 6803u, z);
			}
		}

		public static void Square(uint[] x, uint[] z)
		{
			uint[] array = Nat224.CreateExt();
			Nat224.Square(x, array);
			SecP224K1Field.Reduce(array, z);
		}

		public static void SquareN(uint[] x, int n, uint[] z)
		{
			uint[] array = Nat224.CreateExt();
			Nat224.Square(x, array);
			SecP224K1Field.Reduce(array, z);
			while (--n > 0)
			{
				Nat224.Square(z, array);
				SecP224K1Field.Reduce(array, z);
			}
		}

		public static void Subtract(uint[] x, uint[] y, uint[] z)
		{
			int num = Nat224.Sub(x, y, z);
			if (num != 0)
			{
				Nat.Sub33From(7, 6803u, z);
			}
		}

		public static void SubtractExt(uint[] xx, uint[] yy, uint[] zz)
		{
			int num = Nat.Sub(14, xx, yy, zz);
			if (num != 0 && Nat.SubFrom(SecP224K1Field.PExtInv.Length, SecP224K1Field.PExtInv, zz) != 0)
			{
				Nat.DecAt(14, zz, SecP224K1Field.PExtInv.Length);
			}
		}

		public static void Twice(uint[] x, uint[] z)
		{
			if (Nat.ShiftUpBit(7, x, 0u, z) != 0u || (z[6] == 4294967295u && Nat224.Gte(z, SecP224K1Field.P)))
			{
				Nat.Add33To(7, 6803u, z);
			}
		}
	}
}
