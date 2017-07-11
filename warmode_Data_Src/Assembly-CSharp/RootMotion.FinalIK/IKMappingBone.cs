using System;
using UnityEngine;

namespace RootMotion.FinalIK
{
	[Serializable]
	public class IKMappingBone : IKMapping
	{
		public Transform bone;

		[Range(0f, 1f)]
		public float maintainRotationWeight = 1f;

		private IKMapping.BoneMap boneMap = new IKMapping.BoneMap();

		public IKMappingBone()
		{
		}

		public IKMappingBone(Transform bone)
		{
			this.bone = bone;
		}

		public override bool IsValid(IKSolver solver, Warning.Logger logger = null)
		{
			if (!base.IsValid(solver, logger))
			{
				return false;
			}
			if (this.bone == null)
			{
				if (logger != null)
				{
					logger("IKMappingBone's bone is null.");
				}
				return false;
			}
			return true;
		}

		public void StoreDefaultLocalState()
		{
			this.boneMap.StoreDefaultLocalState();
		}

		public void FixTransforms()
		{
			this.boneMap.FixTransform(false);
		}

		protected override void OnInitiate()
		{
			if (this.boneMap == null)
			{
				this.boneMap = new IKMapping.BoneMap();
			}
			this.boneMap.Initiate(this.bone, this.solver);
		}

		public void ReadPose()
		{
			this.boneMap.MaintainRotation();
		}

		public void WritePose()
		{
			this.boneMap.RotateToMaintain(this.solver.GetIKPositionWeight() * this.maintainRotationWeight);
		}
	}
}
