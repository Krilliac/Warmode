using System;
using UnityEngine;

namespace RootMotion.FinalIK
{
	[Serializable]
	public class IKEffector
	{
		public Transform bone;

		public Transform target;

		[Range(0f, 1f)]
		public float positionWeight;

		[Range(0f, 1f)]
		public float rotationWeight;

		public Vector3 position = Vector3.zero;

		public Quaternion rotation = Quaternion.identity;

		public Vector3 positionOffset;

		public bool effectChildNodes = true;

		[Range(0f, 1f)]
		public float maintainRelativePositionWeight;

		public Transform[] childBones = new Transform[0];

		public Transform planeBone1;

		public Transform planeBone2;

		public Transform planeBone3;

		public Quaternion planeRotationOffset = Quaternion.identity;

		private IKSolver.Node node = new IKSolver.Node();

		private IKSolver.Node planeNode1 = new IKSolver.Node();

		private IKSolver.Node planeNode2 = new IKSolver.Node();

		private IKSolver.Node planeNode3 = new IKSolver.Node();

		private IKSolver.Node[] childNodes = new IKSolver.Node[0];

		private IKSolver solver;

		private float posW;

		private float rotW;

		private Vector3[] localPositions = new Vector3[0];

		private bool usePlaneNodes;

		private Quaternion animatedPlaneRotation = Quaternion.identity;

		private Vector3 animatedPosition;

		private bool firstUpdate;

		public bool isEndEffector
		{
			get;
			private set;
		}

		private Quaternion planeRotation
		{
			get
			{
				Vector3 vector = this.planeNode2.solverPosition - this.planeNode1.solverPosition;
				Vector3 upwards = this.planeNode3.solverPosition - this.planeNode1.solverPosition;
				if (vector == Vector3.zero)
				{
					Warning.Log("Make sure you are not placing 2 or more FBBIK effectors of the same chain to exactly the same position.", this.bone, false);
					return Quaternion.identity;
				}
				return Quaternion.LookRotation(vector, upwards);
			}
		}

		public IKEffector()
		{
		}

		public IKEffector(Transform bone, Transform[] childBones)
		{
			this.bone = bone;
			this.childBones = childBones;
		}

		public IKSolver.Node GetNode()
		{
			return this.node;
		}

		public void PinToBone(float positionWeight, float rotationWeight)
		{
			this.position = this.bone.position;
			this.positionWeight = Mathf.Clamp(positionWeight, 0f, 1f);
			this.rotation = this.bone.rotation;
			this.rotationWeight = Mathf.Clamp(rotationWeight, 0f, 1f);
		}

		public bool IsValid(IKSolver solver, Warning.Logger logger)
		{
			if (this.bone == null)
			{
				if (logger != null)
				{
					logger("IK Effector bone is null.");
				}
				return false;
			}
			if (solver.GetPoint(this.bone) == null)
			{
				if (logger != null)
				{
					logger("IK Effector is referencing to a bone '" + this.bone.name + "' that does not excist in the Node Chain.");
				}
				return false;
			}
			Transform[] array = this.childBones;
			for (int i = 0; i < array.Length; i++)
			{
				Transform x = array[i];
				if (x == null)
				{
					if (logger != null)
					{
						logger("IK Effector contains a null reference.");
					}
					return false;
				}
			}
			Transform[] array2 = this.childBones;
			for (int j = 0; j < array2.Length; j++)
			{
				Transform transform = array2[j];
				if (solver.GetPoint(transform) == null)
				{
					if (logger != null)
					{
						logger("IK Effector is referencing to a bone '" + transform.name + "' that does not excist in the Node Chain.");
					}
					return false;
				}
			}
			if (this.planeBone1 != null && solver.GetPoint(this.planeBone1) == null)
			{
				if (logger != null)
				{
					logger("IK Effector is referencing to a bone '" + this.planeBone1.name + "' that does not excist in the Node Chain.");
				}
				return false;
			}
			if (this.planeBone2 != null && solver.GetPoint(this.planeBone2) == null)
			{
				if (logger != null)
				{
					logger("IK Effector is referencing to a bone '" + this.planeBone2.name + "' that does not excist in the Node Chain.");
				}
				return false;
			}
			if (this.planeBone3 != null && solver.GetPoint(this.planeBone3) == null)
			{
				if (logger != null)
				{
					logger("IK Effector is referencing to a bone '" + this.planeBone3.name + "' that does not excist in the Node Chain.");
				}
				return false;
			}
			return true;
		}

		public void Initiate(IKSolver solver)
		{
			this.solver = solver;
			this.position = this.bone.position;
			this.rotation = this.bone.rotation;
			this.animatedPlaneRotation = Quaternion.identity;
			this.node = (solver.GetPoint(this.bone) as IKSolver.Node);
			if (this.childNodes == null || this.childNodes.Length != this.childBones.Length)
			{
				this.childNodes = new IKSolver.Node[this.childBones.Length];
			}
			for (int i = 0; i < this.childBones.Length; i++)
			{
				this.childNodes[i] = (solver.GetPoint(this.childBones[i]) as IKSolver.Node);
			}
			if (this.localPositions == null || this.localPositions.Length != this.childBones.Length)
			{
				this.localPositions = new Vector3[this.childBones.Length];
			}
			this.usePlaneNodes = false;
			if (this.planeBone1 != null)
			{
				this.planeNode1 = (solver.GetPoint(this.planeBone1) as IKSolver.Node);
				if (this.planeBone2 != null)
				{
					this.planeNode2 = (solver.GetPoint(this.planeBone2) as IKSolver.Node);
					if (this.planeBone3 != null)
					{
						this.planeNode3 = (solver.GetPoint(this.planeBone3) as IKSolver.Node);
						this.usePlaneNodes = true;
					}
				}
				this.isEndEffector = true;
			}
			else
			{
				this.isEndEffector = false;
			}
		}

		public void ResetOffset()
		{
			this.node.offset = Vector3.zero;
			for (int i = 0; i < this.childNodes.Length; i++)
			{
				this.childNodes[i].offset = Vector3.zero;
			}
		}

		public void SetToTarget()
		{
			if (this.target == null)
			{
				return;
			}
			this.position = this.target.position;
			this.rotation = this.target.rotation;
		}

		public void OnPreSolve(bool fullBody)
		{
			this.positionWeight = Mathf.Clamp(this.positionWeight, 0f, 1f);
			this.rotationWeight = Mathf.Clamp(this.rotationWeight, 0f, 1f);
			this.maintainRelativePositionWeight = Mathf.Clamp(this.maintainRelativePositionWeight, 0f, 1f);
			this.posW = this.positionWeight * this.solver.GetIKPositionWeight();
			this.rotW = this.rotationWeight * this.solver.GetIKPositionWeight();
			this.node.effectorPositionWeight = this.posW;
			this.node.effectorRotationWeight = this.rotW;
			this.node.solverRotation = this.rotation;
			if (float.IsInfinity(this.positionOffset.x) || float.IsInfinity(this.positionOffset.y) || float.IsInfinity(this.positionOffset.z))
			{
				Debug.LogError("Invalid IKEffector.positionOffset (contains Infinity)! Please make sure not to set IKEffector.positionOffset to infinite values.", this.bone);
			}
			if (float.IsNaN(this.positionOffset.x) || float.IsNaN(this.positionOffset.y) || float.IsNaN(this.positionOffset.z))
			{
				Debug.LogError("Invalid IKEffector.positionOffset (contains NaN)! Please make sure not to set IKEffector.positionOffset to NaN values.", this.bone);
			}
			if (this.positionOffset.sqrMagnitude > 1E+10f)
			{
				Debug.LogError("Additive effector positionOffset detected in Full Body IK (extremely large value). Make sure you are not circularily adding to effector positionOffset each frame.", this.bone);
			}
			if (float.IsInfinity(this.position.x) || float.IsInfinity(this.position.y) || float.IsInfinity(this.position.z))
			{
				Debug.LogError("Invalid IKEffector.position (contains Infinity)!");
			}
			this.node.offset += this.positionOffset * this.solver.GetIKPositionWeight();
			if (this.effectChildNodes && fullBody)
			{
				for (int i = 0; i < this.childNodes.Length; i++)
				{
					this.localPositions[i] = this.childNodes[i].transform.position - this.node.transform.position;
					this.childNodes[i].offset += this.positionOffset * this.solver.GetIKPositionWeight();
				}
			}
			if (this.usePlaneNodes && this.maintainRelativePositionWeight > 0f)
			{
				this.animatedPlaneRotation = Quaternion.LookRotation(this.planeNode2.transform.position - this.planeNode1.transform.position, this.planeNode3.transform.position - this.planeNode1.transform.position);
			}
			this.firstUpdate = true;
		}

		public void OnPostWrite()
		{
			this.positionOffset = Vector3.zero;
		}

		public void Update()
		{
			if (this.firstUpdate)
			{
				this.animatedPosition = this.node.transform.position + this.node.offset;
				this.firstUpdate = false;
			}
			this.node.solverPosition = Vector3.Lerp(this.GetPosition(out this.planeRotationOffset), this.position, this.posW);
			if (!this.effectChildNodes)
			{
				return;
			}
			for (int i = 0; i < this.childNodes.Length; i++)
			{
				this.childNodes[i].solverPosition = Vector3.Lerp(this.childNodes[i].solverPosition, this.node.solverPosition + this.localPositions[i], this.posW);
			}
		}

		private Vector3 GetPosition(out Quaternion planeRotationOffset)
		{
			planeRotationOffset = Quaternion.identity;
			if (!this.isEndEffector)
			{
				return this.node.solverPosition;
			}
			if (this.maintainRelativePositionWeight <= 0f)
			{
				return this.animatedPosition;
			}
			Vector3 a = this.node.transform.position;
			Vector3 point = a - this.planeNode1.transform.position;
			planeRotationOffset = this.planeRotation * Quaternion.Inverse(this.animatedPlaneRotation);
			a = this.planeNode1.solverPosition + planeRotationOffset * point;
			planeRotationOffset = Quaternion.Lerp(Quaternion.identity, planeRotationOffset, this.maintainRelativePositionWeight);
			return Vector3.Lerp(this.animatedPosition, a + this.node.offset, this.maintainRelativePositionWeight);
		}
	}
}
