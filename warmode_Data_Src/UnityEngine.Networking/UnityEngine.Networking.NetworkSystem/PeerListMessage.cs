using System;

namespace UnityEngine.Networking.NetworkSystem
{
	public class PeerListMessage : MessageBase
	{
		public PeerInfoMessage[] peers;

		public override void Deserialize(NetworkReader reader)
		{
			int num = (int)reader.ReadUInt16();
			this.peers = new PeerInfoMessage[num];
			for (int i = 0; i < this.peers.Length; i++)
			{
				PeerInfoMessage peerInfoMessage = new PeerInfoMessage();
				peerInfoMessage.Deserialize(reader);
				this.peers[i] = peerInfoMessage;
			}
		}

		public override void Serialize(NetworkWriter writer)
		{
			writer.Write((ushort)this.peers.Length);
			PeerInfoMessage[] array = this.peers;
			for (int i = 0; i < array.Length; i++)
			{
				PeerInfoMessage peerInfoMessage = array[i];
				peerInfoMessage.Serialize(writer);
			}
		}
	}
}
