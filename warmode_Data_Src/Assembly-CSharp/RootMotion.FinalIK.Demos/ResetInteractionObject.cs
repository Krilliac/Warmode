using System;
using System.Collections;
using System.Diagnostics;
using UnityEngine;

namespace RootMotion.FinalIK.Demos
{
	public class ResetInteractionObject : MonoBehaviour
	{
		public float resetDelay = 1f;

		private Vector3 defaultPosition;

		private Quaternion defaultRotation;

		private Transform defaultParent;

		private void Start()
		{
			this.defaultPosition = base.transform.position;
			this.defaultRotation = base.transform.rotation;
			this.defaultParent = base.transform.parent;
		}

		private void OnPickUp(Transform t)
		{
			base.StopAllCoroutines();
			base.StartCoroutine(this.ResetObject(Time.time + this.resetDelay));
		}

		[DebuggerHidden]
		private IEnumerator ResetObject(float resetTime)
		{
			ResetInteractionObject.<ResetObject>c__Iterator1C <ResetObject>c__Iterator1C = new ResetInteractionObject.<ResetObject>c__Iterator1C();
			<ResetObject>c__Iterator1C.resetTime = resetTime;
			<ResetObject>c__Iterator1C.<$>resetTime = resetTime;
			<ResetObject>c__Iterator1C.<>f__this = this;
			return <ResetObject>c__Iterator1C;
		}
	}
}
