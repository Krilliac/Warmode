using System;
using UnityEngine;

namespace RootMotion.FinalIK.Demos
{
	public class SimpleAimingSystem : MonoBehaviour
	{
		public AimPoser aimPoser;

		public AimIK aim;

		public LookAtIK lookAt;

		public Transform recursiveMixingTransform;

		[HideInInspector]
		public Vector3 targetPosition;

		private AimPoser.Pose aimPose;

		private AimPoser.Pose lastPose;

		private void Start()
		{
			AimPoser.Pose[] poses = this.aimPoser.poses;
			for (int i = 0; i < poses.Length; i++)
			{
				AimPoser.Pose pose = poses[i];
				base.GetComponent<Animation>()[pose.name].AddMixingTransform(this.recursiveMixingTransform, true);
			}
			this.aim.Disable();
			this.lookAt.Disable();
		}

		private void LateUpdate()
		{
			this.Pose();
			this.aim.solver.SetIKPosition(this.targetPosition);
			this.lookAt.solver.SetIKPosition(this.targetPosition);
			this.aim.solver.Update();
			this.lookAt.solver.Update();
		}

		private void Pose()
		{
			Vector3 direction = this.targetPosition - this.aim.solver.bones[0].transform.position;
			Vector3 localDirection = base.transform.InverseTransformDirection(direction);
			this.aimPose = this.aimPoser.GetPose(localDirection);
			if (this.aimPose != this.lastPose)
			{
				base.GetComponent<Animation>().CrossFade(this.aimPose.name);
				this.aimPoser.SetPoseActive(this.aimPose);
				this.lastPose = this.aimPose;
			}
		}
	}
}
