using System;
using UnityEngine;

namespace RootMotion.FinalIK.Demos
{
	[RequireComponent(typeof(Rigidbody)), RequireComponent(typeof(CapsuleCollider))]
	public abstract class CharacterBase : MonoBehaviour
	{
		protected const float half = 0.5f;

		[SerializeField]
		protected float airborneThreshold = 0.6f;

		[SerializeField]
		private float slopeStartAngle = 50f;

		[SerializeField]
		private float slopeEndAngle = 85f;

		[SerializeField]
		private float spherecastRadius = 0.1f;

		[SerializeField]
		private LayerMask groundLayers;

		[SerializeField]
		private PhysicMaterial zeroFrictionMaterial;

		[SerializeField]
		private PhysicMaterial highFrictionMaterial;

		protected float originalHeight;

		protected Vector3 originalCenter;

		protected CapsuleCollider capsule;

		public abstract void Move(Vector3 deltaPosition);

		protected virtual void Start()
		{
			this.capsule = (base.GetComponent<Collider>() as CapsuleCollider);
			this.originalHeight = this.capsule.height;
			this.originalCenter = this.capsule.center;
			base.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeRotation;
		}

		protected virtual RaycastHit GetSpherecastHit()
		{
			Ray ray = new Ray(base.GetComponent<Rigidbody>().position + Vector3.up * this.airborneThreshold, Vector3.down);
			RaycastHit result = default(RaycastHit);
			Physics.SphereCast(ray, this.spherecastRadius, out result, this.airborneThreshold * 2f, this.groundLayers);
			return result;
		}

		public float GetAngleFromForward(Vector3 worldDirection)
		{
			Vector3 vector = base.transform.InverseTransformDirection(worldDirection);
			return Mathf.Atan2(vector.x, vector.z) * 57.29578f;
		}

		protected void RigidbodyRotateAround(Vector3 point, Vector3 axis, float angle)
		{
			Quaternion quaternion = Quaternion.AngleAxis(angle, axis);
			Vector3 point2 = base.GetComponent<Rigidbody>().position - point;
			base.GetComponent<Rigidbody>().MovePosition(point + quaternion * point2);
			base.GetComponent<Rigidbody>().MoveRotation(quaternion * base.GetComponent<Rigidbody>().rotation);
		}

		protected void ScaleCapsule(float mlp)
		{
			if (this.capsule.height != this.originalHeight * mlp)
			{
				this.capsule.height = Mathf.MoveTowards(this.capsule.height, this.originalHeight * mlp, Time.deltaTime * 4f);
				this.capsule.center = Vector3.MoveTowards(this.capsule.center, this.originalCenter * mlp, Time.deltaTime * 2f);
			}
		}

		protected void HighFriction()
		{
			base.GetComponent<Collider>().material = this.highFrictionMaterial;
		}

		protected void ZeroFriction()
		{
			base.GetComponent<Collider>().material = this.zeroFrictionMaterial;
		}

		protected float GetSlopeDamper(Vector3 velocity, Vector3 groundNormal)
		{
			float num = 90f - Vector3.Angle(velocity, groundNormal);
			num -= this.slopeStartAngle;
			float num2 = this.slopeEndAngle - this.slopeStartAngle;
			return 1f - Mathf.Clamp(num / num2, 0f, 1f);
		}
	}
}
