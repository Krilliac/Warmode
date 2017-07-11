using System;
using UnityEngine;

namespace RootMotion.FinalIK.Demos
{
	public class FPSCharacter : MonoBehaviour
	{
		[SerializeField]
		private AnimationClip aimAnim;

		[SerializeField]
		private Transform mixingTransformRecursive;

		[SerializeField]
		private FPSAiming FPSAiming;

		private float sVel;

		private void Start()
		{
			base.GetComponent<Animation>()[this.aimAnim.name].AddMixingTransform(this.mixingTransformRecursive);
			base.GetComponent<Animation>()[this.aimAnim.name].layer = 1;
			base.GetComponent<Animation>().Play(this.aimAnim.name);
		}

		private void Update()
		{
			this.FPSAiming.sightWeight = Mathf.SmoothDamp(this.FPSAiming.sightWeight, (!Input.GetMouseButton(1)) ? 0f : 1f, ref this.sVel, 0.1f);
			if (this.FPSAiming.sightWeight < 0.001f)
			{
				this.FPSAiming.sightWeight = 0f;
			}
			if (this.FPSAiming.sightWeight > 0.999f)
			{
				this.FPSAiming.sightWeight = 1f;
			}
		}

		private void OnGUI()
		{
			GUI.Label(new Rect((float)(Screen.width - 210), 10f, 200f, 25f), "Hold RMB to aim down the sight");
		}
	}
}
