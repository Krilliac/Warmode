using System;

namespace Org.BouncyCastle.Crypto.Tls
{
	internal class DtlsReplayWindow
	{
		private const long VALID_SEQ_MASK = 281474976710655L;

		private const long WINDOW_SIZE = 64L;

		private long mLatestConfirmedSeq = -1L;

		private long mBitmap;

		internal bool ShouldDiscard(long seq)
		{
			if ((seq & 281474976710655L) != seq)
			{
				return true;
			}
			if (seq <= this.mLatestConfirmedSeq)
			{
				long num = this.mLatestConfirmedSeq - seq;
				if (num >= 64L)
				{
					return true;
				}
				if ((this.mBitmap & 1L << (int)num) != 0L)
				{
					return true;
				}
			}
			return false;
		}

		internal void ReportAuthenticated(long seq)
		{
			if ((seq & 281474976710655L) != seq)
			{
				throw new ArgumentException("out of range", "seq");
			}
			if (seq <= this.mLatestConfirmedSeq)
			{
				long num = this.mLatestConfirmedSeq - seq;
				if (num < 64L)
				{
					this.mBitmap |= 1L << (int)num;
					return;
				}
			}
			else
			{
				long num2 = seq - this.mLatestConfirmedSeq;
				if (num2 >= 64L)
				{
					this.mBitmap = 1L;
				}
				else
				{
					this.mBitmap <<= (int)num2;
					this.mBitmap |= 1L;
				}
				this.mLatestConfirmedSeq = seq;
			}
		}

		internal void Reset()
		{
			this.mLatestConfirmedSeq = -1L;
			this.mBitmap = 0L;
		}
	}
}
