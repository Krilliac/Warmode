using System;
using UnityEngine;

namespace RootMotion.FinalIK.Demos
{
	public class KissingRig : MonoBehaviour
	{
		[Serializable]
		public class Partner
		{
			public FullBodyBipedIK ik;

			public Transform mouth;

			public Transform mouthTarget;

			public Transform touchTargetLeftHand;

			public Transform touchTargetRightHand;

			public float bodyWeightHorizontal = 0.4f;

			public float bodyWeightVertical = 1f;

			public float neckRotationWeight = 0.3f;

			public float headTiltAngle = 10f;

			public Vector3 headTiltAxis;

			private Quaternion neckRotation;

			private Transform neck
			{
				get
				{
					return this.ik.solver.spineMapping.spineBones[this.ik.solver.spineMapping.spineBones.Length - 1];
				}
			}

			public void Initiate()
			{
				this.ik.Disable();
			}

			public void Update(float weight)
			{
				this.ik.solver.leftShoulderEffector.positionWeight = weight;
				this.ik.solver.rightShoulderEffector.positionWeight = weight;
				this.ik.solver.leftHandEffector.positionWeight = weight;
				this.ik.solver.rightHandEffector.positionWeight = weight;
				this.ik.solver.leftHandEffector.rotationWeight = weight;
				this.ik.solver.rightHandEffector.rotationWeight = weight;
				this.ik.solver.bodyEffector.positionWeight = weight;
				this.InverseTransformEffector(FullBodyBipedEffector.LeftShoulder, this.mouth, this.mouthTarget.position, weight);
				this.InverseTransformEffector(FullBodyBipedEffector.RightShoulder, this.mouth, this.mouthTarget.position, weight);
				this.InverseTransformEffector(FullBodyBipedEffector.Body, this.mouth, this.mouthTarget.position, weight);
				this.ik.solver.bodyEffector.position = Vector3.Lerp(new Vector3(this.ik.solver.bodyEffector.position.x, this.ik.solver.bodyEffector.bone.position.y, this.ik.solver.bodyEffector.position.z), this.ik.solver.bodyEffector.position, this.bodyWeightVertical * weight);
				this.ik.solver.bodyEffector.position = Vector3.Lerp(new Vector3(this.ik.solver.bodyEffector.bone.position.x, this.ik.solver.bodyEffector.position.y, this.ik.solver.bodyEffector.bone.position.z), this.ik.solver.bodyEffector.position, this.bodyWeightHorizontal * weight);
				this.ik.solver.leftHandEffector.position = this.touchTargetLeftHand.position;
				this.ik.solver.rightHandEffector.position = this.touchTargetRightHand.position;
				this.ik.solver.leftHandEffector.rotation = this.touchTargetLeftHand.rotation;
				this.ik.solver.rightHandEffector.rotation = this.touchTargetRightHand.rotation;
				this.neckRotation = this.neck.rotation;
				this.ik.solver.Update();
				this.neck.rotation = Quaternion.Slerp(this.neck.rotation, this.neckRotation, this.neckRotationWeight * weight);
				this.ik.references.head.localRotation = Quaternion.AngleAxis(this.headTiltAngle * weight, this.headTiltAxis) * this.ik.references.head.localRotation;
			}

			private void InverseTransformEffector(FullBodyBipedEffector effector, Transform target, Vector3 targetPosition, float weight)
			{
				Vector3 b = this.ik.solver.GetEffector(effector).bone.position - target.position;
				this.ik.solver.GetEffector(effector).position = Vector3.Lerp(this.ik.solver.GetEffector(effector).bone.position, targetPosition + b, weight);
			}
		}

		public KissingRig.Partner partner1;

		public KissingRig.Partner partner2;

		public float weight;

		public int iterations = 3;

		private void Start()
		{
			this.partner1.Initiate();
			this.partner2.Initiate();
		}

		private void LateUpdate()
		{
			for (int i = 0; i < this.iterations; i++)
			{
				this.partner1.Update(this.weight);
				this.partner2.Update(this.weight);
			}
		}
	}
}
