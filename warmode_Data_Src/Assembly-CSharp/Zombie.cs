using System;
using UnityEngine;

public class Zombie : MonoBehaviour
{
	private float lastHitTime;

	private static float repelTimer = 0f;

	private static bool infectedScreen = false;

	private static Texture2D tInfectedScreen;

	private static Rect rInfectedScreen;

	public static AudioClip infectionSound = new AudioClip();

	public static AudioClip attackSound = new AudioClip();

	public static GameObject hitObjectTransform;

	public static Vector3 impulsePoint;

	public static Vector3 impulseDirect;

	private Texture2D tGradient;

	private Rect rInfMsg;

	public static Vector2 repelVector = Vector2.zero;

	public static float repelPower = 1f;

	public void PostAwake()
	{
		Zombie.tInfectedScreen = Resources.Load<Texture2D>("Textures/blood_w");
		Zombie.infectionSound = SND.GetSoundByName("zombie_infection");
		Zombie.attackSound = SND.GetSoundByName("zombie_attack");
		this.tGradient = TEX.GetTextureByName("GUI/left_gradient");
		Zombie.infectedScreen = false;
		this.OnResize();
	}

	public static void Draw()
	{
		if (!Zombie.infectedScreen)
		{
			return;
		}
		Zombie.rInfectedScreen = new Rect(0f, 0f, (float)Screen.width, (float)Screen.height);
		GUI.DrawTexture(Zombie.rInfectedScreen, Zombie.tInfectedScreen);
	}

	public static void SetInfectedScreen(bool val)
	{
		Zombie.infectedScreen = val;
		if (val)
		{
			Zombie.SetInfectedFog();
		}
		else
		{
			Zombie.SetFog();
		}
	}

	public static void SetFog()
	{
		if (ScoreBoard.gamemode != 3)
		{
			Zombie.RemoveFog();
			return;
		}
		Zombie.SetPlayerFog();
	}

	public static void RemoveFog()
	{
		RenderSettings.fog = false;
	}

	public static void SetInfectedFog()
	{
		RenderSettings.fog = true;
		RenderSettings.fogColor = Color.red;
		RenderSettings.fogMode = FogMode.Linear;
		RenderSettings.fogStartDistance = 2f;
		RenderSettings.fogEndDistance = 70f;
		Camera component = GameObject.Find("LocalPlayer/MainCamera").GetComponent<Camera>();
		component.clearFlags = CameraClearFlags.Color;
		component.backgroundColor = RenderSettings.fogColor;
	}

	public static void SetPlayerFog()
	{
		RenderSettings.fog = true;
		RenderSettings.fogColor = new Color(0.4f, 0.4f, 0.4f, 1f);
		RenderSettings.fogMode = FogMode.Linear;
		RenderSettings.fogStartDistance = 1f;
		RenderSettings.fogEndDistance = 14f;
		Camera component = GameObject.Find("LocalPlayer/MainCamera").GetComponent<Camera>();
		component.clearFlags = CameraClearFlags.Color;
		component.backgroundColor = RenderSettings.fogColor;
	}

	public void OnResize()
	{
		this.rInfMsg = new Rect(((float)Screen.width - GUI2.YRES(260f)) / 2f, GUI2.YRES(20f), GUI2.YRES(260f), GUI2.YRES(50f));
	}

	private void OnGUI()
	{
		if (ScoreBoard.gamemode == 3 && GameData.restartroundmode != 1 && BuyMenu.showtime - Time.time > 0f)
		{
			this.DrawInfectionMessage();
		}
	}

	private void DrawInfectionMessage()
	{
		GUI2.DrawTextRes(this.rInfMsg, Lang.Get("До заражения") + " " + (BuyMenu.showtime - Time.time).ToString("0") + " сек.", TextAnchor.MiddleCenter, _Color.Red, 0, 16, true);
	}

	private void Update()
	{
		if (ScoreBoard.gamemode != 3)
		{
			return;
		}
		this.UpdateRepel();
		this.UpdateHit();
	}

	private void UpdateRepel()
	{
		if (Time.realtimeSinceStartup > Zombie.repelTimer)
		{
			Zombie.repelVector = Vector2.zero;
		}
	}

	public static void SetRepel(Vector3 direct, float power, float time)
	{
		Zombie.repelVector = direct;
		Zombie.repelPower = power;
		Zombie.repelTimer = Time.realtimeSinceStartup + time;
	}

	private void UpdateHit()
	{
		if (DeadCam.show || SpecCam.show)
		{
			return;
		}
		if (BasePlayer.team != 0)
		{
			return;
		}
		if (BasePlayer.currweapon == null)
		{
			if (BasePlayer.weapon[7] != null)
			{
				vp_FPWeaponHandler.cs.m_CurrentWeaponID = 0;
				vp_FPInput.cs.Player.SetWeaponByName.Try(BasePlayer.weapon[7].data.selectName);
			}
			return;
		}
		if (BasePlayer.currweapon.data.wid != 31)
		{
			if (BasePlayer.weapon[7] != null)
			{
				BasePlayer.currweapon = null;
				vp_FPWeaponHandler.cs.m_CurrentWeaponID = 0;
				vp_FPInput.cs.Player.SetWeaponByName.Try(BasePlayer.weapon[7].data.selectName);
			}
			return;
		}
		if (Time.realtimeSinceStartup < this.lastHitTime + 0.5f)
		{
			return;
		}
		if (Input.GetMouseButtonDown(0))
		{
			this.lastHitTime = Time.realtimeSinceStartup;
			if (!SpecCam.show)
			{
				Client.cs.send_attack();
			}
			vp_FPWeapon.cs.ZombieAttack();
			if (UnityEngine.Random.Range(0, 2) == 0)
			{
				vp_FPWeapon.SetZombieAnimationState(1);
			}
			else
			{
				vp_FPWeapon.SetZombieAnimationState(2);
			}
		}
	}

	public static void CheckZombieWeapon()
	{
	}
}
