using System;

namespace Org.BouncyCastle.Bcpg.Sig
{
	public class IssuerKeyId : SignatureSubpacket
	{
		public long KeyId
		{
			get
			{
				return (long)(this.data[0] & 255) << 56 | (long)(this.data[1] & 255) << 48 | (long)(this.data[2] & 255) << 40 | (long)(this.data[3] & 255) << 32 | (long)(this.data[4] & 255) << 24 | (long)(this.data[5] & 255) << 16 | (long)(this.data[6] & 255) << 8 | (long)((ulong)this.data[7] & 255uL);
			}
		}

		protected static byte[] KeyIdToBytes(long keyId)
		{
			return new byte[]
			{
				(byte)(keyId >> 56),
				(byte)(keyId >> 48),
				(byte)(keyId >> 40),
				(byte)(keyId >> 32),
				(byte)(keyId >> 24),
				(byte)(keyId >> 16),
				(byte)(keyId >> 8),
				(byte)keyId
			};
		}

		public IssuerKeyId(bool critical, byte[] data) : base(SignatureSubpacketTag.IssuerKeyId, critical, data)
		{
		}

		public IssuerKeyId(bool critical, long keyId) : base(SignatureSubpacketTag.IssuerKeyId, critical, IssuerKeyId.KeyIdToBytes(keyId))
		{
		}
	}
}
