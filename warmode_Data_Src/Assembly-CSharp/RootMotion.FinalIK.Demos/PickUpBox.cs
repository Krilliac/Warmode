using System;
using UnityEngine;

namespace RootMotion.FinalIK.Demos
{
	public class PickUpBox : PickUp2Handed
	{
		protected override void RotatePivot()
		{
			Vector3 normalized = (this.pivot.position - this.interactionSystem.transform.position).normalized;
			normalized.y = 0f;
			Vector3 v = this.obj.transform.InverseTransformDirection(normalized);
			Vector3 axis = QuaTools.GetAxis(v);
			Vector3 axis2 = QuaTools.GetAxis(this.obj.transform.InverseTransformDirection(this.interactionSystem.transform.up));
			this.pivot.localRotation = Quaternion.LookRotation(axis, axis2);
		}
	}
}
