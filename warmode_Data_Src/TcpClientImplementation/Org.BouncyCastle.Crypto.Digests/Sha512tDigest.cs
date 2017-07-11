using Org.BouncyCastle.Utilities;
using System;

namespace Org.BouncyCastle.Crypto.Digests
{
	public class Sha512tDigest : LongDigest
	{
		private const ulong A5 = 11936128518282651045uL;

		private readonly int digestLength;

		private ulong H1t;

		private ulong H2t;

		private ulong H3t;

		private ulong H4t;

		private ulong H5t;

		private ulong H6t;

		private ulong H7t;

		private ulong H8t;

		public override string AlgorithmName
		{
			get
			{
				return "SHA-512/" + this.digestLength * 8;
			}
		}

		public Sha512tDigest(int bitLength)
		{
			if (bitLength >= 512)
			{
				throw new ArgumentException("cannot be >= 512", "bitLength");
			}
			if (bitLength % 8 != 0)
			{
				throw new ArgumentException("needs to be a multiple of 8", "bitLength");
			}
			if (bitLength == 384)
			{
				throw new ArgumentException("cannot be 384 use SHA384 instead", "bitLength");
			}
			this.digestLength = bitLength / 8;
			this.tIvGenerate(this.digestLength * 8);
			this.Reset();
		}

		public Sha512tDigest(Sha512tDigest t) : base(t)
		{
			this.digestLength = t.digestLength;
			this.Reset(t);
		}

		public override int GetDigestSize()
		{
			return this.digestLength;
		}

		public override int DoFinal(byte[] output, int outOff)
		{
			base.Finish();
			Sha512tDigest.UInt64_To_BE(this.H1, output, outOff, this.digestLength);
			Sha512tDigest.UInt64_To_BE(this.H2, output, outOff + 8, this.digestLength - 8);
			Sha512tDigest.UInt64_To_BE(this.H3, output, outOff + 16, this.digestLength - 16);
			Sha512tDigest.UInt64_To_BE(this.H4, output, outOff + 24, this.digestLength - 24);
			Sha512tDigest.UInt64_To_BE(this.H5, output, outOff + 32, this.digestLength - 32);
			Sha512tDigest.UInt64_To_BE(this.H6, output, outOff + 40, this.digestLength - 40);
			Sha512tDigest.UInt64_To_BE(this.H7, output, outOff + 48, this.digestLength - 48);
			Sha512tDigest.UInt64_To_BE(this.H8, output, outOff + 56, this.digestLength - 56);
			this.Reset();
			return this.digestLength;
		}

		public override void Reset()
		{
			base.Reset();
			this.H1 = this.H1t;
			this.H2 = this.H2t;
			this.H3 = this.H3t;
			this.H4 = this.H4t;
			this.H5 = this.H5t;
			this.H6 = this.H6t;
			this.H7 = this.H7t;
			this.H8 = this.H8t;
		}

		private void tIvGenerate(int bitLength)
		{
			this.H1 = 14964410163792538797uL;
			this.H2 = 2216346199247487646uL;
			this.H3 = 11082046791023156622uL;
			this.H4 = 65953792586715988uL;
			this.H5 = 17630457682085488500uL;
			this.H6 = 4512832404995164602uL;
			this.H7 = 13413544941332994254uL;
			this.H8 = 18322165818757711068uL;
			base.Update(83);
			base.Update(72);
			base.Update(65);
			base.Update(45);
			base.Update(53);
			base.Update(49);
			base.Update(50);
			base.Update(47);
			if (bitLength > 100)
			{
				base.Update((byte)(bitLength / 100 + 48));
				bitLength %= 100;
				base.Update((byte)(bitLength / 10 + 48));
				bitLength %= 10;
				base.Update((byte)(bitLength + 48));
			}
			else if (bitLength > 10)
			{
				base.Update((byte)(bitLength / 10 + 48));
				bitLength %= 10;
				base.Update((byte)(bitLength + 48));
			}
			else
			{
				base.Update((byte)(bitLength + 48));
			}
			base.Finish();
			this.H1t = this.H1;
			this.H2t = this.H2;
			this.H3t = this.H3;
			this.H4t = this.H4;
			this.H5t = this.H5;
			this.H6t = this.H6;
			this.H7t = this.H7;
			this.H8t = this.H8;
		}

		private static void UInt64_To_BE(ulong n, byte[] bs, int off, int max)
		{
			if (max > 0)
			{
				Sha512tDigest.UInt32_To_BE((uint)(n >> 32), bs, off, max);
				if (max > 4)
				{
					Sha512tDigest.UInt32_To_BE((uint)n, bs, off + 4, max - 4);
				}
			}
		}

		private static void UInt32_To_BE(uint n, byte[] bs, int off, int max)
		{
			int num = Math.Min(4, max);
			while (--num >= 0)
			{
				int num2 = 8 * (3 - num);
				bs[off + num] = (byte)(n >> num2);
			}
		}

		public override IMemoable Copy()
		{
			return new Sha512tDigest(this);
		}

		public override void Reset(IMemoable other)
		{
			Sha512tDigest sha512tDigest = (Sha512tDigest)other;
			if (this.digestLength != sha512tDigest.digestLength)
			{
				throw new MemoableResetException("digestLength inappropriate in other");
			}
			base.CopyIn(sha512tDigest);
			this.H1t = sha512tDigest.H1t;
			this.H2t = sha512tDigest.H2t;
			this.H3t = sha512tDigest.H3t;
			this.H4t = sha512tDigest.H4t;
			this.H5t = sha512tDigest.H5t;
			this.H6t = sha512tDigest.H6t;
			this.H7t = sha512tDigest.H7t;
			this.H8t = sha512tDigest.H8t;
		}
	}
}
