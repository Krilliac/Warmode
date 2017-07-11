using Org.BouncyCastle.Utilities;
using System;
using System.Collections;
using System.IO;

namespace Org.BouncyCastle.Bcpg
{
	public class UserAttributePacket : ContainedPacket
	{
		private readonly UserAttributeSubpacket[] subpackets;

		public UserAttributePacket(BcpgInputStream bcpgIn)
		{
			UserAttributeSubpacketsParser userAttributeSubpacketsParser = new UserAttributeSubpacketsParser(bcpgIn);
			IList list = Platform.CreateArrayList();
			UserAttributeSubpacket value;
			while ((value = userAttributeSubpacketsParser.ReadPacket()) != null)
			{
				list.Add(value);
			}
			this.subpackets = new UserAttributeSubpacket[list.Count];
			for (int num = 0; num != this.subpackets.Length; num++)
			{
				this.subpackets[num] = (UserAttributeSubpacket)list[num];
			}
		}

		public UserAttributePacket(UserAttributeSubpacket[] subpackets)
		{
			this.subpackets = subpackets;
		}

		public UserAttributeSubpacket[] GetSubpackets()
		{
			return this.subpackets;
		}

		public override void Encode(BcpgOutputStream bcpgOut)
		{
			MemoryStream memoryStream = new MemoryStream();
			for (int num = 0; num != this.subpackets.Length; num++)
			{
				this.subpackets[num].Encode(memoryStream);
			}
			bcpgOut.WritePacket(PacketTag.UserAttribute, memoryStream.ToArray(), false);
		}
	}
}
