using System;
using UnityEngine;

namespace RootMotion.FinalIK.Demos
{
	public class AimBoxing : MonoBehaviour
	{
		public AimIK aimIK;

		public Transform pin;

		private void LateUpdate()
		{
			this.aimIK.solver.transform.LookAt(this.pin.position);
			this.aimIK.solver.IKPosition = base.transform.position;
		}
	}
}
