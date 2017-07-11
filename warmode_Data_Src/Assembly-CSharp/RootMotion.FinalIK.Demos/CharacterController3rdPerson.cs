using System;
using UnityEngine;

namespace RootMotion.FinalIK.Demos
{
	[RequireComponent(typeof(AnimatorController3rdPerson))]
	public class CharacterController3rdPerson : MonoBehaviour
	{
		[SerializeField]
		private CameraController cam;

		private AnimatorController3rdPerson animatorController;

		private static Vector3 inputVector
		{
			get
			{
				return new Vector3(Input.GetAxis("Horizontal"), 0f, Input.GetAxis("Vertical"));
			}
		}

		private static Vector3 inputVectorRaw
		{
			get
			{
				return new Vector3(Input.GetAxisRaw("Horizontal"), 0f, Input.GetAxisRaw("Vertical"));
			}
		}

		private void Start()
		{
			this.animatorController = base.GetComponent<AnimatorController3rdPerson>();
			this.cam.enabled = false;
		}

		private void LateUpdate()
		{
			this.cam.UpdateInput();
			this.cam.UpdateTransform();
			Vector3 inputVector = CharacterController3rdPerson.inputVector;
			bool isMoving = CharacterController3rdPerson.inputVector != Vector3.zero || CharacterController3rdPerson.inputVectorRaw != Vector3.zero;
			Vector3 forward = this.cam.transform.forward;
			Vector3 aimTarget = this.cam.transform.position + forward * 10f;
			this.animatorController.Move(inputVector, isMoving, forward, aimTarget);
		}
	}
}
