using System;

namespace UnityEngine.Networking
{
	[AddComponentMenu("Network/NetworkStartPosition"), DisallowMultipleComponent]
	public class NetworkStartPosition : MonoBehaviour
	{
		public void Awake()
		{
			NetworkManager.RegisterStartPosition(base.transform);
		}

		public void OnDestroy()
		{
			NetworkManager.UnRegisterStartPosition(base.transform);
		}
	}
}
