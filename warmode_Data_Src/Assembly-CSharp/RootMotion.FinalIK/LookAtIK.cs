using System;
using UnityEngine;

namespace RootMotion.FinalIK
{
	[AddComponentMenu("Scripts/RootMotion.FinalIK/IK/Look At IK")]
	public class LookAtIK : IK
	{
		public IKSolverLookAt solver = new IKSolverLookAt();

		public override IKSolver GetIKSolver()
		{
			return this.solver;
		}

		[ContextMenu("User Manual")]
		protected override void OpenUserManual()
		{
			Application.OpenURL("http://www.root-motion.com/finalikdox/html/page8.html");
		}

		[ContextMenu("Scrpt Reference")]
		protected override void OpenScriptReference()
		{
			Application.OpenURL("http://www.root-motion.com/finalikdox/html/class_root_motion_1_1_final_i_k_1_1_look_at_i_k.html");
		}
	}
}
