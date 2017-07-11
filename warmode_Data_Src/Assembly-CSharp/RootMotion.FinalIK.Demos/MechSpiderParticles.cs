using System;
using UnityEngine;

namespace RootMotion.FinalIK.Demos
{
	public class MechSpiderParticles : MonoBehaviour
	{
		public MechSpiderController mechSpiderController;

		private ParticleSystem particles;

		private void Start()
		{
			this.particles = (ParticleSystem)base.GetComponent(typeof(ParticleSystem));
		}

		private void Update()
		{
			float magnitude = this.mechSpiderController.inputVector.magnitude;
			this.particles.emissionRate = Mathf.Clamp(magnitude * 50f, 30f, 50f);
			this.particles.startColor = new Color(this.particles.startColor.r, this.particles.startColor.g, this.particles.startColor.b, Mathf.Clamp(magnitude, 0.4f, 1f));
		}
	}
}
