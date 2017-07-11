using System;
using UnityEngine;

public class vp_FPInput : MonoBehaviour
{
	public static vp_FPInput cs = null;

	public static float LastFireTime = 0f;

	public static float LastJumpTime = 0f;

	public static bool CanPistolFire = true;

	public static float speed = 0f;

	public static float LastZoomTime = 0f;

	public static bool ZoomTap = false;

	public static float DrawTime = 0f;

	public static KeyCode[] control = new KeyCode[32];

	public static bool lockKeyboard = false;

	public vp_FPPlayerEventHandler Player;

	public static bool mouseDown = false;

	public static bool mouseUp = false;

	public static bool grenadeThrowStarting = false;

	public static bool grenadeThrowEnding = false;

	public static bool fastGrenade = false;

	public static bool grenadeActivated = false;

	public static int activeGrenade = 0;

	public static float deactivateGrenadeTimer = 0f;

	protected vp_FPCamera m_FPCamera;

	public Rect[] MouseCursorZones;

	public bool ForceCursor;

	protected Vector2 m_MousePos = Vector2.zero;

	protected bool m_AllowGameplayInput = true;

	public static bool blockrun = false;

	public bool AllowGameplayInput
	{
		get
		{
			return this.m_AllowGameplayInput;
		}
		set
		{
			this.m_AllowGameplayInput = value;
		}
	}

	public Vector2 MousePos
	{
		get
		{
			return this.m_MousePos;
		}
	}

	public bool MouseAcceleration
	{
		get
		{
			return this.m_FPCamera.MouseAcceleration;
		}
		set
		{
			this.m_FPCamera.MouseAcceleration = value;
		}
	}

	public float MouseAccelerationThreshold
	{
		get
		{
			return this.m_FPCamera.MouseAccelerationThreshold;
		}
		set
		{
			this.m_FPCamera.MouseAccelerationThreshold = value;
		}
	}

	protected virtual bool OnValue_AllowGameplayInput
	{
		get
		{
			return this.m_AllowGameplayInput;
		}
		set
		{
			this.m_AllowGameplayInput = value;
		}
	}

	protected virtual bool OnValue_Pause
	{
		get
		{
			return vp_TimeUtility.Paused;
		}
		set
		{
			vp_TimeUtility.Paused = value;
		}
	}

	protected virtual void Update()
	{
		if (vp_FPInput.deactivateGrenadeTimer != 0f && Time.time > vp_FPInput.deactivateGrenadeTimer)
		{
			vp_FPInput.deactivateGrenadeTimer = 0f;
		}
		this.UpdateCursorLock();
		this.UpdatePause();
		if (BasePlayer.deadflag > 0)
		{
			return;
		}
		if (SpecCam.show)
		{
			return;
		}
		if (!vp_FPInput.grenadeActivated)
		{
			this.InputDropWeapon();
		}
		if (BuyMenu.show)
		{
			return;
		}
		if (!vp_FPInput.grenadeActivated)
		{
			this.InputSetWeapon();
		}
		if (vp_FPInput.lockKeyboard)
		{
			this.Player.InputMoveVector.Set(new Vector2(0f, 0f));
			return;
		}
		this.InputMove();
		if (!this.m_AllowGameplayInput || this.Player.Pause.Get())
		{
			return;
		}
		this.InputRun();
		this.InputJump();
		this.InputCrouch();
		this.InputAttack();
		if (!vp_FPInput.grenadeActivated)
		{
			this.InputZoom();
		}
		if (!vp_FPInput.grenadeActivated)
		{
			this.InputReload();
		}
		this.InputWalk();
	}

	public static void UpdateSpeed(bool inRun)
	{
		float num = 1f;
		if (ScoreBoard.gamemode == 3 && BasePlayer.team == 0)
		{
			num = 1.1f;
		}
		if (inRun)
		{
			if (vp_FPCamera.currWeapon != null)
			{
				vp_FPController.cs.MotorAcceleration = WeaponData.GetData(vp_FPCamera.currWeapon.WeaponID).runAcceleration * num;
			}
		}
		else if (vp_FPCamera.currWeapon != null)
		{
			vp_FPController.cs.MotorAcceleration = WeaponData.GetData(vp_FPCamera.currWeapon.WeaponID).walkAcceleration * num;
		}
	}

	protected virtual void InputMove()
	{
		float num = 0f;
		float num2 = 0f;
		if (Input.GetKey(vp_FPInput.control[0]))
		{
			num2 = 1f;
		}
		else if (Input.GetKey(vp_FPInput.control[2]))
		{
			num2 = -1f;
		}
		if (Input.GetKey(vp_FPInput.control[1]))
		{
			num = -1f;
		}
		else if (Input.GetKey(vp_FPInput.control[3]))
		{
			num = 1f;
		}
		if (this.Player.Run.Active)
		{
			if (num2 > 0f)
			{
				num /= 2.25f;
			}
			else
			{
				vp_FPInput.blockrun = true;
			}
		}
		else if (num2 <= 0f)
		{
			vp_FPInput.blockrun = true;
		}
		if (vp_FPInput.speed < 35f)
		{
			vp_FPInput.blockrun = true;
		}
		this.Player.InputMoveVector.Set(new Vector2(num, num2));
	}

	protected virtual void InputRun()
	{
		if (vp_FPInput.blockrun)
		{
			this.Player.Run.TryStop();
			vp_FPInput.UpdateSpeed(false);
			vp_FPInput.blockrun = false;
			return;
		}
		if (vp_FPInput.LastFireTime != 0f && vp_FPInput.LastFireTime + 0.5f > Time.realtimeSinceStartup)
		{
			return;
		}
		if (vp_FPInput.LastJumpTime + 0.4f > Time.time)
		{
			return;
		}
		if (!vp_FPController.cs.Grounded)
		{
			return;
		}
		if (Input.GetKey(vp_FPInput.control[4]))
		{
			this.Player.Run.TryStart();
			vp_FPInput.UpdateSpeed(true);
		}
		else
		{
			this.Player.Run.TryStop();
			vp_FPInput.UpdateSpeed(false);
		}
	}

	protected virtual void InputJump()
	{
		if (Input.GetKey(vp_FPInput.control[5]))
		{
			this.Player.Jump.TryStart();
		}
		else
		{
			this.Player.Jump.Stop(0f);
		}
	}

	protected virtual void InputCrouch()
	{
		if (Input.GetKey(vp_FPInput.control[6]))
		{
			this.Player.Crouch.TryStart();
		}
		else if (vp_FPController.CanStop_CrouchNoLimit())
		{
			this.Player.Crouch.TryStop();
		}
	}

	protected virtual void InputWalk()
	{
		if (Input.GetKey(vp_FPInput.control[20]))
		{
			this.Player.Run.TryStop();
			vp_FPController.cs.MotorAcceleration = 0.06f;
		}
	}

	protected virtual void InputZoom()
	{
		if (this.Player.Reload.Active)
		{
			return;
		}
		if (Time.time < vp_FPInput.LastZoomTime)
		{
			return;
		}
		if ((BasePlayer.currweapon != null && WeaponData.GetData(BasePlayer.currweapon.data.wid).zoomMode == 2) || Options.zoomlock == 1)
		{
			if (vp_FPInput.ZoomTap)
			{
				this.Player.Zoom.TryStart();
				Crosshair.SetActive(false);
			}
			if (Input.GetMouseButton(1))
			{
				if (vp_FPCamera.currWeapon == null)
				{
					return;
				}
				if (WeaponData.GetData(BasePlayer.currweapon.data.wid).zoomMode == 0)
				{
					return;
				}
				if (vp_FPInput.ZoomTap)
				{
					vp_FPInput.ZoomTap = false;
					this.Player.Zoom.TryStop();
					Crosshair.SetActive(true);
					vp_FPInput.LastZoomTime = Time.time + 0.25f;
					Crosshair.SetOffsetNull();
					return;
				}
				vp_FPInput.ZoomTap = true;
				this.Player.Zoom.TryStart();
				Crosshair.SetActive(false);
				vp_FPInput.LastZoomTime = Time.time + 0.25f;
				return;
			}
		}
		else if (Input.GetMouseButton(1))
		{
			if (vp_FPCamera.currWeapon == null)
			{
				return;
			}
			if (WeaponData.GetData(BasePlayer.currweapon.data.wid).zoomMode == 0)
			{
				return;
			}
			vp_FPInput.ZoomTap = false;
			this.Player.Zoom.TryStart();
			Crosshair.SetActive(false);
		}
		else
		{
			if (this.Player.Zoom.Active)
			{
				Crosshair.SetOffsetNull();
			}
			vp_FPInput.ZoomTap = false;
			this.Player.Zoom.TryStop();
			Crosshair.SetActive(true);
		}
	}

	protected virtual void InputAttack()
	{
		if (ScoreBoard.gamemode == 3 && BasePlayer.team == 0)
		{
			Zombie.CheckZombieWeapon();
			return;
		}
		if (Input.GetMouseButton(0) && vp_FPCamera.currWeapon != null && vp_FPCamera.currWeapon.WeaponID != 27 && vp_FPCamera.currWeapon.WeaponID != 29 && vp_FPCamera.currWeapon.WeaponID != 30)
		{
			if (vp_FPCamera.currWeapon != null && vp_FPCamera.currWeapon.WeaponID == 28)
			{
				vp_FPCamera.returnWeapon = vp_FPCamera.currWeapon;
				this.Player.SetWeaponByName.Try("26KNIFE");
			}
			else
			{
				this.Player.Attack.TryStart();
			}
		}
		else if (vp_FPCamera.currWeapon != null && vp_FPCamera.currWeapon.WeaponID != 27 && vp_FPCamera.currWeapon.WeaponID != 29 && vp_FPCamera.currWeapon.WeaponID != 30 && !vp_FPInput.grenadeThrowStarting && !vp_FPInput.grenadeThrowEnding)
		{
			this.Player.Attack.TryStop();
		}
		if (Input.GetMouseButtonDown(0) && vp_FPCamera.currWeapon != null && !vp_FPInput.grenadeThrowStarting && !vp_FPInput.grenadeThrowEnding)
		{
			if (vp_FPCamera.currWeapon.WeaponID == 27 && BasePlayer.fg > 0)
			{
				vp_FPInput.activeGrenade = 0;
				vp_FPInput.grenadeThrowStarting = true;
			}
			else if (vp_FPCamera.currWeapon.WeaponID == 29 && BasePlayer.fb > 0)
			{
				vp_FPInput.activeGrenade = 1;
				vp_FPInput.grenadeThrowStarting = true;
			}
			else if (vp_FPCamera.currWeapon.WeaponID == 30 && BasePlayer.sg > 0)
			{
				vp_FPInput.activeGrenade = 2;
				vp_FPInput.grenadeThrowStarting = true;
			}
		}
		if (Input.GetMouseButtonUp(0) && !Input.GetMouseButton(0) && vp_FPCamera.currWeapon != null && !vp_FPInput.grenadeThrowEnding && vp_FPInput.grenadeActivated)
		{
			if (vp_FPCamera.currWeapon.WeaponID == 27 && BasePlayer.fg > 0)
			{
				BasePlayer.fg--;
				HUD.SetFG(BasePlayer.fg);
				vp_FPInput.grenadeThrowEnding = true;
			}
			else if (vp_FPCamera.currWeapon.WeaponID == 29 && BasePlayer.fb > 0)
			{
				BasePlayer.fb--;
				HUD.SetFB(BasePlayer.fb);
				vp_FPInput.grenadeThrowEnding = true;
			}
			else if (vp_FPCamera.currWeapon.WeaponID == 30 && BasePlayer.sg > 0)
			{
				BasePlayer.sg--;
				HUD.SetSG(BasePlayer.sg);
				vp_FPInput.grenadeThrowEnding = true;
			}
		}
		if (Input.GetMouseButtonUp(0))
		{
			vp_FPInput.CanPistolFire = true;
		}
		if (Input.GetKeyDown(vp_FPInput.control[10]) && !vp_FPInput.grenadeActivated && vp_FPInput.deactivateGrenadeTimer == 0f)
		{
			MonoBehaviour.print("throw grenade");
			if (vp_FPCamera.currWeapon != null && (vp_FPCamera.currWeapon.WeaponID == 27 || vp_FPCamera.currWeapon.WeaponID == 29 || vp_FPCamera.currWeapon.WeaponID == 30))
			{
				if (!vp_FPInput.grenadeThrowStarting && !vp_FPInput.grenadeThrowEnding)
				{
					if (vp_FPCamera.currWeapon.WeaponID == 27 && BasePlayer.fg > 0)
					{
						vp_FPInput.activeGrenade = 0;
						vp_FPInput.grenadeThrowStarting = true;
						BasePlayer.fg--;
						HUD.SetFG(BasePlayer.fg);
						vp_FPInput.grenadeThrowEnding = true;
						vp_FPInput.deactivateGrenadeTimer = Time.time + 0.9f;
					}
					else if (vp_FPCamera.currWeapon.WeaponID == 29 && BasePlayer.fb > 0)
					{
						vp_FPInput.activeGrenade = 1;
						vp_FPInput.grenadeThrowStarting = true;
						BasePlayer.fb--;
						HUD.SetFB(BasePlayer.fb);
						vp_FPInput.grenadeThrowEnding = true;
						vp_FPInput.deactivateGrenadeTimer = Time.time + 0.9f;
					}
					else if (vp_FPCamera.currWeapon.WeaponID == 30 && BasePlayer.sg > 0)
					{
						vp_FPInput.activeGrenade = 2;
						vp_FPInput.grenadeThrowStarting = true;
						BasePlayer.sg--;
						HUD.SetSG(BasePlayer.sg);
						vp_FPInput.grenadeThrowEnding = true;
						vp_FPInput.deactivateGrenadeTimer = Time.time + 0.9f;
					}
				}
			}
			else if (BasePlayer.selectedGrenade == 0 && BasePlayer.fg > 0)
			{
				vp_FPInput.grenadeActivated = true;
				vp_FPInput.fastGrenade = true;
				vp_FPInput.activeGrenade = 0;
				vp_FPCamera.returnWeapon = vp_FPCamera.currWeapon;
				this.Player.SetWeaponByName.Try("27FG");
				BasePlayer.fg--;
				HUD.SetFG(BasePlayer.fg);
				vp_FPInput.deactivateGrenadeTimer = Time.time + 0.9f;
			}
			else if (BasePlayer.selectedGrenade == 1 && BasePlayer.fb > 0)
			{
				vp_FPInput.grenadeActivated = true;
				vp_FPInput.fastGrenade = true;
				vp_FPInput.activeGrenade = 1;
				vp_FPCamera.returnWeapon = vp_FPCamera.currWeapon;
				this.Player.SetWeaponByName.Try("29FB");
				BasePlayer.fb--;
				HUD.SetFG(BasePlayer.fb);
				vp_FPInput.deactivateGrenadeTimer = Time.time + 0.9f;
			}
			else if (BasePlayer.selectedGrenade == 2 && BasePlayer.sg > 0)
			{
				vp_FPInput.grenadeActivated = true;
				vp_FPInput.fastGrenade = true;
				vp_FPInput.activeGrenade = 2;
				vp_FPCamera.returnWeapon = vp_FPCamera.currWeapon;
				this.Player.SetWeaponByName.Try("30SG");
				BasePlayer.sg--;
				HUD.SetFG(BasePlayer.sg);
				vp_FPInput.deactivateGrenadeTimer = Time.time + 0.9f;
			}
		}
	}

	protected virtual void InputReload()
	{
		if (Input.GetKeyDown(vp_FPInput.control[7]))
		{
			this.Player.Reload.TryStart();
			this.Player.Zoom.TryStop();
			Crosshair.SetActive(true);
		}
	}

	protected virtual void InputSetWeapon()
	{
		if (ScoreBoard.gamemode == 3 && BasePlayer.team == 0)
		{
			Zombie.CheckZombieWeapon();
			return;
		}
		if (Input.GetKeyDown(vp_FPInput.control[8]) && vp_FPCamera.lastWeapon != null && this.Player.CurrentWeaponWielded.Get())
		{
			int weaponSlot = vp_FPCamera.lastWeapon.WeaponSlot;
			if (BasePlayer.weapon[weaponSlot] != null)
			{
				vp_FPCamera.lastWeapon = vp_FPCamera.currWeapon;
				vp_FPCamera.currWeapon = null;
				vp_FPCamera.returnWeapon = null;
				this.Player.SetWeaponByName.Try(BasePlayer.weapon[weaponSlot].data.selectName);
				if (weaponSlot == 3)
				{
					BasePlayer.selectedGrenade = 0;
				}
				else if (weaponSlot == 5)
				{
					BasePlayer.selectedGrenade = 1;
				}
				else if (weaponSlot == 6)
				{
					BasePlayer.selectedGrenade = 2;
				}
			}
		}
		if (Input.GetKeyDown(vp_FPInput.control[9]) && BasePlayer.weapon[2] != null && vp_FPCamera.currWeapon != null && vp_FPCamera.currWeapon.WeaponID != 26)
		{
			vp_FPCamera.returnWeapon = vp_FPCamera.currWeapon;
			this.Player.SetWeaponByName.Try("26KNIFE");
		}
		int num = -1;
		if (Input.GetAxis("Mouse ScrollWheel") < 0f)
		{
			if (vp_FPCamera.currWeapon != null)
			{
				if (vp_FPCamera.currWeapon.WeaponSlot == 0)
				{
					num = 1;
				}
				else if (vp_FPCamera.currWeapon.WeaponSlot == 1)
				{
					num = 2;
				}
				else if (vp_FPCamera.currWeapon.WeaponSlot == 2)
				{
					num = 0;
				}
			}
		}
		else if (Input.GetAxis("Mouse ScrollWheel") > 0f && vp_FPCamera.currWeapon != null)
		{
			if (vp_FPCamera.currWeapon.WeaponSlot == 0)
			{
				num = 2;
			}
			else if (vp_FPCamera.currWeapon.WeaponSlot == 2)
			{
				num = 1;
			}
			else if (vp_FPCamera.currWeapon.WeaponSlot == 1)
			{
				num = 0;
			}
		}
		if ((Input.GetKeyDown(KeyCode.Alpha1) || num == 0) && BasePlayer.weapon[0] != null)
		{
			vp_FPCamera.lastWeapon = vp_FPCamera.currWeapon;
			vp_FPCamera.returnWeapon = null;
			this.Player.SetWeaponByName.Try(BasePlayer.weapon[0].data.selectName);
		}
		if ((Input.GetKeyDown(KeyCode.Alpha2) || num == 1) && BasePlayer.weapon[1] != null)
		{
			vp_FPCamera.lastWeapon = vp_FPCamera.currWeapon;
			vp_FPCamera.returnWeapon = null;
			this.Player.SetWeaponByName.Try(BasePlayer.weapon[1].data.selectName);
		}
		if ((Input.GetKeyDown(KeyCode.Alpha3) || num == 2) && BasePlayer.weapon[2] != null)
		{
			vp_FPCamera.lastWeapon = vp_FPCamera.currWeapon;
			vp_FPCamera.returnWeapon = null;
			this.Player.SetWeaponByName.Try("28KNIFERUN");
		}
		if (Input.GetKeyDown(KeyCode.Alpha4))
		{
			vp_FPInput.fastGrenade = false;
			if (vp_FPCamera.currWeapon != null && (vp_FPCamera.currWeapon.WeaponSlot == 3 || vp_FPCamera.currWeapon.WeaponSlot == 5 || vp_FPCamera.currWeapon.WeaponSlot == 6))
			{
				if (BasePlayer.selectedGrenade == 0 && BasePlayer.fb > 0)
				{
					num = 5;
				}
				else if (BasePlayer.selectedGrenade == 0 && BasePlayer.sg > 0)
				{
					num = 6;
				}
				else if (BasePlayer.selectedGrenade == 1 && BasePlayer.sg > 0)
				{
					num = 6;
				}
				else if (BasePlayer.selectedGrenade == 1 && BasePlayer.fg > 0)
				{
					num = 3;
				}
				else if (BasePlayer.selectedGrenade == 2 && BasePlayer.fg > 0)
				{
					num = 3;
				}
				else if (BasePlayer.selectedGrenade == 2 && BasePlayer.fb > 0)
				{
					num = 5;
				}
			}
			else if (BasePlayer.fg > 0)
			{
				num = 3;
			}
			else if (BasePlayer.fb > 0)
			{
				num = 5;
			}
			else if (BasePlayer.sg > 0)
			{
				num = 6;
			}
		}
		if (num == 3)
		{
			if (BasePlayer.weapon[3] == null)
			{
				BasePlayer.currweapon = null;
				vp_FPWeaponHandler.cs.m_CurrentWeaponID = 0;
				BasePlayer.weapon[3] = new CWeapon(WeaponData.GetData(27));
			}
			vp_FPCamera.lastWeapon = vp_FPCamera.currWeapon;
			vp_FPCamera.returnWeapon = null;
			BasePlayer.selectedGrenade = 0;
			this.Player.SetWeaponByName.Try(BasePlayer.weapon[3].data.selectName);
		}
		if (num == 5)
		{
			BasePlayer.currweapon = null;
			vp_FPWeaponHandler.cs.m_CurrentWeaponID = 0;
			BasePlayer.weapon[5] = new CWeapon(WeaponData.GetData(29));
			vp_FPCamera.lastWeapon = vp_FPCamera.currWeapon;
			vp_FPCamera.returnWeapon = null;
			BasePlayer.selectedGrenade = 1;
			this.Player.SetWeaponByName.Try(BasePlayer.weapon[5].data.selectName);
		}
		if (num == 6)
		{
			if (BasePlayer.weapon[6] == null)
			{
				BasePlayer.currweapon = null;
				vp_FPWeaponHandler.cs.m_CurrentWeaponID = 0;
				BasePlayer.weapon[6] = new CWeapon(WeaponData.GetData(30));
			}
			vp_FPCamera.lastWeapon = vp_FPCamera.currWeapon;
			vp_FPCamera.returnWeapon = null;
			BasePlayer.selectedGrenade = 2;
			this.Player.SetWeaponByName.Try(BasePlayer.weapon[6].data.selectName);
		}
		if ((Input.GetKeyDown(KeyCode.Alpha5) || num == 4) && BasePlayer.weapon[4] != null)
		{
			vp_FPCamera.lastWeapon = vp_FPCamera.currWeapon;
			vp_FPCamera.returnWeapon = null;
			this.Player.SetWeaponByName.Try(BasePlayer.weapon[4].data.selectName);
		}
	}

	protected virtual void InputDropWeapon()
	{
		if (ScoreBoard.gamemode == 3 && BasePlayer.team == 0)
		{
			Zombie.CheckZombieWeapon();
			return;
		}
		if (Input.GetKeyDown(vp_FPInput.control[11]) && BasePlayer.currweapon != null)
		{
			vp_FPWeapon.AliveWeaponDrop();
		}
	}

	protected virtual void UpdatePause()
	{
	}

	protected virtual void UpdateCursorLock()
	{
		this.m_MousePos.x = Input.mousePosition.x;
		this.m_MousePos.y = (float)Screen.height - Input.mousePosition.y;
		if (this.ForceCursor)
		{
			Screen.lockCursor = false;
			return;
		}
		if (Input.GetMouseButton(0) || Input.GetMouseButton(1) || Input.GetMouseButton(2))
		{
			if (this.MouseCursorZones.Length > 0)
			{
				Rect[] mouseCursorZones = this.MouseCursorZones;
				for (int i = 0; i < mouseCursorZones.Length; i++)
				{
					Rect rect = mouseCursorZones[i];
					if (rect.Contains(this.m_MousePos))
					{
						Screen.lockCursor = false;
						goto IL_C4;
					}
				}
			}
			Screen.lockCursor = true;
		}
		IL_C4:
		if (Input.GetKeyUp(KeyCode.Return) || Input.GetKeyUp(KeyCode.KeypadEnter))
		{
			Screen.lockCursor = !Screen.lockCursor;
		}
	}

	protected virtual void Awake()
	{
		vp_FPInput.cs = this;
		this.Player = (vp_FPPlayerEventHandler)base.transform.root.GetComponentInChildren(typeof(vp_FPPlayerEventHandler));
		this.m_FPCamera = base.GetComponentInChildren<vp_FPCamera>();
	}

	protected virtual void OnEnable()
	{
		if (this.Player != null)
		{
			this.Player.Register(this);
		}
	}

	protected virtual void OnDisable()
	{
		if (this.Player != null)
		{
			this.Player.Unregister(this);
		}
	}
}
