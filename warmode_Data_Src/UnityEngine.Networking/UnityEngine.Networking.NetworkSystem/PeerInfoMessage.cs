using System;

namespace UnityEngine.Networking.NetworkSystem
{
	public class PeerInfoMessage : MessageBase
	{
		public int connectionId;

		public string address;

		public int port;

		public bool isHost;

		public bool isYou;

		public override void Deserialize(NetworkReader reader)
		{
			this.connectionId = (int)reader.ReadPackedUInt32();
			this.address = reader.ReadString();
			this.port = (int)reader.ReadPackedUInt32();
			this.isHost = reader.ReadBoolean();
			this.isYou = reader.ReadBoolean();
		}

		public override void Serialize(NetworkWriter writer)
		{
			writer.WritePackedUInt32((uint)this.connectionId);
			writer.Write(this.address);
			writer.WritePackedUInt32((uint)this.port);
			writer.Write(this.isHost);
			writer.Write(this.isYou);
		}
	}
}
