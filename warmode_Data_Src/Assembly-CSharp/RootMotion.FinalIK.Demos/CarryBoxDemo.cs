using System;
using UnityEngine;

namespace RootMotion.FinalIK.Demos
{
	public class CarryBoxDemo : MonoBehaviour
	{
		public FullBodyBipedIK ik;

		public Transform leftHandTarget;

		public Transform rightHandTarget;

		private void LateUpdate()
		{
			this.ik.solver.leftHandEffector.position = this.leftHandTarget.position;
			this.ik.solver.leftHandEffector.rotation = this.leftHandTarget.rotation;
			this.ik.solver.rightHandEffector.position = this.rightHandTarget.position;
			this.ik.solver.rightHandEffector.rotation = this.rightHandTarget.rotation;
		}
	}
}
