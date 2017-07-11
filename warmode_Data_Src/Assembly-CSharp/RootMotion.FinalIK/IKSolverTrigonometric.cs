using System;
using UnityEngine;

namespace RootMotion.FinalIK
{
	[Serializable]
	public class IKSolverTrigonometric : IKSolver
	{
		[Serializable]
		public class TrigonometricBone : IKSolver.Bone
		{
			public float sqrMag;

			private Quaternion targetToLocalSpace;

			private Vector3 defaultLocalBendNormal;

			public void Initiate(Vector3 childPosition, Vector3 bendNormal)
			{
				Quaternion rotation = Quaternion.LookRotation(childPosition - this.transform.position, bendNormal);
				this.targetToLocalSpace = QuaTools.RotationToLocalSpace(this.transform.rotation, rotation);
				this.defaultLocalBendNormal = Quaternion.Inverse(this.transform.rotation) * bendNormal;
			}

			public Quaternion GetRotation(Vector3 direction, Vector3 bendNormal)
			{
				return Quaternion.LookRotation(direction, bendNormal) * this.targetToLocalSpace;
			}

			public Vector3 GetBendNormalFromCurrentRotation()
			{
				return this.transform.rotation * this.defaultLocalBendNormal;
			}
		}

		public Transform target;

		[Range(0f, 1f)]
		public float IKRotationWeight = 1f;

		public Quaternion IKRotation;

		public Vector3 bendNormal = Vector3.right;

		public IKSolverTrigonometric.TrigonometricBone bone1 = new IKSolverTrigonometric.TrigonometricBone();

		public IKSolverTrigonometric.TrigonometricBone bone2 = new IKSolverTrigonometric.TrigonometricBone();

		public IKSolverTrigonometric.TrigonometricBone bone3 = new IKSolverTrigonometric.TrigonometricBone();

		protected Vector3 weightIKPosition;

		protected bool directHierarchy = true;

		public void SetBendGoalPosition(Vector3 goalPosition, float weight)
		{
			if (!base.initiated)
			{
				return;
			}
			if (weight <= 0f)
			{
				return;
			}
			Vector3 vector = Vector3.Cross(goalPosition - this.bone1.transform.position, this.IKPosition - this.bone1.transform.position);
			if (vector != Vector3.zero)
			{
				if (weight >= 1f)
				{
					this.bendNormal = vector;
					return;
				}
				this.bendNormal = Vector3.Lerp(this.bendNormal, vector, weight);
			}
		}

		public void SetBendPlaneToCurrent()
		{
			if (!base.initiated)
			{
				return;
			}
			Vector3 lhs = Vector3.Cross(this.bone2.transform.position - this.bone1.transform.position, this.bone3.transform.position - this.bone2.transform.position);
			if (lhs != Vector3.zero)
			{
				this.bendNormal = lhs;
			}
		}

		public void SetIKRotation(Quaternion rotation)
		{
			this.IKRotation = rotation;
		}

		public void SetIKRotationWeight(float weight)
		{
			this.IKRotationWeight = Mathf.Clamp(weight, 0f, 1f);
		}

		public Quaternion GetIKRotation()
		{
			return this.IKRotation;
		}

		public float GetIKRotationWeight()
		{
			return this.IKRotationWeight;
		}

		public override IKSolver.Point[] GetPoints()
		{
			return new IKSolver.Point[]
			{
				this.bone1,
				this.bone2,
				this.bone3
			};
		}

		public override IKSolver.Point GetPoint(Transform transform)
		{
			if (this.bone1.transform == transform)
			{
				return this.bone1;
			}
			if (this.bone2.transform == transform)
			{
				return this.bone2;
			}
			if (this.bone3.transform == transform)
			{
				return this.bone3;
			}
			return null;
		}

		public override void StoreDefaultLocalState()
		{
			this.bone1.StoreDefaultLocalState();
			this.bone2.StoreDefaultLocalState();
			this.bone3.StoreDefaultLocalState();
		}

		public override void FixTransforms()
		{
			this.bone1.FixTransform();
			this.bone2.FixTransform();
			this.bone3.FixTransform();
		}

		public override bool IsValid(bool log)
		{
			if (this.bone1.transform == null || this.bone2.transform == null || this.bone3.transform == null)
			{
				if (log)
				{
					base.LogWarning("Bone transform is null in IKSolverTrigonometric. Can't initiate solver.");
				}
				return false;
			}
			if (this.bone1.transform.position == this.bone2.transform.position)
			{
				if (log)
				{
					base.LogWarning("first bone position is the same as second bone position. Can't initiate solver.");
				}
				return false;
			}
			if (this.bone2.transform.position == this.bone3.transform.position)
			{
				if (log)
				{
					base.LogWarning("second bone position is the same as third bone position. Can't initiate solver.");
				}
				return false;
			}
			Transform transform = (Transform)Hierarchy.ContainsDuplicate(new Transform[]
			{
				this.bone1.transform,
				this.bone2.transform,
				this.bone3.transform
			});
			if (transform != null)
			{
				if (log)
				{
					base.LogWarning(transform.name + " is represented multiple times in a single IK chain. Can't initiate solver.");
				}
				return false;
			}
			return true;
		}

		public bool SetChain(Transform bone1, Transform bone2, Transform bone3, Transform root)
		{
			this.bone1.transform = bone1;
			this.bone2.transform = bone2;
			this.bone3.transform = bone3;
			base.Initiate(root);
			return base.initiated;
		}

		protected override void OnInitiate()
		{
			if (this.bendNormal == Vector3.zero)
			{
				this.bendNormal = Vector3.right;
			}
			this.OnInitiateVirtual();
			this.IKPosition = this.bone3.transform.position;
			this.IKRotation = this.bone3.transform.rotation;
			this.InitiateBones();
			this.directHierarchy = this.IsDirectHierarchy();
		}

		private bool IsDirectHierarchy()
		{
			return !(this.bone3.transform.parent != this.bone2.transform) && !(this.bone2.transform.parent != this.bone1.transform);
		}

		private void InitiateBones()
		{
			this.bone1.Initiate(this.bone2.transform.position, this.bendNormal);
			this.bone2.Initiate(this.bone3.transform.position, this.bendNormal);
			this.SetBendPlaneToCurrent();
		}

		protected override void OnUpdate()
		{
			this.IKPositionWeight = Mathf.Clamp(this.IKPositionWeight, 0f, 1f);
			this.IKRotationWeight = Mathf.Clamp(this.IKRotationWeight, 0f, 1f);
			if (this.target != null)
			{
				this.IKPosition = this.target.position;
				this.IKRotation = this.target.rotation;
			}
			this.OnUpdateVirtual();
			if (this.IKPositionWeight > 0f)
			{
				if (!this.directHierarchy)
				{
					this.bone1.Initiate(this.bone2.transform.position, this.bendNormal);
					this.bone2.Initiate(this.bone3.transform.position, this.bendNormal);
				}
				this.bone1.sqrMag = (this.bone2.transform.position - this.bone1.transform.position).sqrMagnitude;
				this.bone2.sqrMag = (this.bone3.transform.position - this.bone2.transform.position).sqrMagnitude;
				if (this.bendNormal == Vector3.zero && !Warning.logged)
				{
					base.LogWarning("IKSolverTrigonometric Bend Normal is Vector3.zero.");
				}
				this.weightIKPosition = Vector3.Lerp(this.bone3.transform.position, this.IKPosition, this.IKPositionWeight);
				Vector3 vector = Vector3.Lerp(this.bone1.GetBendNormalFromCurrentRotation(), this.bendNormal, this.IKPositionWeight);
				Vector3 vector2 = Vector3.Lerp(this.bone2.transform.position - this.bone1.transform.position, this.GetBendDirection(this.weightIKPosition, vector), this.IKPositionWeight);
				if (vector2 == Vector3.zero)
				{
					vector2 = this.bone2.transform.position - this.bone1.transform.position;
				}
				this.bone1.transform.rotation = this.bone1.GetRotation(vector2, vector);
				this.bone2.transform.rotation = this.bone2.GetRotation(this.weightIKPosition - this.bone2.transform.position, this.bone2.GetBendNormalFromCurrentRotation());
			}
			if (this.IKRotationWeight > 0f)
			{
				this.bone3.transform.rotation = Quaternion.Slerp(this.bone3.transform.rotation, this.IKRotation, this.IKRotationWeight);
			}
			this.OnPostSolveVirtual();
		}

		protected virtual void OnInitiateVirtual()
		{
		}

		protected virtual void OnUpdateVirtual()
		{
		}

		protected virtual void OnPostSolveVirtual()
		{
		}

		protected Vector3 GetBendDirection(Vector3 IKPosition, Vector3 bendNormal)
		{
			Vector3 vector = IKPosition - this.bone1.transform.position;
			if (vector == Vector3.zero)
			{
				return Vector3.zero;
			}
			float sqrMagnitude = vector.sqrMagnitude;
			float num = (float)Math.Sqrt((double)sqrMagnitude);
			float num2 = (sqrMagnitude + this.bone1.sqrMag - this.bone2.sqrMag) / 2f / num;
			float y = (float)Math.Sqrt((double)Mathf.Clamp(this.bone1.sqrMag - num2 * num2, 0f, float.PositiveInfinity));
			Vector3 upwards = Vector3.Cross(vector, bendNormal);
			return Quaternion.LookRotation(vector, upwards) * new Vector3(0f, y, num2);
		}
	}
}
