using System;
using UnityEngine;

namespace RootMotion.FinalIK.Demos
{
	public class CharacterControllerDefault : CharacterControllerBase
	{
		[Serializable]
		public class State
		{
			public string clipName;

			public float animationSpeed = 1f;

			public float moveSpeed = 1f;
		}

		[Serializable]
		public enum RotationMode
		{
			Slerp,
			RotateTowards
		}

		public CameraController cam;

		public CharacterControllerDefault.State[] states;

		public int idleStateIndex;

		public int walkStateIndex = 1;

		public int runStateIndex = 2;

		public float acceleration = 5f;

		public float speedAcceleration = 3f;

		public float angularSpeed = 7f;

		public CharacterControllerDefault.RotationMode rotationMode;

		protected CharacterControllerDefault.State state;

		protected Vector3 moveVector;

		protected float speed;

		protected virtual float accelerationMlp
		{
			get
			{
				return 1f;
			}
		}

		protected virtual void Update()
		{
			if (base.GetInputDirection() != Vector3.zero)
			{
				this.state = ((!Input.GetKey(KeyCode.LeftShift)) ? this.states[this.walkStateIndex] : this.states[this.runStateIndex]);
			}
			else
			{
				this.state = this.states[this.idleStateIndex];
			}
			Vector3 vector = Quaternion.LookRotation(new Vector3(this.cam.transform.forward.x, 0f, this.cam.transform.forward.z)) * base.GetInputDirection();
			if (vector != Vector3.zero)
			{
				Vector3 vector2 = Quaternion.FromToRotation(base.transform.forward, vector) * base.transform.forward;
				Vector3 vector3 = base.transform.forward;
				CharacterControllerDefault.RotationMode rotationMode = this.rotationMode;
				if (rotationMode != CharacterControllerDefault.RotationMode.Slerp)
				{
					if (rotationMode == CharacterControllerDefault.RotationMode.RotateTowards)
					{
						vector3 = Vector3.RotateTowards(vector3, vector2, Time.deltaTime * this.angularSpeed * this.accelerationMlp, 1f);
					}
				}
				else
				{
					vector3 = Vector3.Slerp(vector3, vector2, Time.deltaTime * this.angularSpeed * this.accelerationMlp);
				}
				vector3.y = 0f;
				base.transform.rotation = Quaternion.LookRotation(vector3);
			}
			this.moveVector = Vector3.Lerp(this.moveVector, vector, Time.deltaTime * this.acceleration * this.accelerationMlp);
			this.speed = Mathf.Lerp(this.speed, this.state.moveSpeed, Time.deltaTime * this.speedAcceleration * this.accelerationMlp);
			if (base.GetComponent<Rigidbody>() != null)
			{
				base.GetComponent<Rigidbody>().position += this.moveVector * Time.deltaTime * this.speed;
			}
			else
			{
				base.transform.position += this.moveVector * Time.deltaTime * this.speed;
			}
		}
	}
}
