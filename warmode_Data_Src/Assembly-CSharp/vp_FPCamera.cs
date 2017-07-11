using System;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.ImageEffects;

[RequireComponent(typeof(Camera)), RequireComponent(typeof(AudioListener))]
public class vp_FPCamera : vp_Component
{
	public delegate void BobStepDelegate();

	public static vp_FPCamera cs = null;

	public static vp_FPWeapon lastWeapon = null;

	public static vp_FPWeapon currWeapon = null;

	public static vp_FPWeapon returnWeapon = null;

	private static Vector3 r90 = new Vector3(0f, 90f, 0f);

	public static DepthOfField dof = null;

	private bool m_MouseFreeze = true;

	public vp_FPController FPController;

	protected static Vector2 MouseSensitivity = new Vector2(4f, 4f);

	protected static Vector2 ZoomSensitivity = new Vector2(2f, 2f);

	public static int MouseSmoothSteps = 10;

	public static float MouseSmoothWeight = 0.5f;

	public bool MouseAcceleration;

	public float MouseAccelerationThreshold = 0.4f;

	protected Vector2 m_MouseMove = Vector2.zero;

	protected List<Vector2> m_MouseSmoothBuffer = new List<Vector2>();

	private static float RenderingFieldOfView = 60f;

	public float RenderingZoomDamping = 0.1f;

	protected float m_FinalZoomTime;

	public Vector3 PositionOffset = new Vector3(0f, 1.75f, 0.1f);

	public float PositionGroundLimit = 0.1f;

	public float PositionSpringStiffness = 0.01f;

	public float PositionSpringDamping = 0.25f;

	public float PositionSpring2Stiffness = 0.95f;

	public float PositionSpring2Damping = 0.25f;

	public float PositionKneeling = 0.025f;

	public int PositionKneelingSoftness = 1;

	public float PositionEarthQuakeFactor = 1f;

	protected vp_Spring m_PositionSpring;

	protected vp_Spring m_PositionSpring2;

	protected bool m_DrawCameraCollisionDebugLine;

	public Vector2 RotationPitchLimit = new Vector2(90f, -90f);

	public Vector2 RotationYawLimit = new Vector2(-360f, 360f);

	public float RotationSpringStiffness = 0.01f;

	public float RotationSpringDamping = 0.25f;

	public float RotationKneeling = 0.025f;

	public int RotationKneelingSoftness = 1;

	public float RotationStrafeRoll = 0.01f;

	public float RotationEarthQuakeFactor;

	protected float m_Pitch;

	protected float m_Yaw;

	protected vp_Spring m_RotationSpring;

	protected Vector2 m_InitialRotation = Vector2.zero;

	public float ShakeSpeed;

	public Vector3 ShakeAmplitude = new Vector3(10f, 10f, 0f);

	protected Vector3 m_Shake = Vector3.zero;

	public Vector4 BobRate = new Vector4(0f, 1.4f, 0f, 0.7f);

	public Vector4 BobAmplitude = new Vector4(0f, 0.25f, 0f, 0.5f);

	public float BobInputVelocityScale = 1f;

	public float BobMaxInputVelocity = 100f;

	public bool BobRequireGroundContact = true;

	protected float m_LastBobSpeed;

	protected Vector4 m_CurrentBobAmp = Vector4.zero;

	protected Vector4 m_CurrentBobVal = Vector4.zero;

	protected float m_BobSpeed;

	public vp_FPCamera.BobStepDelegate BobStepCallback;

	public float BobStepThreshold = 10f;

	protected float m_LastUpBob;

	protected bool m_BobWasElevating;

	protected Vector3 m_CameraCollisionStartPos = Vector3.zero;

	protected Vector3 m_CameraCollisionEndPos = Vector3.zero;

	protected RaycastHit m_CameraHit;

	private GameObject cameraFlashFX;

	private Material matFlashFX;

	private float flashFXTime;

	private float flashFXFading = 1f;

	private float flashFXVal;

	private Camera weaponCam;

	private vp_FPPlayerEventHandler m_Player;

	public bool DrawCameraCollisionDebugLine
	{
		get
		{
			return this.m_DrawCameraCollisionDebugLine;
		}
		set
		{
			this.m_DrawCameraCollisionDebugLine = value;
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

	public Vector2 Angle
	{
		get
		{
			return new Vector2(this.m_Pitch, this.m_Yaw);
		}
		set
		{
			this.Pitch = value.x;
			this.Yaw = value.y;
		}
	}

	public Vector3 Forward
	{
		get
		{
			return this.m_Transform.forward;
		}
	}

	public float Pitch
	{
		get
		{
			return this.m_Pitch;
		}
		set
		{
			if (value > 90f)
			{
				value -= 360f;
			}
			this.m_Pitch = value;
		}
	}

	public float Yaw
	{
		get
		{
			return this.m_Yaw;
		}
		set
		{
			this.m_Yaw = value;
		}
	}

	protected virtual Vector2 OnValue_Rotation
	{
		get
		{
			return this.Angle;
		}
		set
		{
			this.Angle = value;
		}
	}

	protected virtual Vector3 OnValue_Forward
	{
		get
		{
			return this.Forward;
		}
	}

	public static void SetFov(float val)
	{
		vp_FPCamera.RenderingFieldOfView = val;
	}

	public static void SetMouseSensitivity(float val)
	{
		if (Options.invertmouse > 0)
		{
			vp_FPCamera.MouseSensitivity = new Vector2(val, -val);
		}
		else
		{
			vp_FPCamera.MouseSensitivity = new Vector2(val, val);
		}
	}

	public static void SetZoomSensitivity(float val)
	{
		if (Options.invertmouse > 0)
		{
			vp_FPCamera.ZoomSensitivity = new Vector2(val, -val);
		}
		else
		{
			vp_FPCamera.ZoomSensitivity = new Vector2(val, val);
		}
	}

	protected override void Awake()
	{
		vp_FPCamera.cs = this;
		base.Awake();
		this.FPController = base.Root.GetComponent<vp_FPController>();
		this.m_InitialRotation = new Vector2(base.Transform.eulerAngles.y, base.Transform.eulerAngles.x);
		base.Parent.gameObject.layer = 30;
		foreach (Transform transform in base.Parent)
		{
			transform.gameObject.layer = 30;
		}
		base.GetComponent<Camera>().cullingMask &= 1056964607;
		base.GetComponent<Camera>().depth = 0f;
		this.weaponCam = null;
		foreach (Transform transform2 in base.Transform)
		{
			this.weaponCam = (Camera)transform2.GetComponent(typeof(Camera));
			if (this.weaponCam != null)
			{
				this.weaponCam.transform.localPosition = Vector3.zero;
				this.weaponCam.transform.localEulerAngles = Vector3.zero;
				this.weaponCam.clearFlags = CameraClearFlags.Depth;
				this.weaponCam.cullingMask = -2147483648;
				this.weaponCam.depth = 1f;
				this.weaponCam.farClipPlane = 100f;
				this.weaponCam.nearClipPlane = 0.01f;
				this.weaponCam.fov = 60f;
				vp_FPCamera.dof = this.weaponCam.GetComponent<DepthOfField>();
				break;
			}
		}
		this.m_PositionSpring = new vp_Spring(base.Transform, vp_Spring.UpdateMode.Position, false);
		this.m_PositionSpring.MinVelocity = 1E-05f;
		this.m_PositionSpring.RestState = this.PositionOffset;
		this.m_PositionSpring2 = new vp_Spring(base.Transform, vp_Spring.UpdateMode.PositionAdditive, false);
		this.m_PositionSpring2.MinVelocity = 1E-05f;
		this.m_RotationSpring = new vp_Spring(base.Transform, vp_Spring.UpdateMode.RotationAdditive, false);
		this.m_RotationSpring.MinVelocity = 1E-05f;
		this.cameraFlashFX = GameObject.Find("WeaponCamera/FlashFX");
		this.matFlashFX = this.cameraFlashFX.GetComponent<MeshRenderer>().material;
		this.matFlashFX.color = Color.white;
		this.SetFlashFX(0f, 3.5f);
	}

	protected override void Start()
	{
		base.Start();
		this.Refresh();
		this.SnapSprings();
		this.SnapZoom();
	}

	protected override void Init()
	{
		base.Init();
	}

	protected override void Update()
	{
		if (this.flashFXTime != 0f && Time.time > this.flashFXTime)
		{
			this.flashFXVal -= Time.deltaTime * this.flashFXFading * 0.4f;
			if (this.flashFXVal <= 0f)
			{
				this.SetFlashFX(0f, 0f);
			}
			else
			{
				this.matFlashFX.color = new Color(1f, 1f, 1f, this.flashFXVal);
			}
		}
		base.Update();
		if (Time.timeScale == 0f)
		{
			return;
		}
		this.UpdateMouseLook();
	}

	protected override void FixedUpdate()
	{
		base.FixedUpdate();
		if (Time.timeScale == 0f)
		{
			return;
		}
		this.UpdateZoom();
		this.UpdateSwaying();
		if (!SpecCam.show)
		{
			this.UpdateBob();
		}
		this.UpdateEarthQuake();
		this.UpdateShakes();
		this.UpdateSprings();
	}

	protected override void LateUpdate()
	{
		base.LateUpdate();
		if (Time.timeScale == 0f)
		{
			return;
		}
		this.m_Transform.position = this.FPController.SmoothPosition;
		this.m_Transform.localPosition += this.m_PositionSpring.State + this.m_PositionSpring2.State;
		Quaternion lhs = Quaternion.AngleAxis(this.m_Yaw + this.m_InitialRotation.x, Vector3.up);
		Quaternion rhs = Quaternion.AngleAxis(0f, Vector3.left);
		base.Parent.rotation = vp_Utility.NaNSafeQuaternion(lhs * rhs, base.Parent.rotation);
		rhs = Quaternion.AngleAxis(-this.m_Pitch - this.m_InitialRotation.y, Vector3.left);
		base.Transform.rotation = vp_Utility.NaNSafeQuaternion(lhs * rhs, base.Transform.rotation);
		base.Transform.localEulerAngles += vp_Utility.NaNSafeVector3(Vector3.forward * this.m_RotationSpring.State.z, default(Vector3));
		base.Transform.localEulerAngles += vp_Utility.NaNSafeVector3(Vector3.right * this.m_RotationSpring.State.x, default(Vector3));
		base.Transform.localEulerAngles += vp_Utility.NaNSafeVector3(Vector3.up * this.m_RotationSpring.State.y, default(Vector3));
		if (vp_FPCamera.currWeapon != null && vp_FPCamera.currWeapon.m_WeaponCamera001 != null)
		{
			base.Transform.localEulerAngles += vp_FPCamera.currWeapon.m_WeaponCamera001.transform.localEulerAngles - vp_FPCamera.currWeapon.m_WeaponCamera001Zero - vp_FPCamera.r90;
		}
		DeadCam.LateUpdate();
		SpecCam.LateUpdate();
	}

	protected virtual void DoCameraCollision()
	{
		this.m_CameraCollisionStartPos = this.FPController.Transform.TransformPoint(0f, this.PositionOffset.y, 0f);
		this.m_CameraCollisionEndPos = base.Transform.position + (base.Transform.position - this.m_CameraCollisionStartPos).normalized * this.FPController.CharacterController.radius;
		if (Physics.Linecast(this.m_CameraCollisionStartPos, this.m_CameraCollisionEndPos, out this.m_CameraHit, vp_Layer.GetExternalBlockers()) && !this.m_CameraHit.collider.isTrigger)
		{
			base.Transform.position = this.m_CameraHit.point - (this.m_CameraHit.point - this.m_CameraCollisionStartPos).normalized * this.FPController.CharacterController.radius;
		}
		if (base.Transform.localPosition.y < this.PositionGroundLimit)
		{
			base.Transform.localPosition = new Vector3(base.Transform.localPosition.x, this.PositionGroundLimit, base.Transform.localPosition.z);
		}
	}

	public virtual void AddForce(Vector3 force)
	{
		this.m_PositionSpring.AddForce(force);
	}

	public virtual void AddForce(float x, float y, float z)
	{
		this.AddForce(new Vector3(x, y, z));
	}

	public virtual void AddForce2(Vector3 force)
	{
		this.m_PositionSpring2.AddForce(force);
	}

	public void AddForce2(float x, float y, float z)
	{
		this.AddForce2(new Vector3(x, y, z));
	}

	public virtual void AddRollForce(float force)
	{
		this.m_RotationSpring.AddForce(Vector3.forward * force);
	}

	public virtual void AddRotationForce(Vector3 force)
	{
		this.m_RotationSpring.AddForce(force);
	}

	public void AddRotationForce(float x, float y, float z)
	{
		this.AddRotationForce(new Vector3(x, y, z));
	}

	public void SetMouseFreeze(bool val)
	{
		this.m_MouseFreeze = val;
		if (!this.m_MouseFreeze)
		{
			vp_FPInput.cs.ForceCursor = false;
			vp_FPInput.cs.AllowGameplayInput = true;
			Cursor.visible = false;
			Screen.lockCursor = true;
		}
		else
		{
			Screen.lockCursor = false;
		}
	}

	protected virtual void UpdateMouseLook()
	{
		if (this.m_MouseFreeze)
		{
			vp_FPInput.cs.ForceCursor = true;
			vp_FPInput.cs.AllowGameplayInput = false;
			Cursor.visible = true;
			return;
		}
		this.m_MouseMove.x = Input.GetAxisRaw("Mouse X");
		this.m_MouseMove.y = Input.GetAxisRaw("Mouse Y");
		Vector2 vector;
		if (Options.rawinput == 0)
		{
			vp_FPCamera.MouseSmoothSteps = Mathf.Clamp(vp_FPCamera.MouseSmoothSteps, 1, 20);
			vp_FPCamera.MouseSmoothWeight = Mathf.Clamp01(vp_FPCamera.MouseSmoothWeight);
			while (this.m_MouseSmoothBuffer.Count > vp_FPCamera.MouseSmoothSteps)
			{
				this.m_MouseSmoothBuffer.RemoveAt(0);
			}
			this.m_MouseSmoothBuffer.Add(this.m_MouseMove);
			float num = 1f;
			Vector2 a = Vector2.zero;
			float num2 = 0f;
			for (int i = this.m_MouseSmoothBuffer.Count - 1; i > 0; i--)
			{
				a += this.m_MouseSmoothBuffer[i] * num;
				num2 += 1f * num;
				num *= vp_FPCamera.MouseSmoothWeight / base.Delta;
			}
			num2 = Mathf.Max(1f, num2);
			vector = vp_Utility.NaNSafeVector2(a / num2, default(Vector2));
		}
		else
		{
			vector = this.m_MouseMove;
		}
		if (this.inZoom())
		{
			this.m_Yaw += vector.x * (vp_FPCamera.ZoomSensitivity.x * (vp_FPCamera.RenderingFieldOfView / 50f));
			this.m_Pitch -= vector.y * (vp_FPCamera.ZoomSensitivity.y * (vp_FPCamera.RenderingFieldOfView / 50f));
		}
		else
		{
			this.m_Yaw += vector.x * vp_FPCamera.MouseSensitivity.x;
			this.m_Pitch -= vector.y * vp_FPCamera.MouseSensitivity.y;
		}
		this.m_Yaw = ((this.m_Yaw >= -360f) ? this.m_Yaw : (this.m_Yaw += 360f));
		this.m_Yaw = ((this.m_Yaw <= 360f) ? this.m_Yaw : (this.m_Yaw -= 360f));
		this.m_Yaw = Mathf.Clamp(this.m_Yaw, this.RotationYawLimit.x, this.RotationYawLimit.y);
		this.m_Pitch = ((this.m_Pitch >= -360f) ? this.m_Pitch : (this.m_Pitch += 360f));
		this.m_Pitch = ((this.m_Pitch <= 360f) ? this.m_Pitch : (this.m_Pitch -= 360f));
		this.m_Pitch = Mathf.Clamp(this.m_Pitch, -this.RotationPitchLimit.x, -this.RotationPitchLimit.y);
	}

	protected virtual void UpdateZoom()
	{
		if (this.m_FinalZoomTime <= Time.time)
		{
			return;
		}
		this.RenderingZoomDamping = Mathf.Max(this.RenderingZoomDamping, 0.01f);
		float t = 1f - (this.m_FinalZoomTime - Time.time) / this.RenderingZoomDamping;
		base.gameObject.GetComponent<Camera>().fov = Mathf.SmoothStep(base.gameObject.GetComponent<Camera>().fov, vp_FPCamera.RenderingFieldOfView, t);
	}

	public virtual void Zoom()
	{
		this.m_FinalZoomTime = Time.time + this.RenderingZoomDamping;
	}

	public virtual bool inZoom()
	{
		return vp_FPCamera.RenderingFieldOfView < 55f;
	}

	public virtual void SnapZoom()
	{
		base.gameObject.GetComponent<Camera>().fov = vp_FPCamera.RenderingFieldOfView;
	}

	protected virtual void UpdateShakes()
	{
		if (this.ShakeSpeed != 0f)
		{
			this.m_Yaw -= this.m_Shake.y;
			this.m_Pitch -= this.m_Shake.x;
			this.m_Shake = Vector3.Scale(vp_SmoothRandom.GetVector3Centered(this.ShakeSpeed), this.ShakeAmplitude);
			this.m_Yaw += this.m_Shake.y;
			this.m_Pitch += this.m_Shake.x;
			this.m_RotationSpring.AddForce(Vector3.forward * this.m_Shake.z * Time.timeScale);
		}
	}

	protected virtual void UpdateBob()
	{
		if (this.BobAmplitude == Vector4.zero || this.BobRate == Vector4.zero)
		{
			return;
		}
		this.m_BobSpeed = ((!this.BobRequireGroundContact || this.FPController.Grounded) ? this.FPController.CharacterController.velocity.sqrMagnitude : 0f);
		this.m_BobSpeed = Mathf.Min(this.m_BobSpeed * this.BobInputVelocityScale, this.BobMaxInputVelocity);
		this.m_BobSpeed = Mathf.Round(this.m_BobSpeed * 1000f) / 1000f;
		if (this.m_BobSpeed == 0f)
		{
			this.m_BobSpeed = Mathf.Min(this.m_LastBobSpeed * 0.93f, this.BobMaxInputVelocity);
		}
		this.m_CurrentBobAmp.y = this.m_BobSpeed * (this.BobAmplitude.y * -0.0001f);
		this.m_CurrentBobVal.y = Mathf.Cos(Time.time * (this.BobRate.y * 10f)) * this.m_CurrentBobAmp.y;
		this.m_CurrentBobAmp.x = this.m_BobSpeed * (this.BobAmplitude.x * 0.0001f);
		this.m_CurrentBobVal.x = Mathf.Cos(Time.time * (this.BobRate.x * 10f)) * this.m_CurrentBobAmp.x;
		this.m_CurrentBobAmp.z = this.m_BobSpeed * (this.BobAmplitude.z * 0.0001f);
		this.m_CurrentBobVal.z = Mathf.Cos(Time.time * (this.BobRate.z * 10f)) * this.m_CurrentBobAmp.z;
		this.m_CurrentBobAmp.w = this.m_BobSpeed * (this.BobAmplitude.w * 0.0001f);
		this.m_CurrentBobVal.w = Mathf.Cos(Time.time * (this.BobRate.w * 10f)) * this.m_CurrentBobAmp.w;
		this.m_PositionSpring.AddForce(this.m_CurrentBobVal * Time.timeScale);
		this.AddRollForce(this.m_CurrentBobVal.w * Time.timeScale);
		this.m_LastBobSpeed = this.m_BobSpeed;
		this.DetectBobStep(this.m_BobSpeed, this.m_CurrentBobVal.y);
	}

	protected virtual void DetectBobStep(float speed, float upBob)
	{
		if (this.BobStepCallback == null)
		{
			return;
		}
		if (speed < this.BobStepThreshold)
		{
			return;
		}
		bool flag = this.m_LastUpBob < upBob;
		this.m_LastUpBob = upBob;
		if (flag && !this.m_BobWasElevating)
		{
			this.BobStepCallback();
		}
		this.m_BobWasElevating = flag;
	}

	protected virtual void UpdateSwaying()
	{
		this.AddRollForce((base.Transform.InverseTransformDirection(this.FPController.CharacterController.velocity * 0.016f) * Time.timeScale).x * this.RotationStrafeRoll);
	}

	protected virtual void UpdateEarthQuake()
	{
		if (this.Player == null)
		{
			return;
		}
		if (!this.Player.Earthquake.Active)
		{
			return;
		}
		if (this.m_PositionSpring.State.y >= this.m_PositionSpring.RestState.y)
		{
			Vector3 o = this.Player.EarthQuakeForce.Get();
			o.y = -o.y;
			this.Player.EarthQuakeForce.Set(o);
		}
		this.m_PositionSpring.AddForce(this.Player.EarthQuakeForce.Get() * this.PositionEarthQuakeFactor);
		this.m_RotationSpring.AddForce(Vector3.forward * (-this.Player.EarthQuakeForce.Get().x * 2f) * this.RotationEarthQuakeFactor);
	}

	protected virtual void UpdateSprings()
	{
		this.m_PositionSpring.FixedUpdate();
		this.m_PositionSpring2.FixedUpdate();
		this.m_RotationSpring.FixedUpdate();
	}

	public virtual void DoBomb(Vector3 positionForce, float minRollForce, float maxRollForce)
	{
		this.AddForce2(positionForce);
		float num = UnityEngine.Random.Range(minRollForce, maxRollForce);
		if (UnityEngine.Random.value > 0.5f)
		{
			num = -num;
		}
		this.AddRollForce(num);
	}

	public override void Refresh()
	{
		if (!Application.isPlaying)
		{
			return;
		}
		if (this.m_PositionSpring != null)
		{
			this.m_PositionSpring.Stiffness = new Vector3(this.PositionSpringStiffness, this.PositionSpringStiffness, this.PositionSpringStiffness);
			this.m_PositionSpring.Damping = Vector3.one - new Vector3(this.PositionSpringDamping, this.PositionSpringDamping, this.PositionSpringDamping);
			this.m_PositionSpring.MinState.y = this.PositionGroundLimit;
			this.m_PositionSpring.RestState = this.PositionOffset;
		}
		if (this.m_PositionSpring2 != null)
		{
			this.m_PositionSpring2.Stiffness = new Vector3(this.PositionSpring2Stiffness, this.PositionSpring2Stiffness, this.PositionSpring2Stiffness);
			this.m_PositionSpring2.Damping = Vector3.one - new Vector3(this.PositionSpring2Damping, this.PositionSpring2Damping, this.PositionSpring2Damping);
			this.m_PositionSpring2.MinState.y = -this.PositionOffset.y + this.PositionGroundLimit;
		}
		if (this.m_RotationSpring != null)
		{
			this.m_RotationSpring.Stiffness = new Vector3(this.RotationSpringStiffness, this.RotationSpringStiffness, this.RotationSpringStiffness);
			this.m_RotationSpring.Damping = Vector3.one - new Vector3(this.RotationSpringDamping, this.RotationSpringDamping, this.RotationSpringDamping);
		}
		this.Zoom();
	}

	public virtual void SnapSprings()
	{
		if (this.m_PositionSpring != null)
		{
			this.m_PositionSpring.RestState = this.PositionOffset;
			this.m_PositionSpring.State = this.PositionOffset;
			this.m_PositionSpring.Stop(true);
		}
		if (this.m_PositionSpring2 != null)
		{
			this.m_PositionSpring2.RestState = Vector3.zero;
			this.m_PositionSpring2.State = Vector3.zero;
			this.m_PositionSpring2.Stop(true);
		}
		if (this.m_RotationSpring != null)
		{
			this.m_RotationSpring.RestState = Vector3.zero;
			this.m_RotationSpring.State = Vector3.zero;
			this.m_RotationSpring.Stop(true);
		}
	}

	public virtual void StopSprings()
	{
		if (this.m_PositionSpring != null)
		{
			this.m_PositionSpring.Stop(true);
		}
		if (this.m_PositionSpring2 != null)
		{
			this.m_PositionSpring2.Stop(true);
		}
		if (this.m_RotationSpring != null)
		{
			this.m_RotationSpring.Stop(true);
		}
		this.m_BobSpeed = 0f;
		this.m_LastBobSpeed = 0f;
	}

	public virtual void Stop()
	{
		this.SnapSprings();
		this.SnapZoom();
		this.Refresh();
	}

	public virtual void SetRotation(Vector2 eulerAngles, bool stop = true, bool resetInitialRotation = true)
	{
		this.Angle = eulerAngles;
		if (stop)
		{
			this.Stop();
		}
		if (resetInitialRotation)
		{
			this.m_InitialRotation = Vector2.zero;
		}
	}

	protected virtual void OnMessage_FallImpact(float impact)
	{
		impact = Mathf.Abs(impact * 55f);
		float num = impact * this.PositionKneeling;
		float num2 = impact * this.RotationKneeling;
		num = Mathf.SmoothStep(0f, 1f, num);
		num2 = Mathf.SmoothStep(0f, 1f, num2);
		num2 = Mathf.SmoothStep(0f, 1f, num2);
		if (this.m_PositionSpring != null)
		{
			this.m_PositionSpring.AddSoftForce(Vector3.down * num, (float)this.PositionKneelingSoftness);
		}
		if (this.m_RotationSpring != null)
		{
			float d = (UnityEngine.Random.value <= 0.5f) ? (-(num2 * 2f)) : (num2 * 2f);
			this.m_RotationSpring.AddSoftForce(Vector3.forward * d, (float)this.RotationKneelingSoftness);
		}
	}

	protected virtual void OnMessage_HeadImpact(float impact)
	{
		if (this.m_RotationSpring != null && Mathf.Abs(this.m_RotationSpring.State.z) < 30f)
		{
			this.m_RotationSpring.AddForce(Vector3.forward * (impact * 20f) * Time.timeScale);
		}
	}

	protected virtual void OnMessage_GroundStomp(float impact)
	{
		this.AddForce2(new Vector3(0f, -1f, 0f) * impact);
	}

	protected virtual void OnMessage_BombShake(float impact)
	{
		this.DoBomb(new Vector3(1f, -10f, 1f) * impact, 1f, 2f);
	}

	protected virtual void OnStart_Zoom()
	{
		if (this.Player == null)
		{
			return;
		}
		this.Player.Run.Stop(0f);
	}

	protected virtual bool CanStart_Run()
	{
		return this.Player == null || !this.Player.Zoom.Active;
	}

	protected virtual void OnMessage_Stop()
	{
		this.Stop();
	}

	public void SetFlashFX(float depth, float time = 3.5f)
	{
		if (depth == 0f)
		{
			this.cameraFlashFX.SetActive(false);
			this.flashFXTime = 0f;
			this.flashFXVal = 0f;
		}
		else
		{
			this.cameraFlashFX.SetActive(true);
			this.flashFXVal = depth;
			this.matFlashFX.color = new Color(1f, 1f, 1f, this.flashFXVal);
			this.flashFXTime = Time.time + time;
			this.flashFXFading = 4f - time;
		}
	}
}
