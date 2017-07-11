using System;
using UnityEngine;

namespace RootMotion.FinalIK.Demos
{
	public class FPSAiming : MonoBehaviour
	{
		[Range(0f, 1f)]
		public float aimWeight = 1f;

		[Range(0f, 1f)]
		public float sightWeight = 1f;

		[Range(0f, 180f)]
		public float maxAngle = 80f;

		[SerializeField]
		private bool animatePhysics;

		[SerializeField]
		private Transform gun;

		[SerializeField]
		private Transform gunTarget;

		[SerializeField]
		private FullBodyBipedIK ik;

		[SerializeField]
		private AimIK gunAim;

		[SerializeField]
		private CameraControllerFPS cam;

		private Vector3 gunTargetDefaultLocalPosition;

		private Quaternion gunTargetDefaultLocalRotation;

		private Vector3 camDefaultLocalPosition;

		private Vector3 camRelativeToGunTarget;

		private bool updateFrame;

		private void Start()
		{
			this.gunTargetDefaultLocalPosition = this.gunTarget.localPosition;
			this.gunTargetDefaultLocalRotation = this.gunTarget.localRotation;
			this.camDefaultLocalPosition = this.cam.transform.localPosition;
			this.cam.enabled = false;
			this.gunAim.Disable();
			this.ik.Disable();
		}

		private void FixedUpdate()
		{
			this.updateFrame = true;
		}

		private void LateUpdate()
		{
			if (!this.animatePhysics)
			{
				this.updateFrame = true;
			}
			if (!this.updateFrame)
			{
				return;
			}
			this.updateFrame = false;
			this.cam.transform.localPosition = this.camDefaultLocalPosition;
			this.camRelativeToGunTarget = this.gunTarget.InverseTransformPoint(this.cam.transform.position);
			this.cam.LateUpdate();
			this.RotateCharacter();
			this.ik.solver.leftHandEffector.positionWeight = ((this.aimWeight <= 0f || this.sightWeight <= 0f) ? 0f : 1f);
			this.ik.solver.rightHandEffector.positionWeight = this.ik.solver.leftHandEffector.positionWeight;
			this.Aiming();
			this.LookDownTheSight();
		}

		private void Aiming()
		{
			if (this.aimWeight <= 0f)
			{
				return;
			}
			Quaternion rotation = this.cam.transform.rotation;
			this.gunAim.solver.IKPosition = this.cam.transform.position + this.cam.transform.forward * 10f;
			this.gunAim.solver.IKPositionWeight = this.aimWeight;
			this.gunAim.solver.Update();
			this.cam.transform.rotation = rotation;
		}

		private void LookDownTheSight()
		{
			float num = this.aimWeight * this.sightWeight;
			if (num <= 0f)
			{
				return;
			}
			this.gunTarget.position = Vector3.Lerp(this.gun.position, this.gunTarget.parent.TransformPoint(this.gunTargetDefaultLocalPosition), num);
			this.gunTarget.rotation = Quaternion.Lerp(this.gun.rotation, this.gunTarget.parent.rotation * this.gunTargetDefaultLocalRotation, num);
			Vector3 position = this.gun.InverseTransformPoint(this.ik.solver.leftHandEffector.bone.position);
			Vector3 position2 = this.gun.InverseTransformPoint(this.ik.solver.rightHandEffector.bone.position);
			Quaternion rhs = Quaternion.Inverse(this.gun.rotation) * this.ik.solver.leftHandEffector.bone.rotation;
			Quaternion rhs2 = Quaternion.Inverse(this.gun.rotation) * this.ik.solver.rightHandEffector.bone.rotation;
			this.ik.solver.leftHandEffector.position = this.gunTarget.TransformPoint(position);
			this.ik.solver.rightHandEffector.position = this.gunTarget.TransformPoint(position2);
			this.ik.solver.headMapping.maintainRotationWeight = 1f;
			this.ik.solver.Update();
			this.ik.solver.leftHandEffector.bone.rotation = this.gunTarget.rotation * rhs;
			this.ik.solver.rightHandEffector.bone.rotation = this.gunTarget.rotation * rhs2;
			this.cam.transform.position = Vector3.Lerp(this.cam.transform.position, this.gun.transform.TransformPoint(this.camRelativeToGunTarget), num);
		}

		private void RotateCharacter()
		{
			if (this.maxAngle >= 180f)
			{
				return;
			}
			if (this.maxAngle <= 0f)
			{
				base.transform.rotation = Quaternion.LookRotation(new Vector3(this.cam.transform.forward.x, 0f, this.cam.transform.forward.z));
				return;
			}
			Vector3 vector = base.transform.InverseTransformDirection(this.cam.transform.forward);
			float num = Mathf.Atan2(vector.x, vector.z) * 57.29578f;
			if (Mathf.Abs(num) > Mathf.Abs(this.maxAngle))
			{
				float angle = num - this.maxAngle;
				if (num < 0f)
				{
					angle = num + this.maxAngle;
				}
				base.transform.rotation = Quaternion.AngleAxis(angle, base.transform.up) * base.transform.rotation;
			}
		}
	}
}
