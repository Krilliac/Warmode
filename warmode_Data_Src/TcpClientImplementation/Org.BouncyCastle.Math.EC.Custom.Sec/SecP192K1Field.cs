using Org.BouncyCastle.Math.Raw;
using System;

namespace Org.BouncyCastle.Math.EC.Custom.Sec
{
	internal class SecP192K1Field
	{
		private const uint P5 = 4294967295u;

		private const uint PExt11 = 4294967295u;

		private const uint PInv33 = 4553u;

		internal static readonly uint[] P = new uint[]
		{
			4294962743u,
			4294967294u,
			4294967295u,
			4294967295u,
			4294967295u,
			4294967295u
		};

		internal static readonly uint[] PExt = new uint[]
		{
			20729809u,
			9106u,
			1u,
			0u,
			0u,
			0u,
			4294958190u,
			4294967293u,
			4294967295u,
			4294967295u,
			4294967295u,
			4294967295u
		};

		private static readonly uint[] PExtInv = new uint[]
		{
			4274237487u,
			4294958189u,
			4294967294u,
			4294967295u,
			4294967295u,
			4294967295u,
			9105u,
			2u
		};

		public static void Add(uint[] x, uint[] y, uint[] z)
		{
			if (Nat192.Add(x, y, z) != 0u || (z[5] == 4294967295u && Nat192.Gte(z, SecP192K1Field.P)))
			{
				Nat.Add33To(6, 4553u, z);
			}
		}

		public static void AddExt(uint[] xx, uint[] yy, uint[] zz)
		{
			if ((Nat.Add(12, xx, yy, zz) != 0u || (zz[11] == 4294967295u && Nat.Gte(12, zz, SecP192K1Field.PExt))) && Nat.AddTo(SecP192K1Field.PExtInv.Length, SecP192K1Field.PExtInv, zz) != 0u)
			{
				Nat.IncAt(12, zz, SecP192K1Field.PExtInv.Length);
			}
		}

		public static void AddOne(uint[] x, uint[] z)
		{
			if (Nat.Inc(6, x, z) != 0u || (z[5] == 4294967295u && Nat192.Gte(z, SecP192K1Field.P)))
			{
				Nat.Add33To(6, 4553u, z);
			}
		}

		public static uint[] FromBigInteger(BigInteger x)
		{
			uint[] array = Nat192.FromBigInteger(x);
			if (array[5] == 4294967295u && Nat192.Gte(array, SecP192K1Field.P))
			{
				Nat192.SubFrom(SecP192K1Field.P, array);
			}
			return array;
		}

		public static void Half(uint[] x, uint[] z)
		{
			if ((x[0] & 1u) == 0u)
			{
				Nat.ShiftDownBit(6, x, 0u, z);
				return;
			}
			uint c = Nat192.Add(x, SecP192K1Field.P, z);
			Nat.ShiftDownBit(6, z, c);
		}

		public static void Multiply(uint[] x, uint[] y, uint[] z)
		{
			uint[] array = Nat192.CreateExt();
			Nat192.Mul(x, y, array);
			SecP192K1Field.Reduce(array, z);
		}

		public static void MultiplyAddToExt(uint[] x, uint[] y, uint[] zz)
		{
			if ((Nat192.MulAddTo(x, y, zz) != 0u || (zz[11] == 4294967295u && Nat.Gte(12, zz, SecP192K1Field.PExt))) && Nat.AddTo(SecP192K1Field.PExtInv.Length, SecP192K1Field.PExtInv, zz) != 0u)
			{
				Nat.IncAt(12, zz, SecP192K1Field.PExtInv.Length);
			}
		}

		public static void Negate(uint[] x, uint[] z)
		{
			if (Nat192.IsZero(x))
			{
				Nat192.Zero(z);
				return;
			}
			Nat192.Sub(SecP192K1Field.P, x, z);
		}

		public static void Reduce(uint[] xx, uint[] z)
		{
			ulong y = Nat192.Mul33Add(4553u, xx, 6, xx, 0, z, 0);
			if (Nat192.Mul33DWordAdd(4553u, y, z, 0) != 0u || (z[5] == 4294967295u && Nat192.Gte(z, SecP192K1Field.P)))
			{
				Nat.Add33To(6, 4553u, z);
			}
		}

		public static void Reduce32(uint x, uint[] z)
		{
			if ((x != 0u && Nat192.Mul33WordAdd(4553u, x, z, 0) != 0u) || (z[5] == 4294967295u && Nat192.Gte(z, SecP192K1Field.P)))
			{
				Nat.Add33To(6, 4553u, z);
			}
		}

		public static void Square(uint[] x, uint[] z)
		{
			uint[] array = Nat192.CreateExt();
			Nat192.Square(x, array);
			SecP192K1Field.Reduce(array, z);
		}

		public static void SquareN(uint[] x, int n, uint[] z)
		{
			uint[] array = Nat192.CreateExt();
			Nat192.Square(x, array);
			SecP192K1Field.Reduce(array, z);
			while (--n > 0)
			{
				Nat192.Square(z, array);
				SecP192K1Field.Reduce(array, z);
			}
		}

		public static void Subtract(uint[] x, uint[] y, uint[] z)
		{
			int num = Nat192.Sub(x, y, z);
			if (num != 0)
			{
				Nat.Sub33From(6, 4553u, z);
			}
		}

		public static void SubtractExt(uint[] xx, uint[] yy, uint[] zz)
		{
			int num = Nat.Sub(12, xx, yy, zz);
			if (num != 0 && Nat.SubFrom(SecP192K1Field.PExtInv.Length, SecP192K1Field.PExtInv, zz) != 0)
			{
				Nat.DecAt(12, zz, SecP192K1Field.PExtInv.Length);
			}
		}

		public static void Twice(uint[] x, uint[] z)
		{
			if (Nat.ShiftUpBit(6, x, 0u, z) != 0u || (z[5] == 4294967295u && Nat192.Gte(z, SecP192K1Field.P)))
			{
				Nat.Add33To(6, 4553u, z);
			}
		}
	}
}
