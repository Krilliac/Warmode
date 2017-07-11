using System;
using UnityEngine;

namespace RootMotion.FinalIK.Demos
{
	public class UserControlThirdPerson : MonoBehaviour
	{
		public struct State
		{
			public Vector3 move;

			public Vector3 lookPos;

			public bool crouch;

			public bool jump;
		}

		[SerializeField]
		private bool walkByDefault;

		[SerializeField]
		private bool canCrouch = true;

		[SerializeField]
		private bool canJump = true;

		public UserControlThirdPerson.State state = default(UserControlThirdPerson.State);

		protected Transform cam;

		private void Start()
		{
			this.cam = Camera.main.transform;
		}

		protected virtual void Update()
		{
			this.state.crouch = (this.canCrouch && Input.GetKey(KeyCode.C));
			this.state.jump = (this.canJump && Input.GetButton("Jump"));
			float axisRaw = Input.GetAxisRaw("Horizontal");
			float axisRaw2 = Input.GetAxisRaw("Vertical");
			Vector3 normalized = Vector3.Scale(this.cam.forward, new Vector3(1f, 0f, 1f)).normalized;
			this.state.move = (axisRaw2 * normalized + axisRaw * this.cam.right).normalized;
			bool key = Input.GetKey(KeyCode.LeftShift);
			float d = (!this.walkByDefault) ? ((!key) ? 1f : 0.5f) : ((!key) ? 0.5f : 1f);
			this.state.move = this.state.move * d;
			this.state.lookPos = base.transform.position + this.cam.forward * 100f;
		}
	}
}
