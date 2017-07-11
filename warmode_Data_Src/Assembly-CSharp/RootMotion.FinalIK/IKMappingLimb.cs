using System;
using UnityEngine;

namespace RootMotion.FinalIK
{
	[Serializable]
	public class IKMappingLimb : IKMapping
	{
		[Serializable]
		public enum BoneMapType
		{
			Parent,
			Bone1,
			Bone2,
			Bone3
		}

		public Transform parentBone;

		public Transform bone1;

		public Transform bone2;

		public Transform bone3;

		[Range(0f, 1f)]
		public float maintainRotationWeight;

		[Range(0f, 1f)]
		public float weight = 1f;

		private IKMapping.BoneMap boneMapParent = new IKMapping.BoneMap();

		private IKMapping.BoneMap boneMap1 = new IKMapping.BoneMap();

		private IKMapping.BoneMap boneMap2 = new IKMapping.BoneMap();

		private IKMapping.BoneMap boneMap3 = new IKMapping.BoneMap();

		public IKMappingLimb()
		{
		}

		public IKMappingLimb(Transform bone1, Transform bone2, Transform bone3, Transform parentBone = null)
		{
			this.SetBones(bone1, bone2, bone3, parentBone);
		}

		public override bool IsValid(IKSolver solver, Warning.Logger logger = null)
		{
			return base.IsValid(solver, logger) && base.BoneIsValid(this.bone1, solver, logger) && base.BoneIsValid(this.bone2, solver, logger) && base.BoneIsValid(this.bone3, solver, logger);
		}

		public IKMapping.BoneMap GetBoneMap(IKMappingLimb.BoneMapType boneMap)
		{
			switch (boneMap)
			{
			case IKMappingLimb.BoneMapType.Parent:
				if (this.parentBone == null)
				{
					Warning.Log("This limb does not have a parent (shoulder) bone", this.bone1, false);
				}
				return this.boneMapParent;
			case IKMappingLimb.BoneMapType.Bone1:
				return this.boneMap1;
			case IKMappingLimb.BoneMapType.Bone2:
				return this.boneMap2;
			default:
				return this.boneMap3;
			}
		}

		public void SetLimbOrientation(Vector3 upper, Vector3 lower)
		{
			this.boneMap1.defaultLocalTargetRotation = Quaternion.Inverse(Quaternion.Inverse(this.bone1.rotation) * Quaternion.LookRotation(this.bone2.position - this.bone1.position, this.bone1.rotation * -upper));
			this.boneMap2.defaultLocalTargetRotation = Quaternion.Inverse(Quaternion.Inverse(this.bone2.rotation) * Quaternion.LookRotation(this.bone3.position - this.bone2.position, this.bone2.rotation * -lower));
		}

		public void SetBones(Transform bone1, Transform bone2, Transform bone3, Transform parentBone = null)
		{
			this.bone1 = bone1;
			this.bone2 = bone2;
			this.bone3 = bone3;
			this.parentBone = parentBone;
		}

		public void StoreDefaultLocalState()
		{
			if (this.parentBone != null)
			{
				this.boneMapParent.StoreDefaultLocalState();
			}
			this.boneMap1.StoreDefaultLocalState();
			this.boneMap2.StoreDefaultLocalState();
			this.boneMap3.StoreDefaultLocalState();
		}

		public void FixTransforms()
		{
			if (this.parentBone != null)
			{
				this.boneMapParent.FixTransform(false);
			}
			this.boneMap1.FixTransform(true);
			this.boneMap2.FixTransform(false);
			this.boneMap3.FixTransform(false);
		}

		protected override void OnInitiate()
		{
			if (this.boneMapParent == null)
			{
				this.boneMapParent = new IKMapping.BoneMap();
			}
			if (this.boneMap1 == null)
			{
				this.boneMap1 = new IKMapping.BoneMap();
			}
			if (this.boneMap2 == null)
			{
				this.boneMap2 = new IKMapping.BoneMap();
			}
			if (this.boneMap3 == null)
			{
				this.boneMap3 = new IKMapping.BoneMap();
			}
			if (this.parentBone != null)
			{
				this.boneMapParent.Initiate(this.parentBone, this.solver);
			}
			this.boneMap1.Initiate(this.bone1, this.solver);
			this.boneMap2.Initiate(this.bone2, this.solver);
			this.boneMap3.Initiate(this.bone3, this.solver);
			this.boneMap1.SetPlane(this.boneMap1.node, this.boneMap2.node, this.boneMap3.node);
			this.boneMap2.SetPlane(this.boneMap2.node, this.boneMap3.node, this.boneMap1.node);
			if (this.parentBone != null)
			{
				this.boneMapParent.SetLocalSwingAxis(this.boneMap1);
			}
		}

		public void ReadPose()
		{
			this.boneMap1.UpdatePlane(true, true);
			this.boneMap2.UpdatePlane(true, false);
			this.weight = Mathf.Clamp(this.weight, 0f, 1f);
			this.boneMap3.MaintainRotation();
		}

		public void WritePose(bool fullBody)
		{
			if (this.weight <= 0f)
			{
				return;
			}
			if (fullBody)
			{
				if (this.parentBone != null)
				{
					this.boneMapParent.Swing(this.boneMap1.node.solverPosition, this.weight);
				}
				this.boneMap1.FixToNode(this.weight, null);
			}
			this.boneMap1.RotateToPlane(this.weight);
			this.boneMap2.RotateToPlane(this.weight);
			this.boneMap3.RotateToMaintain(this.maintainRotationWeight * this.weight * this.solver.GetIKPositionWeight());
			this.boneMap3.RotateToEffector(this.weight);
		}
	}
}
