using System;
using UnityEngine;

namespace RootMotion.FinalIK.Demos
{
	public class MotionAbsorbCharacter : MonoBehaviour
	{
		public Animator animator;

		public MotionAbsorb motionAbsorb;

		public Transform cube;

		public float cubeRandomPosition = 0.1f;

		public AnimationCurve motionAbsorbWeight;

		private Vector3 cubeDefaultPosition;

		private AnimatorStateInfo info;

		private void Start()
		{
			this.cubeDefaultPosition = this.cube.position;
		}

		private void Update()
		{
			this.info = this.animator.GetCurrentAnimatorStateInfo(0);
			this.motionAbsorb.weight = this.motionAbsorbWeight.Evaluate(this.info.normalizedTime - (float)((int)this.info.normalizedTime));
		}

		private void SwingStart()
		{
			this.cube.GetComponent<Rigidbody>().MovePosition(this.cubeDefaultPosition + UnityEngine.Random.insideUnitSphere * this.cubeRandomPosition);
			this.cube.GetComponent<Rigidbody>().MoveRotation(Quaternion.identity);
			this.cube.GetComponent<Rigidbody>().velocity = Vector3.zero;
			this.cube.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
		}
	}
}
