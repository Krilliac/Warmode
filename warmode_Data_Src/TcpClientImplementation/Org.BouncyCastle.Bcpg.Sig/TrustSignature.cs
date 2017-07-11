using System;

namespace Org.BouncyCastle.Bcpg.Sig
{
	public class TrustSignature : SignatureSubpacket
	{
		public int Depth
		{
			get
			{
				return (int)(this.data[0] & 255);
			}
		}

		public int TrustAmount
		{
			get
			{
				return (int)(this.data[1] & 255);
			}
		}

		private static byte[] IntToByteArray(int v1, int v2)
		{
			return new byte[]
			{
				(byte)v1,
				(byte)v2
			};
		}

		public TrustSignature(bool critical, byte[] data) : base(SignatureSubpacketTag.TrustSig, critical, data)
		{
		}

		public TrustSignature(bool critical, int depth, int trustAmount) : base(SignatureSubpacketTag.TrustSig, critical, TrustSignature.IntToByteArray(depth, trustAmount))
		{
		}
	}
}
