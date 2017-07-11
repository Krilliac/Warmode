using System;

namespace UnityEngine.Networking.NetworkSystem
{
	internal class ObjectDestroyMessage : MessageBase
	{
		public NetworkInstanceId netId;

		public override void Deserialize(NetworkReader reader)
		{
			this.netId = reader.ReadNetworkId();
		}

		public override void Serialize(NetworkWriter writer)
		{
			writer.Write(this.netId);
		}
	}
}
