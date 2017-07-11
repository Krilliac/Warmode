using Org.BouncyCastle.Math.Raw;
using System;

namespace Org.BouncyCastle.Math.EC.Custom.Sec
{
	internal class SecP192R1Field
	{
		private const uint P5 = 4294967295u;

		private const uint PExt11 = 4294967295u;

		internal static readonly uint[] P = new uint[]
		{
			4294967295u,
			4294967295u,
			4294967294u,
			4294967295u,
			4294967295u,
			4294967295u
		};

		internal static readonly uint[] PExt = new uint[]
		{
			1u,
			0u,
			2u,
			0u,
			1u,
			0u,
			4294967294u,
			4294967295u,
			4294967293u,
			4294967295u,
			4294967295u,
			4294967295u
		};

		private static readonly uint[] PExtInv = new uint[]
		{
			4294967295u,
			4294967295u,
			4294967293u,
			4294967295u,
			4294967294u,
			4294967295u,
			1u,
			0u,
			2u
		};

		public static void Add(uint[] x, uint[] y, uint[] z)
		{
			if (Nat192.Add(x, y, z) != 0u || (z[5] == 4294967295u && Nat192.Gte(z, SecP192R1Field.P)))
			{
				SecP192R1Field.AddPInvTo(z);
			}
		}

		public static void AddExt(uint[] xx, uint[] yy, uint[] zz)
		{
			if ((Nat.Add(12, xx, yy, zz) != 0u || (zz[11] == 4294967295u && Nat.Gte(12, zz, SecP192R1Field.PExt))) && Nat.AddTo(SecP192R1Field.PExtInv.Length, SecP192R1Field.PExtInv, zz) != 0u)
			{
				Nat.IncAt(12, zz, SecP192R1Field.PExtInv.Length);
			}
		}

		public static void AddOne(uint[] x, uint[] z)
		{
			if (Nat.Inc(6, x, z) != 0u || (z[5] == 4294967295u && Nat192.Gte(z, SecP192R1Field.P)))
			{
				SecP192R1Field.AddPInvTo(z);
			}
		}

		public static uint[] FromBigInteger(BigInteger x)
		{
			uint[] array = Nat192.FromBigInteger(x);
			if (array[5] == 4294967295u && Nat192.Gte(array, SecP192R1Field.P))
			{
				Nat192.SubFrom(SecP192R1Field.P, array);
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
			uint c = Nat192.Add(x, SecP192R1Field.P, z);
			Nat.ShiftDownBit(6, z, c);
		}

		public static void Multiply(uint[] x, uint[] y, uint[] z)
		{
			uint[] array = Nat192.CreateExt();
			Nat192.Mul(x, y, array);
			SecP192R1Field.Reduce(array, z);
		}

		public static void MultiplyAddToExt(uint[] x, uint[] y, uint[] zz)
		{
			if ((Nat192.MulAddTo(x, y, zz) != 0u || (zz[11] == 4294967295u && Nat.Gte(12, zz, SecP192R1Field.PExt))) && Nat.AddTo(SecP192R1Field.PExtInv.Length, SecP192R1Field.PExtInv, zz) != 0u)
			{
				Nat.IncAt(12, zz, SecP192R1Field.PExtInv.Length);
			}
		}

		public static void Negate(uint[] x, uint[] z)
		{
			if (Nat192.IsZero(x))
			{
				Nat192.Zero(z);
				return;
			}
			Nat192.Sub(SecP192R1Field.P, x, z);
		}

		public static void Reduce(uint[] xx, uint[] z)
		{
			ulong num = (ulong)xx[6];
			ulong num2 = (ulong)xx[7];
			ulong num3 = (ulong)xx[8];
			ulong num4 = (ulong)xx[9];
			ulong num5 = (ulong)xx[10];
			ulong num6 = (ulong)xx[11];
			ulong num7 = num + num5;
			ulong num8 = num2 + num6;
			ulong num9 = 0uL;
			num9 += (ulong)xx[0] + num7;
			uint num10 = (uint)num9;
			num9 >>= 32;
			num9 += (ulong)xx[1] + num8;
			z[1] = (uint)num9;
			num9 >>= 32;
			num7 += num3;
			num8 += num4;
			num9 += (ulong)xx[2] + num7;
			ulong num11 = (ulong)((uint)num9);
			num9 >>= 32;
			num9 += (ulong)xx[3] + num8;
			z[3] = (uint)num9;
			num9 >>= 32;
			num7 -= num;
			num8 -= num2;
			num9 += (ulong)xx[4] + num7;
			z[4] = (uint)num9;
			num9 >>= 32;
			num9 += (ulong)xx[5] + num8;
			z[5] = (uint)num9;
			num9 >>= 32;
			num11 += num9;
			num9 += (ulong)num10;
			z[0] = (uint)num9;
			num9 >>= 32;
			if (num9 != 0uL)
			{
				num9 += (ulong)z[1];
				z[1] = (uint)num9;
				num11 += num9 >> 32;
			}
			z[2] = (uint)num11;
			num9 = num11 >> 32;
			if ((num9 != 0uL && Nat.IncAt(6, z, 3) != 0u) || (z[5] == 4294967295u && Nat192.Gte(z, SecP192R1Field.P)))
			{
				SecP192R1Field.AddPInvTo(z);
			}
		}

		public static void Reduce32(uint x, uint[] z)
		{
			ulong num = 0uL;
			if (x != 0u)
			{
				num += (ulong)z[0] + (ulong)x;
				z[0] = (uint)num;
				num >>= 32;
				if (num != 0uL)
				{
					num += (ulong)z[1];
					z[1] = (uint)num;
					num >>= 32;
				}
				num += (ulong)z[2] + (ulong)x;
				z[2] = (uint)num;
				num >>= 32;
			}
			if ((num != 0uL && Nat.IncAt(6, z, 3) != 0u) || (z[5] == 4294967295u && Nat192.Gte(z, SecP192R1Field.P)))
			{
				SecP192R1Field.AddPInvTo(z);
			}
		}

		public static void Square(uint[] x, uint[] z)
		{
			uint[] array = Nat192.CreateExt();
			Nat192.Square(x, array);
			SecP192R1Field.Reduce(array, z);
		}

		public static void SquareN(uint[] x, int n, uint[] z)
		{
			uint[] array = Nat192.CreateExt();
			Nat192.Square(x, array);
			SecP192R1Field.Reduce(array, z);
			while (--n > 0)
			{
				Nat192.Square(z, array);
				SecP192R1Field.Reduce(array, z);
			}
		}

		public static void Subtract(uint[] x, uint[] y, uint[] z)
		{
			int num = Nat192.Sub(x, y, z);
			if (num != 0)
			{
				SecP192R1Field.SubPInvFrom(z);
			}
		}

		public static void SubtractExt(uint[] xx, uint[] yy, uint[] zz)
		{
			int num = Nat.Sub(12, xx, yy, zz);
			if (num != 0 && Nat.SubFrom(SecP192R1Field.PExtInv.Length, SecP192R1Field.PExtInv, zz) != 0)
			{
				Nat.DecAt(12, zz, SecP192R1Field.PExtInv.Length);
			}
		}

		public static void Twice(uint[] x, uint[] z)
		{
			if (Nat.ShiftUpBit(6, x, 0u, z) != 0u || (z[5] == 4294967295u && Nat192.Gte(z, SecP192R1Field.P)))
			{
				SecP192R1Field.AddPInvTo(z);
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
			}
			num += (long)((ulong)z[2] + 1uL);
			z[2] = (uint)num;
			num >>= 32;
			if (num != 0L)
			{
				Nat.IncAt(6, z, 3);
			}
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
			}
			num += (long)((ulong)z[2] - 1uL);
			z[2] = (uint)num;
			num >>= 32;
			if (num != 0L)
			{
				Nat.DecAt(6, z, 3);
			}
		}
	}
}
