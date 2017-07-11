using System;
using System.Collections.Generic;
using UnityEngine;

public class BasePlayer : MonoBehaviour
{
	public static GameObject go = null;

	public static Transform repelPivotTransform = null;

	public static Transform repelTargetTransform = null;

	public static int team = 0;

	public static int deadflag = 1;

	public static int maxammo = 10;

	public static int health = 0;

	public static CWeapon[] weapon = new CWeapon[10];

	public static CWeapon currweapon = null;

	public static int[] ammo = new int[10];

	public static string[] sAmmo = new string[]
	{
		string.Empty,
		string.Empty,
		string.Empty,
		string.Empty,
		string.Empty,
		string.Empty,
		string.Empty,
		string.Empty,
		string.Empty,
		string.Empty
	};

	public static int fg = 0;

	public static int fb = 0;

	public static int sg = 0;

	public static int defuse = 0;

	public static int selectedGrenade = 0;

	public static float spawntime = 0f;

	public static bool bomb = false;

	public static int lastdroppeduid = -1;

	private void Awake()
	{
		BasePlayer.go = base.gameObject;
		BasePlayer.repelPivotTransform = GameObject.Find("LocalPlayer/RepelPivot").transform;
		BasePlayer.repelTargetTransform = GameObject.Find("LocalPlayer/RepelPivot/RepelTarget").transform;
		BasePlayer.weapon[0] = null;
		BasePlayer.weapon[1] = null;
		BasePlayer.weapon[2] = null;
		BasePlayer.weapon[3] = null;
		BasePlayer.weapon[4] = null;
		BasePlayer.weapon[5] = null;
		BasePlayer.weapon[6] = null;
		BasePlayer.weapon[7] = null;
	}

	public static void Init()
	{
		BasePlayer.deadflag = 1;
		BasePlayer.fg = 0;
		BasePlayer.fb = 0;
		BasePlayer.sg = 0;
		Message.SetDead(false);
	}

	public static void DevSpawn()
	{
		int num = UnityEngine.Random.Range(0, 8);
		UnityEngine.Object[] array = UnityEngine.Object.FindObjectsOfType(typeof(Spawn));
		List<Spawn> list = new List<Spawn>();
		List<Spawn> list2 = new List<Spawn>();
		UnityEngine.Object[] array2 = array;
		for (int i = 0; i < array2.Length; i++)
		{
			Spawn spawn = (Spawn)array2[i];
			if (spawn.Team == 0)
			{
				list.Add(spawn);
			}
			else if (spawn.Team == 1)
			{
				list2.Add(spawn);
			}
		}
		GameObject gameObject = null;
		if (BasePlayer.team == 0)
		{
			if (num >= list.Count)
			{
				return;
			}
			gameObject = list[num].gameObject;
		}
		else if (BasePlayer.team == 1)
		{
			if (num >= list2.Count)
			{
				return;
			}
			gameObject = list2[num].gameObject;
		}
		BasePlayer.Spawn(gameObject.transform.position.x, gameObject.transform.position.y, gameObject.transform.position.z, gameObject.transform.eulerAngles.y);
	}

	public static void Spawn(float x, float y, float z, float angle)
	{
		GameObject gameObject = GameObject.Find("LocalPlayer");
		vp_FPCamera vp_FPCamera = (vp_FPCamera)UnityEngine.Object.FindObjectOfType(typeof(vp_FPCamera));
		vp_FPController vp_FPController = (vp_FPController)UnityEngine.Object.FindObjectOfType(typeof(vp_FPController));
		gameObject.transform.position = new Vector3(x, y, z);
		vp_FPCamera.SetRotation(new Vector2(0f, angle), true, true);
		ChooseTeam.SetActive(false);
		vp_FPCamera.SetMouseFreeze(false);
		vp_FPController.m_CharacterController.enabled = true;
		BasePlayer.health = 100;
		Crosshair.SetActive(true);
		Crosshair.forceLockCursor = true;
		HUD.SetActive(true);
		HUD.cs.OnResize();
		BasePlayer.deadflag = 0;
		if (Client.ID != -1)
		{
			PlayerControll.Player[Client.ID].DeadFlag = 0;
		}
		vp_FPInput.cs.AllowGameplayInput = true;
		BasePlayer.selectedGrenade = 0;
		vp_FPInput.grenadeThrowStarting = false;
		vp_FPInput.grenadeThrowEnding = false;
		vp_FPInput.fastGrenade = false;
		vp_FPInput.mouseDown = false;
		vp_FPInput.mouseUp = false;
		if (GameData.restartroundmode != 1 && ScoreBoard.gamemode != 0)
		{
			vp_FPWeapon.RemoveAllMapWeapon();
			vp_FPWeapon.RemoveAllMapSmoke();
		}
		CutoffFX.RemoveFX();
		vp_FPCamera.cs.SetFlashFX(0f, 3.5f);
		BlackScreen.SetActive(false);
		Zombie.SetInfectedScreen(false);
		Zombie.repelVector = Vector2.zero;
		if (Client.ID != -1)
		{
			PlayerControll.Player[Client.ID].bomb = false;
		}
		if (Client.ID != -1)
		{
			PlayerControll.Player[Client.ID].defuse = false;
		}
		ScoreTop.UpdateData();
		if (BasePlayer.weapon[0] == null)
		{
			BasePlayer.weapon[0] = null;
		}
		else
		{
			BasePlayer.weapon[0] = new CWeapon(WeaponData.GetData(BasePlayer.weapon[0].data.wid));
		}
		if (BasePlayer.weapon[1] == null)
		{
			BasePlayer.weapon[1] = new CWeapon(WeaponData.GetData(1));
		}
		else
		{
			BasePlayer.weapon[1] = new CWeapon(WeaponData.GetData(BasePlayer.weapon[1].data.wid));
		}
		if (BasePlayer.weapon[2] == null)
		{
			BasePlayer.weapon[2] = new CWeapon(WeaponData.GetData(28));
		}
		BasePlayer.weapon[3] = null;
		if (BasePlayer.weapon[4] == null)
		{
			BasePlayer.weapon[4] = null;
		}
		BasePlayer.weapon[5] = null;
		BasePlayer.weapon[6] = null;
		if (BasePlayer.weapon[7] == null)
		{
			BasePlayer.weapon[7] = new CWeapon(WeaponData.GetData(31));
		}
		if (BasePlayer.weapon[9] == null)
		{
			BasePlayer.weapon[9] = new CWeapon(WeaponData.GetData(26));
		}
		BasePlayer.currweapon = null;
		BasePlayer.CalcAmmo();
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
		BasePlayer.lastdroppeduid = -1;
		BuyMenu.ShowBuy(x, y, z);
		HitEffect.Reset();
		SpecCam.SetActive(false);
		DeadCam.SetActive(false);
		DeadCam.setspectime = 0f;
		Message.ResetMessage();
		HUD.ResetRespawnBar();
		Award.lastaward = 0;
		global::Console.cs.Command("hud 1");
		HUD.PlayStop();
		Message.SetDead(false);
		PlayerNames.hideradar = false;
		C4.GetPlants();
		C4.isplanting = false;
		C4.isdiffusing = false;
		BasePlayer.spawntime = Time.time;
		CC.CheckOnce();
	}

	public static void FreezePosition(bool val)
	{
		vp_FPInput.lockKeyboard = val;
	}

	public static void CalcAmmo()
	{
		for (int i = 0; i < BasePlayer.maxammo; i++)
		{
			BasePlayer.ammo[i] = 0;
			BasePlayer.sAmmo[i] = BasePlayer.ammo[i].ToString();
		}
		for (int j = 0; j < 2; j++)
		{
			if (BasePlayer.weapon[j] != null)
			{
				int ammoType = BasePlayer.weapon[j].data.ammoType;
				BasePlayer.ammo[ammoType] += BasePlayer.weapon[j].data.maxBackpack;
			}
		}
		for (int k = 0; k < BasePlayer.maxammo; k++)
		{
			BasePlayer.sAmmo[k] = BasePlayer.ammo[k].ToString();
		}
	}

	public static void RefreshAmmo(int ammotype, int clip)
	{
		for (int i = 0; i < BasePlayer.maxammo; i++)
		{
			BasePlayer.ammo[i] = 0;
			BasePlayer.sAmmo[i] = BasePlayer.ammo[i].ToString();
		}
		for (int j = 0; j < 2; j++)
		{
			if (BasePlayer.weapon[j] != null)
			{
				BasePlayer.ammo[ammotype] += BasePlayer.weapon[j].data.maxBackpack;
			}
		}
		for (int k = 0; k < BasePlayer.maxammo; k++)
		{
			BasePlayer.sAmmo[k] = BasePlayer.ammo[k].ToString();
		}
	}

	public static void SetCurrWeapon(int slot, int state)
	{
		if (SpecCam.show)
		{
			return;
		}
		if (slot < 0)
		{
			return;
		}
		if (BasePlayer.weapon[slot] == null)
		{
			return;
		}
		BasePlayer.currweapon = BasePlayer.weapon[slot];
		Client.cs.send_currweapon(BasePlayer.currweapon.data.wid, state);
	}

	public static bool CurrWeaponShot()
	{
		if (SpecCam.show)
		{
			return true;
		}
		if (BasePlayer.currweapon == null)
		{
			return false;
		}
		if (BasePlayer.currweapon.clip <= 0)
		{
			return false;
		}
		BasePlayer.currweapon.clip--;
		BasePlayer.currweapon.sClip = BasePlayer.currweapon.clip.ToString();
		return true;
	}

	public static bool EvenClip()
	{
		return BasePlayer.currweapon != null && BasePlayer.currweapon.clip % 2 == 0;
	}

	public static void CurrWeaponReload()
	{
		if (BasePlayer.currweapon == null)
		{
			return;
		}
		int ammoType = BasePlayer.currweapon.data.ammoType;
		int num = BasePlayer.currweapon.data.maxClip;
		if (BasePlayer.currweapon.data.wid == 9 || BasePlayer.currweapon.data.wid == 16)
		{
			BasePlayer.ammo[ammoType]--;
			BasePlayer.currweapon.clip++;
		}
		else
		{
			BasePlayer.ammo[ammoType] += BasePlayer.currweapon.clip;
			if (num > BasePlayer.ammo[ammoType])
			{
				num = BasePlayer.ammo[ammoType];
			}
			BasePlayer.ammo[ammoType] -= num;
			BasePlayer.currweapon.clip = num;
		}
		if (ScoreBoard.gamemode == 3)
		{
			BasePlayer.CalcAmmo();
		}
		BasePlayer.currweapon.sClip = BasePlayer.currweapon.clip.ToString();
		BasePlayer.sAmmo[ammoType] = BasePlayer.ammo[ammoType].ToString();
	}
}
