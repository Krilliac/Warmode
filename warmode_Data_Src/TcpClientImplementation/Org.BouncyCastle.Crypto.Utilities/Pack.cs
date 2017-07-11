using System;

namespace Org.BouncyCastle.Crypto.Utilities
{
	internal sealed class Pack
	{
		private Pack()
		{
		}

		internal static void UInt16_To_BE(ushort n, byte[] bs)
		{
			bs[0] = (byte)(n >> 8);
			bs[1] = (byte)n;
		}

		internal static void UInt16_To_BE(ushort n, byte[] bs, int off)
		{
			bs[off] = (byte)(n >> 8);
			bs[off + 1] = (byte)n;
		}

		internal static ushort BE_To_UInt16(byte[] bs)
		{
			uint num = (uint)((int)bs[0] << 8 | (int)bs[1]);
			return (ushort)num;
		}

		internal static ushort BE_To_UInt16(byte[] bs, int off)
		{
			uint num = (uint)((int)bs[off] << 8 | (int)bs[off + 1]);
			return (ushort)num;
		}

		internal static byte[] UInt32_To_BE(uint n)
		{
			byte[] array = new byte[4];
			Pack.UInt32_To_BE(n, array, 0);
			return array;
		}

		internal static void UInt32_To_BE(uint n, byte[] bs)
		{
			bs[0] = (byte)(n >> 24);
			bs[1] = (byte)(n >> 16);
			bs[2] = (byte)(n >> 8);
			bs[3] = (byte)n;
		}

		internal static void UInt32_To_BE(uint n, byte[] bs, int off)
		{
			bs[off] = (byte)(n >> 24);
			bs[off + 1] = (byte)(n >> 16);
			bs[off + 2] = (byte)(n >> 8);
			bs[off + 3] = (byte)n;
		}

		internal static byte[] UInt32_To_BE(uint[] ns)
		{
			byte[] array = new byte[4 * ns.Length];
			Pack.UInt32_To_BE(ns, array, 0);
			return array;
		}

		internal static void UInt32_To_BE(uint[] ns, byte[] bs, int off)
		{
			for (int i = 0; i < ns.Length; i++)
			{
				Pack.UInt32_To_BE(ns[i], bs, off);
				off += 4;
			}
		}

		internal static uint BE_To_UInt32(byte[] bs)
		{
			return (uint)((int)bs[0] << 24 | (int)bs[1] << 16 | (int)bs[2] << 8 | (int)bs[3]);
		}

		internal static uint BE_To_UInt32(byte[] bs, int off)
		{
			return (uint)((int)bs[off] << 24 | (int)bs[off + 1] << 16 | (int)bs[off + 2] << 8 | (int)bs[off + 3]);
		}

		internal static void BE_To_UInt32(byte[] bs, int off, uint[] ns)
		{
			for (int i = 0; i < ns.Length; i++)
			{
				ns[i] = Pack.BE_To_UInt32(bs, off);
				off += 4;
			}
		}

		internal static byte[] UInt64_To_BE(ulong n)
		{
			byte[] array = new byte[8];
			Pack.UInt64_To_BE(n, array, 0);
			return array;
		}

		internal static void UInt64_To_BE(ulong n, byte[] bs)
		{
			Pack.UInt32_To_BE((uint)(n >> 32), bs);
			Pack.UInt32_To_BE((uint)n, bs, 4);
		}

		internal static void UInt64_To_BE(ulong n, byte[] bs, int off)
		{
			Pack.UInt32_To_BE((uint)(n >> 32), bs, off);
			Pack.UInt32_To_BE((uint)n, bs, off + 4);
		}

		internal static ulong BE_To_UInt64(byte[] bs)
		{
			uint num = Pack.BE_To_UInt32(bs);
			uint num2 = Pack.BE_To_UInt32(bs, 4);
			return (ulong)num << 32 | (ulong)num2;
		}

		internal static ulong BE_To_UInt64(byte[] bs, int off)
		{
			uint num = Pack.BE_To_UInt32(bs, off);
			uint num2 = Pack.BE_To_UInt32(bs, off + 4);
			return (ulong)num << 32 | (ulong)num2;
		}

		internal static void UInt16_To_LE(ushort n, byte[] bs)
		{
			bs[0] = (byte)n;
			bs[1] = (byte)(n >> 8);
		}

		internal static void UInt16_To_LE(ushort n, byte[] bs, int off)
		{
			bs[off] = (byte)n;
			bs[off + 1] = (byte)(n >> 8);
		}

		internal static ushort LE_To_UInt16(byte[] bs)
		{
			uint num = (uint)((int)bs[0] | (int)bs[1] << 8);
			return (ushort)num;
		}

		internal static ushort LE_To_UInt16(byte[] bs, int off)
		{
			uint num = (uint)((int)bs[off] | (int)bs[off + 1] << 8);
			return (ushort)num;
		}

		internal static byte[] UInt32_To_LE(uint n)
		{
			byte[] array = new byte[4];
			Pack.UInt32_To_LE(n, array, 0);
			return array;
		}

		internal static void UInt32_To_LE(uint n, byte[] bs)
		{
			bs[0] = (byte)n;
			bs[1] = (byte)(n >> 8);
			bs[2] = (byte)(n >> 16);
			bs[3] = (byte)(n >> 24);
		}

		internal static void UInt32_To_LE(uint n, byte[] bs, int off)
		{
			bs[off] = (byte)n;
			bs[off + 1] = (byte)(n >> 8);
			bs[off + 2] = (byte)(n >> 16);
			bs[off + 3] = (byte)(n >> 24);
		}

		internal static byte[] UInt32_To_LE(uint[] ns)
		{
			byte[] array = new byte[4 * ns.Length];
			Pack.UInt32_To_LE(ns, array, 0);
			return array;
		}

		internal static void UInt32_To_LE(uint[] ns, byte[] bs, int off)
		{
			for (int i = 0; i < ns.Length; i++)
			{
				Pack.UInt32_To_LE(ns[i], bs, off);
				off += 4;
			}
		}

		internal static uint LE_To_UInt32(byte[] bs)
		{
			return (uint)((int)bs[0] | (int)bs[1] << 8 | (int)bs[2] << 16 | (int)bs[3] << 24);
		}

		internal static uint LE_To_UInt32(byte[] bs, int off)
		{
			return (uint)((int)bs[off] | (int)bs[off + 1] << 8 | (int)bs[off + 2] << 16 | (int)bs[off + 3] << 24);
		}

		internal static void LE_To_UInt32(byte[] bs, int off, uint[] ns)
		{
			for (int i = 0; i < ns.Length; i++)
			{
				ns[i] = Pack.LE_To_UInt32(bs, off);
				off += 4;
			}
		}

		internal static void LE_To_UInt32(byte[] bs, int bOff, uint[] ns, int nOff, int count)
		{
			for (int i = 0; i < count; i++)
			{
				ns[nOff + i] = Pack.LE_To_UInt32(bs, bOff);
				bOff += 4;
			}
		}

		internal static byte[] UInt64_To_LE(ulong n)
		{
			byte[] array = new byte[8];
			Pack.UInt64_To_LE(n, array, 0);
			return array;
		}

		internal static void UInt64_To_LE(ulong n, byte[] bs)
		{
			Pack.UInt32_To_LE((uint)n, bs);
			Pack.UInt32_To_LE((uint)(n >> 32), bs, 4);
		}

		internal static void UInt64_To_LE(ulong n, byte[] bs, int off)
		{
			Pack.UInt32_To_LE((uint)n, bs, off);
			Pack.UInt32_To_LE((uint)(n >> 32), bs, off + 4);
		}

		internal static ulong LE_To_UInt64(byte[] bs)
		{
			uint num = Pack.LE_To_UInt32(bs);
			uint num2 = Pack.LE_To_UInt32(bs, 4);
			return (ulong)num2 << 32 | (ulong)num;
		}

		internal static ulong LE_To_UInt64(byte[] bs, int off)
		{
			uint num = Pack.LE_To_UInt32(bs, off);
			uint num2 = Pack.LE_To_UInt32(bs, off + 4);
			return (ulong)num2 << 32 | (ulong)num;
		}
	}
}
