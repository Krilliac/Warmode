using System;
using UnityEngine;

namespace RootMotion.FinalIK.Demos
{
	[RequireComponent(typeof(FullBodyBipedIK))]
	public class PendulumExample : MonoBehaviour
	{
		[SerializeField]
		private Transform target;

		[SerializeField]
		private Transform leftHandTarget;

		[SerializeField]
		private Transform rightHandTarget;

		[SerializeField]
		private Transform leftFootTarget;

		[SerializeField]
		private Transform rightFootTarget;

		[SerializeField]
		private Transform pelvisTarget;

		[SerializeField]
		private Transform bodyTarget;

		[SerializeField]
		private Transform headTarget;

		[SerializeField]
		private Vector3 pelvisDownAxis = Vector3.right;

		public float hangingDistanceMlp = 1.3f;

		private FullBodyBipedIK ik;

		private Quaternion rootRelativeToPelvis;

		private Vector3 pelvisToRoot;

		private void Start()
		{
			this.ik = base.GetComponent<FullBodyBipedIK>();
			Quaternion rotation = this.target.rotation;
			this.target.rotation = this.leftHandTarget.rotation;
			FixedJoint fixedJoint = this.target.gameObject.AddComponent<FixedJoint>();
			fixedJoint.connectedBody = this.leftHandTarget.GetComponent<Rigidbody>();
			this.target.rotation = rotation;
			this.rootRelativeToPelvis = Quaternion.Inverse(this.pelvisTarget.rotation) * base.transform.rotation;
			this.pelvisToRoot = Quaternion.Inverse(this.ik.references.pelvis.rotation) * (base.transform.position - this.ik.references.pelvis.position);
			this.ik.solver.leftHandEffector.positionWeight = 1f;
			this.ik.solver.leftHandEffector.rotationWeight = 1f;
		}

		private void LateUpdate()
		{
			base.transform.rotation = this.pelvisTarget.rotation * this.rootRelativeToPelvis;
			base.transform.position = this.pelvisTarget.position + this.pelvisTarget.rotation * this.pelvisToRoot * this.hangingDistanceMlp;
			this.ik.solver.leftHandEffector.position = this.leftHandTarget.position;
			this.ik.solver.leftHandEffector.rotation = this.leftHandTarget.rotation;
			Vector3 fromDirection = this.ik.references.pelvis.rotation * this.pelvisDownAxis;
			Quaternion lhs = Quaternion.FromToRotation(fromDirection, this.rightHandTarget.position - this.headTarget.position);
			this.ik.references.rightUpperArm.rotation = lhs * this.ik.references.rightUpperArm.rotation;
			Quaternion lhs2 = Quaternion.FromToRotation(fromDirection, this.leftFootTarget.position - this.bodyTarget.position);
			this.ik.references.leftThigh.rotation = lhs2 * this.ik.references.leftThigh.rotation;
			Quaternion lhs3 = Quaternion.FromToRotation(fromDirection, this.rightFootTarget.position - this.bodyTarget.position);
			this.ik.references.rightThigh.rotation = lhs3 * this.ik.references.rightThigh.rotation;
		}
	}
}
