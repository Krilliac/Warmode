using System;
using UnityEngine;

namespace RootMotion.FinalIK.Demos
{
	public class PickUpSphere : PickUp2Handed
	{
		protected override void RotatePivot()
		{
			Vector3 b = Vector3.Lerp(this.interactionSystem.ik.solver.leftHandEffector.bone.position, this.interactionSystem.ik.solver.rightHandEffector.bone.position, 0.5f);
			Vector3 forward = this.obj.transform.position - b;
			this.pivot.rotation = Quaternion.LookRotation(forward);
		}
	}
}
