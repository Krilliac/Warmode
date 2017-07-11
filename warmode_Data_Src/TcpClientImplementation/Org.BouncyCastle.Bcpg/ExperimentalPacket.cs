using System;

namespace Org.BouncyCastle.Bcpg
{
	public class ExperimentalPacket : ContainedPacket
	{
		private readonly PacketTag tag;

		private readonly byte[] contents;

		public PacketTag Tag
		{
			get
			{
				return this.tag;
			}
		}

		internal ExperimentalPacket(PacketTag tag, BcpgInputStream bcpgIn)
		{
			this.tag = tag;
			this.contents = bcpgIn.ReadAll();
		}

		public byte[] GetContents()
		{
			return (byte[])this.contents.Clone();
		}

		public override void Encode(BcpgOutputStream bcpgOut)
		{
			bcpgOut.WritePacket(this.tag, this.contents, true);
		}
	}
}
