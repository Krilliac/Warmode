using System;
using UnityEngine;

namespace RootMotion.FinalIK.Demos
{
	public class FBIKBendGoal : MonoBehaviour
	{
		public FullBodyBipedIK ik;

		public FullBodyBipedChain chain;

		public float weight;

		private void Start()
		{
			Debug.Log("FBIKBendGoal is deprecated, you can now a bend goal from the custom inspector of the FullBodyBipedIK component.");
		}

		private void Update()
		{
			if (this.ik == null)
			{
				return;
			}
			this.ik.solver.GetBendConstraint(this.chain).bendGoal = base.transform;
			this.ik.solver.GetBendConstraint(this.chain).weight = this.weight;
		}
	}
}
