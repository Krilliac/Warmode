using System;
using System.Collections;
using System.Diagnostics;
using UnityEngine;

namespace RootMotion.FinalIK.Demos
{
	public class MotionAbsorb : MonoBehaviour
	{
		[Serializable]
		public class Absorber
		{
			[Tooltip("The type of effector (hand, foot, shoulder...) - this is just an enum")]
			public FullBodyBipedEffector effector;

			[Tooltip("How much should motion be absorbed on this effector")]
			public float weight = 1f;

			public void SetToBone(IKSolverFullBodyBiped solver)
			{
				solver.GetEffector(this.effector).position = solver.GetEffector(this.effector).bone.position;
				solver.GetEffector(this.effector).rotation = solver.GetEffector(this.effector).bone.rotation;
			}

			public void SetEffectorWeights(IKSolverFullBodyBiped solver, float w)
			{
				solver.GetEffector(this.effector).positionWeight = w * this.weight;
				solver.GetEffector(this.effector).rotationWeight = w * this.weight;
			}
		}

		[Tooltip("Reference to the FBBIK component")]
		public FullBodyBipedIK ik;

		[Tooltip("Array containing the absorbers")]
		public MotionAbsorb.Absorber[] absorbers;

		[Tooltip("The master weight")]
		public float weight = 1f;

		[Tooltip("Weight falloff curve (how fast will the effect reduce after impact)")]
		public AnimationCurve falloff;

		[Tooltip("How fast will the impact fade away. (if 1, effect lasts for 1 second)")]
		public float falloffSpeed = 1f;

		private float timer;

		private void OnCollisionEnter()
		{
			UnityEngine.Debug.Log("collisionenter");
			if (this.timer > 0f)
			{
				return;
			}
			base.StartCoroutine(this.AbsorbMotion());
		}

		[DebuggerHidden]
		private IEnumerator AbsorbMotion()
		{
			MotionAbsorb.<AbsorbMotion>c__Iterator1B <AbsorbMotion>c__Iterator1B = new MotionAbsorb.<AbsorbMotion>c__Iterator1B();
			<AbsorbMotion>c__Iterator1B.<>f__this = this;
			return <AbsorbMotion>c__Iterator1B;
		}
	}
}
