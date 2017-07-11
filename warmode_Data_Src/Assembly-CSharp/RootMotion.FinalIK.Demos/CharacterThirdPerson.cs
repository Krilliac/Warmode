using System;
using UnityEngine;

namespace RootMotion.FinalIK.Demos
{
	public class CharacterThirdPerson : CharacterBase
	{
		[Serializable]
		public enum MoveMode
		{
			Directional,
			Strafe
		}

		public struct AnimState
		{
			public Vector3 moveDirection;

			public bool jump;

			public bool crouch;

			public bool onGround;

			public bool isStrafing;

			public float yVelocity;
		}

		[Serializable]
		public class AdvancedSettings
		{
			public float stationaryTurnSpeedMlp = 1f;

			public float movingTurnSpeed = 5f;

			public float lookResponseSpeed = 2f;

			public float jumpRepeatDelayTime = 0.25f;

			public float groundStickyEffect = 5f;

			public float platformFriction = 7f;

			public float maxVerticalVelocityOnGround = 3f;

			public float crouchCapsuleScaleMlp = 0.6f;

			public float velocityToGroundTangentWeight = 1f;

			public float wallRunMaxLength = 1f;

			public float wallRunMinHorVelocity = 3f;

			public float wallRunMinMoveMag = 0.6f;

			public float wallRunMinVelocityY = -1f;

			public float wallRunRotationSpeed = 1.5f;

			public float wallRunMaxRotationAngle = 70f;

			public float wallRunWeightSpeed = 5f;
		}

		[SerializeField]
		private CharacterAnimationBase characterAnimation;

		[SerializeField]
		private UserControlThirdPerson userControl;

		[SerializeField]
		private CameraController cam;

		[SerializeField]
		protected Grounder grounder;

		[SerializeField]
		private LayerMask wallRunLayers;

		[Range(1f, 4f), SerializeField]
		private float gravityMultiplier = 2f;

		public CharacterThirdPerson.MoveMode moveMode;

		public float accelerationTime = 0.2f;

		public float airSpeed = 6f;

		public float airControl = 2f;

		public float jumpPower = 12f;

		public bool lookInCameraDirection;

		public CharacterThirdPerson.AdvancedSettings advancedSettings;

		public CharacterThirdPerson.AnimState animState = default(CharacterThirdPerson.AnimState);

		private Vector3 moveDirection;

		private Vector3 lookPosSmooth;

		private Animator animator;

		private Vector3 normal;

		private Vector3 platformVelocity;

		private RaycastHit hit;

		private float jumpLeg;

		private float jumpEndTime;

		private float forwardMlp;

		private float groundDistance;

		private float lastAirTime;

		private float stickyForce;

		private Vector3 wallNormal = Vector3.up;

		private Vector3 moveDirectionVelocity;

		private float wallRunWeight;

		private float lastWallRunWeight;

		private Vector3 fixedDeltaPosition;

		private bool fixedFrame;

		private float wallRunEndTime;

		public bool onGround
		{
			get;
			private set;
		}

		protected override void Start()
		{
			base.Start();
			this.animator = base.GetComponent<Animator>();
			this.wallNormal = Vector3.up;
			if (this.cam != null)
			{
				this.cam.enabled = false;
			}
			this.lookPosSmooth = base.transform.position + base.transform.forward * 10f;
		}

		private void OnAnimatorMove()
		{
			this.Move(this.animator.deltaPosition);
		}

		public override void Move(Vector3 deltaPosition)
		{
			this.fixedDeltaPosition += deltaPosition;
		}

		private void FixedUpdate()
		{
			this.MoveFixed(this.fixedDeltaPosition);
			this.fixedDeltaPosition = Vector3.zero;
			this.Look();
			this.GroundCheck();
			if (this.userControl.state.move == Vector3.zero && this.groundDistance < this.airborneThreshold * 0.5f)
			{
				base.HighFriction();
			}
			else
			{
				base.ZeroFriction();
			}
			if (this.onGround)
			{
				this.animState.jump = this.Jump();
			}
			else
			{
				base.GetComponent<Rigidbody>().AddForce(Physics.gravity * this.gravityMultiplier - Physics.gravity);
			}
			base.ScaleCapsule((!this.userControl.state.crouch) ? 1f : this.advancedSettings.crouchCapsuleScaleMlp);
			this.animState.moveDirection = this.GetMoveDirection();
			this.animState.crouch = this.userControl.state.crouch;
			this.animState.onGround = this.onGround;
			this.animState.yVelocity = base.GetComponent<Rigidbody>().velocity.y;
			this.animState.isStrafing = (this.moveMode == CharacterThirdPerson.MoveMode.Strafe);
			this.fixedFrame = true;
		}

		private void LateUpdate()
		{
			if (this.cam == null)
			{
				return;
			}
			this.cam.UpdateInput();
			if (!this.fixedFrame && base.GetComponent<Rigidbody>().interpolation == RigidbodyInterpolation.None)
			{
				return;
			}
			this.cam.UpdateTransform((base.GetComponent<Rigidbody>().interpolation != RigidbodyInterpolation.None) ? Time.deltaTime : Time.fixedDeltaTime);
			this.fixedFrame = false;
		}

		private void MoveFixed(Vector3 deltaPosition)
		{
			this.WallRun();
			Vector3 vector = deltaPosition / Time.deltaTime;
			vector += new Vector3(this.platformVelocity.x, 0f, this.platformVelocity.z);
			if (this.onGround)
			{
				if (this.advancedSettings.velocityToGroundTangentWeight > 0f)
				{
					Quaternion b = Quaternion.FromToRotation(base.transform.up, this.normal);
					vector = Quaternion.Lerp(Quaternion.identity, b, this.advancedSettings.velocityToGroundTangentWeight) * vector;
				}
			}
			else
			{
				Vector3 b2 = new Vector3(this.userControl.state.move.x * this.airSpeed, 0f, this.userControl.state.move.z * this.airSpeed);
				vector = Vector3.Lerp(base.GetComponent<Rigidbody>().velocity, b2, Time.deltaTime * this.airControl);
			}
			if (this.onGround && Time.time > this.jumpEndTime)
			{
				base.GetComponent<Rigidbody>().velocity = base.GetComponent<Rigidbody>().velocity - base.transform.up * this.stickyForce * Time.deltaTime;
			}
			vector.y = Mathf.Clamp(base.GetComponent<Rigidbody>().velocity.y, base.GetComponent<Rigidbody>().velocity.y, (!this.onGround) ? base.GetComponent<Rigidbody>().velocity.y : this.advancedSettings.maxVerticalVelocityOnGround);
			base.GetComponent<Rigidbody>().velocity = vector;
			float b3 = this.onGround ? base.GetSlopeDamper(-deltaPosition / Time.deltaTime, this.normal) : 1f;
			this.forwardMlp = Mathf.Lerp(this.forwardMlp, b3, Time.deltaTime * 5f);
		}

		private void WallRun()
		{
			bool flag = this.CanWallRun();
			if (this.wallRunWeight > 0f && !flag)
			{
				this.wallRunEndTime = Time.time;
			}
			if (Time.time < this.wallRunEndTime + 0.5f)
			{
				flag = false;
			}
			this.wallRunWeight = Mathf.MoveTowards(this.wallRunWeight, (!flag) ? 0f : 1f, Time.deltaTime * this.advancedSettings.wallRunWeightSpeed);
			if (this.wallRunWeight <= 0f && this.lastWallRunWeight > 0f)
			{
				base.transform.rotation = Quaternion.LookRotation(new Vector3(base.transform.forward.x, 0f, base.transform.forward.z), Vector3.up);
				this.wallNormal = Vector3.up;
			}
			this.lastWallRunWeight = this.wallRunWeight;
			if (this.wallRunWeight <= 0f)
			{
				return;
			}
			if (this.onGround && base.GetComponent<Rigidbody>().velocity.y < 0f)
			{
				base.GetComponent<Rigidbody>().velocity = new Vector3(base.GetComponent<Rigidbody>().velocity.x, 0f, base.GetComponent<Rigidbody>().velocity.z);
			}
			Vector3 forward = base.transform.forward;
			forward.y = 0f;
			RaycastHit raycastHit = default(RaycastHit);
			raycastHit.normal = Vector3.up;
			Physics.Raycast((!this.onGround) ? base.GetComponent<Collider>().bounds.center : base.transform.position, forward, out raycastHit, 3f, this.wallRunLayers);
			this.wallNormal = Vector3.Lerp(this.wallNormal, raycastHit.normal, Time.deltaTime * this.advancedSettings.wallRunRotationSpeed);
			this.wallNormal = Vector3.RotateTowards(Vector3.up, this.wallNormal, this.advancedSettings.wallRunMaxRotationAngle * 0.0174532924f, 0f);
			Vector3 forward2 = base.transform.forward;
			Vector3 vector = this.wallNormal;
			Vector3.OrthoNormalize(ref vector, ref forward2);
			base.transform.rotation = Quaternion.Slerp(Quaternion.LookRotation(forward, Vector3.up), Quaternion.LookRotation(forward2, this.wallNormal), this.wallRunWeight);
		}

		private bool CanWallRun()
		{
			if (Time.time < this.jumpEndTime - 0.1f)
			{
				return false;
			}
			if (Time.time > this.jumpEndTime - 0.1f + this.advancedSettings.wallRunMaxLength)
			{
				return false;
			}
			if (base.GetComponent<Rigidbody>().velocity.y < this.advancedSettings.wallRunMinVelocityY)
			{
				return false;
			}
			if (this.userControl.state.move.magnitude < this.advancedSettings.wallRunMinMoveMag)
			{
				return false;
			}
			Vector3 velocity = base.GetComponent<Rigidbody>().velocity;
			velocity.y = 0f;
			return velocity.magnitude >= this.advancedSettings.wallRunMinHorVelocity;
		}

		private Vector3 GetMoveDirection()
		{
			CharacterThirdPerson.MoveMode moveMode = this.moveMode;
			if (moveMode == CharacterThirdPerson.MoveMode.Directional)
			{
				this.moveDirection = Vector3.SmoothDamp(this.moveDirection, new Vector3(0f, 0f, this.userControl.state.move.magnitude), ref this.moveDirectionVelocity, this.accelerationTime);
				return this.moveDirection * this.forwardMlp;
			}
			if (moveMode != CharacterThirdPerson.MoveMode.Strafe)
			{
				return Vector3.zero;
			}
			this.moveDirection = Vector3.SmoothDamp(this.moveDirection, this.userControl.state.move, ref this.moveDirectionVelocity, this.accelerationTime);
			return base.transform.InverseTransformDirection(this.moveDirection);
		}

		private void Look()
		{
			this.lookPosSmooth = Vector3.Lerp(this.lookPosSmooth, this.userControl.state.lookPos, Time.deltaTime * this.advancedSettings.lookResponseSpeed);
			float num = base.GetAngleFromForward(this.GetLookDirection());
			if (this.userControl.state.move == Vector3.zero)
			{
				num *= (1.01f - Mathf.Abs(num) / 180f) * this.advancedSettings.stationaryTurnSpeedMlp;
			}
			base.RigidbodyRotateAround(this.characterAnimation.GetPivotPoint(), base.transform.up, num * Time.deltaTime * this.advancedSettings.movingTurnSpeed);
		}

		private Vector3 GetLookDirection()
		{
			bool flag = this.userControl.state.move != Vector3.zero;
			CharacterThirdPerson.MoveMode moveMode = this.moveMode;
			if (moveMode != CharacterThirdPerson.MoveMode.Directional)
			{
				if (moveMode != CharacterThirdPerson.MoveMode.Strafe)
				{
					return Vector3.zero;
				}
				if (flag)
				{
					return this.userControl.state.lookPos - base.GetComponent<Rigidbody>().position;
				}
				return (!this.lookInCameraDirection) ? base.transform.forward : (this.userControl.state.lookPos - base.GetComponent<Rigidbody>().position);
			}
			else
			{
				if (flag)
				{
					return this.userControl.state.move;
				}
				return (!this.lookInCameraDirection) ? base.transform.forward : (this.userControl.state.lookPos - base.GetComponent<Rigidbody>().position);
			}
		}

		private bool Jump()
		{
			if (!this.userControl.state.jump)
			{
				return false;
			}
			if (this.userControl.state.crouch)
			{
				return false;
			}
			if (!this.characterAnimation.animationGrounded)
			{
				return false;
			}
			if (Time.time < this.lastAirTime + this.advancedSettings.jumpRepeatDelayTime)
			{
				return false;
			}
			this.onGround = false;
			this.jumpEndTime = Time.time + 0.1f;
			Vector3 velocity = this.userControl.state.move * this.airSpeed;
			base.GetComponent<Rigidbody>().velocity = velocity;
			base.GetComponent<Rigidbody>().velocity += base.transform.up * this.jumpPower;
			return true;
		}

		private RaycastHit GetHit()
		{
			if (this.grounder == null)
			{
				return this.GetSpherecastHit();
			}
			if (this.grounder.solver.quality != Grounding.Quality.Best)
			{
				return this.GetSpherecastHit();
			}
			if (this.grounder.enabled && this.grounder.weight > 0f)
			{
				return this.grounder.solver.rootHit;
			}
			return this.grounder.solver.GetRootHit(10f);
		}

		private void GroundCheck()
		{
			Vector3 b = Vector3.zero;
			float num = 0f;
			this.hit = this.GetHit();
			this.normal = this.hit.normal;
			this.groundDistance = base.GetComponent<Rigidbody>().position.y - this.hit.point.y;
			bool flag = Time.time > this.jumpEndTime && base.GetComponent<Rigidbody>().velocity.y < this.jumpPower * 0.5f;
			if (flag)
			{
				bool onGround = this.onGround;
				this.onGround = false;
				float num2 = onGround ? this.airborneThreshold : (this.airborneThreshold * 0.5f);
				Vector3 velocity = base.GetComponent<Rigidbody>().velocity;
				velocity.y = 0f;
				float magnitude = velocity.magnitude;
				if (this.groundDistance < num2)
				{
					num = this.advancedSettings.groundStickyEffect * magnitude * num2;
					if (this.hit.rigidbody != null)
					{
						b = this.hit.rigidbody.GetPointVelocity(this.hit.point);
					}
					this.onGround = true;
				}
			}
			this.platformVelocity = Vector3.Lerp(this.platformVelocity, b, Time.deltaTime * this.advancedSettings.platformFriction);
			this.stickyForce = num;
			if (!this.onGround)
			{
				this.lastAirTime = Time.time;
			}
		}
	}
}
