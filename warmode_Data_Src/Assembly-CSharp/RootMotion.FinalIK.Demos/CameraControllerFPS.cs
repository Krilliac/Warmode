using System;
using UnityEngine;

namespace RootMotion.FinalIK.Demos
{
	public class CameraControllerFPS : MonoBehaviour
	{
		public float rotationSensitivity = 3f;

		public float yMinLimit = -89f;

		public float yMaxLimit = 89f;

		private float x;

		private float y;

		private void Awake()
		{
			Vector3 eulerAngles = base.transform.eulerAngles;
			this.x = eulerAngles.y;
			this.y = eulerAngles.x;
		}

		public void LateUpdate()
		{
			Screen.lockCursor = true;
			this.x += Input.GetAxis("Mouse X") * this.rotationSensitivity;
			this.y = this.ClampAngle(this.y - Input.GetAxis("Mouse Y") * this.rotationSensitivity, this.yMinLimit, this.yMaxLimit);
			base.transform.rotation = Quaternion.AngleAxis(this.x, Vector3.up) * Quaternion.AngleAxis(this.y, Vector3.right);
		}

		private float ClampAngle(float angle, float min, float max)
		{
			if (angle < -360f)
			{
				angle += 360f;
			}
			if (angle > 360f)
			{
				angle -= 360f;
			}
			return Mathf.Clamp(angle, min, max);
		}
	}
}
