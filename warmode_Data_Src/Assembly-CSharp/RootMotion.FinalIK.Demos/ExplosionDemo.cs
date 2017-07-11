using System;
using UnityEngine;

namespace RootMotion.FinalIK.Demos
{
	public class ExplosionDemo : MonoBehaviour
	{
		public FullBodyBipedIK ik;

		public Rigidbody rigidbody;

		public float forceMlp = 1f;

		public float upForce = 1f;

		public float weightFalloffSpeed = 1f;

		public AnimationCurve weightFalloff;

		public AnimationCurve explosionForceByDistance;

		public AnimationCurve scale;

		private float weight;

		private Vector3 defaultScale = Vector3.one;

		private void Start()
		{
			this.defaultScale = base.transform.localScale;
		}

		private void Update()
		{
			this.weight = Mathf.Clamp(this.weight - Time.deltaTime * this.weightFalloffSpeed, 0f, 1f);
			if (Input.GetKeyDown(KeyCode.E))
			{
				this.ik.solver.IKPositionWeight = 1f;
				this.ik.solver.leftHandEffector.position = this.ik.solver.leftHandEffector.bone.position;
				this.ik.solver.rightHandEffector.position = this.ik.solver.rightHandEffector.bone.position;
				this.ik.solver.leftFootEffector.position = this.ik.solver.leftFootEffector.bone.position;
				this.ik.solver.rightFootEffector.position = this.ik.solver.rightFootEffector.bone.position;
				this.weight = 1f;
				Vector3 vector = this.rigidbody.position - base.transform.position;
				float d = this.explosionForceByDistance.Evaluate(vector.magnitude);
				this.rigidbody.AddForce((vector.normalized + Vector3.up * this.upForce) * d * this.forceMlp, ForceMode.VelocityChange);
			}
			this.SetEffectorWeights(this.weightFalloff.Evaluate(this.weight));
			base.transform.localScale = this.scale.Evaluate(this.weight) * this.defaultScale;
		}

		private void SetEffectorWeights(float w)
		{
			this.ik.solver.leftHandEffector.positionWeight = w;
			this.ik.solver.rightHandEffector.positionWeight = w;
			this.ik.solver.leftFootEffector.positionWeight = w;
			this.ik.solver.rightFootEffector.positionWeight = w;
		}
	}
}
