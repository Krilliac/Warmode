using System;

namespace UnityEngine.Networking.NetworkSystem
{
	internal class AnimationTriggerMessage : MessageBase
	{
		public NetworkInstanceId netId;

		public int hash;

		public override void Deserialize(NetworkReader reader)
		{
			this.netId = reader.ReadNetworkId();
			this.hash = (int)reader.ReadPackedUInt32();
		}

		public override void Serialize(NetworkWriter writer)
		{
			writer.Write(this.netId);
			writer.WritePackedUInt32((uint)this.hash);
		}
	}
}
