using System;
using UnityEngine;

namespace RootMotion.FinalIK.Demos
{
	[RequireComponent(typeof(FullBodyBipedIK)), RequireComponent(typeof(AimIK))]
	public class AnimatorController3rdPersonIK : AnimatorController3rdPerson
	{
		[SerializeField]
		private bool useIK = true;

		[SerializeField]
		private Transform rightHandTarget;

		[SerializeField]
		private Transform leftHandTarget;

		[SerializeField]
		private Transform head;

		[SerializeField]
		private Vector3 headLookAxis = Vector3.forward;

		[SerializeField]
		private float headLookWeight = 1f;

		[SerializeField]
		private Camera firstPersonCam;

		private AimIK aim;

		private FullBodyBipedIK ik;

		private Quaternion rightHandRotation;

		private Quaternion fpsCamDefaultRot;

		private IKEffector leftHand
		{
			get
			{
				return this.ik.solver.leftHandEffector;
			}
		}

		private IKEffector rightHand
		{
			get
			{
				return this.ik.solver.rightHandEffector;
			}
		}

		private void OnGUI()
		{
			GUILayout.Label("Press F to switch Final IK on/off", new GUILayoutOption[0]);
			GUILayout.Label("Press C to toggle between 3rd person/1st person camera", new GUILayoutOption[0]);
		}

		protected override void Start()
		{
			base.Start();
			this.aim = base.GetComponent<AimIK>();
			this.ik = base.GetComponent<FullBodyBipedIK>();
			this.aim.Disable();
			this.ik.Disable();
			this.fpsCamDefaultRot = this.firstPersonCam.transform.localRotation;
		}

		public override void Move(Vector3 moveInput, bool isMoving, Vector3 faceDirection, Vector3 aimTarget)
		{
			base.Move(moveInput, isMoving, faceDirection, aimTarget);
			if (Input.GetKeyDown(KeyCode.C))
			{
				this.firstPersonCam.enabled = !this.firstPersonCam.enabled;
			}
			if (Input.GetKeyDown(KeyCode.F))
			{
				this.useIK = !this.useIK;
			}
			if (!this.useIK)
			{
				return;
			}
			this.aim.solver.IKPosition = aimTarget;
			this.FBBIKPass1();
			this.aim.solver.Update();
			this.FBBIKPass2();
			this.HeadLookAt(aimTarget);
			if (this.firstPersonCam.enabled)
			{
				this.StabilizeFPSCamera();
			}
		}

		private void FBBIKPass1()
		{
			this.rightHandRotation = this.rightHandTarget.rotation;
			this.rightHand.position = this.rightHandTarget.position;
			this.rightHand.positionWeight = 1f;
			this.leftHand.positionWeight = 0f;
			this.ik.solver.Update();
			this.rightHand.bone.rotation = this.rightHandRotation;
		}

		private void FBBIKPass2()
		{
			this.rightHand.position = this.rightHand.bone.position;
			this.rightHandRotation = this.rightHand.bone.rotation;
			this.leftHand.position = this.leftHandTarget.position;
			this.leftHand.positionWeight = 1f;
			this.ik.solver.Update();
			this.rightHand.bone.rotation = this.rightHandRotation;
			this.leftHand.bone.rotation = this.leftHandTarget.rotation;
		}

		private void HeadLookAt(Vector3 lookAtTarget)
		{
			if (this.head == null)
			{
				return;
			}
			Quaternion b = Quaternion.FromToRotation(this.head.rotation * this.headLookAxis, lookAtTarget - this.head.position);
			this.head.rotation = Quaternion.Lerp(Quaternion.identity, b, this.headLookWeight) * this.head.rotation;
		}

		private void StabilizeFPSCamera()
		{
			this.firstPersonCam.transform.localRotation = this.fpsCamDefaultRot;
			Vector3 forward = this.firstPersonCam.transform.forward;
			Vector3 up = this.firstPersonCam.transform.up;
			Vector3.OrthoNormalize(ref forward, ref up);
			forward = this.firstPersonCam.transform.forward;
			Vector3 up2 = Vector3.up;
			Vector3.OrthoNormalize(ref forward, ref up2);
			Quaternion b = Quaternion.FromToRotation(up, up2);
			float t = Vector3.Dot(base.transform.forward, this.firstPersonCam.transform.forward);
			this.firstPersonCam.transform.rotation = Quaternion.Lerp(Quaternion.identity, b, t) * this.firstPersonCam.transform.rotation;
		}
	}
}
