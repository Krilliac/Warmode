using System;
using UnityEngine;

public class HUD : MonoBehaviour
{
	public static HUD cs;

	private static bool show;

	private static bool forcehide;

	private Texture2D tBlack;

	private Texture2D tRed;

	private Texture2D tBlue;

	private Texture2D tHealthIcon;

	private Texture2D tYellow;

	private Texture2D tArmorIcon;

	private Texture2D tAmmoIcon;

	private Texture2D tFGIconOrange;

	private Texture2D tFGIconGray;

	private Texture2D tFGIconWhite;

	private Texture2D tFBIconOrange;

	private Texture2D tFBIconGray;

	private Texture2D tFBIconWhite;

	private Texture2D tSGIconOrange;

	private Texture2D tSGIconGray;

	private Texture2D tSGIconWhite;

	private Texture2D tBombIconNormal;

	private Texture2D tBombIconActive;

	private Texture2D tDiffuseIconNormal;

	private Texture2D tDiffuseIconActive;

	private Texture2D tGradientLeft;

	private Texture2D tGradientRight;

	private Texture2D tMoneyIcon;

	private Rect rAmmoBack;

	private Rect rHealthBack;

	private Rect rGrenBack;

	private Rect rBombBack;

	private Rect rDiffuseBack;

	private Rect rDefuseKitBack;

	private Rect rHealthIcon;

	private Rect rHealthNumber;

	private Rect rArmorIcon;

	private Rect rArmorNumber;

	private Rect rFGIcon;

	private Rect rFBIcon;

	private Rect rSGIcon;

	private Rect rBombIcon;

	private Rect rTopBombIcon;

	private Rect rDiffuseIcon;

	private Rect rDiffuseLine;

	private Rect rDiffuseProgress;

	private Rect rGrenNumber;

	private Rect rAmmoIcon;

	private Rect rAmmoNumber;

	private Rect rAmmoNumber2;

	private Rect rMoneyBack;

	private Rect rMoneyIcon;

	private Rect rMoneyNumber;

	private Color c5 = new Color(1f, 1f, 1f, 0.25f);

	private Color cb = new Color(0f, 0f, 0f, 0.75f);

	public static string sHealth = "-";

	public static string sArmor = "-";

	public static string sFG = "0";

	public static string sFB = "0";

	public static string sSG = "0";

	private static string sMoney = "0";

	private static int Money;

	private static int bombState;

	private static int diffuseState;

	private static float tBombIndic;

	private static float tBombInterv = 1f;

	private static float tDiffuseIndic;

	private static float bombDiffuseVal;

	private static float bombDiffuseMax = 1f;

	private static bool respawnshow;

	private static float respawntime;

	private static float respawnseconds;

	private Rect rRespawnBack;

	private Rect rRespawnSeconds;

	private Rect rRespawnBar;

	private Rect rRespawnHeader;

	private Texture2D tGray0;

	private Texture2D tWhite;

	private static AudioSource a;

	private static AudioClip briefing;

	private static AudioClip champion;

	public static float currwidth;

	public static float currheight;

	public static float lastwidth;

	public void PostAwake()
	{
		HUD.cs = this;
		this.tBlack = TEX.GetTextureByName("black");
		this.tGray0 = TEX.GetTextureByName("gray0");
		this.tWhite = TEX.GetTextureByName("white");
		this.tYellow = TEX.GetTextureByName("yellow");
		this.tHealthIcon = TEX.GetTextureByName("GUI/health_icon");
		this.tArmorIcon = TEX.GetTextureByName("GUI/armor_icon");
		this.tAmmoIcon = TEX.GetTextureByName("GUI/ammo_icon");
		this.tFGIconOrange = TEX.GetTextureByName("GUI/fg_orange");
		this.tFGIconGray = TEX.GetTextureByName("GUI/fg_gray");
		this.tFGIconWhite = TEX.GetTextureByName("GUI/fg_white");
		this.tFBIconOrange = TEX.GetTextureByName("GUI/fb_orange");
		this.tFBIconGray = TEX.GetTextureByName("GUI/fb_gray");
		this.tFBIconWhite = TEX.GetTextureByName("GUI/fb_white");
		this.tSGIconOrange = TEX.GetTextureByName("GUI/sg_orange");
		this.tSGIconGray = TEX.GetTextureByName("GUI/sg_gray");
		this.tSGIconWhite = TEX.GetTextureByName("GUI/sg_white");
		this.tBombIconNormal = TEX.GetTextureByName("GUI/c4_orange");
		this.tBombIconActive = TEX.GetTextureByName("GUI/c4_red");
		this.tDiffuseIconNormal = TEX.GetTextureByName("GUI/defuse_orange");
		this.tDiffuseIconActive = TEX.GetTextureByName("GUI/defuse_red");
		this.tGradientLeft = TEX.GetTextureByName("GUI/left_gradient");
		this.tGradientRight = TEX.GetTextureByName("GUI/right_gradient");
		this.tAmmoIcon = TEX.GetTextureByName("GUI/bullets_icon");
		this.tMoneyIcon = TEX.GetTextureByName("GUI/money_icon");
		this.tRed = TEX.GetTextureByName("red");
		this.tBlue = TEX.GetTextureByName("blue");
		this.OnResize();
		HUD.sHealth = "-";
		HUD.sArmor = "-";
		HUD.sMoney = "0";
		HUD.Money = 0;
		HUD.bombState = 0;
		HUD.diffuseState = 0;
		HUD.show = false;
		if (HUD.a == null)
		{
			HUD.a = base.gameObject.AddComponent<AudioSource>();
		}
		if (HUD.briefing == null)
		{
			HUD.briefing = ContentLoader_.LoadAudio("briefing");
		}
		if (HUD.champion == null)
		{
			HUD.champion = ContentLoader_.LoadAudio("champion");
		}
	}

	public void OnResize()
	{
		this.rHealthBack = new Rect(0f, (float)Screen.height - GUI2.YRES(32f), GUI2.YRES(160f), GUI2.YRES(32f));
		this.rAmmoBack = new Rect((float)Screen.width - GUI2.YRES(96f), this.rHealthBack.y, GUI2.YRES(96f), GUI2.YRES(32f));
		this.rGrenBack = new Rect((float)Screen.width - GUI2.YRES(80f), this.rHealthBack.y - GUI2.YRES(22f), GUI2.YRES(80f), GUI2.YRES(20f));
		this.rMoneyBack = new Rect(0f, this.rHealthBack.y - GUI2.YRES(22f), GUI2.YRES(40f), GUI2.YRES(20f));
		this.rBombBack = new Rect((float)Screen.width - GUI2.YRES(58f), (float)Screen.height - GUI2.YRES(82f), GUI2.YRES(58f), GUI2.YRES(25f));
		this.rDefuseKitBack = new Rect((float)Screen.width - GUI2.YRES(44f), (float)Screen.height - GUI2.YRES(88f), GUI2.YRES(44f), GUI2.YRES(30f));
		this.rHealthIcon = new Rect(GUI2.YRES(4f), (float)Screen.height - GUI2.YRES(28f), GUI2.YRES(24f), GUI2.YRES(24f));
		this.rHealthNumber = new Rect(GUI2.YRES(32f), this.rHealthBack.y, GUI2.YRES(128f), GUI2.YRES(32f));
		this.rArmorIcon = new Rect(GUI2.YRES(80f) + GUI2.YRES(4f), (float)Screen.height - GUI2.YRES(28f), GUI2.YRES(24f), GUI2.YRES(24f));
		this.rArmorNumber = new Rect(GUI2.YRES(80f) + GUI2.YRES(32f), this.rHealthBack.y, GUI2.YRES(128f), GUI2.YRES(32f));
		if (ScoreBoard.gamemode == 3 && BasePlayer.team == 0)
		{
			this.rArmorIcon = new Rect(GUI2.YRES(106f) + GUI2.YRES(4f), (float)Screen.height - GUI2.YRES(28f), GUI2.YRES(24f), GUI2.YRES(24f));
			this.rArmorNumber = new Rect(GUI2.YRES(106f) + GUI2.YRES(32f), this.rHealthBack.y, GUI2.YRES(128f), GUI2.YRES(32f));
		}
		this.rFGIcon = new Rect((float)Screen.width - GUI2.YRES(58f), (float)Screen.height - GUI2.YRES(52f), GUI2.YRES(16f), GUI2.YRES(16f));
		this.rGrenNumber = new Rect((float)Screen.width - GUI2.YRES(18f) - GUI2.YRES(18f), (float)Screen.height - GUI2.YRES(52f), GUI2.YRES(16f), GUI2.YRES(16f));
		this.rFBIcon = new Rect((float)Screen.width - GUI2.YRES(38f), (float)Screen.height - GUI2.YRES(52f), GUI2.YRES(16f), GUI2.YRES(16f));
		this.rSGIcon = new Rect((float)Screen.width - GUI2.YRES(18f), (float)Screen.height - GUI2.YRES(52f), GUI2.YRES(16f), GUI2.YRES(16f));
		this.rBombIcon = new Rect((float)Screen.width - GUI2.YRES(50f), (float)Screen.height - GUI2.YRES(82f), GUI2.YRES(48f), GUI2.YRES(24f));
		this.rDiffuseIcon = new Rect((float)Screen.width - GUI2.YRES(34f), (float)Screen.height - GUI2.YRES(88f), GUI2.YRES(30f), GUI2.YRES(30f));
		this.rDiffuseBack = new Rect((float)Screen.width / 2f - GUI2.YRES(150f), GUI2.YRES(180f), GUI2.YRES(300f), GUI2.YRES(10f));
		this.rDiffuseLine = new Rect((float)Screen.width / 2f - GUI2.YRES(148f), GUI2.YRES(182f), GUI2.YRES(296f), GUI2.YRES(6f));
		this.rAmmoIcon = new Rect((float)Screen.width - GUI2.YRES(32f), (float)Screen.height - GUI2.YRES(16f), GUI2.YRES(28f), GUI2.YRES(14f));
		this.rAmmoNumber = new Rect((float)Screen.width - GUI2.YRES(100f), (float)Screen.height - GUI2.YRES(32f), GUI2.YRES(64f), GUI2.YRES(32f));
		this.rAmmoNumber2 = new Rect((float)Screen.width - GUI2.YRES(32f), (float)Screen.height - GUI2.YRES(32f), GUI2.YRES(28f), GUI2.YRES(16f));
		this.rMoneyIcon = new Rect(GUI2.YRES(4f), (float)Screen.height - GUI2.YRES(52f), GUI2.YRES(16f), GUI2.YRES(16f));
		this.rMoneyNumber = new Rect(GUI2.YRES(4f) + GUI2.YRES(22f), (float)Screen.height - GUI2.YRES(52f), GUI2.YRES(64f), GUI2.YRES(16f));
		this.rRespawnBack = new Rect((float)Screen.width / 2f - GUI2.YRES(90f), (float)Screen.height - GUI2.YRES(80f), GUI2.YRES(180f), GUI2.YRES(20f));
		this.rRespawnSeconds = new Rect((float)Screen.width / 2f - GUI2.YRES(90f), (float)Screen.height - GUI2.YRES(80f), GUI2.YRES(32f), GUI2.YRES(20f));
		this.rRespawnBar = new Rect((float)Screen.width / 2f - GUI2.YRES(90f) + GUI2.YRES(32f) + GUI2.YRES(4f), (float)Screen.height - GUI2.YRES(80f) + GUI2.YRES(4f), GUI2.YRES(140f), GUI2.YRES(12f));
	}

	private void OnGUI()
	{
		if (HUD.forcehide)
		{
			return;
		}
		this.DrawRespawnBar();
		if (!HUD.show)
		{
			return;
		}
		if (SpecCam.show)
		{
			Rect rect = new Rect(0f, (float)Screen.height - GUI2.YRES(32f), (float)Screen.width, GUI2.YRES(32f));
			GUI.color = this.cb;
			GUI.DrawTexture(rect, this.tBlack);
			GUI.color = Color.white;
			if (SpecCam.FID >= 0 && SpecCam.mode == 1 && PlayerControll.Player[SpecCam.FID] != null)
			{
				if (PlayerControll.Player[SpecCam.FID].Team == 0)
				{
					GUI2.DrawTextColorRes(rect, PlayerControll.Player[SpecCam.FID].Name, TextAnchor.MiddleCenter, _Color.Red, 1, 12, true);
				}
				else if (PlayerControll.Player[SpecCam.FID].Team == 1)
				{
					GUI2.DrawTextColorRes(rect, PlayerControll.Player[SpecCam.FID].Name, TextAnchor.MiddleCenter, _Color.Blue, 1, 12, true);
				}
				if (SpecCam.FID == C4.diffuserId)
				{
					this.DrawDiffuseBar();
				}
			}
			else
			{
				GUI2.DrawTextColorRes(rect, Lang.Get("_FREECAM"), TextAnchor.MiddleCenter, _Color.White, 1, 12, true);
			}
			this.DrawTVBars();
			return;
		}
		GUI.DrawTexture(this.rHealthBack, this.tGradientLeft);
		GUI.DrawTexture(this.rAmmoBack, this.tGradientRight);
		if (HUD.bombState != 0)
		{
			GUI.DrawTexture(this.rBombBack, this.tGradientRight);
		}
		GUI.DrawTexture(this.rHealthIcon, this.tHealthIcon);
		GUI2.DrawTextRes(this.rHealthNumber, HUD.sHealth, TextAnchor.MiddleLeft, _Color.White, 0, 28, true);
		GUI.DrawTexture(this.rArmorIcon, this.tArmorIcon);
		GUI2.DrawTextRes(this.rArmorNumber, HUD.sArmor, TextAnchor.MiddleLeft, _Color.White, 0, 28, true);
		GUI.DrawTexture(this.rGrenBack, this.tGradientRight);
		if (BasePlayer.fg > 0)
		{
			if (BasePlayer.selectedGrenade == 0)
			{
				GUI.DrawTexture(this.rFGIcon, this.tFGIconWhite);
			}
			else
			{
				GUI.DrawTexture(this.rFGIcon, this.tFGIconOrange);
			}
		}
		else
		{
			GUI.DrawTexture(this.rFGIcon, this.tFGIconGray);
		}
		if (BasePlayer.fb > 0)
		{
			if (BasePlayer.selectedGrenade == 1)
			{
				GUI.DrawTexture(this.rFBIcon, this.tFBIconWhite);
			}
			else
			{
				GUI.DrawTexture(this.rFBIcon, this.tFBIconOrange);
			}
		}
		else
		{
			GUI.DrawTexture(this.rFBIcon, this.tFBIconGray);
		}
		if (BasePlayer.sg > 0)
		{
			if (BasePlayer.selectedGrenade == 2)
			{
				GUI.DrawTexture(this.rSGIcon, this.tSGIconWhite);
			}
			else
			{
				GUI.DrawTexture(this.rSGIcon, this.tSGIconOrange);
			}
		}
		else
		{
			GUI.DrawTexture(this.rSGIcon, this.tSGIconGray);
		}
		if (HUD.bombState == 1)
		{
			GUI.DrawTexture(this.rBombIcon, this.tBombIconNormal);
			HUD.tBombIndic = 0f;
		}
		if (HUD.bombState == 2)
		{
			if (HUD.tBombIndic < HUD.tBombInterv)
			{
				GUI.DrawTexture(this.rBombIcon, this.tBombIconActive);
			}
			if (HUD.tBombIndic >= HUD.tBombInterv)
			{
				GUI.DrawTexture(this.rBombIcon, this.tBombIconNormal);
			}
			HUD.tBombIndic += Time.deltaTime;
			if (HUD.tBombIndic > HUD.tBombInterv * 2f)
			{
				HUD.tBombIndic = 0f;
			}
		}
		if (HUD.diffuseState == 1)
		{
			GUI.DrawTexture(this.rDefuseKitBack, this.tGradientRight);
			GUI.DrawTexture(this.rDiffuseIcon, this.tDiffuseIconNormal);
		}
		this.DrawDiffuseBar();
		GUI.DrawTexture(this.rMoneyBack, this.tGradientLeft);
		GUI.DrawTexture(this.rMoneyIcon, this.tMoneyIcon);
		GUI2.DrawTextRes(this.rMoneyNumber, HUD.sMoney, TextAnchor.MiddleLeft, _Color.White, 0, 16, true);
		GUI.DrawTexture(this.rAmmoIcon, this.tAmmoIcon);
		if (BasePlayer.currweapon != null && WeaponData.GetData(BasePlayer.currweapon.data.wid).maxBackpack > 0)
		{
			GUI2.DrawTextRes(this.rAmmoNumber, BasePlayer.currweapon.sClip, TextAnchor.MiddleRight, _Color.White, 0, 28, true);
			GUI2.DrawTextRes(this.rAmmoNumber2, BasePlayer.sAmmo[BasePlayer.currweapon.data.ammoType], TextAnchor.MiddleCenter, _Color.White, 0, 16, true);
		}
	}

	private void DrawDiffuseBar()
	{
		if (HUD.bombDiffuseVal >= 0f)
		{
			this.rDiffuseProgress = new Rect((float)Screen.width / 2f - GUI2.YRES(148f), GUI2.YRES(182f), GUI2.YRES(296f * (HUD.bombDiffuseVal / HUD.bombDiffuseMax)), GUI2.YRES(6f));
			GUI.DrawTexture(this.rDiffuseBack, this.tBlack);
			GUI.DrawTexture(this.rDiffuseLine, this.tGray0);
			GUI.DrawTexture(this.rDiffuseProgress, this.tYellow);
		}
	}

	public static void SetActive(bool val)
	{
		HUD.show = val;
	}

	public static void ForceHide(bool val)
	{
		HUD.forcehide = val;
	}

	public static int GetMoney()
	{
		return HUD.Money;
	}

	public static void SetMoney(int val)
	{
		HUD.Money = val;
		HUD.sMoney = HUD.Money.ToString();
	}

	public static void SetMoneyInc(int val)
	{
		HUD.Money += val;
		HUD.sMoney = HUD.Money.ToString();
	}

	public static void SetFG(int val)
	{
		HUD.sFG = val.ToString();
	}

	public static void SetFB(int val)
	{
		HUD.sFB = val.ToString();
	}

	public static void SetSG(int val)
	{
		HUD.sSG = val.ToString();
	}

	public static void SetArmor(int val)
	{
		HUD.sArmor = val.ToString();
	}

	public static void SetBombState(int val)
	{
		HUD.bombState = val;
	}

	public static void SetDiffuseState(int val)
	{
		HUD.diffuseState = val;
	}

	public static void SetDiffuseProgress(float maxVal, float val)
	{
		if (HUD.diffuseState == 1)
		{
			maxVal /= 2f;
		}
		HUD.bombDiffuseMax = maxVal;
		HUD.bombDiffuseVal = ((val >= maxVal) ? maxVal : val);
	}

	private void Update()
	{
		if (HUD.lastwidth != (float)Screen.width)
		{
			HUD.lastwidth = (float)Screen.width;
			base.gameObject.BroadcastMessage("OnResize", SendMessageOptions.DontRequireReceiver);
			HUD.currwidth = (float)Screen.width;
			HUD.currheight = (float)Screen.height;
			MapLoader.UpdateReflectionProbe();
		}
		if (Input.GetKeyDown(KeyCode.F9))
		{
			Options.ToggleFullScreen();
		}
	}

	public static void SetRespawnBar(int val)
	{
		HUD.respawnshow = true;
		HUD.respawnseconds = (float)val;
		HUD.respawntime = Time.time + HUD.respawnseconds;
	}

	public static void ResetRespawnBar()
	{
		HUD.respawnshow = false;
	}

	private void DrawRespawnBar()
	{
		if (!HUD.respawnshow)
		{
			return;
		}
		float num = HUD.respawntime - Time.time;
		if (num <= 0f)
		{
			HUD.respawnshow = false;
		}
		float num2 = num / HUD.respawnseconds;
		GUI.color = new Color(1f, 1f, 1f, 0.75f);
		GUI.DrawTexture(this.rRespawnBack, this.tBlack);
		GUI.color = Color.white;
		GUI.DrawTexture(this.rRespawnSeconds, this.tGray0);
		GUI.DrawTexture(new Rect(this.rRespawnBar.x, this.rRespawnBar.y, this.rRespawnBar.width - this.rRespawnBar.width * num2, this.rRespawnBar.height), this.tWhite);
		GUI2.DrawTextRes(this.rRespawnBar, Lang.Get("_RESPAWN"), TextAnchor.MiddleCenter, _Color.Black, 0, 10, false);
		GUI2.DrawTextRes(this.rRespawnSeconds, num.ToString("00"), TextAnchor.MiddleCenter, _Color.White, 0, 12, false);
	}

	public static void PlayBriefing()
	{
		HUD.a.Stop();
		HUD.a.clip = HUD.briefing;
		HUD.a.volume = Options.menuvol;
		HUD.a.Play();
		HUD.a.playOnAwake = false;
	}

	public static void PlayChampion()
	{
		HUD.a.Stop();
		HUD.a.clip = HUD.champion;
		HUD.a.volume = Options.menuvol;
		HUD.a.Play();
		HUD.a.playOnAwake = false;
	}

	public static void PlayStop()
	{
		HUD.a.Stop();
	}

	private void DrawTVBars()
	{
		int num = (int)GUI2.YRES(26f);
		int num2 = (int)GUI2.YRES(1f);
		int num3 = 0;
		int num4 = 0;
		for (int i = 0; i < 16; i++)
		{
			if (PlayerControll.Player[i] != null)
			{
				if (PlayerControll.Player[i].Team == 0)
				{
					int health = 100;
					if (PlayerControll.Player[i].DeadFlag == 1)
					{
						health = 0;
					}
					this.DrawBarPlayer(new Rect(0f, GUI2.YRES(180f) + (float)((num + num2) * num3), GUI2.YRES(160f), (float)num), i, PlayerControll.Player[i].Name, PlayerControll.Player[i].currweapon, health, 0);
					num3++;
				}
				else if (PlayerControll.Player[i].Team == 1)
				{
					int health2 = 100;
					if (PlayerControll.Player[i].DeadFlag == 1)
					{
						health2 = 0;
					}
					this.DrawBarPlayer(new Rect((float)Screen.width - GUI2.YRES(160f), GUI2.YRES(180f) + (float)((num + num2) * num4), GUI2.YRES(160f), (float)num), i, PlayerControll.Player[i].Name, PlayerControll.Player[i].currweapon, health2, 1);
					num4++;
				}
			}
		}
	}

	private void DrawBarPlayer(Rect r, int id, string name, int wid, int health, int side)
	{
		int num = (int)GUI2.YRES(1f);
		_Color fontcolor = _Color.White;
		if (health == 0)
		{
			fontcolor = _Color.Gray;
		}
		if (side == 0)
		{
			GUI.DrawTexture(r, this.tGradientLeft);
			if (SpecCam.show && SpecCam.mode == 1 && SpecCam.FID == id)
			{
				GUI.DrawTexture(new Rect(r.x, r.y, GUI2.YRES(2f), r.height), this.tYellow);
			}
			GUI2.DrawTextRes(new Rect(r.x + GUI2.YRES(4f), r.y + GUI2.YRES(2f), r.width, GUI2.YRES(16f)), name, TextAnchor.MiddleLeft, fontcolor, 1, 11, true);
			GUI.DrawTexture(new Rect(r.x + GUI2.YRES(4f), r.y + GUI2.YRES(15f), GUI2.YRES(100f), GUI2.YRES(6f)), this.tGray0);
			GUI.DrawTexture(new Rect(r.x + GUI2.YRES(4f) + (float)num, r.y + GUI2.YRES(15f) + (float)num, (GUI2.YRES(100f) - (float)(num * 2)) * ((float)health * 0.01f), GUI2.YRES(6f) - (float)(num * 2)), this.tRed);
			Rect position = new Rect(r.x + GUI2.YRES(108f), r.y, GUI2.YRES(52f), GUI2.YRES(26f));
			if (wid >= 1 && wid <= 5)
			{
				position = new Rect(r.x + GUI2.YRES(108f), r.y + GUI2.YRES(6f), GUI2.YRES(28f), GUI2.YRES(14f));
			}
			if (wid > 0 && WeaponData.GetData(wid).icon2 != null)
			{
				GUI.color = Color.black;
				GUI.DrawTexture(new Rect(position.x + (float)num, position.y + (float)num, position.width, position.height), WeaponData.GetData(wid).icon2);
				GUI.color = Color.white;
				GUI.DrawTexture(position, WeaponData.GetData(wid).icon2);
			}
		}
		else if (side == 1)
		{
			GUI.DrawTexture(r, this.tGradientRight);
			if (SpecCam.show && SpecCam.mode == 1 && SpecCam.FID == id)
			{
				GUI.DrawTexture(new Rect(r.x + r.width - GUI2.YRES(2f), r.y, GUI2.YRES(2f), r.height), this.tYellow);
			}
			GUI2.DrawTextRes(new Rect(r.x, r.y + GUI2.YRES(2f), r.width - GUI2.YRES(4f), GUI2.YRES(16f)), name, TextAnchor.MiddleRight, fontcolor, 1, 11, true);
			GUI.DrawTexture(new Rect(r.x + GUI2.YRES(56f), r.y + GUI2.YRES(15f), GUI2.YRES(100f), GUI2.YRES(6f)), this.tGray0);
			GUI.DrawTexture(new Rect(r.x + GUI2.YRES(56f) + (float)num, r.y + GUI2.YRES(15f) + (float)num, (GUI2.YRES(100f) - (float)(num * 2)) * ((float)health * 0.01f), GUI2.YRES(6f) - (float)(num * 2)), this.tBlue);
			Rect position2 = new Rect(r.x, r.y, GUI2.YRES(52f), GUI2.YRES(26f));
			if (wid >= 1 && wid <= 5)
			{
				position2 = new Rect(r.x + GUI2.YRES(24f), r.y + GUI2.YRES(6f), GUI2.YRES(28f), GUI2.YRES(14f));
			}
			if (wid > 0 && WeaponData.GetData(wid).icon2_inverted != null)
			{
				GUI.color = Color.black;
				GUI.DrawTexture(new Rect(position2.x + (float)num, position2.y + (float)num, position2.width, position2.height), WeaponData.GetData(wid).icon2_inverted);
				GUI.color = Color.white;
				GUI.DrawTexture(position2, WeaponData.GetData(wid).icon2_inverted);
			}
		}
	}
}
