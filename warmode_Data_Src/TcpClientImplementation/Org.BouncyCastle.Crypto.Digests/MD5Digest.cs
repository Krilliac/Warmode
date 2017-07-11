using Org.BouncyCastle.Crypto.Utilities;
using Org.BouncyCastle.Utilities;
using System;

namespace Org.BouncyCastle.Crypto.Digests
{
	public class MD5Digest : GeneralDigest
	{
		private const int DigestLength = 16;

		private uint H1;

		private uint H2;

		private uint H3;

		private uint H4;

		private uint[] X = new uint[16];

		private int xOff;

		private static readonly int S11 = 7;

		private static readonly int S12 = 12;

		private static readonly int S13 = 17;

		private static readonly int S14 = 22;

		private static readonly int S21 = 5;

		private static readonly int S22 = 9;

		private static readonly int S23 = 14;

		private static readonly int S24 = 20;

		private static readonly int S31 = 4;

		private static readonly int S32 = 11;

		private static readonly int S33 = 16;

		private static readonly int S34 = 23;

		private static readonly int S41 = 6;

		private static readonly int S42 = 10;

		private static readonly int S43 = 15;

		private static readonly int S44 = 21;

		public override string AlgorithmName
		{
			get
			{
				return "MD5";
			}
		}

		public MD5Digest()
		{
			this.Reset();
		}

		public MD5Digest(MD5Digest t) : base(t)
		{
			this.CopyIn(t);
		}

		private void CopyIn(MD5Digest t)
		{
			base.CopyIn(t);
			this.H1 = t.H1;
			this.H2 = t.H2;
			this.H3 = t.H3;
			this.H4 = t.H4;
			Array.Copy(t.X, 0, this.X, 0, t.X.Length);
			this.xOff = t.xOff;
		}

		public override int GetDigestSize()
		{
			return 16;
		}

		internal override void ProcessWord(byte[] input, int inOff)
		{
			this.X[this.xOff] = Pack.LE_To_UInt32(input, inOff);
			if (++this.xOff == 16)
			{
				this.ProcessBlock();
			}
		}

		internal override void ProcessLength(long bitLength)
		{
			if (this.xOff > 14)
			{
				if (this.xOff == 15)
				{
					this.X[15] = 0u;
				}
				this.ProcessBlock();
			}
			for (int i = this.xOff; i < 14; i++)
			{
				this.X[i] = 0u;
			}
			this.X[14] = (uint)bitLength;
			this.X[15] = (uint)((ulong)bitLength >> 32);
		}

		public override int DoFinal(byte[] output, int outOff)
		{
			base.Finish();
			Pack.UInt32_To_LE(this.H1, output, outOff);
			Pack.UInt32_To_LE(this.H2, output, outOff + 4);
			Pack.UInt32_To_LE(this.H3, output, outOff + 8);
			Pack.UInt32_To_LE(this.H4, output, outOff + 12);
			this.Reset();
			return 16;
		}

		public override void Reset()
		{
			base.Reset();
			this.H1 = 1732584193u;
			this.H2 = 4023233417u;
			this.H3 = 2562383102u;
			this.H4 = 271733878u;
			this.xOff = 0;
			for (int num = 0; num != this.X.Length; num++)
			{
				this.X[num] = 0u;
			}
		}

		private static uint RotateLeft(uint x, int n)
		{
			return x << n | x >> 32 - n;
		}

		private static uint F(uint u, uint v, uint w)
		{
			return (u & v) | (~u & w);
		}

		private static uint G(uint u, uint v, uint w)
		{
			return (u & w) | (v & ~w);
		}

		private static uint H(uint u, uint v, uint w)
		{
			return u ^ v ^ w;
		}

		private static uint K(uint u, uint v, uint w)
		{
			return v ^ (u | ~w);
		}

		internal override void ProcessBlock()
		{
			uint num = this.H1;
			uint num2 = this.H2;
			uint num3 = this.H3;
			uint num4 = this.H4;
			num = MD5Digest.RotateLeft(num + MD5Digest.F(num2, num3, num4) + this.X[0] + 3614090360u, MD5Digest.S11) + num2;
			num4 = MD5Digest.RotateLeft(num4 + MD5Digest.F(num, num2, num3) + this.X[1] + 3905402710u, MD5Digest.S12) + num;
			num3 = MD5Digest.RotateLeft(num3 + MD5Digest.F(num4, num, num2) + this.X[2] + 606105819u, MD5Digest.S13) + num4;
			num2 = MD5Digest.RotateLeft(num2 + MD5Digest.F(num3, num4, num) + this.X[3] + 3250441966u, MD5Digest.S14) + num3;
			num = MD5Digest.RotateLeft(num + MD5Digest.F(num2, num3, num4) + this.X[4] + 4118548399u, MD5Digest.S11) + num2;
			num4 = MD5Digest.RotateLeft(num4 + MD5Digest.F(num, num2, num3) + this.X[5] + 1200080426u, MD5Digest.S12) + num;
			num3 = MD5Digest.RotateLeft(num3 + MD5Digest.F(num4, num, num2) + this.X[6] + 2821735955u, MD5Digest.S13) + num4;
			num2 = MD5Digest.RotateLeft(num2 + MD5Digest.F(num3, num4, num) + this.X[7] + 4249261313u, MD5Digest.S14) + num3;
			num = MD5Digest.RotateLeft(num + MD5Digest.F(num2, num3, num4) + this.X[8] + 1770035416u, MD5Digest.S11) + num2;
			num4 = MD5Digest.RotateLeft(num4 + MD5Digest.F(num, num2, num3) + this.X[9] + 2336552879u, MD5Digest.S12) + num;
			num3 = MD5Digest.RotateLeft(num3 + MD5Digest.F(num4, num, num2) + this.X[10] + 4294925233u, MD5Digest.S13) + num4;
			num2 = MD5Digest.RotateLeft(num2 + MD5Digest.F(num3, num4, num) + this.X[11] + 2304563134u, MD5Digest.S14) + num3;
			num = MD5Digest.RotateLeft(num + MD5Digest.F(num2, num3, num4) + this.X[12] + 1804603682u, MD5Digest.S11) + num2;
			num4 = MD5Digest.RotateLeft(num4 + MD5Digest.F(num, num2, num3) + this.X[13] + 4254626195u, MD5Digest.S12) + num;
			num3 = MD5Digest.RotateLeft(num3 + MD5Digest.F(num4, num, num2) + this.X[14] + 2792965006u, MD5Digest.S13) + num4;
			num2 = MD5Digest.RotateLeft(num2 + MD5Digest.F(num3, num4, num) + this.X[15] + 1236535329u, MD5Digest.S14) + num3;
			num = MD5Digest.RotateLeft(num + MD5Digest.G(num2, num3, num4) + this.X[1] + 4129170786u, MD5Digest.S21) + num2;
			num4 = MD5Digest.RotateLeft(num4 + MD5Digest.G(num, num2, num3) + this.X[6] + 3225465664u, MD5Digest.S22) + num;
			num3 = MD5Digest.RotateLeft(num3 + MD5Digest.G(num4, num, num2) + this.X[11] + 643717713u, MD5Digest.S23) + num4;
			num2 = MD5Digest.RotateLeft(num2 + MD5Digest.G(num3, num4, num) + this.X[0] + 3921069994u, MD5Digest.S24) + num3;
			num = MD5Digest.RotateLeft(num + MD5Digest.G(num2, num3, num4) + this.X[5] + 3593408605u, MD5Digest.S21) + num2;
			num4 = MD5Digest.RotateLeft(num4 + MD5Digest.G(num, num2, num3) + this.X[10] + 38016083u, MD5Digest.S22) + num;
			num3 = MD5Digest.RotateLeft(num3 + MD5Digest.G(num4, num, num2) + this.X[15] + 3634488961u, MD5Digest.S23) + num4;
			num2 = MD5Digest.RotateLeft(num2 + MD5Digest.G(num3, num4, num) + this.X[4] + 3889429448u, MD5Digest.S24) + num3;
			num = MD5Digest.RotateLeft(num + MD5Digest.G(num2, num3, num4) + this.X[9] + 568446438u, MD5Digest.S21) + num2;
			num4 = MD5Digest.RotateLeft(num4 + MD5Digest.G(num, num2, num3) + this.X[14] + 3275163606u, MD5Digest.S22) + num;
			num3 = MD5Digest.RotateLeft(num3 + MD5Digest.G(num4, num, num2) + this.X[3] + 4107603335u, MD5Digest.S23) + num4;
			num2 = MD5Digest.RotateLeft(num2 + MD5Digest.G(num3, num4, num) + this.X[8] + 1163531501u, MD5Digest.S24) + num3;
			num = MD5Digest.RotateLeft(num + MD5Digest.G(num2, num3, num4) + this.X[13] + 2850285829u, MD5Digest.S21) + num2;
			num4 = MD5Digest.RotateLeft(num4 + MD5Digest.G(num, num2, num3) + this.X[2] + 4243563512u, MD5Digest.S22) + num;
			num3 = MD5Digest.RotateLeft(num3 + MD5Digest.G(num4, num, num2) + this.X[7] + 1735328473u, MD5Digest.S23) + num4;
			num2 = MD5Digest.RotateLeft(num2 + MD5Digest.G(num3, num4, num) + this.X[12] + 2368359562u, MD5Digest.S24) + num3;
			num = MD5Digest.RotateLeft(num + MD5Digest.H(num2, num3, num4) + this.X[5] + 4294588738u, MD5Digest.S31) + num2;
			num4 = MD5Digest.RotateLeft(num4 + MD5Digest.H(num, num2, num3) + this.X[8] + 2272392833u, MD5Digest.S32) + num;
			num3 = MD5Digest.RotateLeft(num3 + MD5Digest.H(num4, num, num2) + this.X[11] + 1839030562u, MD5Digest.S33) + num4;
			num2 = MD5Digest.RotateLeft(num2 + MD5Digest.H(num3, num4, num) + this.X[14] + 4259657740u, MD5Digest.S34) + num3;
			num = MD5Digest.RotateLeft(num + MD5Digest.H(num2, num3, num4) + this.X[1] + 2763975236u, MD5Digest.S31) + num2;
			num4 = MD5Digest.RotateLeft(num4 + MD5Digest.H(num, num2, num3) + this.X[4] + 1272893353u, MD5Digest.S32) + num;
			num3 = MD5Digest.RotateLeft(num3 + MD5Digest.H(num4, num, num2) + this.X[7] + 4139469664u, MD5Digest.S33) + num4;
			num2 = MD5Digest.RotateLeft(num2 + MD5Digest.H(num3, num4, num) + this.X[10] + 3200236656u, MD5Digest.S34) + num3;
			num = MD5Digest.RotateLeft(num + MD5Digest.H(num2, num3, num4) + this.X[13] + 681279174u, MD5Digest.S31) + num2;
			num4 = MD5Digest.RotateLeft(num4 + MD5Digest.H(num, num2, num3) + this.X[0] + 3936430074u, MD5Digest.S32) + num;
			num3 = MD5Digest.RotateLeft(num3 + MD5Digest.H(num4, num, num2) + this.X[3] + 3572445317u, MD5Digest.S33) + num4;
			num2 = MD5Digest.RotateLeft(num2 + MD5Digest.H(num3, num4, num) + this.X[6] + 76029189u, MD5Digest.S34) + num3;
			num = MD5Digest.RotateLeft(num + MD5Digest.H(num2, num3, num4) + this.X[9] + 3654602809u, MD5Digest.S31) + num2;
			num4 = MD5Digest.RotateLeft(num4 + MD5Digest.H(num, num2, num3) + this.X[12] + 3873151461u, MD5Digest.S32) + num;
			num3 = MD5Digest.RotateLeft(num3 + MD5Digest.H(num4, num, num2) + this.X[15] + 530742520u, MD5Digest.S33) + num4;
			num2 = MD5Digest.RotateLeft(num2 + MD5Digest.H(num3, num4, num) + this.X[2] + 3299628645u, MD5Digest.S34) + num3;
			num = MD5Digest.RotateLeft(num + MD5Digest.K(num2, num3, num4) + this.X[0] + 4096336452u, MD5Digest.S41) + num2;
			num4 = MD5Digest.RotateLeft(num4 + MD5Digest.K(num, num2, num3) + this.X[7] + 1126891415u, MD5Digest.S42) + num;
			num3 = MD5Digest.RotateLeft(num3 + MD5Digest.K(num4, num, num2) + this.X[14] + 2878612391u, MD5Digest.S43) + num4;
			num2 = MD5Digest.RotateLeft(num2 + MD5Digest.K(num3, num4, num) + this.X[5] + 4237533241u, MD5Digest.S44) + num3;
			num = MD5Digest.RotateLeft(num + MD5Digest.K(num2, num3, num4) + this.X[12] + 1700485571u, MD5Digest.S41) + num2;
			num4 = MD5Digest.RotateLeft(num4 + MD5Digest.K(num, num2, num3) + this.X[3] + 2399980690u, MD5Digest.S42) + num;
			num3 = MD5Digest.RotateLeft(num3 + MD5Digest.K(num4, num, num2) + this.X[10] + 4293915773u, MD5Digest.S43) + num4;
			num2 = MD5Digest.RotateLeft(num2 + MD5Digest.K(num3, num4, num) + this.X[1] + 2240044497u, MD5Digest.S44) + num3;
			num = MD5Digest.RotateLeft(num + MD5Digest.K(num2, num3, num4) + this.X[8] + 1873313359u, MD5Digest.S41) + num2;
			num4 = MD5Digest.RotateLeft(num4 + MD5Digest.K(num, num2, num3) + this.X[15] + 4264355552u, MD5Digest.S42) + num;
			num3 = MD5Digest.RotateLeft(num3 + MD5Digest.K(num4, num, num2) + this.X[6] + 2734768916u, MD5Digest.S43) + num4;
			num2 = MD5Digest.RotateLeft(num2 + MD5Digest.K(num3, num4, num) + this.X[13] + 1309151649u, MD5Digest.S44) + num3;
			num = MD5Digest.RotateLeft(num + MD5Digest.K(num2, num3, num4) + this.X[4] + 4149444226u, MD5Digest.S41) + num2;
			num4 = MD5Digest.RotateLeft(num4 + MD5Digest.K(num, num2, num3) + this.X[11] + 3174756917u, MD5Digest.S42) + num;
			num3 = MD5Digest.RotateLeft(num3 + MD5Digest.K(num4, num, num2) + this.X[2] + 718787259u, MD5Digest.S43) + num4;
			num2 = MD5Digest.RotateLeft(num2 + MD5Digest.K(num3, num4, num) + this.X[9] + 3951481745u, MD5Digest.S44) + num3;
			this.H1 += num;
			this.H2 += num2;
			this.H3 += num3;
			this.H4 += num4;
			this.xOff = 0;
		}

		public override IMemoable Copy()
		{
			return new MD5Digest(this);
		}

		public override void Reset(IMemoable other)
		{
			MD5Digest t = (MD5Digest)other;
			this.CopyIn(t);
		}
	}
}
