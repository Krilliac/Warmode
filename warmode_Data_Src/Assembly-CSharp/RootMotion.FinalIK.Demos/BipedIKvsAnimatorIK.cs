using System;
using UnityEngine;

namespace RootMotion.FinalIK.Demos
{
	public class BipedIKvsAnimatorIK : MonoBehaviour
	{
		public Animator animator;

		public BipedIK bipedIK;

		public Transform lookAtTargetBiped;

		public Transform lookAtTargetAnimator;

		public float lookAtWeight = 1f;

		public float lookAtBodyWeight = 1f;

		public float lookAtHeadWeight = 1f;

		public float lookAtEyesWeight = 1f;

		public float lookAtClampWeight = 0.5f;

		public float lookAtClampWeightHead = 0.5f;

		public float lookAtClampWeightEyes = 0.5f;

		public Transform footTargetBiped;

		public Transform footTargetAnimator;

		public float footPositionWeight;

		public float footRotationWeight;

		public Transform handTargetBiped;

		public Transform handTargetAnimator;

		public float handPositionWeight;

		public float handRotationWeight;

		private void OnAnimatorIK(int layer)
		{
			this.animator.transform.rotation = this.bipedIK.transform.rotation;
			Vector3 b = this.animator.transform.position - this.bipedIK.transform.position;
			this.lookAtTargetAnimator.position = this.lookAtTargetBiped.position + b;
			this.bipedIK.SetLookAtPosition(this.lookAtTargetBiped.position);
			this.bipedIK.SetLookAtWeight(this.lookAtWeight, this.lookAtBodyWeight, this.lookAtHeadWeight, this.lookAtEyesWeight, this.lookAtClampWeight, this.lookAtClampWeightHead, this.lookAtClampWeightEyes);
			this.animator.SetLookAtPosition(this.lookAtTargetAnimator.position);
			this.animator.SetLookAtWeight(this.lookAtWeight, this.lookAtBodyWeight, this.lookAtHeadWeight, this.lookAtEyesWeight, this.lookAtClampWeight);
			this.footTargetAnimator.position = this.footTargetBiped.position + b;
			this.footTargetAnimator.rotation = this.footTargetBiped.rotation;
			this.bipedIK.SetIKPosition(AvatarIKGoal.LeftFoot, this.footTargetBiped.position);
			this.bipedIK.SetIKRotation(AvatarIKGoal.LeftFoot, this.footTargetBiped.rotation);
			this.bipedIK.SetIKPositionWeight(AvatarIKGoal.LeftFoot, this.footPositionWeight);
			this.bipedIK.SetIKRotationWeight(AvatarIKGoal.LeftFoot, this.footRotationWeight);
			this.animator.SetIKPosition(AvatarIKGoal.LeftFoot, this.footTargetAnimator.position);
			this.animator.SetIKRotation(AvatarIKGoal.LeftFoot, this.footTargetAnimator.rotation);
			this.animator.SetIKPositionWeight(AvatarIKGoal.LeftFoot, this.footPositionWeight);
			this.animator.SetIKRotationWeight(AvatarIKGoal.LeftFoot, this.footRotationWeight);
			this.handTargetAnimator.position = this.handTargetBiped.position + b;
			this.handTargetAnimator.rotation = this.handTargetBiped.rotation;
			this.bipedIK.SetIKPosition(AvatarIKGoal.LeftHand, this.handTargetBiped.position);
			this.bipedIK.SetIKRotation(AvatarIKGoal.LeftHand, this.handTargetBiped.rotation);
			this.bipedIK.SetIKPositionWeight(AvatarIKGoal.LeftHand, this.handPositionWeight);
			this.bipedIK.SetIKRotationWeight(AvatarIKGoal.LeftHand, this.handRotationWeight);
			this.animator.SetIKPosition(AvatarIKGoal.LeftHand, this.handTargetAnimator.position);
			this.animator.SetIKRotation(AvatarIKGoal.LeftHand, this.handTargetAnimator.rotation);
			this.animator.SetIKPositionWeight(AvatarIKGoal.LeftHand, this.handPositionWeight);
			this.animator.SetIKRotationWeight(AvatarIKGoal.LeftHand, this.handRotationWeight);
		}
	}
}
