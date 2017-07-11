using System;
using UnityEngine;

namespace RootMotion.FinalIK.Demos
{
	public class TerrainOffset : MonoBehaviour
	{
		public AimIK aimIK;

		public Vector3 raycastOffset = new Vector3(0f, 2f, 1.5f);

		public LayerMask raycastLayers;

		public float min = -2f;

		public float max = 2f;

		public float lerpSpeed = 10f;

		private RaycastHit hit;

		private Vector3 offset;

		private void LateUpdate()
		{
			Vector3 b = base.transform.rotation * this.raycastOffset;
			Vector3 groundHeightOffset = this.GetGroundHeightOffset(base.transform.position + b);
			this.offset = Vector3.Lerp(this.offset, groundHeightOffset, Time.deltaTime * this.lerpSpeed);
			Vector3 vector = base.transform.position + new Vector3(b.x, 0f, b.z);
			this.aimIK.solver.transform.LookAt(vector);
			this.aimIK.solver.IKPosition = vector + this.offset;
		}

		private Vector3 GetGroundHeightOffset(Vector3 worldPosition)
		{
			Debug.DrawRay(worldPosition, Vector3.down * this.raycastOffset.y * 2f, Color.green);
			if (Physics.Raycast(worldPosition, Vector3.down, out this.hit, this.raycastOffset.y * 2f))
			{
				return Mathf.Clamp(this.hit.point.y - base.transform.position.y, this.min, this.max) * Vector3.up;
			}
			return Vector3.zero;
		}
	}
}
