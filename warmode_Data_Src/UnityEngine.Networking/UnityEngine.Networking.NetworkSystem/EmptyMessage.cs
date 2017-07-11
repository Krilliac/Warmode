using System;

namespace UnityEngine.Networking.NetworkSystem
{
	public class EmptyMessage : MessageBase
	{
		public override void Deserialize(NetworkReader reader)
		{
		}

		public override void Serialize(NetworkWriter writer)
		{
		}
	}
}
