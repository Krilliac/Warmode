using System;

namespace Org.BouncyCastle.Bcpg
{
	public class ModDetectionCodePacket : ContainedPacket
	{
		private readonly byte[] digest;

		internal ModDetectionCodePacket(BcpgInputStream bcpgIn)
		{
			if (bcpgIn == null)
			{
				throw new ArgumentNullException("bcpgIn");
			}
			this.digest = new byte[20];
			bcpgIn.ReadFully(this.digest);
		}

		public ModDetectionCodePacket(byte[] digest)
		{
			if (digest == null)
			{
				throw new ArgumentNullException("digest");
			}
			this.digest = (byte[])digest.Clone();
		}

		public byte[] GetDigest()
		{
			return (byte[])this.digest.Clone();
		}

		public override void Encode(BcpgOutputStream bcpgOut)
		{
			bcpgOut.WritePacket(PacketTag.ModificationDetectionCode, this.digest, false);
		}
	}
}
