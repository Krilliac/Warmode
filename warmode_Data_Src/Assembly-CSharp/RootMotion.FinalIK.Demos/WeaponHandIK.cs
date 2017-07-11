using System;
using UnityEngine;

namespace RootMotion.FinalIK.Demos
{
	public class WeaponHandIK : MonoBehaviour
	{
		public FullBodyBipedIK ik;

		public Transform leftHandTarget;

		private void LateUpdate()
		{
			this.ik.solver.leftHandEffector.position = this.leftHandTarget.position;
			this.ik.solver.leftHandEffector.rotation = this.leftHandTarget.rotation;
		}
	}
}
