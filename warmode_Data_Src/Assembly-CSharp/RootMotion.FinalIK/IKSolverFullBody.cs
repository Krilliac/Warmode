using System;
using UnityEngine;

namespace RootMotion.FinalIK
{
	[Serializable]
	public class IKSolverFullBody : IKSolver
	{
		[Range(0f, 10f)]
		public int iterations = 4;

		public FBIKChain[] chain = new FBIKChain[0];

		public IKEffector[] effectors = new IKEffector[0];

		public IKMappingSpine spineMapping = new IKMappingSpine();

		public IKMappingBone[] boneMappings = new IKMappingBone[0];

		public IKMappingLimb[] limbMappings = new IKMappingLimb[0];

		public IKSolver.UpdateDelegate OnPreRead;

		public IKSolver.UpdateDelegate OnPreSolve;

		public IKSolver.IterationDelegate OnPreIteration;

		public IKSolver.IterationDelegate OnPostIteration;

		public IKSolver.UpdateDelegate OnPreBend;

		public IKSolver.UpdateDelegate OnPostSolve;

		public IKEffector GetEffector(Transform t)
		{
			for (int i = 0; i < this.effectors.Length; i++)
			{
				if (this.effectors[i].bone == t)
				{
					return this.effectors[i];
				}
			}
			return null;
		}

		public FBIKChain GetChain(Transform transform)
		{
			for (int i = 0; i < this.chain.Length; i++)
			{
				for (int j = 0; j < this.chain[i].nodes.Length; j++)
				{
					if (this.chain[i].nodes[j].transform == transform)
					{
						return this.chain[i];
					}
				}
			}
			return null;
		}

		public override IKSolver.Point[] GetPoints()
		{
			int num = 0;
			for (int i = 0; i < this.chain.Length; i++)
			{
				num += this.chain[i].nodes.Length;
			}
			IKSolver.Point[] array = new IKSolver.Point[num];
			int num2 = 0;
			for (int j = 0; j < this.chain.Length; j++)
			{
				for (int k = 0; k < this.chain[j].nodes.Length; k++)
				{
					array[num2] = this.chain[j].nodes[k];
				}
			}
			return array;
		}

		public override IKSolver.Point GetPoint(Transform transform)
		{
			for (int i = 0; i < this.chain.Length; i++)
			{
				for (int j = 0; j < this.chain[i].nodes.Length; j++)
				{
					if (this.chain[i].nodes[j].transform == transform)
					{
						return this.chain[i].nodes[j];
					}
				}
			}
			return null;
		}

		public override bool IsValid(bool log)
		{
			if (this.chain == null)
			{
				if (log)
				{
					base.LogWarning("FBIK chain is null, can't initiate solver.");
				}
				return false;
			}
			if (this.chain.Length == 0)
			{
				if (log)
				{
					base.LogWarning("FBIK chain length is 0, can't initiate solver.");
				}
				return false;
			}
			for (int i = 0; i < this.chain.Length; i++)
			{
				if (log)
				{
					if (!this.chain[i].IsValid(new Warning.Logger(base.LogWarning)))
					{
						return false;
					}
				}
				else if (!this.chain[i].IsValid(null))
				{
					return false;
				}
			}
			IKEffector[] array = this.effectors;
			for (int j = 0; j < array.Length; j++)
			{
				IKEffector iKEffector = array[j];
				if (!iKEffector.IsValid(this, new Warning.Logger(base.LogWarning)))
				{
					return false;
				}
			}
			if (log)
			{
				if (!this.spineMapping.IsValid(this, new Warning.Logger(base.LogWarning)))
				{
					return false;
				}
				IKMappingLimb[] array2 = this.limbMappings;
				for (int k = 0; k < array2.Length; k++)
				{
					IKMappingLimb iKMappingLimb = array2[k];
					if (!iKMappingLimb.IsValid(this, new Warning.Logger(base.LogWarning)))
					{
						return false;
					}
				}
				IKMappingBone[] array3 = this.boneMappings;
				for (int l = 0; l < array3.Length; l++)
				{
					IKMappingBone iKMappingBone = array3[l];
					if (!iKMappingBone.IsValid(this, new Warning.Logger(base.LogWarning)))
					{
						return false;
					}
				}
			}
			else
			{
				if (!this.spineMapping.IsValid(this, null))
				{
					return false;
				}
				IKMappingLimb[] array4 = this.limbMappings;
				for (int m = 0; m < array4.Length; m++)
				{
					IKMappingLimb iKMappingLimb2 = array4[m];
					if (!iKMappingLimb2.IsValid(this, null))
					{
						return false;
					}
				}
				IKMappingBone[] array5 = this.boneMappings;
				for (int n = 0; n < array5.Length; n++)
				{
					IKMappingBone iKMappingBone2 = array5[n];
					if (!iKMappingBone2.IsValid(this, null))
					{
						return false;
					}
				}
			}
			return true;
		}

		public override void StoreDefaultLocalState()
		{
			this.spineMapping.StoreDefaultLocalState();
			for (int i = 0; i < this.limbMappings.Length; i++)
			{
				this.limbMappings[i].StoreDefaultLocalState();
			}
			for (int j = 0; j < this.boneMappings.Length; j++)
			{
				this.boneMappings[j].StoreDefaultLocalState();
			}
		}

		public override void FixTransforms()
		{
			this.spineMapping.FixTransforms();
			for (int i = 0; i < this.limbMappings.Length; i++)
			{
				this.limbMappings[i].FixTransforms();
			}
			for (int j = 0; j < this.boneMappings.Length; j++)
			{
				this.boneMappings[j].FixTransforms();
			}
		}

		protected override void OnInitiate()
		{
			for (int i = 0; i < this.chain.Length; i++)
			{
				this.chain[i].Initiate(this, this.chain);
			}
			IKEffector[] array = this.effectors;
			for (int j = 0; j < array.Length; j++)
			{
				IKEffector iKEffector = array[j];
				iKEffector.Initiate(this);
			}
			this.spineMapping.Initiate(this);
			IKMappingBone[] array2 = this.boneMappings;
			for (int k = 0; k < array2.Length; k++)
			{
				IKMappingBone iKMappingBone = array2[k];
				iKMappingBone.Initiate(this);
			}
			IKMappingLimb[] array3 = this.limbMappings;
			for (int l = 0; l < array3.Length; l++)
			{
				IKMappingLimb iKMappingLimb = array3[l];
				iKMappingLimb.Initiate(this);
			}
		}

		protected override void OnUpdate()
		{
			if (this.IKPositionWeight <= 0f)
			{
				for (int i = 0; i < this.effectors.Length; i++)
				{
					this.effectors[i].positionOffset = Vector3.zero;
				}
				return;
			}
			if (this.chain.Length == 0)
			{
				return;
			}
			this.IKPositionWeight = Mathf.Clamp(this.IKPositionWeight, 0f, 1f);
			if (this.OnPreRead != null)
			{
				this.OnPreRead();
			}
			this.ReadPose();
			if (this.OnPreSolve != null)
			{
				this.OnPreSolve();
			}
			this.Solve();
			if (this.OnPostSolve != null)
			{
				this.OnPostSolve();
			}
			this.WritePose();
			for (int j = 0; j < this.effectors.Length; j++)
			{
				this.effectors[j].OnPostWrite();
			}
		}

		protected virtual void ReadPose()
		{
			for (int i = 0; i < this.chain.Length; i++)
			{
				if (this.chain[i].bendConstraint.initiated)
				{
					this.chain[i].bendConstraint.LimitBend(this.IKPositionWeight, this.GetEffector(this.chain[i].nodes[2].transform).positionWeight);
				}
			}
			for (int j = 0; j < this.effectors.Length; j++)
			{
				this.effectors[j].ResetOffset();
			}
			for (int k = 0; k < this.effectors.Length; k++)
			{
				this.effectors[k].OnPreSolve(this.iterations > 0);
			}
			for (int l = 0; l < this.chain.Length; l++)
			{
				this.chain[l].ReadPose(this.chain, this.iterations > 0);
			}
			if (this.iterations > 0)
			{
				this.spineMapping.ReadPose();
				for (int m = 0; m < this.boneMappings.Length; m++)
				{
					this.boneMappings[m].ReadPose();
				}
			}
			for (int n = 0; n < this.limbMappings.Length; n++)
			{
				this.limbMappings[n].ReadPose();
			}
		}

		protected virtual void Solve()
		{
			if (this.iterations > 0)
			{
				for (int i = 0; i < this.iterations; i++)
				{
					if (this.OnPreIteration != null)
					{
						this.OnPreIteration(i);
					}
					for (int j = 0; j < this.effectors.Length; j++)
					{
						if (this.effectors[j].isEndEffector)
						{
							this.effectors[j].Update();
						}
					}
					this.chain[0].Push(this.chain);
					this.chain[0].Reach(this.chain);
					for (int k = 0; k < this.effectors.Length; k++)
					{
						if (!this.effectors[k].isEndEffector)
						{
							this.effectors[k].Update();
						}
					}
					this.chain[0].SolveTrigonometric(this.chain, false);
					this.chain[0].Stage1(this.chain);
					for (int l = 0; l < this.effectors.Length; l++)
					{
						if (!this.effectors[l].isEndEffector)
						{
							this.effectors[l].Update();
						}
					}
					this.chain[0].Stage2(this.chain[0].nodes[0].solverPosition, this.iterations, this.chain);
					if (this.OnPostIteration != null)
					{
						this.OnPostIteration(i);
					}
				}
			}
			if (this.OnPreBend != null)
			{
				this.OnPreBend();
			}
			for (int m = 0; m < this.effectors.Length; m++)
			{
				if (this.effectors[m].isEndEffector)
				{
					this.effectors[m].Update();
				}
			}
			this.ApplyBendConstraints();
		}

		protected virtual void ApplyBendConstraints()
		{
			this.chain[0].SolveTrigonometric(this.chain, true);
		}

		protected virtual void WritePose()
		{
			if (this.iterations > 0)
			{
				this.spineMapping.WritePose();
				for (int i = 0; i < this.boneMappings.Length; i++)
				{
					this.boneMappings[i].WritePose();
				}
			}
			for (int j = 0; j < this.limbMappings.Length; j++)
			{
				this.limbMappings[j].WritePose(this.iterations > 0);
			}
		}
	}
}
