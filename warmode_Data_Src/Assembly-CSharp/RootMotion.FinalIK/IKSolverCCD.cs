using System;
using UnityEngine;

namespace RootMotion.FinalIK
{
	[Serializable]
	public class IKSolverCCD : IKSolverHeuristic
	{
		public IKSolver.IterationDelegate OnPreIteration;

		public void FadeOutBoneWeights()
		{
			if (this.bones.Length < 2)
			{
				return;
			}
			this.bones[0].weight = 1f;
			float num = 1f / (float)(this.bones.Length - 1);
			for (int i = 1; i < this.bones.Length; i++)
			{
				this.bones[i].weight = num * (float)(this.bones.Length - 1 - i);
			}
		}

		protected override void OnInitiate()
		{
			if (this.firstInitiation || !Application.isPlaying)
			{
				this.IKPosition = this.bones[this.bones.Length - 1].transform.position;
			}
			base.InitiateBones();
		}

		protected override void OnUpdate()
		{
			if (this.IKPositionWeight <= 0f)
			{
				return;
			}
			this.IKPositionWeight = Mathf.Clamp(this.IKPositionWeight, 0f, 1f);
			if (this.target != null)
			{
				this.IKPosition = this.target.position;
			}
			Vector3 vector = (this.maxIterations <= 1) ? Vector3.zero : base.GetSingularityOffset();
			for (int i = 0; i < this.maxIterations; i++)
			{
				if (vector == Vector3.zero && i >= 1 && this.tolerance > 0f && base.positionOffset < this.tolerance * this.tolerance)
				{
					break;
				}
				this.lastLocalDirection = this.localDirection;
				if (this.OnPreIteration != null)
				{
					this.OnPreIteration(i);
				}
				this.Solve(this.IKPosition + ((i != 0) ? Vector3.zero : vector));
			}
			this.lastLocalDirection = this.localDirection;
		}

		private void Solve(Vector3 targetPosition)
		{
			for (int i = this.bones.Length - 2; i > -1; i--)
			{
				Vector3 fromDirection = this.bones[this.bones.Length - 1].transform.position - this.bones[i].transform.position;
				Vector3 toDirection = targetPosition - this.bones[i].transform.position;
				Quaternion quaternion = Quaternion.FromToRotation(fromDirection, toDirection) * this.bones[i].transform.rotation;
				float num = this.bones[i].weight * this.IKPositionWeight;
				if (num >= 1f)
				{
					this.bones[i].transform.rotation = quaternion;
				}
				else
				{
					this.bones[i].transform.rotation = Quaternion.Lerp(this.bones[i].transform.rotation, quaternion, num);
				}
				if (this.useRotationLimits && this.bones[i].rotationLimit != null)
				{
					this.bones[i].rotationLimit.Apply();
				}
			}
		}
	}
}
