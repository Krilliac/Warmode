using Org.BouncyCastle.Crypto.Utilities;
using Org.BouncyCastle.Utilities;
using System;

namespace Org.BouncyCastle.Crypto.Digests
{
	public class Sha384Digest : LongDigest
	{
		private const int DigestLength = 48;

		public override string AlgorithmName
		{
			get
			{
				return "SHA-384";
			}
		}

		public Sha384Digest()
		{
		}

		public Sha384Digest(Sha384Digest t) : base(t)
		{
		}

		public override int GetDigestSize()
		{
			return 48;
		}

		public override int DoFinal(byte[] output, int outOff)
		{
			base.Finish();
			Pack.UInt64_To_BE(this.H1, output, outOff);
			Pack.UInt64_To_BE(this.H2, output, outOff + 8);
			Pack.UInt64_To_BE(this.H3, output, outOff + 16);
			Pack.UInt64_To_BE(this.H4, output, outOff + 24);
			Pack.UInt64_To_BE(this.H5, output, outOff + 32);
			Pack.UInt64_To_BE(this.H6, output, outOff + 40);
			this.Reset();
			return 48;
		}

		public override void Reset()
		{
			base.Reset();
			this.H1 = 14680500436340154072uL;
			this.H2 = 7105036623409894663uL;
			this.H3 = 10473403895298186519uL;
			this.H4 = 1526699215303891257uL;
			this.H5 = 7436329637833083697uL;
			this.H6 = 10282925794625328401uL;
			this.H7 = 15784041429090275239uL;
			this.H8 = 5167115440072839076uL;
		}

		public override IMemoable Copy()
		{
			return new Sha384Digest(this);
		}

		public override void Reset(IMemoable other)
		{
			Sha384Digest t = (Sha384Digest)other;
			base.CopyIn(t);
		}
	}
}
