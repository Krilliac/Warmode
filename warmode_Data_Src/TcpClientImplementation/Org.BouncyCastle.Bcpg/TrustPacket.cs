using System;
using System.IO;

namespace Org.BouncyCastle.Bcpg
{
	public class TrustPacket : ContainedPacket
	{
		private readonly byte[] levelAndTrustAmount;

		public TrustPacket(BcpgInputStream bcpgIn)
		{
			MemoryStream memoryStream = new MemoryStream();
			int num;
			while ((num = bcpgIn.ReadByte()) >= 0)
			{
				memoryStream.WriteByte((byte)num);
			}
			this.levelAndTrustAmount = memoryStream.ToArray();
		}

		public TrustPacket(int trustCode)
		{
			this.levelAndTrustAmount = new byte[]
			{
				(byte)trustCode
			};
		}

		public byte[] GetLevelAndTrustAmount()
		{
			return (byte[])this.levelAndTrustAmount.Clone();
		}

		public override void Encode(BcpgOutputStream bcpgOut)
		{
			bcpgOut.WritePacket(PacketTag.Trust, this.levelAndTrustAmount, true);
		}
	}
}
