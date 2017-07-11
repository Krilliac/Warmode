using Org.BouncyCastle.Crypto.Utilities;
using Org.BouncyCastle.Utilities;
using System;

namespace Org.BouncyCastle.Crypto.Digests
{
	public class Sha512Digest : LongDigest
	{
		private const int DigestLength = 64;

		public override string AlgorithmName
		{
			get
			{
				return "SHA-512";
			}
		}

		public Sha512Digest()
		{
		}

		public Sha512Digest(Sha512Digest t) : base(t)
		{
		}

		public override int GetDigestSize()
		{
			return 64;
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
			Pack.UInt64_To_BE(this.H7, output, outOff + 48);
			Pack.UInt64_To_BE(this.H8, output, outOff + 56);
			this.Reset();
			return 64;
		}

		public override void Reset()
		{
			base.Reset();
			this.H1 = 7640891576956012808uL;
			this.H2 = 13503953896175478587uL;
			this.H3 = 4354685564936845355uL;
			this.H4 = 11912009170470909681uL;
			this.H5 = 5840696475078001361uL;
			this.H6 = 11170449401992604703uL;
			this.H7 = 2270897969802886507uL;
			this.H8 = 6620516959819538809uL;
		}

		public override IMemoable Copy()
		{
			return new Sha512Digest(this);
		}

		public override void Reset(IMemoable other)
		{
			Sha512Digest t = (Sha512Digest)other;
			base.CopyIn(t);
		}
	}
}
