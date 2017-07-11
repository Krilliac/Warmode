using System;
using UnityEngine;

namespace RootMotion.FinalIK
{
	[Serializable]
	public class FBIKChain
	{
		[Serializable]
		public class ChildConstraint
		{
			public float pushElasticity;

			public float pullElasticity;

			[SerializeField]
			private Transform bone1;

			[SerializeField]
			private Transform bone2;

			private float crossFade;

			private float inverseCrossFade;

			[NonSerialized]
			private IKSolver.Node node1;

			[NonSerialized]
			private IKSolver.Node node2;

			[NonSerialized]
			private FBIKChain chain1;

			[NonSerialized]
			private FBIKChain chain2;

			public float nominalDistance
			{
				get;
				private set;
			}

			public bool isRigid
			{
				get;
				private set;
			}

			public ChildConstraint(Transform bone1, Transform bone2, float pushElasticity = 0f, float pullElasticity = 0f)
			{
				this.bone1 = bone1;
				this.bone2 = bone2;
				this.pushElasticity = pushElasticity;
				this.pullElasticity = pullElasticity;
			}

			public void Initiate(IKSolverFullBody solver)
			{
				this.chain1 = solver.GetChain(this.bone1);
				this.chain2 = solver.GetChain(this.bone2);
				this.node1 = this.chain1.nodes[0];
				this.node2 = this.chain2.nodes[0];
				this.OnPreSolve();
			}

			public void OnPreSolve()
			{
				this.nominalDistance = Vector3.Distance(this.node1.transform.position, this.node2.transform.position);
				this.isRigid = (this.pushElasticity <= 0f && this.pullElasticity <= 0f);
				if (this.isRigid)
				{
					float num = this.chain1.pull - this.chain2.pull;
					this.crossFade = 1f - (0.5f + num * 0.5f);
				}
				else
				{
					this.crossFade = 0.5f;
				}
				this.inverseCrossFade = 1f - this.crossFade;
			}

			public void Solve()
			{
				if (this.pushElasticity >= 1f && this.pullElasticity >= 1f)
				{
					return;
				}
				Vector3 a = this.node2.solverPosition - this.node1.solverPosition;
				float magnitude = a.magnitude;
				if (magnitude == this.nominalDistance)
				{
					return;
				}
				if (magnitude == 0f)
				{
					return;
				}
				float num = 1f;
				if (!this.isRigid)
				{
					float num2 = (magnitude <= this.nominalDistance) ? this.pushElasticity : this.pullElasticity;
					num = 1f - num2;
				}
				num *= 1f - this.nominalDistance / magnitude;
				Vector3 a2 = a * num;
				this.node1.solverPosition += a2 * this.crossFade;
				this.node2.solverPosition -= a2 * this.inverseCrossFade;
			}
		}

		[Serializable]
		public enum Smoothing
		{
			None,
			Exponential,
			Cubic
		}

		private const float maxLimbLength = 0.99999f;

		[Range(0f, 1f)]
		public float pin;

		[Range(0f, 1f)]
		public float pull = 1f;

		[Range(0f, 1f)]
		public float push;

		[Range(-1f, 1f)]
		public float pushParent;

		[Range(0f, 1f)]
		public float reach = 0.1f;

		public FBIKChain.Smoothing reachSmoothing = FBIKChain.Smoothing.Exponential;

		public FBIKChain.Smoothing pushSmoothing = FBIKChain.Smoothing.Exponential;

		public IKSolver.Node[] nodes = new IKSolver.Node[0];

		public int[] children = new int[0];

		public FBIKChain.ChildConstraint[] childConstraints = new FBIKChain.ChildConstraint[0];

		public IKConstraintBend bendConstraint = new IKConstraintBend();

		private float rootLength;

		private bool initiated;

		private float length;

		private float distance;

		private IKSolver.Point p;

		private float reachForce;

		private float pullParentSum;

		private float[] crossFades;

		private float sqrMag1;

		private float sqrMag2;

		private float sqrMagDif;

		public FBIKChain()
		{
		}

		public FBIKChain(float pin, float pull, params Transform[] nodeTransforms)
		{
			this.pin = pin;
			this.pull = pull;
			this.SetNodes(nodeTransforms);
			this.children = new int[0];
		}

		public void SetNodes(params Transform[] boneTransforms)
		{
			this.nodes = new IKSolver.Node[boneTransforms.Length];
			for (int i = 0; i < boneTransforms.Length; i++)
			{
				this.nodes[i] = new IKSolver.Node(boneTransforms[i]);
			}
		}

		public bool IsValid(Warning.Logger logger = null)
		{
			if (this.nodes.Length == 0)
			{
				if (logger != null)
				{
					logger("FBIK chain contains no nodes.");
				}
				return false;
			}
			IKSolver.Node[] array = this.nodes;
			for (int i = 0; i < array.Length; i++)
			{
				IKSolver.Node node = array[i];
				if (node.transform == null)
				{
					if (logger != null)
					{
						logger("Node transform is null in FBIK chain.");
					}
					return false;
				}
			}
			return true;
		}

		public void Initiate(IKSolver solver, FBIKChain[] chain)
		{
			this.initiated = false;
			IKSolver.Node[] array = this.nodes;
			for (int i = 0; i < array.Length; i++)
			{
				IKSolver.Node node = array[i];
				node.solverPosition = node.transform.position;
			}
			this.CalculateBoneLengths(chain);
			FBIKChain.ChildConstraint[] array2 = this.childConstraints;
			for (int j = 0; j < array2.Length; j++)
			{
				FBIKChain.ChildConstraint childConstraint = array2[j];
				childConstraint.Initiate(solver as IKSolverFullBody);
			}
			if (this.nodes.Length == 3)
			{
				this.bendConstraint.SetBones(this.nodes[0].transform, this.nodes[1].transform, this.nodes[2].transform);
				this.bendConstraint.Initiate(solver as IKSolverFullBody);
			}
			this.crossFades = new float[this.children.Length];
			this.initiated = true;
		}

		public void ReadPose(FBIKChain[] chain, bool fullBody)
		{
			if (!this.initiated)
			{
				return;
			}
			for (int i = 0; i < this.nodes.Length; i++)
			{
				this.nodes[i].solverPosition = this.nodes[i].transform.position + this.nodes[i].offset;
			}
			this.CalculateBoneLengths(chain);
			if (fullBody)
			{
				for (int j = 0; j < this.childConstraints.Length; j++)
				{
					this.childConstraints[j].OnPreSolve();
				}
				if (this.children.Length > 0)
				{
					float num = this.nodes[this.nodes.Length - 1].effectorPositionWeight;
					for (int k = 0; k < this.children.Length; k++)
					{
						num += chain[this.children[k]].nodes[0].effectorPositionWeight * chain[this.children[k]].pull;
					}
					num = Mathf.Clamp(num, 1f, float.PositiveInfinity);
					for (int l = 0; l < this.children.Length; l++)
					{
						this.crossFades[l] = chain[this.children[l]].nodes[0].effectorPositionWeight * chain[this.children[l]].pull / num;
					}
				}
				this.pullParentSum = 0f;
				for (int m = 0; m < this.children.Length; m++)
				{
					this.pullParentSum += chain[this.children[m]].pull;
				}
				this.pullParentSum = Mathf.Clamp(this.pullParentSum, 1f, float.PositiveInfinity);
				if (this.nodes.Length == 3)
				{
					this.reachForce = this.reach * Mathf.Clamp(this.nodes[2].effectorPositionWeight, 0f, 1f);
				}
				else
				{
					this.reachForce = 0f;
				}
				if (this.push > 0f && this.nodes.Length > 1)
				{
					this.distance = Vector3.Distance(this.nodes[0].transform.position, this.nodes[this.nodes.Length - 1].transform.position);
				}
			}
		}

		private void CalculateBoneLengths(FBIKChain[] chain)
		{
			this.length = 0f;
			for (int i = 0; i < this.nodes.Length - 1; i++)
			{
				this.nodes[i].length = Vector3.Distance(this.nodes[i].transform.position, this.nodes[i + 1].transform.position);
				this.length += this.nodes[i].length;
				if (this.nodes[i].length == 0f)
				{
					Warning.Log(string.Concat(new string[]
					{
						"Bone ",
						this.nodes[i].transform.name,
						" - ",
						this.nodes[i + 1].transform.name,
						" length is zero, can not solve."
					}), this.nodes[i].transform, false);
					return;
				}
			}
			for (int j = 0; j < this.children.Length; j++)
			{
				chain[this.children[j]].rootLength = (chain[this.children[j]].nodes[0].transform.position - this.nodes[this.nodes.Length - 1].transform.position).magnitude;
				if (chain[this.children[j]].rootLength == 0f)
				{
					return;
				}
			}
			if (this.nodes.Length == 3)
			{
				this.sqrMag1 = this.nodes[0].length * this.nodes[0].length;
				this.sqrMag2 = this.nodes[1].length * this.nodes[1].length;
				this.sqrMagDif = this.sqrMag1 - this.sqrMag2;
			}
		}

		public void Reach(FBIKChain[] chain)
		{
			if (!this.initiated)
			{
				return;
			}
			for (int i = 0; i < this.children.Length; i++)
			{
				chain[this.children[i]].Reach(chain);
			}
			if (this.reachForce <= 0f)
			{
				return;
			}
			Vector3 vector = this.nodes[2].solverPosition - this.nodes[0].solverPosition;
			if (vector == Vector3.zero)
			{
				return;
			}
			float magnitude = vector.magnitude;
			Vector3 a = vector / magnitude * this.length;
			float num = Mathf.Clamp(magnitude / this.length, 1f - this.reachForce, 1f + this.reachForce) - 1f;
			num = Mathf.Clamp(num + this.reachForce, -1f, 1f);
			FBIKChain.Smoothing smoothing = this.reachSmoothing;
			if (smoothing != FBIKChain.Smoothing.Exponential)
			{
				if (smoothing == FBIKChain.Smoothing.Cubic)
				{
					num *= num * num;
				}
			}
			else
			{
				num *= num;
			}
			Vector3 vector2 = a * Mathf.Clamp(num, 0f, magnitude);
			this.nodes[0].solverPosition += vector2 * (1f - this.nodes[0].effectorPositionWeight);
			this.nodes[2].solverPosition += vector2;
		}

		public Vector3 Push(FBIKChain[] chain)
		{
			Vector3 vector = Vector3.zero;
			for (int i = 0; i < this.children.Length; i++)
			{
				vector += chain[this.children[i]].Push(chain) * chain[this.children[i]].pushParent;
			}
			this.nodes[this.nodes.Length - 1].solverPosition += vector;
			if (this.nodes.Length < 2)
			{
				return Vector3.zero;
			}
			if (this.push <= 0f)
			{
				return Vector3.zero;
			}
			Vector3 a = this.nodes[2].solverPosition - this.nodes[0].solverPosition;
			float magnitude = a.magnitude;
			if (magnitude == 0f)
			{
				return Vector3.zero;
			}
			float num = 1f - magnitude / this.distance;
			if (num <= 0f)
			{
				return Vector3.zero;
			}
			FBIKChain.Smoothing smoothing = this.pushSmoothing;
			if (smoothing != FBIKChain.Smoothing.Exponential)
			{
				if (smoothing == FBIKChain.Smoothing.Cubic)
				{
					num *= num * num;
				}
			}
			else
			{
				num *= num;
			}
			Vector3 vector2 = -a * num * this.push;
			this.nodes[0].solverPosition += vector2;
			return vector2;
		}

		public void SolveTrigonometric(FBIKChain[] chain, bool calculateBendDirection = false)
		{
			if (!this.initiated)
			{
				return;
			}
			for (int i = 0; i < this.children.Length; i++)
			{
				chain[this.children[i]].SolveTrigonometric(chain, calculateBendDirection);
			}
			if (this.nodes.Length != 3)
			{
				return;
			}
			Vector3 a = this.nodes[2].solverPosition - this.nodes[0].solverPosition;
			float magnitude = a.magnitude;
			if (magnitude == 0f)
			{
				return;
			}
			float num = Mathf.Clamp(magnitude, 0f, this.length * 0.99999f);
			Vector3 direction = a / magnitude * num;
			Vector3 bendDirection = (!calculateBendDirection || !this.bendConstraint.initiated) ? (this.nodes[1].solverPosition - this.nodes[0].solverPosition) : this.bendConstraint.GetDir();
			Vector3 dirToBendPoint = this.GetDirToBendPoint(direction, bendDirection, num);
			this.nodes[1].solverPosition = this.nodes[0].solverPosition + dirToBendPoint;
		}

		public void Stage1(FBIKChain[] chain)
		{
			for (int i = 0; i < this.children.Length; i++)
			{
				chain[this.children[i]].Stage1(chain);
			}
			if (this.children.Length == 0)
			{
				this.ForwardReach(this.nodes[this.nodes.Length - 1].solverPosition);
				return;
			}
			Vector3 a = this.nodes[this.nodes.Length - 1].solverPosition;
			this.SolveChildConstraints();
			for (int j = 0; j < this.children.Length; j++)
			{
				Vector3 a2 = chain[this.children[j]].nodes[0].solverPosition;
				if (chain[this.children[j]].rootLength > 0f)
				{
					a2 = IKSolverFABRIK.SolveJoint(this.nodes[this.nodes.Length - 1].solverPosition, chain[this.children[j]].nodes[0].solverPosition, chain[this.children[j]].rootLength);
				}
				if (this.pullParentSum > 0f)
				{
					a += (a2 - this.nodes[this.nodes.Length - 1].solverPosition) * (chain[this.children[j]].pull / this.pullParentSum);
				}
			}
			this.ForwardReach(Vector3.Lerp(a, this.nodes[this.nodes.Length - 1].solverPosition, this.pin));
		}

		public void Stage2(Vector3 position, int iterations, FBIKChain[] chain)
		{
			this.BackwardReach(position);
			int num = Mathf.Clamp(iterations, 2, 4);
			if (this.childConstraints.Length > 0)
			{
				for (int i = 0; i < num; i++)
				{
					this.SolveConstraintSystems(chain);
				}
			}
			for (int j = 0; j < this.children.Length; j++)
			{
				chain[this.children[j]].Stage2(this.nodes[this.nodes.Length - 1].solverPosition, iterations, chain);
			}
		}

		public void SolveConstraintSystems(FBIKChain[] chain)
		{
			this.SolveChildConstraints();
			for (int i = 0; i < this.children.Length; i++)
			{
				this.SolveLinearConstraint(this.nodes[this.nodes.Length - 1], chain[this.children[i]].nodes[0], this.crossFades[i], chain[this.children[i]].rootLength);
			}
		}

		protected Vector3 GetDirToBendPoint(Vector3 direction, Vector3 bendDirection, float directionMagnitude)
		{
			float num = (directionMagnitude * directionMagnitude + this.sqrMagDif) / 2f / directionMagnitude;
			float y = (float)Math.Sqrt((double)Mathf.Clamp(this.sqrMag1 - num * num, 0f, float.PositiveInfinity));
			if (direction == Vector3.zero)
			{
				return Vector3.zero;
			}
			return Quaternion.LookRotation(direction, bendDirection) * new Vector3(0f, y, num);
		}

		private void SolveChildConstraints()
		{
			for (int i = 0; i < this.childConstraints.Length; i++)
			{
				this.childConstraints[i].Solve();
			}
		}

		private void SolveLinearConstraint(IKSolver.Node node1, IKSolver.Node node2, float crossFade, float distance)
		{
			Vector3 a = node2.solverPosition - node1.solverPosition;
			float magnitude = a.magnitude;
			if (distance == magnitude)
			{
				return;
			}
			if (magnitude == 0f)
			{
				return;
			}
			Vector3 a2 = a * (1f - distance / magnitude);
			node1.solverPosition += a2 * crossFade;
			node2.solverPosition -= a2 * (1f - crossFade);
		}

		public void ForwardReach(Vector3 position)
		{
			this.nodes[this.nodes.Length - 1].solverPosition = position;
			for (int i = this.nodes.Length - 2; i > -1; i--)
			{
				this.nodes[i].solverPosition = IKSolverFABRIK.SolveJoint(this.nodes[i].solverPosition, this.nodes[i + 1].solverPosition, this.nodes[i].length);
			}
		}

		private void BackwardReach(Vector3 position)
		{
			if (this.rootLength > 0f)
			{
				position = IKSolverFABRIK.SolveJoint(this.nodes[0].solverPosition, position, this.rootLength);
			}
			this.nodes[0].solverPosition = position;
			for (int i = 1; i < this.nodes.Length; i++)
			{
				this.nodes[i].solverPosition = IKSolverFABRIK.SolveJoint(this.nodes[i].solverPosition, this.nodes[i - 1].solverPosition, this.nodes[i - 1].length);
			}
		}
	}
}
