using System;

namespace UnityEngine.Networking
{
	public abstract class MessageBase
	{
		public virtual void Deserialize(NetworkReader reader)
		{
		}

		public virtual void Serialize(NetworkWriter writer)
		{
		}
	}
}
