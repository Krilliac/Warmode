using System;
using UnityEngine;

namespace RootMotion.FinalIK.Demos
{
	public class EffectorOffset : OffsetModifier
	{
		[Range(0f, 1f)]
		public float handsMaintainRelativePositionWeight;

		public Vector3 bodyOffset;

		public Vector3 leftShoulderOffset;

		public Vector3 rightShoulderOffset;

		public Vector3 leftThighOffset;

		public Vector3 rightThighOffset;

		public Vector3 leftHandOffset;

		public Vector3 rightHandOffset;

		public Vector3 leftFootOffset;

		public Vector3 rightFootOffset;

		protected override void OnModifyOffset()
		{
			this.ik.solver.leftHandEffector.maintainRelativePositionWeight = this.handsMaintainRelativePositionWeight;
			this.ik.solver.rightHandEffector.maintainRelativePositionWeight = this.handsMaintainRelativePositionWeight;
			this.ik.solver.bodyEffector.positionOffset += base.transform.rotation * this.bodyOffset;
			this.ik.solver.leftShoulderEffector.positionOffset += base.transform.rotation * this.leftShoulderOffset;
			this.ik.solver.rightShoulderEffector.positionOffset += base.transform.rotation * this.rightShoulderOffset;
			this.ik.solver.leftThighEffector.positionOffset += base.transform.rotation * this.leftThighOffset;
			this.ik.solver.rightThighEffector.positionOffset += base.transform.rotation * this.rightThighOffset;
			this.ik.solver.leftHandEffector.positionOffset += base.transform.rotation * this.leftHandOffset;
			this.ik.solver.rightHandEffector.positionOffset += base.transform.rotation * this.rightHandOffset;
			this.ik.solver.leftFootEffector.positionOffset += base.transform.rotation * this.leftFootOffset;
			this.ik.solver.rightFootEffector.positionOffset += base.transform.rotation * this.rightFootOffset;
		}
	}
}
