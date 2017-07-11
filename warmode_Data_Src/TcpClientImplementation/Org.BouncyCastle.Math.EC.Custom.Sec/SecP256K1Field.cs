using Org.BouncyCastle.Math.Raw;
using System;

namespace Org.BouncyCastle.Math.EC.Custom.Sec
{
	internal class SecP256K1Field
	{
		private const uint P7 = 4294967295u;

		private const uint PExt15 = 4294967295u;

		private const uint PInv33 = 977u;

		internal static readonly uint[] P = new uint[]
		{
			4294966319u,
			4294967294u,
			4294967295u,
			4294967295u,
			4294967295u,
			4294967295u,
			4294967295u,
			4294967295u
		};

		internal static readonly uint[] PExt = new uint[]
		{
			954529u,
			1954u,
			1u,
			0u,
			0u,
			0u,
			0u,
			0u,
			4294965342u,
			4294967293u,
			4294967295u,
			4294967295u,
			4294967295u,
			4294967295u,
			4294967295u,
			4294967295u
		};

		private static readonly uint[] PExtInv = new uint[]
		{
			4294012767u,
			4294965341u,
			4294967294u,
			4294967295u,
			4294967295u,
			4294967295u,
			4294967295u,
			4294967295u,
			1953u,
			2u
		};

		public static void Add(uint[] x, uint[] y, uint[] z)
		{
			if (Nat256.Add(x, y, z) != 0u || (z[7] == 4294967295u && Nat256.Gte(z, SecP256K1Field.P)))
			{
				Nat.Add33To(8, 977u, z);
			}
		}

		public static void AddExt(uint[] xx, uint[] yy, uint[] zz)
		{
			if ((Nat.Add(16, xx, yy, zz) != 0u || (zz[15] == 4294967295u && Nat.Gte(16, zz, SecP256K1Field.PExt))) && Nat.AddTo(SecP256K1Field.PExtInv.Length, SecP256K1Field.PExtInv, zz) != 0u)
			{
				Nat.IncAt(16, zz, SecP256K1Field.PExtInv.Length);
			}
		}

		public static void AddOne(uint[] x, uint[] z)
		{
			if (Nat.Inc(8, x, z) != 0u || (z[7] == 4294967295u && Nat256.Gte(z, SecP256K1Field.P)))
			{
				Nat.Add33To(8, 977u, z);
			}
		}

		public static uint[] FromBigInteger(BigInteger x)
		{
			uint[] array = Nat256.FromBigInteger(x);
			if (array[7] == 4294967295u && Nat256.Gte(array, SecP256K1Field.P))
			{
				Nat256.SubFrom(SecP256K1Field.P, array);
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
			uint c = Nat256.Add(x, SecP256K1Field.P, z);
			Nat.ShiftDownBit(8, z, c);
		}

		public static void Multiply(uint[] x, uint[] y, uint[] z)
		{
			uint[] array = Nat256.CreateExt();
			Nat256.Mul(x, y, array);
			SecP256K1Field.Reduce(array, z);
		}

		public static void MultiplyAddToExt(uint[] x, uint[] y, uint[] zz)
		{
			if ((Nat256.MulAddTo(x, y, zz) != 0u || (zz[15] == 4294967295u && Nat.Gte(16, zz, SecP256K1Field.PExt))) && Nat.AddTo(SecP256K1Field.PExtInv.Length, SecP256K1Field.PExtInv, zz) != 0u)
			{
				Nat.IncAt(16, zz, SecP256K1Field.PExtInv.Length);
			}
		}

		public static void Negate(uint[] x, uint[] z)
		{
			if (Nat256.IsZero(x))
			{
				Nat256.Zero(z);
				return;
			}
			Nat256.Sub(SecP256K1Field.P, x, z);
		}

		public static void Reduce(uint[] xx, uint[] z)
		{
			ulong y = Nat256.Mul33Add(977u, xx, 8, xx, 0, z, 0);
			if (Nat256.Mul33DWordAdd(977u, y, z, 0) != 0u || (z[7] == 4294967295u && Nat256.Gte(z, SecP256K1Field.P)))
			{
				Nat.Add33To(8, 977u, z);
			}
		}

		public static void Reduce32(uint x, uint[] z)
		{
			if ((x != 0u && Nat256.Mul33WordAdd(977u, x, z, 0) != 0u) || (z[7] == 4294967295u && Nat256.Gte(z, SecP256K1Field.P)))
			{
				Nat.Add33To(8, 977u, z);
			}
		}

		public static void Square(uint[] x, uint[] z)
		{
			uint[] array = Nat256.CreateExt();
			Nat256.Square(x, array);
			SecP256K1Field.Reduce(array, z);
		}

		public static void SquareN(uint[] x, int n, uint[] z)
		{
			uint[] array = Nat256.CreateExt();
			Nat256.Square(x, array);
			SecP256K1Field.Reduce(array, z);
			while (--n > 0)
			{
				Nat256.Square(z, array);
				SecP256K1Field.Reduce(array, z);
			}
		}

		public static void Subtract(uint[] x, uint[] y, uint[] z)
		{
			int num = Nat256.Sub(x, y, z);
			if (num != 0)
			{
				Nat.Sub33From(8, 977u, z);
			}
		}

		public static void SubtractExt(uint[] xx, uint[] yy, uint[] zz)
		{
			int num = Nat.Sub(16, xx, yy, zz);
			if (num != 0 && Nat.SubFrom(SecP256K1Field.PExtInv.Length, SecP256K1Field.PExtInv, zz) != 0)
			{
				Nat.DecAt(16, zz, SecP256K1Field.PExtInv.Length);
			}
		}

		public static void Twice(uint[] x, uint[] z)
		{
			if (Nat.ShiftUpBit(8, x, 0u, z) != 0u || (z[7] == 4294967295u && Nat256.Gte(z, SecP256K1Field.P)))
			{
				Nat.Add33To(8, 977u, z);
			}
		}
	}
}
