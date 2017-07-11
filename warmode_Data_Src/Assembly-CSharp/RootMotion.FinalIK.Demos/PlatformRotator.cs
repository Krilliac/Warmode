using System;
using System.Collections;
using System.Diagnostics;
using UnityEngine;

namespace RootMotion.FinalIK.Demos
{
	public class PlatformRotator : MonoBehaviour
	{
		public float maxAngle = 70f;

		public float switchRotationTime = 0.5f;

		public float random = 0.5f;

		public float rotationSpeed = 50f;

		public Vector3 movePosition;

		public float moveSpeed = 5f;

		private Quaternion defaultRotation;

		private Quaternion targetRotation;

		private Vector3 targetPosition;

		private Vector3 velocity;

		private void Start()
		{
			this.defaultRotation = base.transform.rotation;
			this.targetPosition = base.transform.position + this.movePosition;
			base.StartCoroutine(this.SwitchRotation());
		}

		private void FixedUpdate()
		{
			base.GetComponent<Rigidbody>().MovePosition(Vector3.SmoothDamp(base.GetComponent<Rigidbody>().position, this.targetPosition, ref this.velocity, 1f, this.moveSpeed));
			if (Vector3.Distance(base.GetComponent<Rigidbody>().position, this.targetPosition) < 0.1f)
			{
				this.movePosition = -this.movePosition;
				this.targetPosition += this.movePosition;
			}
			base.GetComponent<Rigidbody>().rotation = Quaternion.RotateTowards(base.GetComponent<Rigidbody>().rotation, this.targetRotation, this.rotationSpeed * Time.deltaTime);
		}

		[DebuggerHidden]
		private IEnumerator SwitchRotation()
		{
			PlatformRotator.<SwitchRotation>c__Iterator1E <SwitchRotation>c__Iterator1E = new PlatformRotator.<SwitchRotation>c__Iterator1E();
			<SwitchRotation>c__Iterator1E.<>f__this = this;
			return <SwitchRotation>c__Iterator1E;
		}
	}
}
