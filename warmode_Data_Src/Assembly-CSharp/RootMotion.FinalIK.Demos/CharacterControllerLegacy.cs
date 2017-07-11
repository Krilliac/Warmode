using System;
using UnityEngine;

namespace RootMotion.FinalIK.Demos
{
	public class CharacterControllerLegacy : CharacterControllerDefault
	{
		public AnimationCurve animationSpeedRelativeToVelocity;

		protected override void Update()
		{
			base.Update();
			base.GetComponent<Animation>().CrossFade(this.state.clipName, 0.3f, PlayMode.StopSameLayer);
			base.GetComponent<Animation>()[this.state.clipName].speed = this.animationSpeedRelativeToVelocity.Evaluate(this.moveVector.magnitude) * this.state.animationSpeed;
		}
	}
}
