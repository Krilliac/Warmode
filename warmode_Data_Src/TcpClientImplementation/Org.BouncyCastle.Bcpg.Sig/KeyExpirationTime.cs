using System;

namespace Org.BouncyCastle.Bcpg.Sig
{
	public class KeyExpirationTime : SignatureSubpacket
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

		public KeyExpirationTime(bool critical, byte[] data) : base(SignatureSubpacketTag.KeyExpireTime, critical, data)
		{
		}

		public KeyExpirationTime(bool critical, long seconds) : base(SignatureSubpacketTag.KeyExpireTime, critical, KeyExpirationTime.TimeToBytes(seconds))
		{
		}
	}
}
