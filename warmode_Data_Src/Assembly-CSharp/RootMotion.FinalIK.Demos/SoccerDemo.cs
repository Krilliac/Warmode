using System;
using System.Collections;
using System.Diagnostics;
using UnityEngine;

namespace RootMotion.FinalIK.Demos
{
	[RequireComponent(typeof(Animator))]
	public class SoccerDemo : MonoBehaviour
	{
		private Animator animator;

		private Vector3 defaultPosition;

		private Quaternion defaultRotation;

		private void Start()
		{
			this.animator = base.GetComponent<Animator>();
			this.defaultPosition = base.transform.position;
			this.defaultRotation = base.transform.rotation;
			base.StartCoroutine(this.ResetDelayed());
		}

		[DebuggerHidden]
		private IEnumerator ResetDelayed()
		{
			SoccerDemo.<ResetDelayed>c__Iterator1D <ResetDelayed>c__Iterator1D = new SoccerDemo.<ResetDelayed>c__Iterator1D();
			<ResetDelayed>c__Iterator1D.<>f__this = this;
			return <ResetDelayed>c__Iterator1D;
		}
	}
}
