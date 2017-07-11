using System;

namespace UnityEngine.Networking.NetworkSystem
{
	internal class CRCMessage : MessageBase
	{
		public CRCMessageEntry[] scripts;

		public override void Deserialize(NetworkReader reader)
		{
			int num = (int)reader.ReadUInt16();
			this.scripts = new CRCMessageEntry[num];
			for (int i = 0; i < this.scripts.Length; i++)
			{
				CRCMessageEntry cRCMessageEntry = default(CRCMessageEntry);
				cRCMessageEntry.name = reader.ReadString();
				cRCMessageEntry.channel = reader.ReadByte();
				this.scripts[i] = cRCMessageEntry;
			}
		}

		public override void Serialize(NetworkWriter writer)
		{
			writer.Write((ushort)this.scripts.Length);
			CRCMessageEntry[] array = this.scripts;
			for (int i = 0; i < array.Length; i++)
			{
				CRCMessageEntry cRCMessageEntry = array[i];
				writer.Write(cRCMessageEntry.name);
				writer.Write(cRCMessageEntry.channel);
			}
		}
	}
}
