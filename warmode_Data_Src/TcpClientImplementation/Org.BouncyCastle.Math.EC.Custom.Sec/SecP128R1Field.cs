using Org.BouncyCastle.Math.Raw;
using System;

namespace Org.BouncyCastle.Math.EC.Custom.Sec
{
	internal class SecP128R1Field
	{
		private const uint P3 = 4294967293u;

		private const uint PExt7 = 4294967292u;

		internal static readonly uint[] P = new uint[]
		{
			4294967295u,
			4294967295u,
			4294967295u,
			4294967293u
		};

		internal static readonly uint[] PExt = new uint[]
		{
			1u,
			0u,
			0u,
			4u,
			4294967294u,
			4294967295u,
			3u,
			4294967292u
		};

		private static readonly uint[] PExtInv = new uint[]
		{
			4294967295u,
			4294967295u,
			4294967295u,
			4294967291u,
			1u,
			0u,
			4294967292u,
			3u
		};

		public static void Add(uint[] x, uint[] y, uint[] z)
		{
			if (Nat128.Add(x, y, z) != 0u || (z[3] == 4294967293u && Nat128.Gte(z, SecP128R1Field.P)))
			{
				SecP128R1Field.AddPInvTo(z);
			}
		}

		public static void AddExt(uint[] xx, uint[] yy, uint[] zz)
		{
			if (Nat256.Add(xx, yy, zz) != 0u || (zz[7] == 4294967292u && Nat256.Gte(zz, SecP128R1Field.PExt)))
			{
				Nat.AddTo(SecP128R1Field.PExtInv.Length, SecP128R1Field.PExtInv, zz);
			}
		}

		public static void AddOne(uint[] x, uint[] z)
		{
			if (Nat.Inc(4, x, z) != 0u || (z[3] == 4294967293u && Nat128.Gte(z, SecP128R1Field.P)))
			{
				SecP128R1Field.AddPInvTo(z);
			}
		}

		public static uint[] FromBigInteger(BigInteger x)
		{
			uint[] array = Nat128.FromBigInteger(x);
			if (array[3] == 4294967293u && Nat128.Gte(array, SecP128R1Field.P))
			{
				Nat128.SubFrom(SecP128R1Field.P, array);
			}
			return array;
		}

		public static void Half(uint[] x, uint[] z)
		{
			if ((x[0] & 1u) == 0u)
			{
				Nat.ShiftDownBit(4, x, 0u, z);
				return;
			}
			uint c = Nat128.Add(x, SecP128R1Field.P, z);
			Nat.ShiftDownBit(4, z, c);
		}

		public static void Multiply(uint[] x, uint[] y, uint[] z)
		{
			uint[] array = Nat128.CreateExt();
			Nat128.Mul(x, y, array);
			SecP128R1Field.Reduce(array, z);
		}

		public static void MultiplyAddToExt(uint[] x, uint[] y, uint[] zz)
		{
			if (Nat128.MulAddTo(x, y, zz) != 0u || (zz[7] == 4294967292u && Nat256.Gte(zz, SecP128R1Field.PExt)))
			{
				Nat.AddTo(SecP128R1Field.PExtInv.Length, SecP128R1Field.PExtInv, zz);
			}
		}

		public static void Negate(uint[] x, uint[] z)
		{
			if (Nat128.IsZero(x))
			{
				Nat128.Zero(z);
				return;
			}
			Nat128.Sub(SecP128R1Field.P, x, z);
		}

		public static void Reduce(uint[] xx, uint[] z)
		{
			ulong num = (ulong)xx[0];
			ulong num2 = (ulong)xx[1];
			ulong num3 = (ulong)xx[2];
			ulong num4 = (ulong)xx[3];
			ulong num5 = (ulong)xx[4];
			ulong num6 = (ulong)xx[5];
			ulong num7 = (ulong)xx[6];
			ulong num8 = (ulong)xx[7];
			num4 += num8;
			num7 += num8 << 1;
			num3 += num7;
			num6 += num7 << 1;
			num2 += num6;
			num5 += num6 << 1;
			num += num5;
			num4 += num5 << 1;
			z[0] = (uint)num;
			num2 += num >> 32;
			z[1] = (uint)num2;
			num3 += num2 >> 32;
			z[2] = (uint)num3;
			num4 += num3 >> 32;
			z[3] = (uint)num4;
			SecP128R1Field.Reduce32((uint)(num4 >> 32), z);
		}

		public static void Reduce32(uint x, uint[] z)
		{
			while (x != 0u)
			{
				ulong num = (ulong)x;
				ulong num2 = (ulong)z[0] + num;
				z[0] = (uint)num2;
				num2 >>= 32;
				if (num2 != 0uL)
				{
					num2 += (ulong)z[1];
					z[1] = (uint)num2;
					num2 >>= 32;
					num2 += (ulong)z[2];
					z[2] = (uint)num2;
					num2 >>= 32;
				}
				num2 += (ulong)z[3] + (num << 1);
				z[3] = (uint)num2;
				num2 >>= 32;
				x = (uint)num2;
			}
		}

		public static void Square(uint[] x, uint[] z)
		{
			uint[] array = Nat128.CreateExt();
			Nat128.Square(x, array);
			SecP128R1Field.Reduce(array, z);
		}

		public static void SquareN(uint[] x, int n, uint[] z)
		{
			uint[] array = Nat128.CreateExt();
			Nat128.Square(x, array);
			SecP128R1Field.Reduce(array, z);
			while (--n > 0)
			{
				Nat128.Square(z, array);
				SecP128R1Field.Reduce(array, z);
			}
		}

		public static void Subtract(uint[] x, uint[] y, uint[] z)
		{
			int num = Nat128.Sub(x, y, z);
			if (num != 0)
			{
				SecP128R1Field.SubPInvFrom(z);
			}
		}

		public static void SubtractExt(uint[] xx, uint[] yy, uint[] zz)
		{
			int num = Nat.Sub(10, xx, yy, zz);
			if (num != 0)
			{
				Nat.SubFrom(SecP128R1Field.PExtInv.Length, SecP128R1Field.PExtInv, zz);
			}
		}

		public static void Twice(uint[] x, uint[] z)
		{
			if (Nat.ShiftUpBit(4, x, 0u, z) != 0u || (z[3] == 4294967293u && Nat128.Gte(z, SecP128R1Field.P)))
			{
				SecP128R1Field.AddPInvTo(z);
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
			num += (long)((ulong)z[3] + 2uL);
			z[3] = (uint)num;
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
			num += (long)((ulong)z[3] - 2uL);
			z[3] = (uint)num;
		}
	}
}
