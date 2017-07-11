using System;
using UnityEngine;

namespace RootMotion.FinalIK.Demos
{
	[RequireComponent(typeof(Animator))]
	public class AnimatorController3rdPerson : MonoBehaviour
	{
		public float rotateSpeed = 7f;

		public float blendSpeed = 10f;

		public float maxAngle = 90f;

		public float moveSpeed = 1.5f;

		public float rootMotionWeight;

		protected Animator animator;

		protected Vector3 moveBlend;

		protected Vector3 moveInput;

		protected Vector3 velocity;

		protected virtual void Start()
		{
			this.animator = base.GetComponent<Animator>();
		}

		private void OnAnimatorMove()
		{
			this.velocity = Vector3.Lerp(this.velocity, base.transform.rotation * Vector3.ClampMagnitude(this.moveInput, 1f) * this.moveSpeed, Time.deltaTime * this.blendSpeed);
			base.transform.position += Vector3.Lerp(this.velocity * Time.deltaTime, this.animator.deltaPosition, this.rootMotionWeight);
		}

		public virtual void Move(Vector3 moveInput, bool isMoving, Vector3 faceDirection, Vector3 aimTarget)
		{
			this.moveInput = moveInput;
			Vector3 vector = base.transform.InverseTransformDirection(faceDirection);
			float num = Mathf.Atan2(vector.x, vector.z) * 57.29578f;
			float num2 = num * Time.deltaTime * this.rotateSpeed;
			if (num > this.maxAngle)
			{
				num2 = Mathf.Clamp(num2, num - this.maxAngle, num2);
			}
			if (num < -this.maxAngle)
			{
				num2 = Mathf.Clamp(num2, num2, num + this.maxAngle);
			}
			base.transform.Rotate(Vector3.up, num2);
			this.moveBlend = Vector3.Lerp(this.moveBlend, moveInput, Time.deltaTime * this.blendSpeed);
			this.animator.SetFloat("X", this.moveBlend.x);
			this.animator.SetFloat("Z", this.moveBlend.z);
			this.animator.SetBool("IsMoving", isMoving);
		}
	}
}
