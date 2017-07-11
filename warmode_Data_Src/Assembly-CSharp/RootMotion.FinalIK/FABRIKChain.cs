using System;
using UnityEngine;

namespace RootMotion.FinalIK
{
	[Serializable]
	public class FABRIKChain
	{
		public FABRIK ik;

		[Range(0f, 1f)]
		public float pull = 1f;

		[Range(0f, 1f)]
		public float pin = 1f;

		public int[] children = new int[0];

		public bool IsValid(Warning.Logger logger)
		{
			if (this.ik == null)
			{
				if (logger != null)
				{
					logger("IK unassigned in FABRIKChain.");
				}
				return false;
			}
			return this.ik.solver.IsValid(logger != null);
		}

		public void Initiate()
		{
			this.ik.Disable();
		}

		public void Stage1(FABRIKChain[] chain)
		{
			for (int i = 0; i < this.children.Length; i++)
			{
				chain[this.children[i]].Stage1(chain);
			}
			if (this.children.Length == 0)
			{
				this.ik.solver.SolveForward(this.ik.solver.GetIKPosition());
				return;
			}
			this.ik.solver.SolveForward(this.GetCentroid(chain));
		}

		public void Stage2(Vector3 rootPosition, FABRIKChain[] chain)
		{
			this.ik.solver.SolveBackward(rootPosition);
			for (int i = 0; i < this.children.Length; i++)
			{
				chain[this.children[i]].Stage2(this.ik.solver.bones[this.ik.solver.bones.Length - 1].transform.position, chain);
			}
		}

		private Vector3 GetCentroid(FABRIKChain[] chain)
		{
			Vector3 iKPosition = this.ik.solver.GetIKPosition();
			if (this.pin >= 1f)
			{
				return iKPosition;
			}
			float num = 0f;
			for (int i = 0; i < this.children.Length; i++)
			{
				num += chain[this.children[i]].pull;
			}
			if (num <= 0f)
			{
				return iKPosition;
			}
			if (num < 1f)
			{
				num = 1f;
			}
			Vector3 vector = iKPosition;
			for (int j = 0; j < this.children.Length; j++)
			{
				Vector3 a = chain[this.children[j]].ik.solver.bones[0].solverPosition - iKPosition;
				float d = chain[this.children[j]].pull / num;
				vector += a * d;
			}
			if (this.pin <= 0f)
			{
				return vector;
			}
			return vector + (iKPosition - vector) * this.pin;
		}
	}
}
