using Org.BouncyCastle.Crypto.Utilities;
using Org.BouncyCastle.Utilities;
using System;

namespace Org.BouncyCastle.Crypto.Digests
{
	public abstract class LongDigest : IDigest, IMemoable
	{
		private int MyByteLength = 128;

		private byte[] xBuf;

		private int xBufOff;

		private long byteCount1;

		private long byteCount2;

		internal ulong H1;

		internal ulong H2;

		internal ulong H3;

		internal ulong H4;

		internal ulong H5;

		internal ulong H6;

		internal ulong H7;

		internal ulong H8;

		private ulong[] W = new ulong[80];

		private int wOff;

		internal static readonly ulong[] K = new ulong[]
		{
			4794697086780616226uL,
			8158064640168781261uL,
			13096744586834688815uL,
			16840607885511220156uL,
			4131703408338449720uL,
			6480981068601479193uL,
			10538285296894168987uL,
			12329834152419229976uL,
			15566598209576043074uL,
			1334009975649890238uL,
			2608012711638119052uL,
			6128411473006802146uL,
			8268148722764581231uL,
			9286055187155687089uL,
			11230858885718282805uL,
			13951009754708518548uL,
			16472876342353939154uL,
			17275323862435702243uL,
			1135362057144423861uL,
			2597628984639134821uL,
			3308224258029322869uL,
			5365058923640841347uL,
			6679025012923562964uL,
			8573033837759648693uL,
			10970295158949994411uL,
			12119686244451234320uL,
			12683024718118986047uL,
			13788192230050041572uL,
			14330467153632333762uL,
			15395433587784984357uL,
			489312712824947311uL,
			1452737877330783856uL,
			2861767655752347644uL,
			3322285676063803686uL,
			5560940570517711597uL,
			5996557281743188959uL,
			7280758554555802590uL,
			8532644243296465576uL,
			9350256976987008742uL,
			10552545826968843579uL,
			11727347734174303076uL,
			12113106623233404929uL,
			14000437183269869457uL,
			14369950271660146224uL,
			15101387698204529176uL,
			15463397548674623760uL,
			17586052441742319658uL,
			1182934255886127544uL,
			1847814050463011016uL,
			2177327727835720531uL,
			2830643537854262169uL,
			3796741975233480872uL,
			4115178125766777443uL,
			5681478168544905931uL,
			6601373596472566643uL,
			7507060721942968483uL,
			8399075790359081724uL,
			8693463985226723168uL,
			9568029438360202098uL,
			10144078919501101548uL,
			10430055236837252648uL,
			11840083180663258601uL,
			13761210420658862357uL,
			14299343276471374635uL,
			14566680578165727644uL,
			15097957966210449927uL,
			16922976911328602910uL,
			17689382322260857208uL,
			500013540394364858uL,
			748580250866718886uL,
			1242879168328830382uL,
			1977374033974150939uL,
			2944078676154940804uL,
			3659926193048069267uL,
			4368137639120453308uL,
			4836135668995329356uL,
			5532061633213252278uL,
			6448918945643986474uL,
			6902733635092675308uL,
			7801388544844847127uL
		};

		public abstract string AlgorithmName
		{
			get;
		}

		internal LongDigest()
		{
			this.xBuf = new byte[8];
			this.Reset();
		}

		internal LongDigest(LongDigest t)
		{
			this.xBuf = new byte[t.xBuf.Length];
			this.CopyIn(t);
		}

		protected void CopyIn(LongDigest t)
		{
			Array.Copy(t.xBuf, 0, this.xBuf, 0, t.xBuf.Length);
			this.xBufOff = t.xBufOff;
			this.byteCount1 = t.byteCount1;
			this.byteCount2 = t.byteCount2;
			this.H1 = t.H1;
			this.H2 = t.H2;
			this.H3 = t.H3;
			this.H4 = t.H4;
			this.H5 = t.H5;
			this.H6 = t.H6;
			this.H7 = t.H7;
			this.H8 = t.H8;
			Array.Copy(t.W, 0, this.W, 0, t.W.Length);
			this.wOff = t.wOff;
		}

		public void Update(byte input)
		{
			this.xBuf[this.xBufOff++] = input;
			if (this.xBufOff == this.xBuf.Length)
			{
				this.ProcessWord(this.xBuf, 0);
				this.xBufOff = 0;
			}
			this.byteCount1 += 1L;
		}

		public void BlockUpdate(byte[] input, int inOff, int length)
		{
			while (this.xBufOff != 0)
			{
				if (length <= 0)
				{
					break;
				}
				this.Update(input[inOff]);
				inOff++;
				length--;
			}
			while (length > this.xBuf.Length)
			{
				this.ProcessWord(input, inOff);
				inOff += this.xBuf.Length;
				length -= this.xBuf.Length;
				this.byteCount1 += (long)this.xBuf.Length;
			}
			while (length > 0)
			{
				this.Update(input[inOff]);
				inOff++;
				length--;
			}
		}

		public void Finish()
		{
			this.AdjustByteCounts();
			long lowW = this.byteCount1 << 3;
			long hiW = this.byteCount2;
			this.Update(128);
			while (this.xBufOff != 0)
			{
				this.Update(0);
			}
			this.ProcessLength(lowW, hiW);
			this.ProcessBlock();
		}

		public virtual void Reset()
		{
			this.byteCount1 = 0L;
			this.byteCount2 = 0L;
			this.xBufOff = 0;
			for (int i = 0; i < this.xBuf.Length; i++)
			{
				this.xBuf[i] = 0;
			}
			this.wOff = 0;
			Array.Clear(this.W, 0, this.W.Length);
		}

		internal void ProcessWord(byte[] input, int inOff)
		{
			this.W[this.wOff] = Pack.BE_To_UInt64(input, inOff);
			if (++this.wOff == 16)
			{
				this.ProcessBlock();
			}
		}

		private void AdjustByteCounts()
		{
			if (this.byteCount1 > 2305843009213693951L)
			{
				this.byteCount2 += (long)((ulong)this.byteCount1 >> 61);
				this.byteCount1 &= 2305843009213693951L;
			}
		}

		internal void ProcessLength(long lowW, long hiW)
		{
			if (this.wOff > 14)
			{
				this.ProcessBlock();
			}
			this.W[14] = (ulong)hiW;
			this.W[15] = (ulong)lowW;
		}

		internal void ProcessBlock()
		{
			this.AdjustByteCounts();
			for (int i = 16; i <= 79; i++)
			{
				this.W[i] = LongDigest.Sigma1(this.W[i - 2]) + this.W[i - 7] + LongDigest.Sigma0(this.W[i - 15]) + this.W[i - 16];
			}
			ulong num = this.H1;
			ulong num2 = this.H2;
			ulong num3 = this.H3;
			ulong num4 = this.H4;
			ulong num5 = this.H5;
			ulong num6 = this.H6;
			ulong num7 = this.H7;
			ulong num8 = this.H8;
			int num9 = 0;
			for (int j = 0; j < 10; j++)
			{
				num8 += LongDigest.Sum1(num5) + LongDigest.Ch(num5, num6, num7) + LongDigest.K[num9] + this.W[num9++];
				num4 += num8;
				num8 += LongDigest.Sum0(num) + LongDigest.Maj(num, num2, num3);
				num7 += LongDigest.Sum1(num4) + LongDigest.Ch(num4, num5, num6) + LongDigest.K[num9] + this.W[num9++];
				num3 += num7;
				num7 += LongDigest.Sum0(num8) + LongDigest.Maj(num8, num, num2);
				num6 += LongDigest.Sum1(num3) + LongDigest.Ch(num3, num4, num5) + LongDigest.K[num9] + this.W[num9++];
				num2 += num6;
				num6 += LongDigest.Sum0(num7) + LongDigest.Maj(num7, num8, num);
				num5 += LongDigest.Sum1(num2) + LongDigest.Ch(num2, num3, num4) + LongDigest.K[num9] + this.W[num9++];
				num += num5;
				num5 += LongDigest.Sum0(num6) + LongDigest.Maj(num6, num7, num8);
				num4 += LongDigest.Sum1(num) + LongDigest.Ch(num, num2, num3) + LongDigest.K[num9] + this.W[num9++];
				num8 += num4;
				num4 += LongDigest.Sum0(num5) + LongDigest.Maj(num5, num6, num7);
				num3 += LongDigest.Sum1(num8) + LongDigest.Ch(num8, num, num2) + LongDigest.K[num9] + this.W[num9++];
				num7 += num3;
				num3 += LongDigest.Sum0(num4) + LongDigest.Maj(num4, num5, num6);
				num2 += LongDigest.Sum1(num7) + LongDigest.Ch(num7, num8, num) + LongDigest.K[num9] + this.W[num9++];
				num6 += num2;
				num2 += LongDigest.Sum0(num3) + LongDigest.Maj(num3, num4, num5);
				num += LongDigest.Sum1(num6) + LongDigest.Ch(num6, num7, num8) + LongDigest.K[num9] + this.W[num9++];
				num5 += num;
				num += LongDigest.Sum0(num2) + LongDigest.Maj(num2, num3, num4);
			}
			this.H1 += num;
			this.H2 += num2;
			this.H3 += num3;
			this.H4 += num4;
			this.H5 += num5;
			this.H6 += num6;
			this.H7 += num7;
			this.H8 += num8;
			this.wOff = 0;
			Array.Clear(this.W, 0, 16);
		}

		private static ulong Ch(ulong x, ulong y, ulong z)
		{
			return (x & y) ^ (~x & z);
		}

		private static ulong Maj(ulong x, ulong y, ulong z)
		{
			return (x & y) ^ (x & z) ^ (y & z);
		}

		private static ulong Sum0(ulong x)
		{
			return (x << 36 | x >> 28) ^ (x << 30 | x >> 34) ^ (x << 25 | x >> 39);
		}

		private static ulong Sum1(ulong x)
		{
			return (x << 50 | x >> 14) ^ (x << 46 | x >> 18) ^ (x << 23 | x >> 41);
		}

		private static ulong Sigma0(ulong x)
		{
			return (x << 63 | x >> 1) ^ (x << 56 | x >> 8) ^ x >> 7;
		}

		private static ulong Sigma1(ulong x)
		{
			return (x << 45 | x >> 19) ^ (x << 3 | x >> 61) ^ x >> 6;
		}

		public int GetByteLength()
		{
			return this.MyByteLength;
		}

		public abstract int GetDigestSize();

		public abstract int DoFinal(byte[] output, int outOff);

		public abstract IMemoable Copy();

		public abstract void Reset(IMemoable t);
	}
}
