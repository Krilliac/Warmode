using System;

namespace UnityEngine.Networking
{
	public class PlayerController
	{
		internal const short kMaxLocalPlayers = 8;

		public const int MaxPlayersPerClient = 32;

		public short playerControllerId = -1;

		public NetworkIdentity unetView;

		public GameObject gameObject;

		public bool IsValid
		{
			get
			{
				return this.playerControllerId != -1;
			}
		}

		public PlayerController()
		{
		}

		internal PlayerController(GameObject go, short playerControllerId)
		{
			this.gameObject = go;
			this.unetView = go.GetComponent<NetworkIdentity>();
			this.playerControllerId = playerControllerId;
		}

		public override string ToString()
		{
			return string.Format("ID={0} NetworkIdentity NetID={1} Player={2}", new object[]
			{
				this.playerControllerId,
				(!(this.unetView != null)) ? "null" : this.unetView.netId.ToString(),
				(!(this.gameObject != null)) ? "null" : this.gameObject.name
			});
		}
	}
}
