using System;

namespace Org.BouncyCastle.Crypto.Tls
{
	internal class DtlsEpoch
	{
		private readonly DtlsReplayWindow mReplayWindow = new DtlsReplayWindow();

		private readonly int mEpoch;

		private readonly TlsCipher mCipher;

		private long mSequenceNumber;

		internal TlsCipher Cipher
		{
			get
			{
				return this.mCipher;
			}
		}

		internal int Epoch
		{
			get
			{
				return this.mEpoch;
			}
		}

		internal DtlsReplayWindow ReplayWindow
		{
			get
			{
				return this.mReplayWindow;
			}
		}

		internal long SequenceNumber
		{
			get
			{
				return this.mSequenceNumber;
			}
		}

		internal DtlsEpoch(int epoch, TlsCipher cipher)
		{
			if (epoch < 0)
			{
				throw new ArgumentException("must be >= 0", "epoch");
			}
			if (cipher == null)
			{
				throw new ArgumentNullException("cipher");
			}
			this.mEpoch = epoch;
			this.mCipher = cipher;
		}

		internal long AllocateSequenceNumber()
		{
			long result;
			this.mSequenceNumber = (result = this.mSequenceNumber) + 1L;
			return result;
		}
	}
}
