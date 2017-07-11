using System;
using UnityEngine;

[RequireComponent(typeof(vp_FPPlayerEventHandler)), RequireComponent(typeof(CharacterController)), RequireComponent(typeof(vp_FPInput))]
public class vp_FPController : vp_Component
{
	public static vp_FPController cs;

	public static float LastDamageTime;

	public CharacterController m_CharacterController;

	public Vector3 m_FixedPosition = Vector3.zero;

	protected Vector3 m_SmoothPosition = Vector3.zero;

	public static bool m_LastGrounded;

	protected bool m_Grounded;

	protected bool m_HeadContact;

	protected RaycastHit m_GroundHit;

	protected RaycastHit m_LastGroundHit;

	protected RaycastHit m_CeilingHit;

	protected RaycastHit m_WallHit;

	protected float m_FallImpact;

	protected float m_FallTime;

	public static bool canStopCrouch = true;

	public float MotorAcceleration = 0.18f;

	public float MotorDamping = 0.17f;

	public float MotorAirSpeed = 0.35f;

	public float MotorSlopeSpeedUp = 1f;

	public float MotorSlopeSpeedDown = 1f;

	public bool MotorFreeFly;

	protected Vector3 m_MoveDirection = Vector3.zero;

	protected float m_SlopeFactor = 1f;

	protected Vector3 m_MotorThrottle = Vector3.zero;

	protected float m_MotorAirSpeedModifier = 1f;

	protected float m_CurrentAntiBumpOffset;

	protected Vector2 m_MoveVector = Vector2.zero;

	public float SpeedDeforce = 1f;

	public float MotorJumpForce = 0.18f;

	public float MotorJumpForceDamping = 0.08f;

	public float MotorJumpForceHold = 0.003f;

	public float MotorJumpForceHoldDamping = 0.5f;

	protected int m_MotorJumpForceHoldSkipFrames;

	protected float m_MotorJumpForceAcc;

	private bool m_MotorJumpDone = true;

	protected float m_FallSpeed;

	protected float m_LastFallSpeed;

	protected float m_HighestFallSpeed;

	public float PhysicsForceDamping = 0.05f;

	public float PhysicsPushForce = 5f;

	public float PhysicsGravityModifier = 0.2f;

	public float PhysicsSlopeSlideLimit = 30f;

	public float PhysicsSlopeSlidiness = 0.15f;

	public float PhysicsWallBounce;

	public float PhysicsWallFriction;

	public bool PhysicsHasCollisionTrigger = true;

	protected GameObject m_Trigger;

	protected Vector3 m_ExternalForce = Vector3.zero;

	protected Vector3[] m_SmoothForceFrame = new Vector3[120];

	protected bool m_Slide;

	protected bool m_SlideFast;

	protected float m_SlideFallSpeed;

	protected float m_OnSteepGroundSince;

	protected float m_SlopeSlideSpeed;

	protected Vector3 m_PredictedPos = Vector3.zero;

	protected Vector3 m_PrevPos = Vector3.zero;

	protected Vector3 m_PrevDir = Vector3.zero;

	protected Vector3 m_NewDir = Vector3.zero;

	protected float m_ForceImpact;

	protected float m_ForceMultiplier;

	protected Vector3 CapsuleBottom = Vector3.zero;

	protected Vector3 CapsuleTop = Vector3.zero;

	protected float m_SkinWidth = 0.08f;

	protected Transform m_Platform;

	protected Vector3 m_PositionOnPlatform = Vector3.zero;

	protected float m_LastPlatformAngle;

	protected Vector3 m_LastPlatformPos = Vector3.zero;

	protected float m_NormalHeight;

	protected Vector3 m_NormalCenter = Vector3.zero;

	protected float m_CrouchHeight;

	protected Vector3 m_CrouchCenter = Vector3.zero;

	protected float steps_time;

	protected float steps_interval_last;

	protected AudioSource steps_audio;

	protected AudioClip[] steps_walk_concrete = new AudioClip[4];

	protected AudioClip[] steps_run_concrete = new AudioClip[4];

	protected AudioClip[] steps_walk_gravel = new AudioClip[4];

	protected AudioClip[] steps_run_gravel = new AudioClip[4];

	private vp_FPPlayerEventHandler m_Player;

	public bool Grounded
	{
		get
		{
			return this.m_Grounded;
		}
	}

	public bool HeadContact
	{
		get
		{
			return this.m_HeadContact;
		}
	}

	public Vector3 GroundNormal
	{
		get
		{
			return this.m_GroundHit.normal;
		}
	}

	public float GroundAngle
	{
		get
		{
			return Vector3.Angle(this.m_GroundHit.normal, Vector3.up);
		}
	}

	public Transform GroundTransform
	{
		get
		{
			return this.m_GroundHit.transform;
		}
	}

	public Vector3 SmoothPosition
	{
		get
		{
			return this.m_SmoothPosition;
		}
	}

	public Vector3 Velocity
	{
		get
		{
			return this.m_CharacterController.velocity;
		}
	}

	public Vector3 PrevPosition
	{
		get
		{
			return this.m_PrevPos;
		}
	}

	public CharacterController CharacterController
	{
		get
		{
			return this.m_CharacterController;
		}
	}

	private vp_FPPlayerEventHandler Player
	{
		get
		{
			if (this.m_Player == null && this.EventHandler != null)
			{
				this.m_Player = (vp_FPPlayerEventHandler)this.EventHandler;
			}
			return this.m_Player;
		}
	}

	protected virtual Vector3 OnValue_Position
	{
		get
		{
			return base.Transform.position;
		}
		set
		{
			this.SetPosition(value);
		}
	}

	protected virtual Vector2 OnValue_InputMoveVector
	{
		get
		{
			return this.m_MoveVector;
		}
		set
		{
			this.m_MoveVector = value.normalized;
		}
	}

	protected virtual Transform OnValue_Platform
	{
		get
		{
			return this.m_Platform;
		}
	}

	protected override void Awake()
	{
		vp_FPController.cs = this;
		base.Awake();
		this.m_CharacterController = base.gameObject.GetComponent<CharacterController>();
		this.m_NormalHeight = this.CharacterController.height;
		this.CharacterController.center = (this.m_NormalCenter = new Vector3(0f, this.m_NormalHeight * 0.5f, 0f));
		this.CharacterController.radius = this.m_NormalHeight * 0.25f;
		this.m_CrouchHeight = this.m_NormalHeight * 0.5f;
		this.m_CrouchCenter = this.m_NormalCenter * 0.5f;
		this.m_CharacterController.enabled = false;
		this.steps_audio = base.gameObject.AddComponent<AudioSource>();
		this.steps_walk_concrete[0] = SND.GetSoundByName("player/movement/walk_concrete1");
		this.steps_walk_concrete[1] = SND.GetSoundByName("player/movement/walk_concrete2");
		this.steps_walk_concrete[2] = SND.GetSoundByName("player/movement/walk_concrete3");
		this.steps_walk_concrete[3] = SND.GetSoundByName("player/movement/walk_concrete4");
		this.steps_run_concrete[0] = SND.GetSoundByName("player/movement/run_concrete1");
		this.steps_run_concrete[1] = SND.GetSoundByName("player/movement/run_concrete2");
		this.steps_run_concrete[2] = SND.GetSoundByName("player/movement/run_concrete3");
		this.steps_run_concrete[3] = SND.GetSoundByName("player/movement/run_concrete4");
		this.steps_walk_gravel[0] = SND.GetSoundByName("player/movement/walk_gravel1");
		this.steps_walk_gravel[1] = SND.GetSoundByName("player/movement/walk_gravel2");
		this.steps_walk_gravel[2] = SND.GetSoundByName("player/movement/walk_gravel3");
		this.steps_walk_gravel[3] = SND.GetSoundByName("player/movement/walk_gravel4");
		this.steps_run_gravel[0] = SND.GetSoundByName("player/movement/run_gravel1");
		this.steps_run_gravel[1] = SND.GetSoundByName("player/movement/run_gravel2");
		this.steps_run_gravel[2] = SND.GetSoundByName("player/movement/run_gravel3");
		this.steps_run_gravel[3] = SND.GetSoundByName("player/movement/run_gravel4");
	}

	protected override void Start()
	{
		base.Start();
		this.SetPosition(base.Transform.position);
		if (this.PhysicsHasCollisionTrigger)
		{
			this.m_Trigger = new GameObject("Trigger");
			this.m_Trigger.transform.parent = this.m_Transform;
			CapsuleCollider capsuleCollider = this.m_Trigger.AddComponent<CapsuleCollider>();
			capsuleCollider.isTrigger = true;
			capsuleCollider.radius = this.CharacterController.radius + this.m_SkinWidth;
			capsuleCollider.height = this.CharacterController.height + this.m_SkinWidth * 2f;
			capsuleCollider.center = this.CharacterController.center;
			this.m_Trigger.layer = 30;
			this.m_Trigger.transform.localPosition = Vector3.zero;
		}
	}

	protected override void Update()
	{
		vp_FPController.canStopCrouch = this.CanStop_Crouch();
		base.Update();
		this.SmoothMove();
		float sqrMagnitude = this.Velocity.sqrMagnitude;
		if (Time.time > this.steps_time && this.m_Grounded && !SpecCam.show)
		{
			if (!this.Player.Crouch.Active && !this.Player.Zoom.Active)
			{
				if (sqrMagnitude > 50f)
				{
					this.steps_audio.volume = 0.6f * Options.gamevol;
					this.steps_time = Time.time + 0.31f + UnityEngine.Random.Range(0f, 0.025f);
					int num = UnityEngine.Random.Range(0, 3);
					if (num == 0)
					{
						num = 3;
					}
					this.steps_audio.pitch = UnityEngine.Random.Range(0.9f, 1.1f);
					if (this.steps_run_gravel[num] != null)
					{
						this.steps_audio.PlayOneShot(this.steps_run_gravel[num]);
					}
				}
				else if (sqrMagnitude > 8f)
				{
					this.steps_audio.volume = 0.8f * Options.gamevol;
					this.steps_time = Time.time + 0.38f + UnityEngine.Random.Range(0f, 0.05f);
					int num2 = UnityEngine.Random.Range(0, 3);
					if (num2 == 0)
					{
						num2 = 3;
					}
					this.steps_audio.pitch = UnityEngine.Random.Range(0.9f, 1.1f);
					if (this.steps_walk_gravel[num2] != null)
					{
						this.steps_audio.PlayOneShot(this.steps_walk_gravel[num2]);
					}
				}
			}
		}
		Crosshair.SetOffset((int)sqrMagnitude);
		vp_FPInput.speed = sqrMagnitude;
		SpecCam.Update();
	}

	protected override void FixedUpdate()
	{
		if (Time.timeScale == 0f)
		{
			return;
		}
		this.UpdateMotor();
		this.UpdateJump();
		this.UpdateForces();
		this.UpdateSliding();
		this.FixedMove();
		this.UpdateCollisions();
		this.UpdatePlatformMove();
		this.m_PrevPos = base.Transform.position;
	}

	protected virtual void UpdateMotor()
	{
		this.UpdateThrottleWalk();
		this.m_MotorThrottle = vp_Utility.SnapToZero(this.m_MotorThrottle, 0.0001f);
	}

	protected virtual void UpdateThrottleWalk()
	{
		this.UpdateSlopeFactor();
		this.m_MotorAirSpeedModifier = ((!this.m_Grounded) ? this.MotorAirSpeed : 1f);
		float d = 1f;
		if (vp_FPController.LastDamageTime + 0.1f > Time.time)
		{
			d = 0.25f;
		}
		this.m_MotorThrottle += this.m_MoveVector.y * (base.Transform.TransformDirection(Vector3.forward * (this.MotorAcceleration * 0.1f) * this.m_MotorAirSpeedModifier) * this.m_SlopeFactor) * d;
		this.m_MotorThrottle += this.m_MoveVector.x * (base.Transform.TransformDirection(Vector3.right * (this.MotorAcceleration * 0.1f) * this.m_MotorAirSpeedModifier) * this.m_SlopeFactor) * d;
		this.m_MotorThrottle.x = this.m_MotorThrottle.x / (1f + this.MotorDamping * this.m_MotorAirSpeedModifier * Time.timeScale);
		this.m_MotorThrottle.z = this.m_MotorThrottle.z / (1f + this.MotorDamping * this.m_MotorAirSpeedModifier * Time.timeScale);
	}

	protected virtual void UpdateJump()
	{
		if (this.m_HeadContact)
		{
			this.Player.Jump.Stop(1f);
		}
		this.UpdateJumpForceWalk();
		this.m_MotorThrottle.y = this.m_MotorThrottle.y + this.m_MotorJumpForceAcc * Time.timeScale;
		this.m_MotorJumpForceAcc /= 1f + this.MotorJumpForceHoldDamping * Time.timeScale;
		this.m_MotorThrottle.y = this.m_MotorThrottle.y / (1f + this.MotorJumpForceDamping * Time.timeScale);
	}

	protected virtual void UpdateJumpForceWalk()
	{
		if (this.Player.Jump.Active && !this.m_Grounded)
		{
			if (this.m_MotorJumpForceHoldSkipFrames > 2)
			{
				if (this.m_CharacterController.velocity.y >= 0f)
				{
					this.m_MotorJumpForceAcc += this.MotorJumpForceHold;
				}
			}
			else
			{
				this.m_MotorJumpForceHoldSkipFrames++;
			}
		}
	}

	protected virtual void UpdateJumpForceFree()
	{
		if (this.Player.Jump.Active && this.Player.Crouch.Active)
		{
			return;
		}
		if (this.Player.Jump.Active)
		{
			this.m_MotorJumpForceAcc += this.MotorJumpForceHold;
		}
		else if (this.Player.Crouch.Active)
		{
			this.m_MotorJumpForceAcc -= this.MotorJumpForceHold;
			if (this.Grounded && this.CharacterController.height == this.m_NormalHeight)
			{
				this.CharacterController.height = this.m_CrouchHeight;
				this.CharacterController.center = this.m_CrouchCenter;
			}
		}
	}

	protected virtual void UpdateForces()
	{
		if (this.m_Grounded && this.m_FallSpeed <= 0f)
		{
			this.m_FallSpeed = Physics.gravity.y * (this.PhysicsGravityModifier * 0.002f);
		}
		else
		{
			this.m_FallSpeed += Physics.gravity.y * (this.PhysicsGravityModifier * 0.002f);
		}
		if (this.m_FallSpeed < this.m_LastFallSpeed)
		{
			this.m_HighestFallSpeed = this.m_FallSpeed;
		}
		this.m_LastFallSpeed = this.m_FallSpeed;
		if (this.m_SmoothForceFrame[0] != Vector3.zero)
		{
			this.AddForceInternal(this.m_SmoothForceFrame[0]);
			for (int i = 0; i < 120; i++)
			{
				this.m_SmoothForceFrame[i] = ((i >= 119) ? Vector3.zero : this.m_SmoothForceFrame[i + 1]);
				if (this.m_SmoothForceFrame[i] == Vector3.zero)
				{
					break;
				}
			}
		}
		this.m_ExternalForce /= 1f + this.PhysicsForceDamping * Time.timeScale;
	}

	protected virtual void UpdateSliding()
	{
		bool slideFast = this.m_SlideFast;
		bool slide = this.m_Slide;
		this.m_Slide = false;
		if (!this.m_Grounded)
		{
			this.m_OnSteepGroundSince = 0f;
			this.m_SlideFast = false;
		}
		else if (this.GroundAngle > this.PhysicsSlopeSlideLimit)
		{
			this.m_Slide = true;
			if (this.GroundAngle <= this.m_CharacterController.slopeLimit)
			{
				this.m_SlopeSlideSpeed = Mathf.Max(this.m_SlopeSlideSpeed, this.PhysicsSlopeSlidiness * 0.01f);
				this.m_OnSteepGroundSince = 0f;
				this.m_SlideFast = false;
				this.m_SlopeSlideSpeed = ((Mathf.Abs(this.m_SlopeSlideSpeed) >= 0.0001f) ? (this.m_SlopeSlideSpeed / (1f + 0.05f * Time.timeScale)) : 0f);
			}
			else
			{
				if (this.m_SlopeSlideSpeed > 0.01f)
				{
					this.m_SlideFast = true;
				}
				if (this.m_OnSteepGroundSince == 0f)
				{
					this.m_OnSteepGroundSince = Time.time;
				}
				this.m_SlopeSlideSpeed += this.PhysicsSlopeSlidiness * 0.01f * ((Time.time - this.m_OnSteepGroundSince) * 0.125f) * Time.timeScale;
				this.m_SlopeSlideSpeed = Mathf.Max(this.PhysicsSlopeSlidiness * 0.01f, this.m_SlopeSlideSpeed);
			}
			this.AddForce(Vector3.Cross(Vector3.Cross(this.GroundNormal, Vector3.down), this.GroundNormal) * this.m_SlopeSlideSpeed * Time.timeScale);
		}
		else
		{
			this.m_OnSteepGroundSince = 0f;
			this.m_SlideFast = false;
			this.m_SlopeSlideSpeed = 0f;
		}
		if (this.m_MotorThrottle != Vector3.zero)
		{
			this.m_Slide = false;
		}
		if (this.m_SlideFast)
		{
			this.m_SlideFallSpeed = base.Transform.position.y;
		}
		else if (slideFast && !this.Grounded)
		{
			this.m_FallSpeed = base.Transform.position.y - this.m_SlideFallSpeed;
		}
		if (slide != this.m_Slide)
		{
			this.Player.SetState("Slide", this.m_Slide, true, false);
		}
		if (slideFast != this.m_SlideFast)
		{
			this.Player.SetState("SlideFast", this.m_SlideFast, true, false);
		}
	}

	protected virtual void FixedMove()
	{
		this.m_MoveDirection = Vector3.zero;
		this.m_MoveDirection += this.m_ExternalForce;
		this.m_MoveDirection += this.m_MotorThrottle;
		this.m_MoveDirection.y = this.m_MoveDirection.y + this.m_FallSpeed;
		this.m_CurrentAntiBumpOffset = 0f;
		if (this.m_Grounded && this.m_MotorThrottle.y <= 0.001f)
		{
			this.m_CurrentAntiBumpOffset = Mathf.Max(this.m_CharacterController.stepOffset, Vector3.Scale(this.m_MoveDirection, Vector3.one - Vector3.up).magnitude);
			this.m_MoveDirection += this.m_CurrentAntiBumpOffset * Vector3.down;
		}
		this.m_PredictedPos = base.Transform.position + vp_Utility.NaNSafeVector3(this.m_MoveDirection * base.Delta * Time.timeScale, default(Vector3));
		if (this.m_Platform != null && this.m_PositionOnPlatform != Vector3.zero)
		{
			this.m_CharacterController.Move(vp_Utility.NaNSafeVector3(this.m_Platform.TransformPoint(this.m_PositionOnPlatform) - this.m_Transform.position, default(Vector3)));
		}
		if (this.m_CharacterController.enabled)
		{
			this.m_CharacterController.Move(vp_Utility.NaNSafeVector3(this.m_MoveDirection * base.Delta * Time.timeScale, default(Vector3)));
		}
		if (this.Player.Dead.Active)
		{
			this.m_MoveVector = Vector2.zero;
			return;
		}
		Physics.SphereCast(new Ray(base.Transform.position + Vector3.up * this.m_CharacterController.radius, Vector3.down), this.m_CharacterController.radius, out this.m_GroundHit, this.m_SkinWidth + 0.001f, vp_Layer.GetExternalBlockers());
		vp_FPController.m_LastGrounded = this.m_Grounded;
		this.m_Grounded = (this.m_GroundHit.collider != null);
		if (this.m_Grounded != vp_FPController.m_LastGrounded)
		{
			if (this.m_Grounded)
			{
				vp_FPInput.LastJumpTime = Time.time;
				this.Player.Run.Stop(0f);
				if (this.m_FallTime == 0f)
				{
					this.m_FallTime = Time.realtimeSinceStartup;
				}
				float num = Time.realtimeSinceStartup - this.m_FallTime;
				this.m_FallTime = 0f;
				if (Time.time > BasePlayer.spawntime + 3f && num >= 1f)
				{
					Client.cs.send_fall(num);
				}
			}
			else if (this.m_FallTime == 0f)
			{
				this.m_FallTime = Time.realtimeSinceStartup;
			}
		}
		if (!this.m_Grounded && this.m_CharacterController.velocity.y > 0f)
		{
			Physics.SphereCast(new Ray(base.Transform.position, Vector3.up), this.m_CharacterController.radius, out this.m_CeilingHit, this.m_CharacterController.height - (this.m_CharacterController.radius - this.m_SkinWidth) + 0.01f, vp_Layer.GetExternalBlockers());
			this.m_HeadContact = (this.m_CeilingHit.collider != null);
		}
		else
		{
			this.m_HeadContact = false;
		}
		if (this.m_GroundHit.transform == null && this.m_LastGroundHit.transform != null)
		{
			if (this.m_Platform != null && this.m_PositionOnPlatform != Vector3.zero)
			{
				this.AddForce(this.m_Platform.position - this.m_LastPlatformPos);
				this.m_Platform = null;
			}
			if (this.m_CurrentAntiBumpOffset != 0f)
			{
				this.m_CharacterController.Move(vp_Utility.NaNSafeVector3(this.m_CurrentAntiBumpOffset * Vector3.up, default(Vector3)) * base.Delta * Time.timeScale);
				this.m_PredictedPos += vp_Utility.NaNSafeVector3(this.m_CurrentAntiBumpOffset * Vector3.up, default(Vector3)) * base.Delta * Time.timeScale;
				this.m_MoveDirection += this.m_CurrentAntiBumpOffset * Vector3.up;
			}
		}
	}

	protected virtual void SmoothMove()
	{
		if (Time.timeScale == 0f)
		{
			return;
		}
		this.m_FixedPosition = base.Transform.position;
		base.Transform.position = this.m_SmoothPosition;
		if (this.m_CharacterController.enabled)
		{
			this.m_CharacterController.Move(vp_Utility.NaNSafeVector3(this.m_MoveDirection * base.Delta * Time.timeScale, default(Vector3)));
		}
		this.m_SmoothPosition = base.Transform.position;
		base.Transform.position = this.m_FixedPosition;
		if (Vector3.Distance(base.Transform.position, this.m_SmoothPosition) > this.m_CharacterController.radius)
		{
			this.m_SmoothPosition = base.Transform.position;
		}
		if (this.m_Platform != null && (this.m_LastPlatformPos.y < this.m_Platform.position.y || this.m_LastPlatformPos.y > this.m_Platform.position.y))
		{
			this.m_SmoothPosition.y = base.Transform.position.y;
		}
		this.m_SmoothPosition = Vector3.Lerp(this.m_SmoothPosition, base.Transform.position, Time.deltaTime);
	}

	protected virtual void UpdateCollisions()
	{
		if (this.m_GroundHit.transform != null && this.m_GroundHit.transform != this.m_LastGroundHit.transform)
		{
			this.m_SmoothPosition.y = base.Transform.position.y;
			if (!this.MotorFreeFly)
			{
				this.m_FallImpact = -this.m_HighestFallSpeed * Time.timeScale;
			}
			else
			{
				this.m_FallImpact = -(this.CharacterController.velocity.y * 0.01f) * Time.timeScale;
			}
			this.DeflectDownForce();
			this.m_HighestFallSpeed = 0f;
			this.Player.FallImpact.Send(this.m_FallImpact);
			this.m_MotorThrottle.y = 0f;
			this.m_MotorJumpForceAcc = 0f;
			this.m_MotorJumpForceHoldSkipFrames = 0;
		}
		else
		{
			this.m_FallImpact = 0f;
		}
		this.m_LastGroundHit = this.m_GroundHit;
		if (this.m_PredictedPos.y > base.Transform.position.y && (this.m_ExternalForce.y > 0f || this.m_MotorThrottle.y > 0f))
		{
			this.DeflectUpForce();
		}
		if (this.m_PredictedPos.x != base.Transform.position.x || (this.m_PredictedPos.z != base.Transform.position.z && this.m_ExternalForce != Vector3.zero))
		{
			this.DeflectHorizontalForce();
		}
	}

	private void UpdatePlatformMove()
	{
		if (this.m_Platform == null)
		{
			return;
		}
		this.m_PositionOnPlatform = this.m_Platform.InverseTransformPoint(this.m_Transform.position);
		this.m_Player.Rotation.Set(new Vector2(this.m_Player.Rotation.Get().x, this.m_Player.Rotation.Get().y - Mathf.DeltaAngle(this.m_Platform.eulerAngles.y, this.m_LastPlatformAngle)));
		this.m_LastPlatformAngle = this.m_Platform.eulerAngles.y;
		this.m_LastPlatformPos = this.m_Platform.position;
		this.m_SmoothPosition = base.Transform.position;
	}

	protected virtual void UpdateSlopeFactor()
	{
		if (!this.m_Grounded)
		{
			this.m_SlopeFactor = 1f;
			return;
		}
		this.m_SlopeFactor = 1f + (1f - Vector3.Angle(this.m_GroundHit.normal, this.m_MotorThrottle) / 90f);
		if (Mathf.Abs(1f - this.m_SlopeFactor) < 0.01f)
		{
			this.m_SlopeFactor = 1f;
		}
		else if (this.m_SlopeFactor > 1f)
		{
			if (this.MotorSlopeSpeedDown == 1f)
			{
				this.m_SlopeFactor = 1f / this.m_SlopeFactor;
				this.m_SlopeFactor *= 1.2f;
			}
			else
			{
				this.m_SlopeFactor *= this.MotorSlopeSpeedDown;
			}
		}
		else
		{
			if (this.MotorSlopeSpeedUp == 1f)
			{
				this.m_SlopeFactor *= 1.2f;
			}
			else
			{
				this.m_SlopeFactor *= this.MotorSlopeSpeedUp;
			}
			this.m_SlopeFactor = ((this.GroundAngle <= this.m_CharacterController.slopeLimit) ? this.m_SlopeFactor : 0f);
		}
	}

	public virtual void SetPosition(Vector3 position)
	{
		base.Transform.position = position;
		this.m_SmoothPosition = position;
		this.m_PrevPos = position;
	}

	protected virtual void AddForceInternal(Vector3 force)
	{
		this.m_ExternalForce += force;
	}

	public virtual void AddForce(float x, float y, float z)
	{
		this.AddForce(new Vector3(x, y, z));
	}

	public virtual void AddForce(Vector3 force)
	{
		if (Time.timeScale >= 1f)
		{
			this.AddForceInternal(force);
		}
		else
		{
			this.AddSoftForce(force, 1f);
		}
	}

	public void AddRepelForce(Vector3 force)
	{
		this.AddForce(force);
	}

	public virtual void AddSoftForce(Vector3 force, float frames)
	{
		force /= Time.timeScale;
		frames = Mathf.Clamp(frames, 1f, 120f);
		this.AddForceInternal(force / frames);
		for (int i = 0; i < Mathf.RoundToInt(frames) - 1; i++)
		{
			this.m_SmoothForceFrame[i] += force / frames;
		}
	}

	public virtual void StopSoftForce()
	{
		for (int i = 0; i < 120; i++)
		{
			if (this.m_SmoothForceFrame[i] == Vector3.zero)
			{
				break;
			}
			this.m_SmoothForceFrame[i] = Vector3.zero;
		}
	}

	public virtual void Stop()
	{
		this.m_CharacterController.Move(Vector3.zero);
		this.m_MotorThrottle = Vector3.zero;
		this.m_ExternalForce = Vector3.zero;
		this.StopSoftForce();
		this.m_MoveVector = Vector2.zero;
		this.m_FallSpeed = 0f;
		this.m_SmoothPosition = base.Transform.position;
	}

	public virtual void DeflectDownForce()
	{
		if (this.GroundAngle > this.PhysicsSlopeSlideLimit)
		{
			this.m_SlopeSlideSpeed = this.m_FallImpact * (0.25f * Time.timeScale);
		}
		if (this.GroundAngle > 85f)
		{
			this.m_MotorThrottle += vp_Utility.HorizontalVector(this.GroundNormal * this.m_FallImpact);
			this.m_Grounded = false;
		}
	}

	protected virtual void DeflectUpForce()
	{
		if (!this.m_HeadContact)
		{
			return;
		}
		this.m_NewDir = Vector3.Cross(Vector3.Cross(this.m_CeilingHit.normal, Vector3.up), this.m_CeilingHit.normal);
		this.m_ForceImpact = this.m_MotorThrottle.y + this.m_ExternalForce.y;
		this.m_ForceImpact *= 0.3f;
		Vector3 a = this.m_NewDir * (this.m_MotorThrottle.y + this.m_ExternalForce.y) * (1f - this.PhysicsWallFriction);
		this.m_ForceImpact -= a.magnitude;
		this.AddForce(a * Time.timeScale);
		this.m_MotorThrottle.y = 0f;
		this.m_ExternalForce.y = 0f;
		this.m_FallSpeed = 0f;
		this.m_NewDir.x = base.Transform.InverseTransformDirection(this.m_NewDir).x;
		this.Player.HeadImpact.Send((this.m_NewDir.x >= 0f && (this.m_NewDir.x != 0f || UnityEngine.Random.value >= 0.5f)) ? this.m_ForceImpact : (-this.m_ForceImpact));
	}

	protected virtual void DeflectHorizontalForce()
	{
		this.m_PredictedPos.y = base.Transform.position.y;
		this.m_PrevPos.y = base.Transform.position.y;
		this.m_PrevDir = (this.m_PredictedPos - this.m_PrevPos).normalized;
		this.CapsuleBottom = this.m_PrevPos + Vector3.up * this.m_CharacterController.radius;
		this.CapsuleTop = this.CapsuleBottom + Vector3.up * (this.m_CharacterController.height - this.m_CharacterController.radius * 2f);
		if (!Physics.CapsuleCast(this.CapsuleBottom, this.CapsuleTop, this.m_CharacterController.radius, this.m_PrevDir, out this.m_WallHit, Vector3.Distance(this.m_PrevPos, this.m_PredictedPos), vp_Layer.GetExternalBlockers()))
		{
			return;
		}
		this.m_NewDir = Vector3.Cross(this.m_WallHit.normal, Vector3.up).normalized;
		if (Vector3.Dot(Vector3.Cross(this.m_WallHit.point - base.Transform.position, this.m_PrevPos - base.Transform.position), Vector3.up) > 0f)
		{
			this.m_NewDir = -this.m_NewDir;
		}
		this.m_ForceMultiplier = Mathf.Abs(Vector3.Dot(this.m_PrevDir, this.m_NewDir)) * (1f - this.PhysicsWallFriction);
		if (this.PhysicsWallBounce > 0f)
		{
			this.m_NewDir = Vector3.Lerp(this.m_NewDir, Vector3.Reflect(this.m_PrevDir, this.m_WallHit.normal), this.PhysicsWallBounce);
			this.m_ForceMultiplier = Mathf.Lerp(this.m_ForceMultiplier, 1f, this.PhysicsWallBounce * (1f - this.PhysicsWallFriction));
		}
		this.m_ForceImpact = 0f;
		float y = this.m_ExternalForce.y;
		this.m_ExternalForce.y = 0f;
		this.m_ForceImpact = this.m_ExternalForce.magnitude;
		this.m_ExternalForce = this.m_NewDir * this.m_ExternalForce.magnitude * this.m_ForceMultiplier;
		this.m_ForceImpact -= this.m_ExternalForce.magnitude;
		for (int i = 0; i < 120; i++)
		{
			if (this.m_SmoothForceFrame[i] == Vector3.zero)
			{
				break;
			}
			this.m_SmoothForceFrame[i] = this.m_SmoothForceFrame[i].magnitude * this.m_NewDir * this.m_ForceMultiplier;
		}
		this.m_ExternalForce.y = y;
	}

	protected virtual void OnControllerColliderHit(ControllerColliderHit hit)
	{
		Rigidbody attachedRigidbody = hit.collider.attachedRigidbody;
		if (attachedRigidbody == null || attachedRigidbody.isKinematic)
		{
			return;
		}
		if (hit.moveDirection.y < -0.3f)
		{
			return;
		}
		Vector3 a = new Vector3(hit.moveDirection.x, 0f, hit.moveDirection.z);
		attachedRigidbody.velocity = a * (this.PhysicsPushForce / attachedRigidbody.mass);
	}

	protected virtual bool CanStart_Jump()
	{
		return this.MotorFreeFly || (this.m_Grounded && this.m_MotorJumpDone && this.GroundAngle <= this.m_CharacterController.slopeLimit);
	}

	protected virtual bool CanStart_Run()
	{
		return !this.Player.Crouch.Active;
	}

	protected virtual void OnStart_Jump()
	{
		this.m_MotorJumpDone = false;
		this.m_SmoothPosition.y = base.Transform.position.y;
		if (this.MotorFreeFly && !this.Grounded)
		{
			return;
		}
		this.m_MotorThrottle.y = this.MotorJumpForce / Time.timeScale;
	}

	protected virtual void OnStop_Jump()
	{
		this.m_MotorJumpDone = true;
	}

	protected virtual bool CanStop_Crouch()
	{
		if (Physics.SphereCast(new Ray(base.Transform.position, Vector3.up), this.m_CharacterController.radius, this.m_NormalHeight - this.m_CharacterController.radius + 0.01f, vp_Layer.GetExternalBlockers()))
		{
			this.Player.Crouch.NextAllowedStopTime = Time.time;
			return false;
		}
		return true;
	}

	public static bool CanStop_CrouchNoLimit()
	{
		return !Physics.Raycast(BasePlayer.go.transform.position, Vector3.up, 2f, vp_Layer.GetExternalBlockers());
	}

	protected virtual void OnStart_Crouch()
	{
		if (this.MotorFreeFly && !this.Grounded)
		{
			return;
		}
		this.CharacterController.height = this.m_CrouchHeight;
		this.CharacterController.center = this.m_CrouchCenter;
	}

	protected virtual void OnStop_Crouch()
	{
		this.CharacterController.height = this.m_NormalHeight;
		this.CharacterController.center = this.m_NormalCenter;
	}

	protected virtual void OnMessage_ForceImpact(Vector3 force)
	{
		this.AddForce(force);
	}

	protected virtual void OnMessage_Stop()
	{
		this.Stop();
	}

	public Vector2 GetMoveVector()
	{
		return this.m_MoveVector * this.MotorAcceleration * 5f;
	}
}
