using Org.BouncyCastle.Utilities.Date;
using System;

namespace Org.BouncyCastle.Bcpg.Sig
{
	public class SignatureCreationTime : SignatureSubpacket
	{
		protected static byte[] TimeToBytes(DateTime time)
		{
			long num = DateTimeUtilities.DateTimeToUnixMs(time) / 1000L;
			return new byte[]
			{
				(byte)(num >> 24),
				(byte)(num >> 16),
				(byte)(num >> 8),
				(byte)num
			};
		}

		public SignatureCreationTime(bool critical, byte[] data) : base(SignatureSubpacketTag.CreationTime, critical, data)
		{
		}

		public SignatureCreationTime(bool critical, DateTime date) : base(SignatureSubpacketTag.CreationTime, critical, SignatureCreationTime.TimeToBytes(date))
		{
		}

		public DateTime GetTime()
		{
			long num = (long)((ulong)((int)this.data[0] << 24 | (int)this.data[1] << 16 | (int)this.data[2] << 8 | (int)this.data[3]));
			return DateTimeUtilities.UnixMsToDateTime(num * 1000L);
		}
	}
}
