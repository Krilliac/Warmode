using System;
using System.Collections.Generic;

namespace UnityEngine.Networking
{
	[AddComponentMenu("Network/NetworkProximityChecker"), RequireComponent(typeof(NetworkIdentity))]
	public class NetworkProximityChecker : NetworkBehaviour
	{
		public enum CheckMethod
		{
			Physics3D,
			Physics2D
		}

		public int visRange = 10;

		public float visUpdateInterval = 1f;

		public NetworkProximityChecker.CheckMethod checkMethod;

		public bool forceHidden;

		private float m_VisUpdateTime;

		private void Update()
		{
			if (!NetworkServer.active)
			{
				return;
			}
			if (Time.time - this.m_VisUpdateTime > this.visUpdateInterval)
			{
				base.GetComponent<NetworkIdentity>().RebuildObservers(false);
				this.m_VisUpdateTime = Time.time;
			}
		}

		public override bool OnCheckObserver(NetworkConnection newObserver)
		{
			if (this.forceHidden)
			{
				return false;
			}
			GameObject gameObject = null;
			foreach (PlayerController current in newObserver.playerControllers)
			{
				if (current != null && current.gameObject != null)
				{
					gameObject = current.gameObject;
					break;
				}
			}
			if (gameObject == null)
			{
				return false;
			}
			Vector3 position = gameObject.transform.position;
			return (position - base.transform.position).magnitude < (float)this.visRange;
		}

		public override bool OnRebuildObservers(HashSet<NetworkConnection> observers, bool initial)
		{
			if (this.forceHidden)
			{
				NetworkIdentity component = base.GetComponent<NetworkIdentity>();
				if (component.connectionToClient != null)
				{
					observers.Add(component.connectionToClient);
				}
				return true;
			}
			NetworkProximityChecker.CheckMethod checkMethod = this.checkMethod;
			if (checkMethod == NetworkProximityChecker.CheckMethod.Physics3D)
			{
				Collider[] array = Physics.OverlapSphere(base.transform.position, (float)this.visRange);
				Collider[] array2 = array;
				for (int i = 0; i < array2.Length; i++)
				{
					Collider collider = array2[i];
					NetworkIdentity component2 = collider.GetComponent<NetworkIdentity>();
					if (component2 != null && component2.connectionToClient != null)
					{
						observers.Add(component2.connectionToClient);
					}
				}
				return true;
			}
			if (checkMethod != NetworkProximityChecker.CheckMethod.Physics2D)
			{
				return false;
			}
			Collider2D[] array3 = Physics2D.OverlapCircleAll(base.transform.position, (float)this.visRange);
			Collider2D[] array4 = array3;
			for (int j = 0; j < array4.Length; j++)
			{
				Collider2D collider2D = array4[j];
				NetworkIdentity component3 = collider2D.GetComponent<NetworkIdentity>();
				if (component3 != null && component3.connectionToClient != null)
				{
					observers.Add(component3.connectionToClient);
				}
			}
			return true;
		}

		public override void OnSetLocalVisibility(bool vis)
		{
			NetworkProximityChecker.SetVis(base.gameObject, vis);
		}

		private static void SetVis(GameObject go, bool vis)
		{
			Renderer[] components = go.GetComponents<Renderer>();
			for (int i = 0; i < components.Length; i++)
			{
				Renderer renderer = components[i];
				renderer.enabled = vis;
			}
			for (int j = 0; j < go.transform.childCount; j++)
			{
				Transform child = go.transform.GetChild(j);
				NetworkProximityChecker.SetVis(child.gameObject, vis);
			}
		}
	}
}
