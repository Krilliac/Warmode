using System;
using UnityEngine;

namespace RootMotion.FinalIK.Demos
{
	public class CharacterControllerSimpleAim : MonoBehaviour
	{
		public SimpleAimingSystem aimingSystem;

		public Transform target;

		private void LateUpdate()
		{
			this.aimingSystem.targetPosition = this.target.position;
		}
	}
}
