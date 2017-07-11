using System;
using UnityEngine;

namespace RootMotion
{
	public class TriggerEventBroadcaster : MonoBehaviour
	{
		public GameObject target;

		private void OnTriggerEnter(Collider collider)
		{
			if (this.target != null)
			{
				this.target.SendMessage("OnTriggerEnter", collider, SendMessageOptions.DontRequireReceiver);
			}
		}

		private void OnTriggerStay(Collider collider)
		{
			if (this.target != null)
			{
				this.target.SendMessage("OnTriggerStay", collider, SendMessageOptions.DontRequireReceiver);
			}
		}

		private void OnTriggerExit(Collider collider)
		{
			if (this.target != null)
			{
				this.target.SendMessage("OnTriggerExit", collider, SendMessageOptions.DontRequireReceiver);
			}
		}
	}
}
