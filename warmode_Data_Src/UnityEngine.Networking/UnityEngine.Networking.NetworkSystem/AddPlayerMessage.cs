using System;

namespace UnityEngine.Networking.NetworkSystem
{
	public class AddPlayerMessage : MessageBase
	{
		public short playerControllerId;

		public int msgSize;

		public byte[] msgData;

		public override void Deserialize(NetworkReader reader)
		{
			this.playerControllerId = (short)reader.ReadUInt16();
			this.msgData = reader.ReadBytesAndSize();
			if (this.msgData == null)
			{
				this.msgSize = 0;
			}
			else
			{
				this.msgSize = this.msgData.Length;
			}
		}

		public override void Serialize(NetworkWriter writer)
		{
			writer.Write((ushort)this.playerControllerId);
			writer.WriteBytesAndSize(this.msgData, this.msgSize);
		}
	}
}
