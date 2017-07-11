using System;
using UnityEngine;

namespace RootMotion.FinalIK.Demos
{
	public class BendGoal : MonoBehaviour
	{
		public LimbIK limbIK;

		[Range(0f, 1f)]
		public float weight = 1f;

		private void Start()
		{
			Debug.Log("BendGoal is deprecated, you can now a bend goal from the custom inspector of the LimbIK component.");
		}

		private void LateUpdate()
		{
			if (this.limbIK == null)
			{
				return;
			}
			this.limbIK.solver.SetBendGoalPosition(base.transform.position, this.weight);
		}
	}
}
