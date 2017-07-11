using System;

namespace UnityEngine.Networking.NetworkSystem
{
	internal class ObjectSpawnFinishedMessage : MessageBase
	{
		public uint state;

		public override void Deserialize(NetworkReader reader)
		{
			this.state = reader.ReadPackedUInt32();
		}

		public override void Serialize(NetworkWriter writer)
		{
			writer.WritePackedUInt32(this.state);
		}
	}
}
