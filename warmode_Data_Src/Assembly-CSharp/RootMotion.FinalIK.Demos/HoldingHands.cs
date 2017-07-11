using System;
using UnityEngine;

namespace RootMotion.FinalIK.Demos
{
	public class HoldingHands : MonoBehaviour
	{
		public FullBodyBipedIK rightHandChar;

		public FullBodyBipedIK leftHandChar;

		public Transform rightHandTarget;

		public Transform leftHandTarget;

		public float crossFade;

		public float speed = 10f;

		private Quaternion rightHandRotation;

		private Quaternion leftHandRotation;

		private void Start()
		{
			this.rightHandRotation = Quaternion.Inverse(this.rightHandChar.solver.rightHandEffector.bone.rotation) * base.transform.rotation;
			this.leftHandRotation = Quaternion.Inverse(this.leftHandChar.solver.leftHandEffector.bone.rotation) * base.transform.rotation;
		}

		private void LateUpdate()
		{
			Vector3 b = Vector3.Lerp(this.rightHandChar.solver.rightHandEffector.bone.position, this.leftHandChar.solver.leftHandEffector.bone.position, this.crossFade);
			base.transform.position = Vector3.Lerp(base.transform.position, b, Time.deltaTime * this.speed);
			base.transform.rotation = Quaternion.Slerp(this.rightHandChar.solver.rightHandEffector.bone.rotation * this.rightHandRotation, this.leftHandChar.solver.leftHandEffector.bone.rotation * this.leftHandRotation, this.crossFade);
			this.rightHandChar.solver.rightHandEffector.position = this.rightHandTarget.position;
			this.rightHandChar.solver.rightHandEffector.rotation = this.rightHandTarget.rotation;
			this.leftHandChar.solver.leftHandEffector.position = this.leftHandTarget.position;
			this.leftHandChar.solver.leftHandEffector.rotation = this.leftHandTarget.rotation;
		}
	}
}
