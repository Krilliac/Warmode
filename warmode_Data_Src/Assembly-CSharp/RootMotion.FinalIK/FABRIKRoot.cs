using System;
using UnityEngine;

namespace RootMotion.FinalIK
{
	[AddComponentMenu("Scripts/RootMotion.FinalIK/IK/FABRIK Root")]
	public class FABRIKRoot : IK
	{
		public IKSolverFABRIKRoot solver = new IKSolverFABRIKRoot();

		public override IKSolver GetIKSolver()
		{
			return this.solver;
		}

		[ContextMenu("User Manual")]
		protected override void OpenUserManual()
		{
			Application.OpenURL("http://www.root-motion.com/finalikdox/html/page5.html");
		}

		[ContextMenu("Scrpt Reference")]
		protected override void OpenScriptReference()
		{
			Application.OpenURL("http://www.root-motion.com/finalikdox/html/class_root_motion_1_1_final_i_k_1_1_f_a_b_r_i_k_root.html");
		}
	}
}
