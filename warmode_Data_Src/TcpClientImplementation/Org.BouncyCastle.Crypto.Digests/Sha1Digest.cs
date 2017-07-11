using Org.BouncyCastle.Crypto.Utilities;
using Org.BouncyCastle.Utilities;
using System;

namespace Org.BouncyCastle.Crypto.Digests
{
	public class Sha1Digest : GeneralDigest
	{
		private const int DigestLength = 20;

		private const uint Y1 = 1518500249u;

		private const uint Y2 = 1859775393u;

		private const uint Y3 = 2400959708u;

		private const uint Y4 = 3395469782u;

		private uint H1;

		private uint H2;

		private uint H3;

		private uint H4;

		private uint H5;

		private uint[] X = new uint[80];

		private int xOff;

		public override string AlgorithmName
		{
			get
			{
				return "SHA-1";
			}
		}

		public Sha1Digest()
		{
			this.Reset();
		}

		public Sha1Digest(Sha1Digest t) : base(t)
		{
			this.CopyIn(t);
		}

		private void CopyIn(Sha1Digest t)
		{
			base.CopyIn(t);
			this.H1 = t.H1;
			this.H2 = t.H2;
			this.H3 = t.H3;
			this.H4 = t.H4;
			this.H5 = t.H5;
			Array.Copy(t.X, 0, this.X, 0, t.X.Length);
			this.xOff = t.xOff;
		}

		public override int GetDigestSize()
		{
			return 20;
		}

		internal override void ProcessWord(byte[] input, int inOff)
		{
			this.X[this.xOff] = Pack.BE_To_UInt32(input, inOff);
			if (++this.xOff == 16)
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
			this.X[14] = (uint)((ulong)bitLength >> 32);
			this.X[15] = (uint)bitLength;
		}

		public override int DoFinal(byte[] output, int outOff)
		{
			base.Finish();
			Pack.UInt32_To_BE(this.H1, output, outOff);
			Pack.UInt32_To_BE(this.H2, output, outOff + 4);
			Pack.UInt32_To_BE(this.H3, output, outOff + 8);
			Pack.UInt32_To_BE(this.H4, output, outOff + 12);
			Pack.UInt32_To_BE(this.H5, output, outOff + 16);
			this.Reset();
			return 20;
		}

		public override void Reset()
		{
			base.Reset();
			this.H1 = 1732584193u;
			this.H2 = 4023233417u;
			this.H3 = 2562383102u;
			this.H4 = 271733878u;
			this.H5 = 3285377520u;
			this.xOff = 0;
			Array.Clear(this.X, 0, this.X.Length);
		}

		private static uint F(uint u, uint v, uint w)
		{
			return (u & v) | (~u & w);
		}

		private static uint H(uint u, uint v, uint w)
		{
			return u ^ v ^ w;
		}

		private static uint G(uint u, uint v, uint w)
		{
			return (u & v) | (u & w) | (v & w);
		}

		internal override void ProcessBlock()
		{
			for (int i = 16; i < 80; i++)
			{
				uint num = this.X[i - 3] ^ this.X[i - 8] ^ this.X[i - 14] ^ this.X[i - 16];
				this.X[i] = (num << 1 | num >> 31);
			}
			uint num2 = this.H1;
			uint num3 = this.H2;
			uint num4 = this.H3;
			uint num5 = this.H4;
			uint num6 = this.H5;
			int num7 = 0;
			for (int j = 0; j < 4; j++)
			{
				num6 += (num2 << 5 | num2 >> 27) + Sha1Digest.F(num3, num4, num5) + this.X[num7++] + 1518500249u;
				num3 = (num3 << 30 | num3 >> 2);
				num5 += (num6 << 5 | num6 >> 27) + Sha1Digest.F(num2, num3, num4) + this.X[num7++] + 1518500249u;
				num2 = (num2 << 30 | num2 >> 2);
				num4 += (num5 << 5 | num5 >> 27) + Sha1Digest.F(num6, num2, num3) + this.X[num7++] + 1518500249u;
				num6 = (num6 << 30 | num6 >> 2);
				num3 += (num4 << 5 | num4 >> 27) + Sha1Digest.F(num5, num6, num2) + this.X[num7++] + 1518500249u;
				num5 = (num5 << 30 | num5 >> 2);
				num2 += (num3 << 5 | num3 >> 27) + Sha1Digest.F(num4, num5, num6) + this.X[num7++] + 1518500249u;
				num4 = (num4 << 30 | num4 >> 2);
			}
			for (int k = 0; k < 4; k++)
			{
				num6 += (num2 << 5 | num2 >> 27) + Sha1Digest.H(num3, num4, num5) + this.X[num7++] + 1859775393u;
				num3 = (num3 << 30 | num3 >> 2);
				num5 += (num6 << 5 | num6 >> 27) + Sha1Digest.H(num2, num3, num4) + this.X[num7++] + 1859775393u;
				num2 = (num2 << 30 | num2 >> 2);
				num4 += (num5 << 5 | num5 >> 27) + Sha1Digest.H(num6, num2, num3) + this.X[num7++] + 1859775393u;
				num6 = (num6 << 30 | num6 >> 2);
				num3 += (num4 << 5 | num4 >> 27) + Sha1Digest.H(num5, num6, num2) + this.X[num7++] + 1859775393u;
				num5 = (num5 << 30 | num5 >> 2);
				num2 += (num3 << 5 | num3 >> 27) + Sha1Digest.H(num4, num5, num6) + this.X[num7++] + 1859775393u;
				num4 = (num4 << 30 | num4 >> 2);
			}
			for (int l = 0; l < 4; l++)
			{
				num6 += (num2 << 5 | num2 >> 27) + Sha1Digest.G(num3, num4, num5) + this.X[num7++] + 2400959708u;
				num3 = (num3 << 30 | num3 >> 2);
				num5 += (num6 << 5 | num6 >> 27) + Sha1Digest.G(num2, num3, num4) + this.X[num7++] + 2400959708u;
				num2 = (num2 << 30 | num2 >> 2);
				num4 += (num5 << 5 | num5 >> 27) + Sha1Digest.G(num6, num2, num3) + this.X[num7++] + 2400959708u;
				num6 = (num6 << 30 | num6 >> 2);
				num3 += (num4 << 5 | num4 >> 27) + Sha1Digest.G(num5, num6, num2) + this.X[num7++] + 2400959708u;
				num5 = (num5 << 30 | num5 >> 2);
				num2 += (num3 << 5 | num3 >> 27) + Sha1Digest.G(num4, num5, num6) + this.X[num7++] + 2400959708u;
				num4 = (num4 << 30 | num4 >> 2);
			}
			for (int m = 0; m < 4; m++)
			{
				num6 += (num2 << 5 | num2 >> 27) + Sha1Digest.H(num3, num4, num5) + this.X[num7++] + 3395469782u;
				num3 = (num3 << 30 | num3 >> 2);
				num5 += (num6 << 5 | num6 >> 27) + Sha1Digest.H(num2, num3, num4) + this.X[num7++] + 3395469782u;
				num2 = (num2 << 30 | num2 >> 2);
				num4 += (num5 << 5 | num5 >> 27) + Sha1Digest.H(num6, num2, num3) + this.X[num7++] + 3395469782u;
				num6 = (num6 << 30 | num6 >> 2);
				num3 += (num4 << 5 | num4 >> 27) + Sha1Digest.H(num5, num6, num2) + this.X[num7++] + 3395469782u;
				num5 = (num5 << 30 | num5 >> 2);
				num2 += (num3 << 5 | num3 >> 27) + Sha1Digest.H(num4, num5, num6) + this.X[num7++] + 3395469782u;
				num4 = (num4 << 30 | num4 >> 2);
			}
			this.H1 += num2;
			this.H2 += num3;
			this.H3 += num4;
			this.H4 += num5;
			this.H5 += num6;
			this.xOff = 0;
			Array.Clear(this.X, 0, 16);
		}

		public override IMemoable Copy()
		{
			return new Sha1Digest(this);
		}

		public override void Reset(IMemoable other)
		{
			Sha1Digest t = (Sha1Digest)other;
			this.CopyIn(t);
		}
	}
}
