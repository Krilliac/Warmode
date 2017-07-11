using System;
using UnityEngine;

namespace RootMotion.FinalIK
{
	[AddComponentMenu("Scripts/RootMotion.FinalIK/IK/Limb IK")]
	public class LimbIK : IK
	{
		public IKSolverLimb solver = new IKSolverLimb();

		public override IKSolver GetIKSolver()
		{
			return this.solver;
		}

		[ContextMenu("User Manual")]
		protected override void OpenUserManual()
		{
			Application.OpenURL("http://www.root-motion.com/finalikdox/html/page7.html");
		}

		[ContextMenu("Scrpt Reference")]
		protected override void OpenScriptReference()
		{
			Application.OpenURL("http://www.root-motion.com/finalikdox/html/class_root_motion_1_1_final_i_k_1_1_limb_i_k.html");
		}
	}
}
