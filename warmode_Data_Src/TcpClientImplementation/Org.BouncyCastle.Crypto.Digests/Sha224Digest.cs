using Org.BouncyCastle.Crypto.Utilities;
using Org.BouncyCastle.Utilities;
using System;

namespace Org.BouncyCastle.Crypto.Digests
{
	public class Sha224Digest : GeneralDigest
	{
		private const int DigestLength = 28;

		private uint H1;

		private uint H2;

		private uint H3;

		private uint H4;

		private uint H5;

		private uint H6;

		private uint H7;

		private uint H8;

		private uint[] X = new uint[64];

		private int xOff;

		internal static readonly uint[] K = new uint[]
		{
			1116352408u,
			1899447441u,
			3049323471u,
			3921009573u,
			961987163u,
			1508970993u,
			2453635748u,
			2870763221u,
			3624381080u,
			310598401u,
			607225278u,
			1426881987u,
			1925078388u,
			2162078206u,
			2614888103u,
			3248222580u,
			3835390401u,
			4022224774u,
			264347078u,
			604807628u,
			770255983u,
			1249150122u,
			1555081692u,
			1996064986u,
			2554220882u,
			2821834349u,
			2952996808u,
			3210313671u,
			3336571891u,
			3584528711u,
			113926993u,
			338241895u,
			666307205u,
			773529912u,
			1294757372u,
			1396182291u,
			1695183700u,
			1986661051u,
			2177026350u,
			2456956037u,
			2730485921u,
			2820302411u,
			3259730800u,
			3345764771u,
			3516065817u,
			3600352804u,
			4094571909u,
			275423344u,
			430227734u,
			506948616u,
			659060556u,
			883997877u,
			958139571u,
			1322822218u,
			1537002063u,
			1747873779u,
			1955562222u,
			2024104815u,
			2227730452u,
			2361852424u,
			2428436474u,
			2756734187u,
			3204031479u,
			3329325298u
		};

		public override string AlgorithmName
		{
			get
			{
				return "SHA-224";
			}
		}

		public Sha224Digest()
		{
			this.Reset();
		}

		public Sha224Digest(Sha224Digest t) : base(t)
		{
			this.CopyIn(t);
		}

		private void CopyIn(Sha224Digest t)
		{
			base.CopyIn(t);
			this.H1 = t.H1;
			this.H2 = t.H2;
			this.H3 = t.H3;
			this.H4 = t.H4;
			this.H5 = t.H5;
			this.H6 = t.H6;
			this.H7 = t.H7;
			this.H8 = t.H8;
			Array.Copy(t.X, 0, this.X, 0, t.X.Length);
			this.xOff = t.xOff;
		}

		public override int GetDigestSize()
		{
			return 28;
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
			Pack.UInt32_To_BE(this.H6, output, outOff + 20);
			Pack.UInt32_To_BE(this.H7, output, outOff + 24);
			this.Reset();
			return 28;
		}

		public override void Reset()
		{
			base.Reset();
			this.H1 = 3238371032u;
			this.H2 = 914150663u;
			this.H3 = 812702999u;
			this.H4 = 4144912697u;
			this.H5 = 4290775857u;
			this.H6 = 1750603025u;
			this.H7 = 1694076839u;
			this.H8 = 3204075428u;
			this.xOff = 0;
			Array.Clear(this.X, 0, this.X.Length);
		}

		internal override void ProcessBlock()
		{
			for (int i = 16; i <= 63; i++)
			{
				this.X[i] = Sha224Digest.Theta1(this.X[i - 2]) + this.X[i - 7] + Sha224Digest.Theta0(this.X[i - 15]) + this.X[i - 16];
			}
			uint num = this.H1;
			uint num2 = this.H2;
			uint num3 = this.H3;
			uint num4 = this.H4;
			uint num5 = this.H5;
			uint num6 = this.H6;
			uint num7 = this.H7;
			uint num8 = this.H8;
			int num9 = 0;
			for (int j = 0; j < 8; j++)
			{
				num8 += Sha224Digest.Sum1(num5) + Sha224Digest.Ch(num5, num6, num7) + Sha224Digest.K[num9] + this.X[num9];
				num4 += num8;
				num8 += Sha224Digest.Sum0(num) + Sha224Digest.Maj(num, num2, num3);
				num9++;
				num7 += Sha224Digest.Sum1(num4) + Sha224Digest.Ch(num4, num5, num6) + Sha224Digest.K[num9] + this.X[num9];
				num3 += num7;
				num7 += Sha224Digest.Sum0(num8) + Sha224Digest.Maj(num8, num, num2);
				num9++;
				num6 += Sha224Digest.Sum1(num3) + Sha224Digest.Ch(num3, num4, num5) + Sha224Digest.K[num9] + this.X[num9];
				num2 += num6;
				num6 += Sha224Digest.Sum0(num7) + Sha224Digest.Maj(num7, num8, num);
				num9++;
				num5 += Sha224Digest.Sum1(num2) + Sha224Digest.Ch(num2, num3, num4) + Sha224Digest.K[num9] + this.X[num9];
				num += num5;
				num5 += Sha224Digest.Sum0(num6) + Sha224Digest.Maj(num6, num7, num8);
				num9++;
				num4 += Sha224Digest.Sum1(num) + Sha224Digest.Ch(num, num2, num3) + Sha224Digest.K[num9] + this.X[num9];
				num8 += num4;
				num4 += Sha224Digest.Sum0(num5) + Sha224Digest.Maj(num5, num6, num7);
				num9++;
				num3 += Sha224Digest.Sum1(num8) + Sha224Digest.Ch(num8, num, num2) + Sha224Digest.K[num9] + this.X[num9];
				num7 += num3;
				num3 += Sha224Digest.Sum0(num4) + Sha224Digest.Maj(num4, num5, num6);
				num9++;
				num2 += Sha224Digest.Sum1(num7) + Sha224Digest.Ch(num7, num8, num) + Sha224Digest.K[num9] + this.X[num9];
				num6 += num2;
				num2 += Sha224Digest.Sum0(num3) + Sha224Digest.Maj(num3, num4, num5);
				num9++;
				num += Sha224Digest.Sum1(num6) + Sha224Digest.Ch(num6, num7, num8) + Sha224Digest.K[num9] + this.X[num9];
				num5 += num;
				num += Sha224Digest.Sum0(num2) + Sha224Digest.Maj(num2, num3, num4);
				num9++;
			}
			this.H1 += num;
			this.H2 += num2;
			this.H3 += num3;
			this.H4 += num4;
			this.H5 += num5;
			this.H6 += num6;
			this.H7 += num7;
			this.H8 += num8;
			this.xOff = 0;
			Array.Clear(this.X, 0, 16);
		}

		private static uint Ch(uint x, uint y, uint z)
		{
			return (x & y) ^ (~x & z);
		}

		private static uint Maj(uint x, uint y, uint z)
		{
			return (x & y) ^ (x & z) ^ (y & z);
		}

		private static uint Sum0(uint x)
		{
			return (x >> 2 | x << 30) ^ (x >> 13 | x << 19) ^ (x >> 22 | x << 10);
		}

		private static uint Sum1(uint x)
		{
			return (x >> 6 | x << 26) ^ (x >> 11 | x << 21) ^ (x >> 25 | x << 7);
		}

		private static uint Theta0(uint x)
		{
			return (x >> 7 | x << 25) ^ (x >> 18 | x << 14) ^ x >> 3;
		}

		private static uint Theta1(uint x)
		{
			return (x >> 17 | x << 15) ^ (x >> 19 | x << 13) ^ x >> 10;
		}

		public override IMemoable Copy()
		{
			return new Sha224Digest(this);
		}

		public override void Reset(IMemoable other)
		{
			Sha224Digest t = (Sha224Digest)other;
			this.CopyIn(t);
		}
	}
}
