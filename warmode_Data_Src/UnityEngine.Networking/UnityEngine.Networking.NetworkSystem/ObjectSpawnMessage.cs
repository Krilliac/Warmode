using System;

namespace UnityEngine.Networking.NetworkSystem
{
	internal class ObjectSpawnMessage : MessageBase
	{
		public NetworkInstanceId netId;

		public NetworkHash128 assetId;

		public Vector3 position;

		public byte[] payload;

		public override void Deserialize(NetworkReader reader)
		{
			this.netId = reader.ReadNetworkId();
			this.assetId = reader.ReadNetworkHash128();
			this.position = reader.ReadVector3();
			this.payload = reader.ReadBytesAndSize();
		}

		public override void Serialize(NetworkWriter writer)
		{
			writer.Write(this.netId);
			writer.Write(this.assetId);
			writer.Write(this.position);
			writer.WriteBytesFull(this.payload);
		}
	}
}
