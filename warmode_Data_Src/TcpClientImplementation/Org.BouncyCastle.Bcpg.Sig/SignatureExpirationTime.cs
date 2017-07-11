using System;

namespace Org.BouncyCastle.Bcpg.Sig
{
	public class SignatureExpirationTime : SignatureSubpacket
	{
		public long Time
		{
			get
			{
				return (long)(this.data[0] & 255) << 24 | (long)(this.data[1] & 255) << 16 | (long)(this.data[2] & 255) << 8 | (long)((ulong)this.data[3] & 255uL);
			}
		}

		protected static byte[] TimeToBytes(long t)
		{
			return new byte[]
			{
				(byte)(t >> 24),
				(byte)(t >> 16),
				(byte)(t >> 8),
				(byte)t
			};
		}

		public SignatureExpirationTime(bool critical, byte[] data) : base(SignatureSubpacketTag.ExpireTime, critical, data)
		{
		}

		public SignatureExpirationTime(bool critical, long seconds) : base(SignatureSubpacketTag.ExpireTime, critical, SignatureExpirationTime.TimeToBytes(seconds))
		{
		}
	}
}
