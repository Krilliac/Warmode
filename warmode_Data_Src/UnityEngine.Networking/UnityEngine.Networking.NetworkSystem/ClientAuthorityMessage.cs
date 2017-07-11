using System;

namespace UnityEngine.Networking.NetworkSystem
{
	internal class ClientAuthorityMessage : MessageBase
	{
		public NetworkInstanceId netId;

		public bool authority;

		public override void Deserialize(NetworkReader reader)
		{
			this.netId = reader.ReadNetworkId();
			this.authority = reader.ReadBoolean();
		}

		public override void Serialize(NetworkWriter writer)
		{
			writer.Write(this.netId);
			writer.Write(this.authority);
		}
	}
}
