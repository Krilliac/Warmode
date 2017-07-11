using Org.BouncyCastle.Utilities;
using System;

namespace Org.BouncyCastle.Crypto.Digests
{
	public class RipeMD128Digest : GeneralDigest
	{
		private const int DigestLength = 16;

		private int H0;

		private int H1;

		private int H2;

		private int H3;

		private int[] X = new int[16];

		private int xOff;

		public override string AlgorithmName
		{
			get
			{
				return "RIPEMD128";
			}
		}

		public RipeMD128Digest()
		{
			this.Reset();
		}

		public RipeMD128Digest(RipeMD128Digest t) : base(t)
		{
			this.CopyIn(t);
		}

		private void CopyIn(RipeMD128Digest t)
		{
			base.CopyIn(t);
			this.H0 = t.H0;
			this.H1 = t.H1;
			this.H2 = t.H2;
			this.H3 = t.H3;
			Array.Copy(t.X, 0, this.X, 0, t.X.Length);
			this.xOff = t.xOff;
		}

		public override int GetDigestSize()
		{
			return 16;
		}

		internal override void ProcessWord(byte[] input, int inOff)
		{
			this.X[this.xOff++] = ((int)(input[inOff] & 255) | (int)(input[inOff + 1] & 255) << 8 | (int)(input[inOff + 2] & 255) << 16 | (int)(input[inOff + 3] & 255) << 24);
			if (this.xOff == 16)
			{
				this.ProcessBlock();
			}
		}

		internal override void ProcessLength(long bitLength)
		{
			if (this.xOff > 14)
			{
				this.ProcessBlock();
			}
			this.X[14] = (int)(bitLength & (long)((ulong)-1));
			this.X[15] = (int)((ulong)bitLength >> 32);
		}

		private void UnpackWord(int word, byte[] outBytes, int outOff)
		{
			outBytes[outOff] = (byte)word;
			outBytes[outOff + 1] = (byte)((uint)word >> 8);
			outBytes[outOff + 2] = (byte)((uint)word >> 16);
			outBytes[outOff + 3] = (byte)((uint)word >> 24);
		}

		public override int DoFinal(byte[] output, int outOff)
		{
			base.Finish();
			this.UnpackWord(this.H0, output, outOff);
			this.UnpackWord(this.H1, output, outOff + 4);
			this.UnpackWord(this.H2, output, outOff + 8);
			this.UnpackWord(this.H3, output, outOff + 12);
			this.Reset();
			return 16;
		}

		public override void Reset()
		{
			base.Reset();
			this.H0 = 1732584193;
			this.H1 = -271733879;
			this.H2 = -1732584194;
			this.H3 = 271733878;
			this.xOff = 0;
			for (int num = 0; num != this.X.Length; num++)
			{
				this.X[num] = 0;
			}
		}

		private int RL(int x, int n)
		{
			return x << n | (int)((uint)x >> 32 - n);
		}

		private int F1(int x, int y, int z)
		{
			return x ^ y ^ z;
		}

		private int F2(int x, int y, int z)
		{
			return (x & y) | (~x & z);
		}

		private int F3(int x, int y, int z)
		{
			return (x | ~y) ^ z;
		}

		private int F4(int x, int y, int z)
		{
			return (x & z) | (y & ~z);
		}

		private int F1(int a, int b, int c, int d, int x, int s)
		{
			return this.RL(a + this.F1(b, c, d) + x, s);
		}

		private int F2(int a, int b, int c, int d, int x, int s)
		{
			return this.RL(a + this.F2(b, c, d) + x + 1518500249, s);
		}

		private int F3(int a, int b, int c, int d, int x, int s)
		{
			return this.RL(a + this.F3(b, c, d) + x + 1859775393, s);
		}

		private int F4(int a, int b, int c, int d, int x, int s)
		{
			return this.RL(a + this.F4(b, c, d) + x + -1894007588, s);
		}

		private int FF1(int a, int b, int c, int d, int x, int s)
		{
			return this.RL(a + this.F1(b, c, d) + x, s);
		}

		private int FF2(int a, int b, int c, int d, int x, int s)
		{
			return this.RL(a + this.F2(b, c, d) + x + 1836072691, s);
		}

		private int FF3(int a, int b, int c, int d, int x, int s)
		{
			return this.RL(a + this.F3(b, c, d) + x + 1548603684, s);
		}

		private int FF4(int a, int b, int c, int d, int x, int s)
		{
			return this.RL(a + this.F4(b, c, d) + x + 1352829926, s);
		}

		internal override void ProcessBlock()
		{
			int num2;
			int num = num2 = this.H0;
			int num4;
			int num3 = num4 = this.H1;
			int num6;
			int num5 = num6 = this.H2;
			int num8;
			int num7 = num8 = this.H3;
			num2 = this.F1(num2, num4, num6, num8, this.X[0], 11);
			num8 = this.F1(num8, num2, num4, num6, this.X[1], 14);
			num6 = this.F1(num6, num8, num2, num4, this.X[2], 15);
			num4 = this.F1(num4, num6, num8, num2, this.X[3], 12);
			num2 = this.F1(num2, num4, num6, num8, this.X[4], 5);
			num8 = this.F1(num8, num2, num4, num6, this.X[5], 8);
			num6 = this.F1(num6, num8, num2, num4, this.X[6], 7);
			num4 = this.F1(num4, num6, num8, num2, this.X[7], 9);
			num2 = this.F1(num2, num4, num6, num8, this.X[8], 11);
			num8 = this.F1(num8, num2, num4, num6, this.X[9], 13);
			num6 = this.F1(num6, num8, num2, num4, this.X[10], 14);
			num4 = this.F1(num4, num6, num8, num2, this.X[11], 15);
			num2 = this.F1(num2, num4, num6, num8, this.X[12], 6);
			num8 = this.F1(num8, num2, num4, num6, this.X[13], 7);
			num6 = this.F1(num6, num8, num2, num4, this.X[14], 9);
			num4 = this.F1(num4, num6, num8, num2, this.X[15], 8);
			num2 = this.F2(num2, num4, num6, num8, this.X[7], 7);
			num8 = this.F2(num8, num2, num4, num6, this.X[4], 6);
			num6 = this.F2(num6, num8, num2, num4, this.X[13], 8);
			num4 = this.F2(num4, num6, num8, num2, this.X[1], 13);
			num2 = this.F2(num2, num4, num6, num8, this.X[10], 11);
			num8 = this.F2(num8, num2, num4, num6, this.X[6], 9);
			num6 = this.F2(num6, num8, num2, num4, this.X[15], 7);
			num4 = this.F2(num4, num6, num8, num2, this.X[3], 15);
			num2 = this.F2(num2, num4, num6, num8, this.X[12], 7);
			num8 = this.F2(num8, num2, num4, num6, this.X[0], 12);
			num6 = this.F2(num6, num8, num2, num4, this.X[9], 15);
			num4 = this.F2(num4, num6, num8, num2, this.X[5], 9);
			num2 = this.F2(num2, num4, num6, num8, this.X[2], 11);
			num8 = this.F2(num8, num2, num4, num6, this.X[14], 7);
			num6 = this.F2(num6, num8, num2, num4, this.X[11], 13);
			num4 = this.F2(num4, num6, num8, num2, this.X[8], 12);
			num2 = this.F3(num2, num4, num6, num8, this.X[3], 11);
			num8 = this.F3(num8, num2, num4, num6, this.X[10], 13);
			num6 = this.F3(num6, num8, num2, num4, this.X[14], 6);
			num4 = this.F3(num4, num6, num8, num2, this.X[4], 7);
			num2 = this.F3(num2, num4, num6, num8, this.X[9], 14);
			num8 = this.F3(num8, num2, num4, num6, this.X[15], 9);
			num6 = this.F3(num6, num8, num2, num4, this.X[8], 13);
			num4 = this.F3(num4, num6, num8, num2, this.X[1], 15);
			num2 = this.F3(num2, num4, num6, num8, this.X[2], 14);
			num8 = this.F3(num8, num2, num4, num6, this.X[7], 8);
			num6 = this.F3(num6, num8, num2, num4, this.X[0], 13);
			num4 = this.F3(num4, num6, num8, num2, this.X[6], 6);
			num2 = this.F3(num2, num4, num6, num8, this.X[13], 5);
			num8 = this.F3(num8, num2, num4, num6, this.X[11], 12);
			num6 = this.F3(num6, num8, num2, num4, this.X[5], 7);
			num4 = this.F3(num4, num6, num8, num2, this.X[12], 5);
			num2 = this.F4(num2, num4, num6, num8, this.X[1], 11);
			num8 = this.F4(num8, num2, num4, num6, this.X[9], 12);
			num6 = this.F4(num6, num8, num2, num4, this.X[11], 14);
			num4 = this.F4(num4, num6, num8, num2, this.X[10], 15);
			num2 = this.F4(num2, num4, num6, num8, this.X[0], 14);
			num8 = this.F4(num8, num2, num4, num6, this.X[8], 15);
			num6 = this.F4(num6, num8, num2, num4, this.X[12], 9);
			num4 = this.F4(num4, num6, num8, num2, this.X[4], 8);
			num2 = this.F4(num2, num4, num6, num8, this.X[13], 9);
			num8 = this.F4(num8, num2, num4, num6, this.X[3], 14);
			num6 = this.F4(num6, num8, num2, num4, this.X[7], 5);
			num4 = this.F4(num4, num6, num8, num2, this.X[15], 6);
			num2 = this.F4(num2, num4, num6, num8, this.X[14], 8);
			num8 = this.F4(num8, num2, num4, num6, this.X[5], 6);
			num6 = this.F4(num6, num8, num2, num4, this.X[6], 5);
			num4 = this.F4(num4, num6, num8, num2, this.X[2], 12);
			num = this.FF4(num, num3, num5, num7, this.X[5], 8);
			num7 = this.FF4(num7, num, num3, num5, this.X[14], 9);
			num5 = this.FF4(num5, num7, num, num3, this.X[7], 9);
			num3 = this.FF4(num3, num5, num7, num, this.X[0], 11);
			num = this.FF4(num, num3, num5, num7, this.X[9], 13);
			num7 = this.FF4(num7, num, num3, num5, this.X[2], 15);
			num5 = this.FF4(num5, num7, num, num3, this.X[11], 15);
			num3 = this.FF4(num3, num5, num7, num, this.X[4], 5);
			num = this.FF4(num, num3, num5, num7, this.X[13], 7);
			num7 = this.FF4(num7, num, num3, num5, this.X[6], 7);
			num5 = this.FF4(num5, num7, num, num3, this.X[15], 8);
			num3 = this.FF4(num3, num5, num7, num, this.X[8], 11);
			num = this.FF4(num, num3, num5, num7, this.X[1], 14);
			num7 = this.FF4(num7, num, num3, num5, this.X[10], 14);
			num5 = this.FF4(num5, num7, num, num3, this.X[3], 12);
			num3 = this.FF4(num3, num5, num7, num, this.X[12], 6);
			num = this.FF3(num, num3, num5, num7, this.X[6], 9);
			num7 = this.FF3(num7, num, num3, num5, this.X[11], 13);
			num5 = this.FF3(num5, num7, num, num3, this.X[3], 15);
			num3 = this.FF3(num3, num5, num7, num, this.X[7], 7);
			num = this.FF3(num, num3, num5, num7, this.X[0], 12);
			num7 = this.FF3(num7, num, num3, num5, this.X[13], 8);
			num5 = this.FF3(num5, num7, num, num3, this.X[5], 9);
			num3 = this.FF3(num3, num5, num7, num, this.X[10], 11);
			num = this.FF3(num, num3, num5, num7, this.X[14], 7);
			num7 = this.FF3(num7, num, num3, num5, this.X[15], 7);
			num5 = this.FF3(num5, num7, num, num3, this.X[8], 12);
			num3 = this.FF3(num3, num5, num7, num, this.X[12], 7);
			num = this.FF3(num, num3, num5, num7, this.X[4], 6);
			num7 = this.FF3(num7, num, num3, num5, this.X[9], 15);
			num5 = this.FF3(num5, num7, num, num3, this.X[1], 13);
			num3 = this.FF3(num3, num5, num7, num, this.X[2], 11);
			num = this.FF2(num, num3, num5, num7, this.X[15], 9);
			num7 = this.FF2(num7, num, num3, num5, this.X[5], 7);
			num5 = this.FF2(num5, num7, num, num3, this.X[1], 15);
			num3 = this.FF2(num3, num5, num7, num, this.X[3], 11);
			num = this.FF2(num, num3, num5, num7, this.X[7], 8);
			num7 = this.FF2(num7, num, num3, num5, this.X[14], 6);
			num5 = this.FF2(num5, num7, num, num3, this.X[6], 6);
			num3 = this.FF2(num3, num5, num7, num, this.X[9], 14);
			num = this.FF2(num, num3, num5, num7, this.X[11], 12);
			num7 = this.FF2(num7, num, num3, num5, this.X[8], 13);
			num5 = this.FF2(num5, num7, num, num3, this.X[12], 5);
			num3 = this.FF2(num3, num5, num7, num, this.X[2], 14);
			num = this.FF2(num, num3, num5, num7, this.X[10], 13);
			num7 = this.FF2(num7, num, num3, num5, this.X[0], 13);
			num5 = this.FF2(num5, num7, num, num3, this.X[4], 7);
			num3 = this.FF2(num3, num5, num7, num, this.X[13], 5);
			num = this.FF1(num, num3, num5, num7, this.X[8], 15);
			num7 = this.FF1(num7, num, num3, num5, this.X[6], 5);
			num5 = this.FF1(num5, num7, num, num3, this.X[4], 8);
			num3 = this.FF1(num3, num5, num7, num, this.X[1], 11);
			num = this.FF1(num, num3, num5, num7, this.X[3], 14);
			num7 = this.FF1(num7, num, num3, num5, this.X[11], 14);
			num5 = this.FF1(num5, num7, num, num3, this.X[15], 6);
			num3 = this.FF1(num3, num5, num7, num, this.X[0], 14);
			num = this.FF1(num, num3, num5, num7, this.X[5], 6);
			num7 = this.FF1(num7, num, num3, num5, this.X[12], 9);
			num5 = this.FF1(num5, num7, num, num3, this.X[2], 12);
			num3 = this.FF1(num3, num5, num7, num, this.X[13], 9);
			num = this.FF1(num, num3, num5, num7, this.X[9], 12);
			num7 = this.FF1(num7, num, num3, num5, this.X[7], 5);
			num5 = this.FF1(num5, num7, num, num3, this.X[10], 15);
			num3 = this.FF1(num3, num5, num7, num, this.X[14], 8);
			num7 += num6 + this.H1;
			this.H1 = this.H2 + num8 + num;
			this.H2 = this.H3 + num2 + num3;
			this.H3 = this.H0 + num4 + num5;
			this.H0 = num7;
			this.xOff = 0;
			for (int num9 = 0; num9 != this.X.Length; num9++)
			{
				this.X[num9] = 0;
			}
		}

		public override IMemoable Copy()
		{
			return new RipeMD128Digest(this);
		}

		public override void Reset(IMemoable other)
		{
			RipeMD128Digest t = (RipeMD128Digest)other;
			this.CopyIn(t);
		}
	}
}
