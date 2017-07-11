using System;
using System.Collections.Generic;
using UnityEngine;

public class vp_FPWeapon : vp_Component
{
	public static vp_FPWeapon cs = null;

	public string WeaponPrefab;

	private GameObject pgoWeaponPrefab;

	public GameObject m_WeaponModel;

	public GameObject m_WeaponCoreModel;

	public GameObject m_WeaponCamera001;

	public GameObject[] m_HandModel = new GameObject[2];

	public GameObject m_PartModelFG;

	public GameObject m_PartModelFB;

	public GameObject m_PartModelSG;

	private int m_WeaponID = -1;

	private int m_WeaponSlot = -1;

	public Vector3 m_WeaponCamera001Zero = Vector3.zero;

	protected bool m_MaterialHD;

	private Texture2D m_WeaponZoom;

	private Texture2D tBlack;

	private static bool m_DrawZoom = false;

	private static float ForceDeactivate = 0f;

	private static float ForceHideObject = 0f;

	private static GameObject MuzzleLight = null;

	private static GameObject MuzzleWeaponLight = null;

	private static float MuzzleLightTime = 0f;

	public float dof_distance;

	private static string knife_animation_name = string.Empty;

	protected CharacterController Controller;

	public float RenderingZoomDamping = 0.1f;

	public float CameraZoom = 50f;

	protected float m_FinalZoomTime;

	public static GameObject[] WeaponOnMapArray = new GameObject[256];

	public static Vector3[] PrevWeaponPos = new Vector3[256];

	public static Vector3[] PrevWeaponRot = new Vector3[256];

	public float RenderingFieldOfView = 35f;

	public Vector2 RenderingClippingPlanes = new Vector2(0.01f, 10f);

	public float RenderingZScale = 1f;

	public Vector3 PositionOffset = new Vector3(0.15f, -0.15f, -0.15f);

	public float PositionSpringStiffness = 0.01f;

	public float PositionSpringDamping = 0.25f;

	public float PositionFallRetract = 1f;

	public float PositionPivotSpringStiffness = 0.01f;

	public float PositionPivotSpringDamping = 0.25f;

	public float PositionSpring2Stiffness = 0.95f;

	public float PositionSpring2Damping = 0.25f;

	public float PositionKneeling = 0.06f;

	public int PositionKneelingSoftness = 1;

	public Vector3 PositionWalkSlide = new Vector3(0.5f, 0.75f, 0.5f);

	public Vector3 PositionPivot = Vector3.zero;

	public Vector3 RotationPivot = Vector3.zero;

	public float PositionInputVelocityScale = 1f;

	public float PositionMaxInputVelocity = 25f;

	protected vp_Spring m_PositionSpring;

	protected vp_Spring m_PositionSpring2;

	protected vp_Spring m_PositionPivotSpring;

	protected vp_Spring m_RotationPivotSpring;

	protected GameObject m_WeaponCamera;

	protected GameObject m_WeaponGroup;

	protected GameObject m_Pivot;

	protected Transform m_WeaponGroupTransform;

	public Vector3 RotationOffset = Vector3.zero;

	public float RotationSpringStiffness = 0.01f;

	public float RotationSpringDamping = 0.25f;

	public float RotationPivotSpringStiffness = 0.01f;

	public float RotationPivotSpringDamping = 0.25f;

	public float RotationSpring2Stiffness = 0.95f;

	public float RotationSpring2Damping = 0.25f;

	public float RotationKneeling;

	public int RotationKneelingSoftness = 1;

	public Vector3 RotationLookSway = new Vector3(1f, 0.7f, 0f);

	public Vector3 RotationStrafeSway = new Vector3(0.3f, 1f, 1.5f);

	public Vector3 RotationFallSway = new Vector3(1f, -0.5f, -3f);

	public float RotationSlopeSway = 0.5f;

	public float RotationInputVelocityScale = 1f;

	public float RotationMaxInputVelocity = 15f;

	protected vp_Spring m_RotationSpring;

	protected vp_Spring m_RotationSpring2;

	protected Vector3 m_SwayVel = Vector3.zero;

	protected Vector3 m_FallSway = Vector3.zero;

	public float RetractionDistance;

	public Vector2 RetractionOffset = new Vector2(0f, 0f);

	public float RetractionRelaxSpeed = 0.25f;

	protected bool m_DrawRetractionDebugLine;

	public float ShakeSpeed = 0.05f;

	public Vector3 ShakeAmplitude = new Vector3(0.25f, 0f, 2f);

	protected Vector3 m_Shake = Vector3.zero;

	public Vector4 BobRate = new Vector4(0.9f, 0.45f, 0f, 0f);

	public Vector4 BobAmplitude = new Vector4(0.35f, 0.5f, 0f, 0f);

	public float BobInputVelocityScale = 1f;

	public float BobMaxInputVelocity = 100f;

	public bool BobRequireGroundContact = true;

	protected float m_LastBobSpeed;

	protected Vector4 m_CurrentBobAmp = Vector4.zero;

	protected Vector4 m_CurrentBobVal = Vector4.zero;

	protected float m_BobSpeed;

	public Vector3 StepPositionForce = new Vector3(0f, -0.0012f, -0.0012f);

	public Vector3 StepRotationForce = new Vector3(0f, 0f, 0f);

	public int StepSoftness = 4;

	public float StepMinVelocity;

	public float StepPositionBalance;

	public float StepRotationBalance;

	public float StepForceScale = 1f;

	protected float m_LastUpBob;

	protected bool m_BobWasElevating;

	protected Vector3 m_PosStep = Vector3.zero;

	protected Vector3 m_RotStep = Vector3.zero;

	public string SoundWield;

	public string SoundUnWield;

	public AudioClip sndWield;

	public AudioClip sndUnWield;

	public string AnimationWield;

	public string AnimationUnWield;

	private static int bombAnimationState = -1;

	private static int grenadeAnimationState = 1;

	private static int zombieAnimationState = -1;

	public List<UnityEngine.Object> AnimationAmbient = new List<UnityEngine.Object>();

	protected List<bool> m_AmbAnimPlayed = new List<bool>();

	public Vector2 AmbientInterval = new Vector2(2.5f, 7.5f);

	protected int m_CurrentAmbientAnimation;

	protected vp_Timer.Handle m_AnimationAmbientTimer = new vp_Timer.Handle();

	public Vector3 PositionExitOffset = new Vector3(0f, -1f, 0f);

	public Vector3 RotationExitOffset = new Vector3(40f, 0f, 0f);

	protected bool m_Wielded = true;

	private static RaycastHit hit;

	public float grenadeThrowStartingTime = -1f;

	public float grenadeThrowEndingTime = -1f;

	private static GameObject flashGo = null;

	private static GameObject smokeGo = null;

	protected Vector2 m_MouseMove = Vector2.zero;

	private vp_FPPlayerEventHandler m_Player;

	private bool started;

	private static int entuid = 0;

	public bool Wielded
	{
		get
		{
			return this.m_Wielded && base.Rendering;
		}
	}

	public GameObject WeaponCamera
	{
		get
		{
			return this.m_WeaponCamera;
		}
	}

	public GameObject WeaponModel
	{
		get
		{
			return this.m_WeaponModel;
		}
	}

	public GameObject WeaponCoreModel
	{
		get
		{
			return this.m_WeaponCoreModel;
		}
	}

	public int WeaponSlot
	{
		get
		{
			return this.m_WeaponSlot;
		}
	}

	public int WeaponID
	{
		get
		{
			return this.m_WeaponID;
		}
	}

	public Vector3 DefaultPosition
	{
		get
		{
			return (Vector3)base.DefaultState.Preset.GetFieldValue("PositionOffset");
		}
	}

	public Vector3 DefaultRotation
	{
		get
		{
			return (Vector3)base.DefaultState.Preset.GetFieldValue("RotationOffset");
		}
	}

	public bool DrawRetractionDebugLine
	{
		get
		{
			return this.m_DrawRetractionDebugLine;
		}
		set
		{
			this.m_DrawRetractionDebugLine = value;
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

	protected override void Awake()
	{
		vp_FPWeapon.cs = this;
		vp_FPWeapon.bombAnimationState = -1;
		vp_FPWeapon.grenadeAnimationState = -1;
		vp_FPWeapon.zombieAnimationState = -1;
		base.Awake();
		if (base.transform.parent == null)
		{
			Debug.LogError("Error (" + this + ") Must not be placed in scene root. Disabling self.");
			vp_Utility.Activate(base.gameObject, false);
			return;
		}
		this.Controller = base.Transform.root.GetComponent<CharacterController>();
		if (this.Controller == null)
		{
			Debug.LogError("Error (" + this + ") Could not find CharacterController. Disabling self.");
			vp_Utility.Activate(base.gameObject, false);
			return;
		}
		base.Transform.eulerAngles = Vector3.zero;
		foreach (Transform transform in base.Transform.parent)
		{
			Camera camera = (Camera)transform.GetComponent(typeof(Camera));
			if (camera != null)
			{
				this.m_WeaponCamera = camera.gameObject;
				break;
			}
		}
		if (base.GetComponent<Collider>() != null)
		{
			base.GetComponent<Collider>().enabled = false;
		}
		string text = string.Empty;
		int num = 0;
		int num2 = 0;
		if (int.TryParse(base.gameObject.name[0] + "\0", out num))
		{
			text += num.ToString();
		}
		if (int.TryParse(base.gameObject.name[1] + "\0", out num2))
		{
			text += num2.ToString();
		}
		int.TryParse(text, out this.m_WeaponID);
		this.m_WeaponSlot = WeaponData.GetData(this.m_WeaponID).slot;
		if (vp_FPWeapon.MuzzleLight == null || vp_FPWeapon.MuzzleWeaponLight == null)
		{
			vp_FPWeapon.MuzzleLight = GameObject.Find("MuzzleLight");
			vp_FPWeapon.MuzzleLight.GetComponent<Light>().enabled = false;
			vp_FPWeapon.MuzzleWeaponLight = GameObject.Find("MuzzleWeaponLight");
			vp_FPWeapon.MuzzleWeaponLight.GetComponent<Light>().enabled = false;
		}
		this.sndWield = SND.GetSoundByName(this.SoundWield);
		this.sndUnWield = SND.GetSoundByName(this.SoundUnWield);
	}

	public void SetMuzzleLight()
	{
		if (this.m_WeaponID == 8)
		{
			return;
		}
		vp_FPWeapon.MuzzleLight.GetComponent<Light>().enabled = true;
		vp_FPWeapon.MuzzleWeaponLight.GetComponent<Light>().enabled = true;
		vp_FPWeapon.MuzzleLightTime = Time.time + 0.05f;
	}

	protected override void Start()
	{
		base.Start();
		this.InstantiateWeaponModel();
		this.m_WeaponGroup = new GameObject(base.name + "Transform");
		this.m_WeaponGroupTransform = this.m_WeaponGroup.transform;
		this.m_WeaponGroupTransform.parent = base.Transform.parent;
		this.m_WeaponGroupTransform.localPosition = this.PositionOffset;
		vp_Layer.Set(this.m_WeaponGroup, 31, false);
		base.Transform.parent = this.m_WeaponGroupTransform;
		base.Transform.localPosition = Vector3.zero;
		this.m_WeaponGroupTransform.localEulerAngles = this.RotationOffset;
		if (this.m_WeaponCamera != null && vp_Utility.IsActive(this.m_WeaponCamera.gameObject))
		{
			vp_Layer.Set(base.gameObject, 31, true);
		}
		this.m_Pivot = GameObject.CreatePrimitive(PrimitiveType.Sphere);
		this.m_Pivot.name = "Pivot";
		this.m_Pivot.GetComponent<Collider>().enabled = false;
		this.m_Pivot.gameObject.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
		this.m_Pivot.transform.parent = this.m_WeaponGroupTransform;
		this.m_Pivot.transform.localPosition = Vector3.zero;
		this.m_Pivot.layer = 31;
		vp_Utility.Activate(this.m_Pivot.gameObject, false);
		if (Application.isEditor)
		{
			Material material = new Material(Shader.Find("Transparent/Diffuse"));
			material.color = new Color(0f, 0f, 1f, 0.5f);
			this.m_Pivot.GetComponent<Renderer>().material = material;
		}
		this.m_PositionSpring = new vp_Spring(this.m_WeaponGroup.gameObject.transform, vp_Spring.UpdateMode.Position, true);
		this.m_PositionSpring.RestState = this.PositionOffset;
		this.m_PositionPivotSpring = new vp_Spring(base.Transform, vp_Spring.UpdateMode.Position, true);
		this.m_PositionPivotSpring.RestState = this.PositionPivot;
		this.m_PositionSpring2 = new vp_Spring(base.Transform, vp_Spring.UpdateMode.PositionAdditive, true);
		this.m_PositionSpring2.MinVelocity = 1E-05f;
		this.m_RotationSpring = new vp_Spring(this.m_WeaponGroup.gameObject.transform, vp_Spring.UpdateMode.Rotation, true);
		this.m_RotationSpring.RestState = this.RotationOffset;
		this.m_RotationPivotSpring = new vp_Spring(base.Transform, vp_Spring.UpdateMode.Rotation, true);
		this.m_RotationPivotSpring.RestState = this.RotationPivot;
		this.m_RotationSpring2 = new vp_Spring(this.m_WeaponGroup.gameObject.transform, vp_Spring.UpdateMode.RotationAdditive, true);
		this.m_RotationSpring2.MinVelocity = 1E-05f;
		this.SnapSprings();
		this.Refresh();
		this.started = true;
	}

	public virtual void InstantiateWeaponModel()
	{
		if (this.WeaponPrefab == string.Empty)
		{
			this.WeaponPrefab = "weapon_" + WeaponData.GetData(this.m_WeaponID).wName.ToLower();
		}
		if (this.pgoWeaponPrefab == null)
		{
			this.pgoWeaponPrefab = (Resources.Load("Prefabs/" + this.WeaponPrefab) as GameObject);
		}
		if (this.pgoWeaponPrefab == null)
		{
			this.pgoWeaponPrefab = (Resources.Load("Prefabs/weapons/" + this.WeaponPrefab) as GameObject);
		}
		if (this.pgoWeaponPrefab == null)
		{
			this.pgoWeaponPrefab = ContentLoader_.LoadGameObject(this.WeaponPrefab);
		}
		if (this.pgoWeaponPrefab != null)
		{
			if (this.m_WeaponModel != null && this.m_WeaponModel != base.gameObject)
			{
				UnityEngine.Object.Destroy(this.m_WeaponModel);
			}
			this.m_WeaponModel = UnityEngine.Object.Instantiate<GameObject>(this.pgoWeaponPrefab);
			this.m_WeaponModel.transform.parent = base.transform;
			this.m_WeaponModel.transform.localPosition = Vector3.zero;
			this.m_WeaponModel.transform.localScale = new Vector3(1f, 1f, this.RenderingZScale);
			this.m_WeaponModel.transform.localEulerAngles = Vector3.zero;
			if (this.m_WeaponCamera != null && vp_Utility.IsActive(this.m_WeaponCamera.gameObject))
			{
				vp_Layer.Set(this.m_WeaponModel, 31, true);
			}
			this.m_HandModel[0] = GameObject.Find(this.m_WeaponModel.name + "/weapon/hands");
			this.m_HandModel[1] = GameObject.Find(this.m_WeaponModel.name + "/weapon/hands2");
			if (this.m_HandModel[0] == null)
			{
				this.m_HandModel[0] = GameObject.Find(this.m_WeaponModel.name + "/weapon/merc_hands");
				SkinnedMeshRenderer component = this.m_HandModel[0].GetComponent<SkinnedMeshRenderer>();
				if (component && this.m_WeaponID != 31)
				{
					component.sharedMaterials = new Material[]
					{
						ContentLoader_.LoadMaterial("merc_body_D"),
						ContentLoader_.LoadMaterial("glowes_D")
					};
				}
			}
			if (this.m_HandModel[1] == null)
			{
				this.m_HandModel[1] = GameObject.Find(this.m_WeaponModel.name + "/weapon/warcorp_hands");
				SkinnedMeshRenderer component2 = this.m_HandModel[1].GetComponent<SkinnedMeshRenderer>();
				if (component2)
				{
					component2.sharedMaterials = new Material[]
					{
						ContentLoader_.LoadMaterial("warcorp_body_D"),
						ContentLoader_.LoadMaterial("glowes_D")
					};
				}
			}
			this.m_PartModelFG = GameObject.Find(this.m_WeaponModel.name + "/weapon/FragGrenade");
			this.m_PartModelFB = GameObject.Find(this.m_WeaponModel.name + "/weapon/FlashGrenade");
			this.m_PartModelSG = GameObject.Find(this.m_WeaponModel.name + "/weapon/SmokeGrenade");
			this.m_WeaponCoreModel = GameObject.Find(this.m_WeaponModel.name + "/weapon");
			this.m_WeaponCamera001 = GameObject.Find(this.m_WeaponModel.name + "/weapon/Camera001");
			if (this.m_WeaponCamera001 != null)
			{
				this.m_WeaponCamera001Zero = this.m_WeaponCamera001.transform.localEulerAngles;
			}
			this.m_WeaponZoom = TEX.GetTextureByName(this.m_WeaponID.ToString() + "_scope");
			this.tBlack = TEX.GetTextureByName("black");
		}
		else
		{
			this.m_WeaponModel = base.gameObject;
		}
		base.gameObject.SendMessage("WeaponLoaded", SendMessageOptions.DontRequireReceiver);
		base.CacheRenderers();
	}

	protected override void Init()
	{
		vp_FPWeapon.flashGo = ContentLoader_.LoadGameObject("fb_flash");
		vp_FPWeapon.smokeGo = ContentLoader_.LoadGameObject("sg_smoke");
		base.Init();
		this.ScheduleAmbientAnimation();
	}

	protected override void Update()
	{
		if (vp_FPWeapon.bombAnimationState >= 0)
		{
			this.PlayBombAnimation(vp_FPWeapon.bombAnimationState);
			vp_FPWeapon.bombAnimationState = -1;
		}
		if (vp_FPWeapon.zombieAnimationState >= 0)
		{
			this.PlayZombieAnimation(vp_FPWeapon.zombieAnimationState);
			vp_FPWeapon.zombieAnimationState = -1;
		}
		if (vp_FPWeapon.grenadeAnimationState >= 0)
		{
			if (this.m_WeaponCoreModel == null)
			{
				return;
			}
			if (vp_FPWeapon.grenadeAnimationState == 1)
			{
				this.m_WeaponCoreModel.GetComponent<Animation>().Play("Shot_Start");
			}
			else if (vp_FPWeapon.grenadeAnimationState == 2)
			{
				this.m_WeaponCoreModel.GetComponent<Animation>().Play("Shot_End");
			}
			else
			{
				this.m_WeaponCoreModel.GetComponent<Animation>().Stop();
			}
			vp_FPWeapon.grenadeAnimationState = -1;
		}
		base.Update();
		if (this.m_Player && this.m_Player.Zoom.Active)
		{
			this.UpdateSprings();
		}
		this.UpdateMouseLook();
		if (vp_FPWeapon.MuzzleLightTime != 0f && Time.time > vp_FPWeapon.MuzzleLightTime)
		{
			vp_FPWeapon.MuzzleLight.GetComponent<Light>().enabled = false;
			vp_FPWeapon.MuzzleWeaponLight.GetComponent<Light>().enabled = false;
			vp_FPWeapon.MuzzleLightTime = 0f;
		}
		if (this.m_Player && this.m_WeaponID == 28)
		{
			if (this.m_Player.Run.Active)
			{
				if (!(vp_FPWeapon.knife_animation_name == "Draw") || !this.m_WeaponCoreModel.GetComponent<Animation>().isPlaying)
				{
					if (vp_FPWeapon.knife_animation_name != "Run")
					{
						this.m_WeaponCoreModel.GetComponent<Animation>()["Run"].speed = 1.15f;
						this.m_WeaponCoreModel.GetComponent<Animation>().clip = this.m_WeaponCoreModel.GetComponent<Animation>()["Run"].clip;
						vp_FPWeapon.knife_animation_name = "Run";
						this.m_WeaponCoreModel.GetComponent<Animation>().CrossFade("Run");
					}
				}
			}
			else if (vp_FPWeapon.knife_animation_name == "Run")
			{
				this.m_WeaponCoreModel.GetComponent<Animation>().clip = this.m_WeaponCoreModel.GetComponent<Animation>()["Idle"].clip;
				vp_FPWeapon.knife_animation_name = "Idle";
				this.m_WeaponCoreModel.GetComponent<Animation>().CrossFade("Idle");
			}
		}
		if (!this.m_Player)
		{
			return;
		}
		if (!vp_FPWeapon.m_DrawZoom)
		{
			if (!this.m_Player.Zoom.Active)
			{
				return;
			}
			if (this.RenderingFieldOfView > this.m_WeaponCamera.gameObject.GetComponent<Camera>().fov + 1f)
			{
				return;
			}
			vp_FPWeapon.m_DrawZoom = true;
			vp_FPCamera.SetFov(this.CameraZoom);
			if (this.m_WeaponZoom != null)
			{
				vp_Utility.HideWeapon(this.m_WeaponCoreModel, false);
			}
		}
		else if (vp_FPWeapon.m_DrawZoom && (!this.m_Player.Zoom.Active || this.RenderingFieldOfView > this.m_WeaponCamera.gameObject.GetComponent<Camera>().fov + 1f))
		{
			vp_FPWeapon.m_DrawZoom = false;
			vp_FPCamera.SetFov(60f);
			if (this.m_WeaponZoom != null)
			{
				vp_Utility.HideWeapon(this.m_WeaponCoreModel, true);
				this.HideHands();
			}
			if (vp_FPCamera.dof != null)
			{
				vp_FPCamera.dof.enabled = false;
			}
		}
	}

	protected override void FixedUpdate()
	{
		base.FixedUpdate();
		if (Input.GetKey(KeyCode.J))
		{
			this.m_WeaponCoreModel.GetComponent<Animation>().Play("Shot_End");
		}
		if (Time.timeScale != 0f && this.started)
		{
			this.UpdateZoom();
			this.UpdateSwaying();
			if (!SpecCam.show)
			{
				this.UpdateBob();
			}
			this.UpdateEarthQuake();
			this.UpdateStep();
			this.UpdateShakes();
			this.UpdateSprings();
		}
		if (vp_FPInput.grenadeThrowStarting)
		{
			vp_FPInput.grenadeThrowStarting = false;
			this.grenadeThrowStartingTime = Time.time;
			vp_FPWeapon.SetGrenadeAnimationState(1);
			vp_FPInput.grenadeActivated = true;
		}
		if (vp_FPInput.grenadeThrowEnding && this.grenadeThrowStartingTime + 0.44f < Time.time)
		{
			this.grenadeThrowStartingTime = -1f;
			vp_FPInput.grenadeThrowEnding = false;
			vp_FPWeapon.SetGrenadeAnimationState(2);
			vp_FPWeapon.ForceDeactivate = Time.time + 0.32f;
			vp_FPWeapon.ForceHideObject = Time.time + 0.1f;
		}
		if (vp_FPWeapon.ForceDeactivate != 0f && Time.time > vp_FPWeapon.ForceDeactivate)
		{
			this.Deactivate();
			if (vp_FPInput.activeGrenade == 0 && BasePlayer.weapon[3] != null)
			{
				BasePlayer.weapon[3] = null;
			}
			else if (vp_FPInput.activeGrenade == 1 && BasePlayer.weapon[5] != null)
			{
				BasePlayer.weapon[5] = null;
			}
			else if (vp_FPInput.activeGrenade == 2 && BasePlayer.weapon[6] != null)
			{
				BasePlayer.weapon[6] = null;
			}
			if (vp_FPInput.activeGrenade == 0 && BasePlayer.fb > 0)
			{
				BasePlayer.selectedGrenade = 1;
			}
			else if (vp_FPInput.activeGrenade == 0 && BasePlayer.sg > 0)
			{
				BasePlayer.selectedGrenade = 2;
			}
			else if (vp_FPInput.activeGrenade == 1 && BasePlayer.sg > 0)
			{
				BasePlayer.selectedGrenade = 2;
			}
			else if (vp_FPInput.activeGrenade == 1 && BasePlayer.fg > 0)
			{
				BasePlayer.selectedGrenade = 0;
			}
			else if (vp_FPInput.activeGrenade == 2 && BasePlayer.fg > 0)
			{
				BasePlayer.selectedGrenade = 0;
			}
			else if (vp_FPInput.activeGrenade == 2 && BasePlayer.fb > 0)
			{
				BasePlayer.selectedGrenade = 1;
			}
			if (vp_FPCamera.returnWeapon != null)
			{
				int weaponSlot = vp_FPCamera.returnWeapon.WeaponSlot;
				if (BasePlayer.weapon[weaponSlot] != null)
				{
					vp_FPCamera.returnWeapon = null;
					this.Player.SetWeaponByName.Try(BasePlayer.weapon[weaponSlot].data.selectName);
				}
			}
			else if (BasePlayer.weapon[0] != null)
			{
				this.Player.SetWeaponByName.Try(BasePlayer.weapon[0].data.selectName);
			}
			else if (BasePlayer.weapon[1] != null)
			{
				this.Player.SetWeaponByName.Try(BasePlayer.weapon[1].data.selectName);
			}
			else if (BasePlayer.weapon[2] != null)
			{
				this.Player.SetWeaponByName.Try(BasePlayer.weapon[2].data.selectName);
			}
		}
		if (vp_FPWeapon.ForceHideObject != 0f && Time.time > vp_FPWeapon.ForceHideObject)
		{
			vp_FPWeapon.ForceHideObject = 0f;
			if (vp_FPInput.activeGrenade == 0)
			{
				this.m_PartModelFG.SetActive(false);
			}
			if (vp_FPInput.activeGrenade == 1)
			{
				this.m_PartModelFB.SetActive(false);
			}
			if (vp_FPInput.activeGrenade == 2)
			{
				this.m_PartModelSG.SetActive(false);
			}
			Vector3 vector = Vector3.Normalize(vp_FPController.cs.SmoothPosition - vp_FPController.cs.PrevPosition);
			Vector3 pos = Camera.main.transform.position + Camera.main.transform.forward * 0.75f;
			Vector3 zero = Vector3.zero;
			Vector3 force = vp_FPController.cs.Velocity * 50f + Camera.main.transform.forward * 1100f + Camera.main.transform.up * 75f;
			Vector3 torque = new Vector3(90f, 90f, 90f);
			Client.cs.send_ent(0, vp_FPInput.activeGrenade, pos, zero, force, torque);
		}
	}

	public virtual void AddForce2(Vector3 positional, Vector3 angular)
	{
		this.m_PositionSpring2.AddForce(positional);
		this.m_RotationSpring2.AddForce(angular);
	}

	public virtual void AddForce2(float xPos, float yPos, float zPos, float xRot, float yRot, float zRot)
	{
		this.AddForce2(new Vector3(xPos, yPos, zPos), new Vector3(xRot, yRot, zRot));
	}

	public virtual void AddForce(Vector3 force)
	{
		this.m_PositionSpring.AddForce(force);
	}

	public virtual void AddForce(float x, float y, float z)
	{
		this.AddForce(new Vector3(x, y, z));
	}

	public virtual void AddForce(Vector3 positional, Vector3 angular)
	{
		this.m_PositionSpring.AddForce(positional);
		this.m_RotationSpring.AddForce(angular);
	}

	public virtual void AddForce(float xPos, float yPos, float zPos, float xRot, float yRot, float zRot)
	{
		this.AddForce(new Vector3(xPos, yPos, zPos), new Vector3(xRot, yRot, zRot));
	}

	public virtual void AddSoftForce(Vector3 force, int frames)
	{
		this.m_PositionSpring.AddSoftForce(force, (float)frames);
	}

	public virtual void AddSoftForce(float x, float y, float z, int frames)
	{
		this.AddSoftForce(new Vector3(x, y, z), frames);
	}

	public virtual void AddSoftForce(Vector3 positional, Vector3 angular, int frames)
	{
		this.m_PositionSpring.AddSoftForce(positional, (float)frames);
		this.m_RotationSpring.AddSoftForce(angular, (float)frames);
	}

	public virtual void AddSoftForce(float xPos, float yPos, float zPos, float xRot, float yRot, float zRot, int frames)
	{
		this.AddSoftForce(new Vector3(xPos, yPos, zPos), new Vector3(xRot, yRot, zRot), frames);
	}

	protected virtual void UpdateMouseLook()
	{
		this.m_MouseMove.x = Input.GetAxisRaw("Mouse X") / base.Delta * Time.timeScale * Time.timeScale;
		this.m_MouseMove.y = Input.GetAxisRaw("Mouse Y") / base.Delta * Time.timeScale * Time.timeScale;
		this.m_MouseMove *= this.RotationInputVelocityScale;
		this.m_MouseMove = Vector3.Min(this.m_MouseMove, Vector3.one * this.RotationMaxInputVelocity);
		this.m_MouseMove = Vector3.Max(this.m_MouseMove, Vector3.one * -this.RotationMaxInputVelocity);
	}

	protected virtual void UpdateZoom()
	{
		if (this.m_FinalZoomTime <= Time.time)
		{
			return;
		}
		if (!this.m_Wielded)
		{
			return;
		}
		this.RenderingZoomDamping = Mathf.Max(this.RenderingZoomDamping, 0.01f);
		float t = 1f - (this.m_FinalZoomTime - Time.time) / this.RenderingZoomDamping;
		if (this.m_WeaponCamera != null && vp_Utility.IsActive(this.m_WeaponCamera.gameObject))
		{
			this.m_WeaponCamera.GetComponent<Camera>().fov = Mathf.SmoothStep(this.m_WeaponCamera.gameObject.GetComponent<Camera>().fov, this.RenderingFieldOfView, t);
		}
	}

	public virtual void Zoom()
	{
		this.m_FinalZoomTime = Time.time + this.RenderingZoomDamping;
	}

	public virtual void SnapZoom()
	{
		if (this.m_WeaponCamera != null && vp_Utility.IsActive(this.m_WeaponCamera.gameObject))
		{
			this.m_WeaponCamera.GetComponent<Camera>().fov = this.RenderingFieldOfView;
		}
	}

	protected virtual void UpdateShakes()
	{
		if (this.ShakeSpeed != 0f)
		{
			this.m_Shake = Vector3.Scale(vp_SmoothRandom.GetVector3Centered(this.ShakeSpeed), this.ShakeAmplitude);
			this.m_RotationSpring.AddForce(this.m_Shake * Time.timeScale);
		}
	}

	protected virtual void UpdateRetraction(bool firstIteration = true)
	{
		if (this.RetractionDistance == 0f)
		{
			return;
		}
		Vector3 vector = this.WeaponModel.transform.TransformPoint(this.RetractionOffset);
		Vector3 end = vector + this.WeaponModel.transform.forward * this.RetractionDistance;
		RaycastHit raycastHit;
		if (Physics.Linecast(vector, end, out raycastHit, vp_Layer.GetExternalBlockers()) && !raycastHit.collider.isTrigger)
		{
			this.WeaponModel.transform.position = raycastHit.point - (raycastHit.point - vector).normalized * (this.RetractionDistance * 0.99f);
			this.WeaponModel.transform.localPosition = Vector3.forward * Mathf.Min(this.WeaponModel.transform.localPosition.z, 0f);
		}
		else if (firstIteration && this.WeaponModel.transform.localPosition != Vector3.zero && this.WeaponModel != base.gameObject)
		{
			this.WeaponModel.transform.localPosition = Vector3.forward * Mathf.SmoothStep(this.WeaponModel.transform.localPosition.z, 0f, this.RetractionRelaxSpeed * Time.timeScale);
			this.UpdateRetraction(false);
		}
	}

	protected virtual void UpdateBob()
	{
		if (this.BobAmplitude == Vector4.zero || this.BobRate == Vector4.zero)
		{
			return;
		}
		this.m_BobSpeed = ((!this.BobRequireGroundContact || this.Controller.isGrounded) ? this.Controller.velocity.sqrMagnitude : 0f);
		this.m_BobSpeed = Mathf.Min(this.m_BobSpeed * this.BobInputVelocityScale, this.BobMaxInputVelocity);
		this.m_BobSpeed = Mathf.Round(this.m_BobSpeed * 1000f) / 1000f;
		if (this.m_BobSpeed == 0f)
		{
			this.m_BobSpeed = Mathf.Min(this.m_LastBobSpeed * 0.93f, this.BobMaxInputVelocity);
		}
		this.m_CurrentBobAmp.x = this.m_BobSpeed * (this.BobAmplitude.x * -0.0001f);
		this.m_CurrentBobVal.x = Mathf.Cos(Time.time * (this.BobRate.x * 10f)) * this.m_CurrentBobAmp.x;
		this.m_CurrentBobAmp.y = this.m_BobSpeed * (this.BobAmplitude.y * 0.0001f);
		this.m_CurrentBobVal.y = Mathf.Cos(Time.time * (this.BobRate.y * 10f)) * this.m_CurrentBobAmp.y;
		this.m_CurrentBobAmp.z = this.m_BobSpeed * (this.BobAmplitude.z * 0.0001f);
		this.m_CurrentBobVal.z = Mathf.Cos(Time.time * (this.BobRate.z * 10f)) * this.m_CurrentBobAmp.z;
		this.m_CurrentBobAmp.w = this.m_BobSpeed * (this.BobAmplitude.w * 0.0001f);
		this.m_CurrentBobVal.w = Mathf.Cos(Time.time * (this.BobRate.w * 10f)) * this.m_CurrentBobAmp.w;
		this.m_RotationSpring.AddForce(this.m_CurrentBobVal * Time.timeScale);
		this.m_PositionSpring.AddForce(Vector3.forward * this.m_CurrentBobVal.w * Time.timeScale);
		this.m_LastBobSpeed = this.m_BobSpeed;
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
		if (!this.Controller.isGrounded)
		{
			return;
		}
		Vector3 vector = this.Player.EarthQuakeForce.Get();
		this.AddForce(new Vector3(0f, 0f, -vector.z * 0.015f), new Vector3(vector.y * 2f, -vector.x, vector.x * 2f));
	}

	protected virtual void UpdateSprings()
	{
		this.m_PositionSpring.FixedUpdate();
		this.m_PositionPivotSpring.FixedUpdate();
		this.m_RotationPivotSpring.FixedUpdate();
		this.m_PositionSpring2.FixedUpdate();
		this.m_RotationSpring.FixedUpdate();
		this.m_RotationSpring2.FixedUpdate();
	}

	protected virtual void UpdateStep()
	{
		if (this.StepMinVelocity <= 0f || (this.BobRequireGroundContact && !this.Controller.isGrounded) || this.Controller.velocity.sqrMagnitude < this.StepMinVelocity)
		{
			return;
		}
		bool flag = this.m_LastUpBob < this.m_CurrentBobVal.x;
		this.m_LastUpBob = this.m_CurrentBobVal.x;
		if (flag && !this.m_BobWasElevating)
		{
			if (Mathf.Cos(Time.time * (this.BobRate.x * 5f)) > 0f)
			{
				this.m_PosStep = this.StepPositionForce - this.StepPositionForce * this.StepPositionBalance;
				this.m_RotStep = this.StepRotationForce - this.StepPositionForce * this.StepRotationBalance;
			}
			else
			{
				this.m_PosStep = this.StepPositionForce + this.StepPositionForce * this.StepPositionBalance;
				this.m_RotStep = Vector3.Scale(this.StepRotationForce - this.StepPositionForce * this.StepRotationBalance, -Vector3.one + Vector3.right * 2f);
			}
			this.AddSoftForce(this.m_PosStep * this.StepForceScale, this.m_RotStep * this.StepForceScale, this.StepSoftness);
		}
		this.m_BobWasElevating = flag;
	}

	protected virtual void UpdateSwaying()
	{
		this.m_SwayVel = this.Controller.velocity * this.PositionInputVelocityScale;
		this.m_SwayVel = Vector3.Min(this.m_SwayVel, Vector3.one * this.PositionMaxInputVelocity);
		this.m_SwayVel = Vector3.Max(this.m_SwayVel, Vector3.one * -this.PositionMaxInputVelocity);
		this.m_SwayVel *= Time.timeScale;
		Vector3 vector = base.Transform.InverseTransformDirection(this.m_SwayVel / 60f);
		this.m_RotationSpring.AddForce(new Vector3(this.m_MouseMove.y * (this.RotationLookSway.x * 0.025f), this.m_MouseMove.x * (this.RotationLookSway.y * -0.025f), this.m_MouseMove.x * (this.RotationLookSway.z * -0.025f)));
		this.m_FallSway = this.RotationFallSway * (this.m_SwayVel.y * 0.005f);
		if (this.Controller.isGrounded)
		{
			this.m_FallSway *= this.RotationSlopeSway;
		}
		this.m_FallSway.z = Mathf.Max(0f, this.m_FallSway.z);
		this.m_RotationSpring.AddForce(this.m_FallSway);
		this.m_PositionSpring.AddForce(Vector3.forward * -Mathf.Abs(this.m_SwayVel.y * (this.PositionFallRetract * 2.5E-05f)));
		this.m_PositionSpring.AddForce(new Vector3(vector.x * (this.PositionWalkSlide.x * 0.0016f), -Mathf.Abs(vector.x * (this.PositionWalkSlide.y * 0.0016f)), -vector.z * (this.PositionWalkSlide.z * 0.0016f)));
		this.m_RotationSpring.AddForce(new Vector3(-Mathf.Abs(vector.x * (this.RotationStrafeSway.x * 0.16f)), -(vector.x * (this.RotationStrafeSway.y * 0.16f)), vector.x * (this.RotationStrafeSway.z * 0.16f)));
	}

	public virtual void ResetSprings(float positionReset, float rotationReset, float positionPauseTime = 0f, float rotationPauseTime = 0f)
	{
		this.m_PositionSpring.State = Vector3.Lerp(this.m_PositionSpring.State, this.m_PositionSpring.RestState, positionReset);
		this.m_RotationSpring.State = Vector3.Lerp(this.m_RotationSpring.State, this.m_RotationSpring.RestState, rotationReset);
		this.m_PositionPivotSpring.State = Vector3.Lerp(this.m_PositionPivotSpring.State, this.m_PositionPivotSpring.RestState, positionReset);
		this.m_RotationPivotSpring.State = Vector3.Lerp(this.m_RotationPivotSpring.State, this.m_RotationPivotSpring.RestState, rotationReset);
		if (positionPauseTime != 0f)
		{
			this.m_PositionSpring.ForceVelocityFadeIn(positionPauseTime);
		}
		if (rotationPauseTime != 0f)
		{
			this.m_RotationSpring.ForceVelocityFadeIn(rotationPauseTime);
		}
		if (positionPauseTime != 0f)
		{
			this.m_PositionPivotSpring.ForceVelocityFadeIn(positionPauseTime);
		}
		if (rotationPauseTime != 0f)
		{
			this.m_RotationPivotSpring.ForceVelocityFadeIn(rotationPauseTime);
		}
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
			this.m_PositionSpring.RestState = this.PositionOffset - this.PositionPivot;
		}
		if (this.m_PositionPivotSpring != null)
		{
			this.m_PositionPivotSpring.Stiffness = new Vector3(this.PositionPivotSpringStiffness, this.PositionPivotSpringStiffness, this.PositionPivotSpringStiffness);
			this.m_PositionPivotSpring.Damping = Vector3.one - new Vector3(this.PositionPivotSpringDamping, this.PositionPivotSpringDamping, this.PositionPivotSpringDamping);
			this.m_PositionPivotSpring.RestState = this.PositionPivot;
		}
		if (this.m_RotationPivotSpring != null)
		{
			this.m_RotationPivotSpring.Stiffness = new Vector3(this.RotationPivotSpringStiffness, this.RotationPivotSpringStiffness, this.RotationPivotSpringStiffness);
			this.m_RotationPivotSpring.Damping = Vector3.one - new Vector3(this.RotationPivotSpringDamping, this.RotationPivotSpringDamping, this.RotationPivotSpringDamping);
			this.m_RotationPivotSpring.RestState = this.RotationPivot;
		}
		if (this.m_PositionSpring2 != null)
		{
			this.m_PositionSpring2.Stiffness = new Vector3(this.PositionSpring2Stiffness, this.PositionSpring2Stiffness, this.PositionSpring2Stiffness);
			this.m_PositionSpring2.Damping = Vector3.one - new Vector3(this.PositionSpring2Damping, this.PositionSpring2Damping, this.PositionSpring2Damping);
			this.m_PositionSpring2.RestState = Vector3.zero;
		}
		if (this.m_RotationSpring != null)
		{
			this.m_RotationSpring.Stiffness = new Vector3(this.RotationSpringStiffness, this.RotationSpringStiffness, this.RotationSpringStiffness);
			this.m_RotationSpring.Damping = Vector3.one - new Vector3(this.RotationSpringDamping, this.RotationSpringDamping, this.RotationSpringDamping);
			this.m_RotationSpring.RestState = this.RotationOffset;
		}
		if (this.m_RotationSpring2 != null)
		{
			this.m_RotationSpring2.Stiffness = new Vector3(this.RotationSpring2Stiffness, this.RotationSpring2Stiffness, this.RotationSpring2Stiffness);
			this.m_RotationSpring2.Damping = Vector3.one - new Vector3(this.RotationSpring2Damping, this.RotationSpring2Damping, this.RotationSpring2Damping);
			this.m_RotationSpring2.RestState = Vector3.zero;
		}
		if (base.Rendering)
		{
			if (this.m_WeaponCamera != null && vp_Utility.IsActive(this.m_WeaponCamera.gameObject))
			{
				this.m_WeaponCamera.GetComponent<Camera>().nearClipPlane = this.RenderingClippingPlanes.x;
				this.m_WeaponCamera.GetComponent<Camera>().farClipPlane = this.RenderingClippingPlanes.y;
			}
			this.Zoom();
		}
	}

	public override void Activate()
	{
		this.m_Wielded = true;
		base.Rendering = true;
		this.m_DeactivationTimer.Cancel();
		this.SnapZoom();
		if (this.m_WeaponGroup != null && !vp_Utility.IsActive(this.m_WeaponGroup))
		{
			vp_Utility.Activate(this.m_WeaponGroup, true);
		}
		this.SetPivotVisible(false);
		vp_FPWeapon.ForceDeactivate = 0f;
		vp_FPWeapon.ForceHideObject = 0f;
		int state = (!vp_FPInput.fastGrenade) ? 0 : 1;
		BasePlayer.SetCurrWeapon(this.WeaponSlot, state);
		vp_FPInput.CanPistolFire = true;
		vp_FPCamera.currWeapon = this;
		if (this.m_WeaponID == 26)
		{
			Transform transform = Camera.main.transform;
			transform.eulerAngles = new Vector3(transform.rotation.eulerAngles.x, this.Player.transform.eulerAngles.y, 0f);
			bool flag = false;
			Ray[] array = new Ray[5];
			float d = 0.1f;
			float d2 = 0.15f;
			float d3 = 0.2f;
			array[0] = new Ray(transform.position, transform.forward);
			array[1] = new Ray(transform.position + transform.right.normalized * d, transform.forward + transform.right.normalized * d2);
			array[2] = new Ray(transform.position - transform.right.normalized * d, transform.forward - transform.right.normalized * d2);
			array[3] = new Ray(transform.position + transform.right.normalized * d, transform.forward + transform.right.normalized * d3);
			array[4] = new Ray(transform.position - transform.right.normalized * d, transform.forward - transform.right.normalized * d3);
			RaycastHit[] array2 = new RaycastHit[5];
			float num = 1.5f;
			vp_DamageHandlerPlayer vp_DamageHandlerPlayer = null;
			for (int i = 0; i < array.Length; i++)
			{
				Debug.DrawRay(array[i].origin, array[i].direction, Color.red);
				if (Physics.Raycast(array[i], out array2[i], num, vp_Layer.GetBulletBlockers()) && array2[i].distance <= num)
				{
					num = array2[i].distance;
					vp_DamageHandlerPlayer = array2[i].transform.GetComponent<vp_DamageHandlerPlayer>();
					if (vp_DamageHandlerPlayer != null)
					{
						vp_HitscanBullet.lastray = array[i];
						vp_HitscanBullet.lasthit = array2[i];
					}
				}
			}
			if (vp_DamageHandlerPlayer != null)
			{
				flag = true;
				vp_DamageHandlerPlayer.Damage();
			}
			if (flag)
			{
				vp_FPWeapon.knife_animation_name = "knife_hit";
				vp_FPWeapon.ForceDeactivate = Time.time + 0.5f;
				if (base.Parent.GetComponent<AudioSource>() != null && vp_Utility.IsActive(base.gameObject) && this.sndWield != null)
				{
					base.Parent.GetComponent<AudioSource>().pitch = 1f;
					base.Parent.GetComponent<AudioSource>().PlayOneShot(this.sndWield);
				}
			}
			else
			{
				vp_FPWeapon.knife_animation_name = "knife_attack";
				vp_FPWeapon.ForceDeactivate = Time.time + 0.2f;
				if (base.Parent.GetComponent<AudioSource>() != null && vp_Utility.IsActive(base.gameObject) && this.SoundUnWield != null)
				{
					base.Parent.GetComponent<AudioSource>().pitch = 1f;
					base.Parent.GetComponent<AudioSource>().PlayOneShot(this.sndUnWield);
				}
			}
		}
		else if (this.m_WeaponID != 31)
		{
			if (vp_FPInput.fastGrenade && WeaponData.GetData(this.WeaponID).weaponClass == 6)
			{
				vp_FPWeapon.ForceDeactivate = Time.time + 0.52f;
				vp_FPWeapon.ForceHideObject = Time.time + 0.25f;
			}
			else if (WeaponData.GetData(this.WeaponID).weaponClass == 9)
			{
			}
		}
		Crosshair.SetDynamic(WeaponData.GetData(this.WeaponID).crosshairDynamic);
		vp_FPInput.UpdateSpeed(this.Player.Run.Active);
	}

	public override void Deactivate()
	{
		vp_FPWeapon.ForceDeactivate = 0f;
		vp_FPWeapon.ForceHideObject = 0f;
		this.m_Wielded = false;
		if (this.m_WeaponGroup != null && vp_Utility.IsActive(this.m_WeaponGroup))
		{
			vp_Utility.Activate(this.m_WeaponGroup, false);
		}
		vp_FPCamera.currWeapon = null;
		vp_FPInput.LastZoomTime = Time.time + 0.5f;
		vp_FPInput.ZoomTap = false;
		vp_FPCamera.SetFov(60f);
		vp_FPInput.grenadeActivated = false;
	}

	public virtual void SnapPivot()
	{
		if (this.m_PositionSpring != null)
		{
			this.m_PositionSpring.RestState = this.PositionOffset - this.PositionPivot;
			this.m_PositionSpring.State = this.PositionOffset - this.PositionPivot;
		}
		if (this.m_WeaponGroup != null)
		{
			this.m_WeaponGroupTransform.localPosition = this.PositionOffset - this.PositionPivot;
		}
		if (this.m_PositionPivotSpring != null)
		{
			this.m_PositionPivotSpring.RestState = this.PositionPivot;
			this.m_PositionPivotSpring.State = this.PositionPivot;
		}
		if (this.m_RotationPivotSpring != null)
		{
			this.m_RotationPivotSpring.RestState = this.RotationPivot;
			this.m_RotationPivotSpring.State = this.RotationPivot;
		}
		base.Transform.localPosition = this.PositionPivot;
		base.Transform.localEulerAngles = this.RotationPivot;
	}

	public virtual void SetPivotVisible(bool visible)
	{
		if (this.m_Pivot == null)
		{
			return;
		}
		vp_Utility.Activate(this.m_Pivot.gameObject, visible);
	}

	public virtual void SnapToExit()
	{
		this.RotationOffset = this.RotationExitOffset;
		this.PositionOffset = this.PositionExitOffset;
		this.SnapSprings();
		this.SnapPivot();
	}

	public virtual void SnapSprings()
	{
		if (this.m_PositionSpring != null)
		{
			this.m_PositionSpring.RestState = this.PositionOffset - this.PositionPivot;
			this.m_PositionSpring.State = this.PositionOffset - this.PositionPivot;
			this.m_PositionSpring.Stop(true);
		}
		if (this.m_WeaponGroup != null)
		{
			this.m_WeaponGroupTransform.localPosition = this.PositionOffset - this.PositionPivot;
		}
		if (this.m_PositionPivotSpring != null)
		{
			this.m_PositionPivotSpring.RestState = this.PositionPivot;
			this.m_PositionPivotSpring.State = this.PositionPivot;
			this.m_PositionPivotSpring.Stop(true);
		}
		base.Transform.localPosition = this.PositionPivot;
		if (this.m_PositionSpring2 != null)
		{
			this.m_PositionSpring2.RestState = Vector3.zero;
			this.m_PositionSpring2.State = Vector3.zero;
			this.m_PositionSpring2.Stop(true);
		}
		if (this.m_RotationPivotSpring != null)
		{
			this.m_RotationPivotSpring.RestState = this.RotationPivot;
			this.m_RotationPivotSpring.State = this.RotationPivot;
			this.m_RotationPivotSpring.Stop(true);
		}
		base.Transform.localEulerAngles = this.RotationPivot;
		if (this.m_RotationSpring != null)
		{
			this.m_RotationSpring.RestState = this.RotationOffset;
			this.m_RotationSpring.State = this.RotationOffset;
			this.m_RotationSpring.Stop(true);
		}
		if (this.m_RotationSpring2 != null)
		{
			this.m_RotationSpring2.RestState = Vector3.zero;
			this.m_RotationSpring2.State = Vector3.zero;
			this.m_RotationSpring2.Stop(true);
		}
	}

	public virtual void StopSprings()
	{
		if (this.m_PositionSpring != null)
		{
			this.m_PositionSpring.Stop(true);
		}
		if (this.m_PositionPivotSpring != null)
		{
			this.m_PositionPivotSpring.Stop(true);
		}
		if (this.m_PositionSpring2 != null)
		{
			this.m_PositionSpring2.Stop(true);
		}
		if (this.m_RotationSpring != null)
		{
			this.m_RotationSpring.Stop(true);
		}
		if (this.m_RotationPivotSpring != null)
		{
			this.m_RotationPivotSpring.Stop(true);
		}
		if (this.m_RotationSpring2 != null)
		{
			this.m_RotationSpring2.Stop(true);
		}
	}

	public virtual void Wield(bool showWeapon = true)
	{
		if (showWeapon)
		{
			this.SnapToExit();
		}
		this.PositionOffset = ((!showWeapon) ? this.PositionExitOffset : this.DefaultPosition);
		this.RotationOffset = ((!showWeapon) ? this.RotationExitOffset : this.DefaultRotation);
		this.m_Wielded = showWeapon;
		this.Refresh();
		base.StateManager.CombineStates();
		if (base.Parent.GetComponent<AudioSource>() != null && this.m_WeaponID != 26 && ((!showWeapon) ? this.SoundUnWield : this.SoundWield) != null && vp_Utility.IsActive(base.gameObject))
		{
			base.Parent.GetComponent<AudioSource>().pitch = Time.timeScale;
			base.Parent.GetComponent<AudioSource>().PlayOneShot((!showWeapon) ? this.sndUnWield : this.sndWield);
		}
		if (vp_FPInput.fastGrenade && WeaponData.GetData(this.m_WeaponID).weaponClass == 6)
		{
			this.AnimationWield = "Shot_Full";
		}
		else if (this.AnimationWield == string.Empty)
		{
			this.AnimationWield = "Draw";
		}
		if (vp_Utility.IsActive(base.gameObject))
		{
			if (this.m_WeaponID == 26)
			{
				this.m_WeaponCoreModel.GetComponent<Animation>().Play(vp_FPWeapon.knife_animation_name);
			}
			else
			{
				vp_FPWeapon.knife_animation_name = "Draw";
				if (this.m_WeaponCoreModel == null)
				{
					MonoBehaviour.print("m_WeaponCoreModel is null");
				}
				if (showWeapon && this.AnimationWield != string.Empty)
				{
					this.m_WeaponCoreModel.GetComponent<Animation>().Play(this.AnimationWield);
				}
				if (!showWeapon && this.AnimationUnWield != string.Empty)
				{
					this.m_WeaponCoreModel.GetComponent<Animation>().Play(this.AnimationUnWield);
				}
			}
		}
		vp_FPInput.DrawTime = Time.time + WeaponData.GetData(this.m_WeaponID).drawTime;
		this.HideHands();
		this.SetSkin();
	}

	public static void SetBombAnimationState(int val)
	{
		vp_FPWeapon.bombAnimationState = val;
	}

	public static void SetZombieAnimationState(int val)
	{
		vp_FPWeapon.zombieAnimationState = val;
	}

	public static void SetGrenadeAnimationState(int val)
	{
		vp_FPWeapon.grenadeAnimationState = val;
		Client.cs.send_grenade_anim(val);
	}

	private void PlayBombAnimation(int state)
	{
		if (this.m_WeaponCoreModel == null)
		{
			return;
		}
		if (state == 0)
		{
			this.m_WeaponCoreModel.GetComponent<Animation>().Stop();
		}
		else if (state == 1)
		{
			this.m_WeaponCoreModel.GetComponent<Animation>().Play("Idle");
		}
		else if (state == 2)
		{
			this.m_WeaponCoreModel.GetComponent<Animation>().Play("Draw");
		}
		else if (state == 3)
		{
			this.m_WeaponCoreModel.GetComponent<Animation>().Play("Shot");
		}
		if (base.Parent.GetComponent<AudioSource>() == null)
		{
			return;
		}
		base.Parent.GetComponent<AudioSource>().pitch = 1f;
		if (state == 1)
		{
			base.Parent.GetComponent<AudioSource>().Stop();
		}
		else if (state == 3)
		{
			base.Parent.GetComponent<AudioSource>().PlayOneShot(C4.plantStartSound);
		}
	}

	private void PlayZombieAnimation(int state)
	{
		if (this.m_WeaponCoreModel == null)
		{
			return;
		}
		this.m_WeaponCoreModel.GetComponent<Animation>().Stop();
		if (state == 0)
		{
			this.m_WeaponCoreModel.GetComponent<Animation>().Stop();
		}
		else if (state == 1)
		{
			this.m_WeaponCoreModel.GetComponent<Animation>().Play("Attack1");
		}
		else if (state == 2)
		{
			this.m_WeaponCoreModel.GetComponent<Animation>().Play("Attack2");
		}
		else if (state == 3)
		{
			this.m_WeaponCoreModel.GetComponent<Animation>().Play("Run");
		}
		if (base.Parent.GetComponent<AudioSource>() == null)
		{
			return;
		}
		base.Parent.GetComponent<AudioSource>().pitch = 1f;
		if (state == 0)
		{
			base.Parent.GetComponent<AudioSource>().Stop();
		}
		else if (state == 1 || state == 2)
		{
			base.Parent.GetComponent<AudioSource>().PlayOneShot(Zombie.attackSound);
		}
	}

	private void HideHands()
	{
		if (this.m_HandModel[0])
		{
			this.m_HandModel[0].SetActive(false);
		}
		if (this.m_HandModel[1])
		{
			this.m_HandModel[1].SetActive(false);
		}
		if (this.m_HandModel[BasePlayer.team])
		{
			this.m_HandModel[BasePlayer.team].SetActive(true);
		}
	}

	private void SetSkin()
	{
		if (this.m_WeaponCoreModel != null)
		{
			Component[] componentsInChildren = this.m_WeaponCoreModel.GetComponentsInChildren(typeof(Renderer));
			for (int i = 0; i < componentsInChildren.Length; i++)
			{
				Renderer renderer = (Renderer)componentsInChildren[i];
				if (!(renderer.name == "merc_hands"))
				{
					if (!(renderer.name == "warcorp_hands"))
					{
						int num = 0;
						if (SpecCam.show && SpecCam.mode == 1 && SpecCam.FID >= 0)
						{
							if (PlayerControll.Player[SpecCam.FID].currentWeapon[this.m_WeaponID] > 0)
							{
								num = PlayerControll.Player[SpecCam.FID].currentWeapon[this.m_WeaponID];
							}
						}
						else if (BaseData.currentWeapon[this.m_WeaponID] > 0)
						{
							num = BaseData.currentWeapon[this.m_WeaponID];
						}
						if (num > 0)
						{
							Texture2D textureByName = TEX.GetTextureByName(MenuShop.shopdata[num].iconname);
							if (textureByName)
							{
								renderer.materials[0].SetTexture(0, textureByName);
							}
						}
						else
						{
							Texture2D texture2D = null;
							if (this.m_WeaponID == 6)
							{
								texture2D = TEX.GetTextureByName("AK47_d");
							}
							if (this.m_WeaponID == 7)
							{
								texture2D = TEX.GetTextureByName("AKS74U_d");
							}
							if (this.m_WeaponID == 8)
							{
								texture2D = TEX.GetTextureByName("ASVAL_d");
							}
							if (this.m_WeaponID == 18)
							{
								texture2D = TEX.GetTextureByName("AUG_d");
							}
							if (this.m_WeaponID == 22)
							{
								texture2D = TEX.GetTextureByName("AWP_d");
							}
							if (this.m_WeaponID == 2)
							{
								texture2D = TEX.GetTextureByName("BERETTA_d");
							}
							if (this.m_WeaponID == 9)
							{
								texture2D = TEX.GetTextureByName("BM4_d");
							}
							if (this.m_WeaponID == 3)
							{
								texture2D = TEX.GetTextureByName("COLT_d");
							}
							if (this.m_WeaponID == 4)
							{
								texture2D = TEX.GetTextureByName("DEAGLE_d");
							}
							if (this.m_WeaponID == 10)
							{
								texture2D = TEX.GetTextureByName("FAMAS_d");
							}
							if (this.m_WeaponID == 1)
							{
								texture2D = TEX.GetTextureByName("GLOCK17_d");
							}
							if (this.m_WeaponID == 11)
							{
								texture2D = TEX.GetTextureByName("M4A1_d");
							}
							if (this.m_WeaponID == 21)
							{
								texture2D = TEX.GetTextureByName("M24_d");
							}
							if (this.m_WeaponID == 23)
							{
								texture2D = TEX.GetTextureByName("M90_d");
							}
							if (this.m_WeaponID == 20)
							{
								texture2D = TEX.GetTextureByName("M110_d");
							}
							if (this.m_WeaponID == 25)
							{
								texture2D = TEX.GetTextureByName("M249_d");
							}
							if (this.m_WeaponID == 12)
							{
								texture2D = TEX.GetTextureByName("MP5_d");
							}
							if (this.m_WeaponID == 13)
							{
								texture2D = TEX.GetTextureByName("MP7_d");
							}
							if (this.m_WeaponID == 14)
							{
								texture2D = TEX.GetTextureByName("P90_d");
							}
							if (this.m_WeaponID == 24)
							{
								texture2D = TEX.GetTextureByName("PKP_d");
							}
							if (this.m_WeaponID == 15)
							{
								texture2D = TEX.GetTextureByName("QBZ95_d");
							}
							if (this.m_WeaponID == 5)
							{
								texture2D = TEX.GetTextureByName("REMINGTON_d");
							}
							if (this.m_WeaponID == 16)
							{
								texture2D = TEX.GetTextureByName("SPAS12_d");
							}
							if (this.m_WeaponID == 19)
							{
								texture2D = TEX.GetTextureByName("SVD_d");
							}
							if (this.m_WeaponID == 17)
							{
								texture2D = TEX.GetTextureByName("UMP45_d");
							}
							if (texture2D)
							{
								renderer.materials[0].SetTexture(0, texture2D);
							}
						}
					}
				}
			}
		}
	}

	public virtual void testshake()
	{
		MonoBehaviour.print("testshake");
	}

	public virtual void ScheduleAmbientAnimation()
	{
		if (this.AnimationAmbient.Count == 0 || !vp_Utility.IsActive(base.gameObject))
		{
			return;
		}
		vp_Timer.In(UnityEngine.Random.Range(this.AmbientInterval.x, this.AmbientInterval.y), delegate
		{
			if (vp_Utility.IsActive(base.gameObject))
			{
				this.m_CurrentAmbientAnimation = UnityEngine.Random.Range(0, this.AnimationAmbient.Count);
				if (this.AnimationAmbient[this.m_CurrentAmbientAnimation] != null)
				{
					this.m_WeaponModel.GetComponent<Animation>().CrossFadeQueued(this.AnimationAmbient[this.m_CurrentAmbientAnimation].name);
					this.ScheduleAmbientAnimation();
				}
			}
		}, this.m_AnimationAmbientTimer);
	}

	protected virtual void OnMessage_FallImpact(float impact)
	{
		if (this.m_PositionSpring != null)
		{
			this.m_PositionSpring.AddSoftForce(Vector3.down * impact * this.PositionKneeling, (float)this.PositionKneelingSoftness);
		}
		if (this.m_RotationSpring != null)
		{
			this.m_RotationSpring.AddSoftForce(Vector3.right * impact * this.RotationKneeling, (float)this.RotationKneelingSoftness);
		}
	}

	protected virtual void OnMessage_HeadImpact(float impact)
	{
		this.AddForce(Vector3.zero, Vector3.forward * (Mathf.Abs(impact) * 20f) * Time.timeScale);
	}

	protected virtual void OnMessage_GroundStomp(float impact)
	{
		this.AddForce(Vector3.zero, new Vector3(-0.25f, 0f, 0f) * impact);
	}

	protected virtual void OnMessage_BombShake(float impact)
	{
		this.AddForce(Vector3.zero, new Vector3(-0.3f, 0.1f, 0.5f) * impact);
	}

	private void OnGUI()
	{
		if (!vp_FPWeapon.m_DrawZoom)
		{
			return;
		}
		if (!this.m_WeaponZoom)
		{
			return;
		}
		float width = (float)(Screen.width - Screen.height) / 2f;
		GUI.DrawTexture(new Rect((float)(Screen.width - Screen.height) / 2f, 0f, (float)Screen.height, (float)Screen.height), this.m_WeaponZoom);
		GUI.DrawTexture(new Rect(0f, 0f, width, (float)Screen.height), this.tBlack);
		GUI.DrawTexture(new Rect((float)Screen.width - (float)(Screen.width - Screen.height) / 2f, 0f, width, (float)Screen.height), this.tBlack);
	}

	public static void AliveWeaponDrop()
	{
		int wid = BasePlayer.currweapon.data.wid;
		if (!WeaponData.GetData(wid).canDrop)
		{
			return;
		}
		int clip = BasePlayer.currweapon.clip;
		int mode = 0;
		if (wid != 50)
		{
			BasePlayer.ammo[BasePlayer.currweapon.data.ammoType] -= clip;
		}
		else
		{
			mode = 3;
		}
		byte puuid = vp_FPWeapon.WeaponPlayerCollision(WeaponData.GetData(wid).slot);
		Vector3 pos = Camera.main.transform.position - new Vector3(0f, 0.12f, 0f) + Camera.main.transform.forward * 0.5f;
		Vector3 rot = new Vector3(-90f, BasePlayer.go.transform.eulerAngles.y + 90f, -90f);
		Vector3 force = new Vector3(vp_FPController.cs.Velocity.x * 60f, vp_FPController.cs.Velocity.y * 60f, vp_FPController.cs.Velocity.z * 60f) + Camera.main.transform.forward * 200f;
		Vector3 zero = new Vector3(1f, 0f, 1f);
		if (wid == 50)
		{
			zero = Vector3.zero;
		}
		Client.cs.send_weapon_drop(mode, wid, clip, puuid, pos, rot, force, zero);
	}

	public static byte WeaponPlayerCollision(int slot)
	{
		for (byte b = 0; b < 255; b += 1)
		{
			if (vp_FPWeapon.WeaponOnMapArray[(int)b] != null)
			{
				WeaponDrop component = vp_FPWeapon.WeaponOnMapArray[(int)b].GetComponent<WeaponDrop>();
				if (component != null && WeaponData.GetData(component.wid).slot == slot && component.playerCollision)
				{
					return b;
				}
			}
		}
		return 255;
	}

	public static void CreateWeaponDrop(int mode, int id, int uid, int wid, int customskin, byte puuid, Vector3 pos, Vector3 rot, Vector3 force, Vector3 torque)
	{
		GameObject gameObject = ContentLoader_.LoadGameObject("p_" + WeaponData.GetData(wid).wName.ToLower());
		if (gameObject == null)
		{
			return;
		}
		if (id == Client.ID && puuid == 255 && mode != 2 && mode != 4)
		{
			int slot = WeaponData.GetData(wid).slot;
			BasePlayer.weapon[slot] = null;
			BasePlayer.currweapon = null;
			vp_FPWeaponHandler.cs.m_CurrentWeaponID = 0;
			if (BasePlayer.weapon[0] != null)
			{
				vp_FPInput.cs.Player.SetWeaponByName.Try(BasePlayer.weapon[0].data.selectName);
			}
			else if (BasePlayer.weapon[1] != null)
			{
				vp_FPInput.cs.Player.SetWeaponByName.Try(BasePlayer.weapon[1].data.selectName);
			}
			else if (BasePlayer.weapon[2] != null)
			{
				vp_FPInput.cs.Player.SetWeaponByName.Try(BasePlayer.weapon[2].data.selectName);
			}
		}
		GameObject gameObject2 = UnityEngine.Object.Instantiate<GameObject>(gameObject);
		gameObject2.transform.position = pos;
		gameObject2.transform.eulerAngles = rot;
		gameObject2.name = "weapondrop_" + uid.ToString();
		gameObject2.layer = 22;
		if (customskin > 0)
		{
			Texture2D textureByName = TEX.GetTextureByName(MenuShop.shopdata[customskin].iconname);
			if (textureByName)
			{
				gameObject2.GetComponent<MeshRenderer>().material.SetTexture(0, textureByName);
			}
		}
		vp_FPWeapon.WeaponOnMapArray[uid] = gameObject2;
		WeaponDrop weaponDrop = gameObject2.AddComponent<WeaponDrop>();
		weaponDrop.sendPackets = false;
		weaponDrop.isGrounded = false;
		weaponDrop.wid = wid;
		weaponDrop.ownerid = id;
		weaponDrop.uid = uid;
		weaponDrop.customskin = customskin;
		weaponDrop.PrevPos = pos;
		weaponDrop.PrevRot = rot;
		weaponDrop.NewPos = pos;
		weaponDrop.NewRot = rot;
		gameObject2.AddComponent<BoxCollider>();
		PhysicMaterial material = Resources.Load("WeaponDrop") as PhysicMaterial;
		gameObject2.GetComponent<BoxCollider>().material = material;
		gameObject2.AddComponent<Rigidbody>();
		gameObject2.GetComponent<Rigidbody>().mass = 1.2f;
		gameObject2.GetComponent<Rigidbody>().collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
		gameObject2.GetComponent<Rigidbody>().AddForce(force);
		gameObject2.GetComponent<Rigidbody>().AddTorque(torque);
	}

	public static void CreateMapWeaponDrop(int id, int uid, int wid, int customskin, Vector3 pos, Vector3 rot)
	{
		GameObject gameObject = ContentLoader_.LoadGameObject("p_" + WeaponData.GetData(wid).wName.ToLower());
		if (gameObject == null)
		{
			return;
		}
		GameObject gameObject2 = UnityEngine.Object.Instantiate<GameObject>(gameObject);
		gameObject2.transform.position = pos;
		gameObject2.transform.eulerAngles = rot;
		gameObject2.name = "weapondrop_" + uid.ToString();
		gameObject2.layer = 22;
		if (customskin > 0)
		{
			Texture2D textureByName = TEX.GetTextureByName(MenuShop.shopdata[customskin].iconname);
			if (textureByName)
			{
				gameObject2.GetComponent<MeshRenderer>().material.SetTexture(0, textureByName);
			}
		}
		vp_FPWeapon.WeaponOnMapArray[uid] = gameObject2;
		WeaponDrop weaponDrop = gameObject2.AddComponent<WeaponDrop>();
		weaponDrop.sendPackets = false;
		weaponDrop.isGrounded = true;
		weaponDrop.wid = wid;
		weaponDrop.ownerid = id;
		weaponDrop.uid = uid;
		weaponDrop.customskin = customskin;
		weaponDrop.PrevPos = pos;
		weaponDrop.PrevRot = rot;
		weaponDrop.NewPos = pos;
		weaponDrop.NewRot = rot;
	}

	public static void CreateGrenadeFlash(Vector3 pos)
	{
		Transform transform = Camera.main.transform;
		Vector3 forward = transform.forward;
		Vector3 to = pos - transform.position;
		LayerMask mask = 1;
		if (!Physics.Linecast(transform.position, pos, out vp_FPWeapon.hit, mask))
		{
			float num = 10f;
			float num2 = 40f;
			float num3 = 80f;
			float num4 = 120f;
			float num5 = 1f;
			float num6 = 3.5f;
			float num7 = Vector3.Angle(forward, to);
			float num8 = Vector3.Distance(transform.position, pos);
			float num9 = num5;
			if (num7 < num3)
			{
				num9 += (num3 - num7) / num3 * (num6 - num5);
			}
			float num10 = num6;
			if (num8 > num)
			{
				num10 -= (num8 - num) / (num2 - num) * (num6 - num5);
			}
			float num11 = (num9 + num10) / 2f;
			float num12 = 1f;
			if (num7 > num3)
			{
				num12 = 1f - (num7 - num3) / (num4 - num3);
			}
			if (num7 <= num4 && num8 <= num2)
			{
				CutoffFX.SetFX(num12, num11);
				vp_FPCamera.cs.SetFlashFX(num12, num11);
			}
		}
		GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(vp_FPWeapon.flashGo);
		gameObject.transform.position = pos;
	}

	public static void CreateGrenadeSmoke(Vector3 pos)
	{
		GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(vp_FPWeapon.smokeGo);
		gameObject.transform.position = pos;
	}

	public static void SetWeaponPos(int uid, Vector3 pos, Vector3 rot)
	{
		GameObject gameObject = vp_FPWeapon.WeaponOnMapArray[uid];
		if (gameObject == null)
		{
			return;
		}
		MonoBehaviour.print("SetWeaponPos");
		WeaponDrop component = gameObject.GetComponent<WeaponDrop>();
		component.NewPos = pos;
		component.NewRot = rot;
	}

	public static void RemoveMapWeapon(int uid)
	{
		GameObject gameObject = vp_FPWeapon.WeaponOnMapArray[uid];
		if (gameObject == null)
		{
			return;
		}
		UnityEngine.Object.Destroy(gameObject);
		vp_FPWeapon.WeaponOnMapArray[uid] = null;
	}

	public static void RemoveAllMapWeapon()
	{
		UnityEngine.Object[] array = UnityEngine.Object.FindObjectsOfType(typeof(WeaponDrop));
		UnityEngine.Object[] array2 = array;
		for (int i = 0; i < array2.Length; i++)
		{
			WeaponDrop weaponDrop = (WeaponDrop)array2[i];
			UnityEngine.Object.Destroy(weaponDrop.gameObject);
		}
		for (int j = 0; j < vp_FPWeapon.WeaponOnMapArray.Length; j++)
		{
			if (vp_FPWeapon.WeaponOnMapArray[j] == null)
			{
				return;
			}
			vp_FPWeapon.WeaponOnMapArray[j] = null;
		}
	}

	public static void RemoveAllMapSmoke()
	{
		UnityEngine.Object[] array = UnityEngine.Object.FindObjectsOfType(typeof(GrenadeSmoke));
		UnityEngine.Object[] array2 = array;
		for (int i = 0; i < array2.Length; i++)
		{
			GrenadeSmoke grenadeSmoke = (GrenadeSmoke)array2[i];
			UnityEngine.Object.Destroy(grenadeSmoke.gameObject);
		}
		UnityEngine.Object[] array3 = UnityEngine.Object.FindObjectsOfType(typeof(GrenadeSG));
		UnityEngine.Object[] array4 = array3;
		for (int j = 0; j < array4.Length; j++)
		{
			GrenadeSG grenadeSG = (GrenadeSG)array4[j];
			UnityEngine.Object.Destroy(grenadeSG.gameObject);
		}
	}

	private void UpdateWeaponDrop()
	{
	}

	public void ZombieAttack()
	{
		Transform transform = Camera.main.transform;
		transform.eulerAngles = new Vector3(transform.rotation.eulerAngles.x, this.Player.transform.eulerAngles.y, 0f);
		Ray[] array = new Ray[5];
		float d = 0.1f;
		float d2 = 0.15f;
		array[0] = new Ray(transform.position, transform.forward);
		array[1] = new Ray(transform.position + transform.up.normalized * d, transform.forward + transform.up.normalized * d2);
		array[2] = new Ray(transform.position - transform.up.normalized * d, transform.forward - transform.up.normalized * d2);
		array[3] = new Ray(transform.position + transform.right.normalized * d, transform.forward + transform.right.normalized * d2);
		array[4] = new Ray(transform.position - transform.right.normalized * d, transform.forward - transform.right.normalized * d2);
		RaycastHit[] array2 = new RaycastHit[5];
		float num = 1.4f;
		vp_DamageHandlerPlayer vp_DamageHandlerPlayer = null;
		for (int i = 0; i < array.Length; i++)
		{
			Debug.DrawRay(array[i].origin, array[i].direction, Color.red);
			if (Physics.Raycast(array[i], out array2[i], num, vp_Layer.GetBulletBlockers()) && array2[i].distance <= num)
			{
				num = array2[i].distance;
				vp_DamageHandlerPlayer = array2[i].transform.GetComponent<vp_DamageHandlerPlayer>();
				if (vp_DamageHandlerPlayer != null)
				{
					vp_HitscanBullet.lastray = array[i];
					vp_HitscanBullet.lasthit = array2[i];
				}
			}
		}
		if (vp_DamageHandlerPlayer != null)
		{
			vp_DamageHandlerPlayer.Damage();
		}
	}
}
