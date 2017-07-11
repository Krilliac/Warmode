using System;

namespace UnityEngine.Networking.NetworkSystem
{
	internal class AnimationParametersMessage : MessageBase
	{
		public NetworkInstanceId netId;

		public byte[] parameters;

		public override void Deserialize(NetworkReader reader)
		{
			this.netId = reader.ReadNetworkId();
			this.parameters = reader.ReadBytesAndSize();
		}

		public override void Serialize(NetworkWriter writer)
		{
			writer.Write(this.netId);
			if (this.parameters == null)
			{
				writer.WriteBytesAndSize(this.parameters, 0);
			}
			else
			{
				writer.WriteBytesAndSize(this.parameters, this.parameters.Length);
			}
		}
	}
}
