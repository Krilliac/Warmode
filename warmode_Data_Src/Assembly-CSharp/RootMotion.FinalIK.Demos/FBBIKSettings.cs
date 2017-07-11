using System;
using UnityEngine;

namespace RootMotion.FinalIK.Demos
{
	public class FBBIKSettings : MonoBehaviour
	{
		[Serializable]
		public class Limb
		{
			public FBIKChain.Smoothing reachSmoothing;

			public float maintainRelativePositionWeight;

			public float mappingWeight = 1f;

			public void Apply(FullBodyBipedChain chain, IKSolverFullBodyBiped solver)
			{
				solver.GetChain(chain).reachSmoothing = this.reachSmoothing;
				solver.GetEndEffector(chain).maintainRelativePositionWeight = this.maintainRelativePositionWeight;
				solver.GetLimbMapping(chain).weight = this.mappingWeight;
			}
		}

		public FullBodyBipedIK ik;

		public bool disableAfterStart;

		public FBBIKSettings.Limb leftArm;

		public FBBIKSettings.Limb rightArm;

		public FBBIKSettings.Limb leftLeg;

		public FBBIKSettings.Limb rightLeg;

		public float rootPin;

		public bool bodyEffectChildNodes = true;

		public void UpdateSettings()
		{
			if (this.ik == null)
			{
				return;
			}
			this.leftArm.Apply(FullBodyBipedChain.LeftArm, this.ik.solver);
			this.rightArm.Apply(FullBodyBipedChain.RightArm, this.ik.solver);
			this.leftLeg.Apply(FullBodyBipedChain.LeftLeg, this.ik.solver);
			this.rightLeg.Apply(FullBodyBipedChain.RightLeg, this.ik.solver);
			this.ik.solver.chain[0].pin = this.rootPin;
			this.ik.solver.bodyEffector.effectChildNodes = this.bodyEffectChildNodes;
		}

		private void Start()
		{
			Debug.Log("FBBIKSettings is deprecated, you can now edit all the settings from the custom inspector of the FullBodyBipedIK component.");
			this.UpdateSettings();
			if (this.disableAfterStart)
			{
				base.enabled = false;
			}
		}

		private void Update()
		{
			this.UpdateSettings();
		}
	}
}
