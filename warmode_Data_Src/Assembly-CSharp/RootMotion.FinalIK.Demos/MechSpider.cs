using System;
using UnityEngine;

namespace RootMotion.FinalIK.Demos
{
	public class MechSpider : MonoBehaviour
	{
		public LayerMask raycastLayers;

		public Transform body;

		public MechSpiderLeg[] legs;

		public float legRotationWeight = 1f;

		public float rootPositionSpeed = 5f;

		public float rootRotationSpeed = 30f;

		public float breatheSpeed = 2f;

		public float breatheMagnitude = 0.2f;

		public float height = 3.5f;

		public float minHeight = 2f;

		public float raycastHeight = 10f;

		public float raycastDistance = 5f;

		private Vector3 lastPosition;

		private Vector3 defaultBodyLocalPosition;

		private float sine;

		private RaycastHit rootHit;

		private void Update()
		{
			Vector3 legsPlaneNormal = this.GetLegsPlaneNormal();
			Quaternion lhs = Quaternion.FromToRotation(base.transform.up, legsPlaneNormal);
			base.transform.rotation = Quaternion.Slerp(base.transform.rotation, lhs * base.transform.rotation, Time.deltaTime * this.rootRotationSpeed);
			Vector3 legCentroid = this.GetLegCentroid();
			Vector3 a = Vector3.Project(legCentroid + base.transform.up * this.height - base.transform.position, base.transform.up);
			base.transform.position += a * Time.deltaTime * this.rootPositionSpeed;
			if (Physics.Raycast(base.transform.position + base.transform.up * this.raycastHeight, -base.transform.up, out this.rootHit, this.raycastHeight + this.raycastDistance, this.raycastLayers))
			{
				this.rootHit.distance = this.rootHit.distance - (this.raycastHeight + 2f);
				if (this.rootHit.distance < 0f)
				{
					Vector3 b = base.transform.position - base.transform.up * this.rootHit.distance;
					base.transform.position = Vector3.Lerp(base.transform.position, b, Time.deltaTime * this.rootPositionSpeed);
				}
			}
			this.sine += Time.deltaTime * this.breatheSpeed;
			if (this.sine >= 6.28318548f)
			{
				this.sine -= 6.28318548f;
			}
			float d = Mathf.Sin(this.sine) * this.breatheMagnitude;
			Vector3 b2 = base.transform.up * d;
			this.body.transform.position = base.transform.position + b2;
		}

		private Vector3 GetLegCentroid()
		{
			Vector3 vector = Vector3.zero;
			float d = 1f / (float)this.legs.Length;
			for (int i = 0; i < this.legs.Length; i++)
			{
				vector += this.legs[i].position * d;
			}
			return vector;
		}

		private Vector3 GetLegsPlaneNormal()
		{
			Vector3 vector = base.transform.up;
			if (this.legRotationWeight <= 0f)
			{
				return vector;
			}
			float t = 1f / Mathf.Lerp((float)this.legs.Length, 1f, this.legRotationWeight);
			for (int i = 0; i < this.legs.Length; i++)
			{
				Vector3 vector2 = this.legs[i].position - (base.transform.position - base.transform.up * this.height);
				Vector3 up = base.transform.up;
				Vector3 fromDirection = vector2;
				Vector3.OrthoNormalize(ref up, ref fromDirection);
				Quaternion quaternion = Quaternion.FromToRotation(fromDirection, vector2);
				quaternion = Quaternion.Lerp(Quaternion.identity, quaternion, t);
				vector = quaternion * vector;
			}
			return vector;
		}
	}
}
