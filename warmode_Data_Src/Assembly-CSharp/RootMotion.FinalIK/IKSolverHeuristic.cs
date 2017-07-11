using System;
using UnityEngine;

namespace RootMotion.FinalIK
{
	[Serializable]
	public class IKSolverHeuristic : IKSolver
	{
		public Transform target;

		public float tolerance;

		public int maxIterations = 4;

		public bool useRotationLimits = true;

		public IKSolver.Bone[] bones = new IKSolver.Bone[0];

		protected Vector3 lastLocalDirection;

		protected float chainLength;

		protected virtual int minBones
		{
			get
			{
				return 2;
			}
		}

		protected virtual bool boneLengthCanBeZero
		{
			get
			{
				return true;
			}
		}

		protected virtual bool allowCommonParent
		{
			get
			{
				return false;
			}
		}

		protected virtual Vector3 localDirection
		{
			get
			{
				return this.bones[0].transform.InverseTransformDirection(this.bones[this.bones.Length - 1].transform.position - this.bones[0].transform.position);
			}
		}

		protected float positionOffset
		{
			get
			{
				return Vector3.SqrMagnitude(this.localDirection - this.lastLocalDirection);
			}
		}

		public bool SetChain(Transform[] hierarchy, Transform root)
		{
			if (this.bones == null || this.bones.Length != hierarchy.Length)
			{
				this.bones = new IKSolver.Bone[hierarchy.Length];
			}
			for (int i = 0; i < hierarchy.Length; i++)
			{
				if (this.bones[i] == null)
				{
					this.bones[i] = new IKSolver.Bone();
				}
				this.bones[i].transform = hierarchy[i];
			}
			base.Initiate(root);
			return base.initiated;
		}

		public override void StoreDefaultLocalState()
		{
			for (int i = 0; i < this.bones.Length; i++)
			{
				this.bones[i].StoreDefaultLocalState();
			}
		}

		public override void FixTransforms()
		{
			for (int i = 0; i < this.bones.Length; i++)
			{
				this.bones[i].FixTransform();
			}
		}

		public override bool IsValid(bool log)
		{
			if (this.bones.Length == 0)
			{
				if (log)
				{
					base.LogWarning("IK chain has no bones. Can not initiate solver.");
				}
				return false;
			}
			if (this.bones.Length < this.minBones)
			{
				if (log)
				{
					base.LogWarning("IK chain has less than " + this.minBones + " bones. Can not initiate solver.");
				}
				return false;
			}
			IKSolver.Bone[] array = this.bones;
			for (int i = 0; i < array.Length; i++)
			{
				IKSolver.Bone bone = array[i];
				if (bone.transform == null)
				{
					if (log)
					{
						base.LogWarning("Bone transform is null in IK chain. Can not initiate solver.");
					}
					return false;
				}
			}
			if (!this.allowCommonParent && !IKSolver.HierarchyIsValid(this.bones))
			{
				if (log)
				{
					base.LogWarning("IK requires for it's bones to be parented to each other. Invalid bone hierarchy detected.");
				}
				return false;
			}
			Transform transform = IKSolver.ContainsDuplicateBone(this.bones);
			if (transform != null)
			{
				if (log)
				{
					base.LogWarning(transform.name + " is represented multiple times in a single IK chain. Can nott initiate solver.");
				}
				return false;
			}
			if (!this.boneLengthCanBeZero)
			{
				for (int j = 0; j < this.bones.Length - 1; j++)
				{
					float magnitude = (this.bones[j].transform.position - this.bones[j + 1].transform.position).magnitude;
					if (magnitude == 0f)
					{
						if (log)
						{
							base.LogWarning("Bone " + j + " length is zero. Can nott initiate solver.");
						}
						return false;
					}
				}
			}
			return true;
		}

		public override IKSolver.Point[] GetPoints()
		{
			return this.bones;
		}

		public override IKSolver.Point GetPoint(Transform transform)
		{
			for (int i = 0; i < this.bones.Length; i++)
			{
				if (this.bones[i].transform == transform)
				{
					return this.bones[i];
				}
			}
			return null;
		}

		protected override void OnInitiate()
		{
		}

		protected override void OnUpdate()
		{
		}

		protected void InitiateBones()
		{
			this.chainLength = 0f;
			for (int i = 0; i < this.bones.Length; i++)
			{
				if (i < this.bones.Length - 1)
				{
					this.bones[i].length = (this.bones[i].transform.position - this.bones[i + 1].transform.position).magnitude;
					this.chainLength += this.bones[i].length;
					Vector3 position = this.bones[i + 1].transform.position;
					this.bones[i].axis = Quaternion.Inverse(this.bones[i].transform.rotation) * (position - this.bones[i].transform.position);
					if (this.bones[i].rotationLimit != null)
					{
						this.bones[i].rotationLimit.Disable();
					}
				}
				else
				{
					this.bones[i].axis = Quaternion.Inverse(this.bones[i].transform.rotation) * (this.bones[this.bones.Length - 1].transform.position - this.bones[0].transform.position);
				}
			}
		}

		protected Vector3 GetSingularityOffset()
		{
			if (!this.SingularityDetected())
			{
				return Vector3.zero;
			}
			Vector3 normalized = (this.IKPosition - this.bones[0].transform.position).normalized;
			Vector3 rhs = new Vector3(normalized.y, normalized.z, normalized.x);
			if (this.useRotationLimits && this.bones[this.bones.Length - 2].rotationLimit != null && this.bones[this.bones.Length - 2].rotationLimit is RotationLimitHinge)
			{
				rhs = this.bones[this.bones.Length - 2].transform.rotation * this.bones[this.bones.Length - 2].rotationLimit.axis;
			}
			return Vector3.Cross(normalized, rhs) * this.bones[this.bones.Length - 2].length * 0.5f;
		}

		private bool SingularityDetected()
		{
			if (!base.initiated)
			{
				return false;
			}
			Vector3 a = this.bones[this.bones.Length - 1].transform.position - this.bones[0].transform.position;
			Vector3 a2 = this.IKPosition - this.bones[0].transform.position;
			float magnitude = a.magnitude;
			float magnitude2 = a2.magnitude;
			if (magnitude < magnitude2)
			{
				return false;
			}
			if (magnitude < this.chainLength - this.bones[this.bones.Length - 2].length * 0.1f)
			{
				return false;
			}
			if (magnitude == 0f)
			{
				return false;
			}
			if (magnitude2 == 0f)
			{
				return false;
			}
			if (magnitude2 > magnitude)
			{
				return false;
			}
			float num = Vector3.Dot(a / magnitude, a2 / magnitude2);
			return num >= 0.999f;
		}
	}
}
