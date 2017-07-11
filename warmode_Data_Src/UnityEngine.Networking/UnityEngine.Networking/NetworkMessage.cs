using System;

namespace UnityEngine.Networking
{
	public class NetworkMessage
	{
		public short msgType;

		public NetworkConnection conn;

		public NetworkReader reader;

		public int channelId;

		public static string Dump(byte[] payload, int sz)
		{
			string text = "[";
			for (int i = 0; i < sz; i++)
			{
				text = text + payload[i] + " ";
			}
			return text + "]";
		}

		public TMsg ReadMessage<TMsg>() where TMsg : MessageBase, new()
		{
			TMsg result = Activator.CreateInstance<TMsg>();
			result.Deserialize(this.reader);
			return result;
		}

		public void ReadMessage<TMsg>(TMsg msg) where TMsg : MessageBase
		{
			msg.Deserialize(this.reader);
		}
	}
}
