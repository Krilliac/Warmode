using Org.BouncyCastle.Crypto.Utilities;
using Org.BouncyCastle.Utilities;
using System;

namespace Org.BouncyCastle.Crypto.Digests
{
	public class SM3Digest : GeneralDigest
	{
		private const int DIGEST_LENGTH = 32;

		private const int BLOCK_SIZE = 16;

		private uint[] V = new uint[8];

		private uint[] inwords = new uint[16];

		private int xOff;

		private uint[] W = new uint[68];

		private uint[] W1 = new uint[64];

		private static readonly uint[] T;

		public override string AlgorithmName
		{
			get
			{
				return "SM3";
			}
		}

		static SM3Digest()
		{
			SM3Digest.T = new uint[64];
			for (int i = 0; i < 16; i++)
			{
				uint num = 2043430169u;
				SM3Digest.T[i] = (num << i | num >> 32 - i);
			}
			for (int j = 16; j < 64; j++)
			{
				int num2 = j % 32;
				uint num3 = 2055708042u;
				SM3Digest.T[j] = (num3 << num2 | num3 >> 32 - num2);
			}
		}

		public SM3Digest()
		{
			this.Reset();
		}

		public SM3Digest(SM3Digest t) : base(t)
		{
			this.CopyIn(t);
		}

		private void CopyIn(SM3Digest t)
		{
			Array.Copy(t.V, 0, this.V, 0, this.V.Length);
			Array.Copy(t.inwords, 0, this.inwords, 0, this.inwords.Length);
			this.xOff = t.xOff;
		}

		public override int GetDigestSize()
		{
			return 32;
		}

		public override IMemoable Copy()
		{
			return new SM3Digest(this);
		}

		public override void Reset(IMemoable other)
		{
			SM3Digest t = (SM3Digest)other;
			base.CopyIn(t);
			this.CopyIn(t);
		}

		public override void Reset()
		{
			base.Reset();
			this.V[0] = 1937774191u;
			this.V[1] = 1226093241u;
			this.V[2] = 388252375u;
			this.V[3] = 3666478592u;
			this.V[4] = 2842636476u;
			this.V[5] = 372324522u;
			this.V[6] = 3817729613u;
			this.V[7] = 2969243214u;
			this.xOff = 0;
		}

		public override int DoFinal(byte[] output, int outOff)
		{
			base.Finish();
			Pack.UInt32_To_BE(this.V[0], output, outOff);
			Pack.UInt32_To_BE(this.V[1], output, outOff + 4);
			Pack.UInt32_To_BE(this.V[2], output, outOff + 8);
			Pack.UInt32_To_BE(this.V[3], output, outOff + 12);
			Pack.UInt32_To_BE(this.V[4], output, outOff + 16);
			Pack.UInt32_To_BE(this.V[5], output, outOff + 20);
			Pack.UInt32_To_BE(this.V[6], output, outOff + 24);
			Pack.UInt32_To_BE(this.V[7], output, outOff + 28);
			this.Reset();
			return 32;
		}

		internal override void ProcessWord(byte[] input, int inOff)
		{
			uint num = Pack.BE_To_UInt32(input, inOff);
			this.inwords[this.xOff] = num;
			this.xOff++;
			if (this.xOff >= 16)
			{
				this.ProcessBlock();
			}
		}

		internal override void ProcessLength(long bitLength)
		{
			if (this.xOff > 14)
			{
				this.inwords[this.xOff] = 0u;
				this.xOff++;
				this.ProcessBlock();
			}
			while (this.xOff < 14)
			{
				this.inwords[this.xOff] = 0u;
				this.xOff++;
			}
			this.inwords[this.xOff++] = (uint)(bitLength >> 32);
			this.inwords[this.xOff++] = (uint)bitLength;
		}

		private uint P0(uint x)
		{
			uint num = x << 9 | x >> 23;
			uint num2 = x << 17 | x >> 15;
			return x ^ num ^ num2;
		}

		private uint P1(uint x)
		{
			uint num = x << 15 | x >> 17;
			uint num2 = x << 23 | x >> 9;
			return x ^ num ^ num2;
		}

		private uint FF0(uint x, uint y, uint z)
		{
			return x ^ y ^ z;
		}

		private uint FF1(uint x, uint y, uint z)
		{
			return (x & y) | (x & z) | (y & z);
		}

		private uint GG0(uint x, uint y, uint z)
		{
			return x ^ y ^ z;
		}

		private uint GG1(uint x, uint y, uint z)
		{
			return (x & y) | (~x & z);
		}

		internal override void ProcessBlock()
		{
			for (int i = 0; i < 16; i++)
			{
				this.W[i] = this.inwords[i];
			}
			for (int j = 16; j < 68; j++)
			{
				uint num = this.W[j - 3];
				uint num2 = num << 15 | num >> 17;
				uint num3 = this.W[j - 13];
				uint num4 = num3 << 7 | num3 >> 25;
				this.W[j] = (this.P1(this.W[j - 16] ^ this.W[j - 9] ^ num2) ^ num4 ^ this.W[j - 6]);
			}
			for (int k = 0; k < 64; k++)
			{
				this.W1[k] = (this.W[k] ^ this.W[k + 4]);
			}
			uint num5 = this.V[0];
			uint num6 = this.V[1];
			uint num7 = this.V[2];
			uint num8 = this.V[3];
			uint num9 = this.V[4];
			uint num10 = this.V[5];
			uint num11 = this.V[6];
			uint num12 = this.V[7];
			for (int l = 0; l < 16; l++)
			{
				uint num13 = num5 << 12 | num5 >> 20;
				uint num14 = num13 + num9 + SM3Digest.T[l];
				uint num15 = num14 << 7 | num14 >> 25;
				uint num16 = num15 ^ num13;
				uint num17 = this.FF0(num5, num6, num7) + num8 + num16 + this.W1[l];
				uint x = this.GG0(num9, num10, num11) + num12 + num15 + this.W[l];
				num8 = num7;
				num7 = (num6 << 9 | num6 >> 23);
				num6 = num5;
				num5 = num17;
				num12 = num11;
				num11 = (num10 << 19 | num10 >> 13);
				num10 = num9;
				num9 = this.P0(x);
			}
			for (int m = 16; m < 64; m++)
			{
				uint num18 = num5 << 12 | num5 >> 20;
				uint num19 = num18 + num9 + SM3Digest.T[m];
				uint num20 = num19 << 7 | num19 >> 25;
				uint num21 = num20 ^ num18;
				uint num22 = this.FF1(num5, num6, num7) + num8 + num21 + this.W1[m];
				uint x2 = this.GG1(num9, num10, num11) + num12 + num20 + this.W[m];
				num8 = num7;
				num7 = (num6 << 9 | num6 >> 23);
				num6 = num5;
				num5 = num22;
				num12 = num11;
				num11 = (num10 << 19 | num10 >> 13);
				num10 = num9;
				num9 = this.P0(x2);
			}
			this.V[0] ^= num5;
			this.V[1] ^= num6;
			this.V[2] ^= num7;
			this.V[3] ^= num8;
			this.V[4] ^= num9;
			this.V[5] ^= num10;
			this.V[6] ^= num11;
			this.V[7] ^= num12;
			this.xOff = 0;
		}
	}
}
