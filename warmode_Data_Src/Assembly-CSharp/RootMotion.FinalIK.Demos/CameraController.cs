using System;
using UnityEngine;

namespace RootMotion.FinalIK.Demos
{
	public class CameraController : MonoBehaviour
	{
		[Serializable]
		public enum UpdateMode
		{
			Update,
			FixedUpdate,
			LateUpdate
		}

		public Transform target;

		public CameraController.UpdateMode updateMode = CameraController.UpdateMode.LateUpdate;

		public bool lockCursor = true;

		public bool smoothFollow;

		public float followSpeed = 10f;

		public float distance = 10f;

		public float minDistance = 4f;

		public float maxDistance = 10f;

		public float zoomSpeed = 10f;

		public float zoomSensitivity = 1f;

		public float rotationSensitivity = 3.5f;

		public float yMinLimit = -20f;

		public float yMaxLimit = 80f;

		public Vector3 offset = new Vector3(0f, 1.5f, 0.5f);

		public bool rotateAlways = true;

		public bool rotateOnLeftButton;

		public bool rotateOnRightButton;

		public bool rotateOnMiddleButton;

		private Vector3 targetDistance;

		private Vector3 position;

		private Quaternion rotation = Quaternion.identity;

		private Vector3 smoothPosition;

		public float x
		{
			get;
			private set;
		}

		public float y
		{
			get;
			private set;
		}

		public float distanceTarget
		{
			get;
			private set;
		}

		private float zoomAdd
		{
			get
			{
				float axis = Input.GetAxis("Mouse ScrollWheel");
				if (axis > 0f)
				{
					return -this.zoomSensitivity;
				}
				if (axis < 0f)
				{
					return this.zoomSensitivity;
				}
				return 0f;
			}
		}

		private void Awake()
		{
			Vector3 eulerAngles = base.transform.eulerAngles;
			this.x = eulerAngles.y;
			this.y = eulerAngles.x;
			this.distanceTarget = this.distance;
			this.smoothPosition = base.transform.position;
		}

		private void Update()
		{
			if (this.updateMode == CameraController.UpdateMode.Update)
			{
				this.UpdateTransform();
			}
		}

		private void FixedUpdate()
		{
			if (this.updateMode == CameraController.UpdateMode.FixedUpdate)
			{
				this.UpdateTransform();
			}
		}

		private void LateUpdate()
		{
			this.UpdateInput();
			if (this.updateMode == CameraController.UpdateMode.LateUpdate)
			{
				this.UpdateTransform();
			}
		}

		public void UpdateInput()
		{
			if (this.target == null || !base.GetComponent<Camera>().enabled)
			{
				return;
			}
			if (this.lockCursor)
			{
				Screen.lockCursor = true;
			}
			bool flag = this.rotateAlways || (this.rotateOnLeftButton && Input.GetMouseButton(0)) || (this.rotateOnRightButton && Input.GetMouseButton(1)) || (this.rotateOnMiddleButton && Input.GetMouseButton(2));
			if (flag)
			{
				this.x += Input.GetAxis("Mouse X") * this.rotationSensitivity;
				this.y = this.ClampAngle(this.y - Input.GetAxis("Mouse Y") * this.rotationSensitivity, this.yMinLimit, this.yMaxLimit);
			}
			this.distanceTarget = Mathf.Clamp(this.distanceTarget + this.zoomAdd, this.minDistance, this.maxDistance);
		}

		public void UpdateTransform()
		{
			this.UpdateTransform(Time.deltaTime);
		}

		public void UpdateTransform(float deltaTime)
		{
			if (this.target == null || !base.GetComponent<Camera>().enabled)
			{
				return;
			}
			this.distance += (this.distanceTarget - this.distance) * this.zoomSpeed * deltaTime;
			this.rotation = Quaternion.AngleAxis(this.x, Vector3.up) * Quaternion.AngleAxis(this.y, Vector3.right);
			if (!this.smoothFollow)
			{
				this.smoothPosition = this.target.position;
			}
			else
			{
				this.smoothPosition = Vector3.Lerp(this.smoothPosition, this.target.position, deltaTime * this.followSpeed);
			}
			this.position = this.smoothPosition + this.rotation * (this.offset - Vector3.forward * this.distance);
			base.transform.position = this.position;
			base.transform.rotation = this.rotation;
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
