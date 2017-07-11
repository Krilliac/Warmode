using System;

namespace Org.BouncyCastle.Bcpg
{
	public class MarkerPacket : ContainedPacket
	{
		private byte[] marker = new byte[]
		{
			80,
			71,
			80
		};

		public MarkerPacket(BcpgInputStream bcpgIn)
		{
			bcpgIn.ReadFully(this.marker);
		}

		public override void Encode(BcpgOutputStream bcpgOut)
		{
			bcpgOut.WritePacket(PacketTag.Marker, this.marker, true);
		}
	}
}
