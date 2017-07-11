using System;
using UnityEngine;

public class Main : MonoBehaviour
{
	private static Texture2D tVig;

	private static Texture2D tBlack;

	public static Texture2D avatar;

	private static Texture2D tLoadingBackgr;

	private static Texture2D tLoadingCooler;

	private static Texture2D tLoadingProgr0;

	private static Texture2D tLoadingProgr1;

	private static Texture2D tLoadingProgr2;

	private static Texture2D tLoadingProgr3;

	private static Texture2D tLoadingProgr4;

	private static Texture2D tLoadingProgr5;

	private static Texture2D tLoadingProgr6;

	private static Texture2D tLoadingProgr7;

	private Rect rLoadingBackgr;

	private Rect rLoadingCooler;

	private Rect rLoadingProgr;

	private Rect rLoadingText;

	private Rect rBombIcon;

	private float coolerAngle;

	private static bool inbuySteam;

	public static bool inbuySocial;

	private int count;

	public static float lastwidth;

	public void PostAwake()
	{
		Main.tBlack = TEX.GetTextureByName("black");
		Main.tLoadingBackgr = (Resources.Load("GUI/loading_backgr") as Texture2D);
		Main.tLoadingCooler = (Resources.Load("GUI/loading_cooler") as Texture2D);
		Main.tLoadingProgr0 = (Resources.Load("GUI/loading_progr0") as Texture2D);
		Main.tLoadingProgr1 = (Resources.Load("GUI/loading_progr1") as Texture2D);
		Main.tLoadingProgr2 = (Resources.Load("GUI/loading_progr2") as Texture2D);
		Main.tLoadingProgr3 = (Resources.Load("GUI/loading_progr3") as Texture2D);
		Main.tLoadingProgr4 = (Resources.Load("GUI/loading_progr4") as Texture2D);
		Main.tLoadingProgr5 = (Resources.Load("GUI/loading_progr5") as Texture2D);
		Main.tLoadingProgr6 = (Resources.Load("GUI/loading_progr6") as Texture2D);
		Main.tLoadingProgr7 = (Resources.Load("GUI/loading_progr7") as Texture2D);
		this.OnResize();
		Options.Load();
		Options.Apply();
		Options.ApplyResolutionOnce();
		if (GameData.gSteam)
		{
			Steam.Init();
		}
		if (GameData.gSteam)
		{
			this.LoadAvatar();
		}
		Main.HideAll();
		if (GameData.gSteam)
		{
			Main.MainAuthSteam();
		}
		if (GameData.gSteam)
		{
			WebHandler.get_servers();
		}
		if (GameData.gSteam && Steam.active && Steam.logged)
		{
			Main.MainInventory();
		}
		if (GameData.gSteam)
		{
			MenuFriends.Load();
		}
		if (PlayerPrefs.HasKey("reconnect"))
		{
			PlayerPrefs.DeleteKey("reconnect");
			Application.LoadLevel("game");
		}
	}

	public static void MainAuthSteam()
	{
		if (!Steam.active || !Steam.logged)
		{
			return;
		}
		if (!BaseData.Auth)
		{
			BaseData.banCost = string.Empty;
			BaseData.uid = Steam.steamid.ToString();
			if (BaseData.key == string.Empty && PlayerPrefs.HasKey(BaseData.uid + "_key"))
			{
				BaseData.key = PlayerPrefs.GetString(BaseData.uid + "_key");
			}
			if (BaseData.iWarid == 0 && PlayerPrefs.HasKey(BaseData.uid + "_wid"))
			{
				BaseData.iWarid = PlayerPrefs.GetInt(BaseData.uid + "_wid");
			}
			if (BaseData.key == string.Empty || BaseData.iWarid == 0)
			{
				string ticket = Steam.GetTicket();
				WebHandler.get_warkey("&ticket=" + ticket);
				Main.inbuySteam = true;
			}
			else
			{
				WebHandler.get_auth(string.Concat(new object[]
				{
					"&key=",
					BaseData.key,
					"&version=",
					Client.version,
					"&name=",
					BaseData.Name
				}));
			}
		}
		else
		{
			MenuPlayer.SetActive(true);
		}
		BaseData.warsession = "zerosession";
	}

	public static void MainInventory()
	{
		if (PlayerPrefs.HasKey(BaseData.uid + "_badge_back"))
		{
			BaseData.badge_back = PlayerPrefs.GetInt(BaseData.uid + "_badge_back");
		}
		if (PlayerPrefs.HasKey(BaseData.uid + "_badge_icon"))
		{
			BaseData.badge_icon = PlayerPrefs.GetInt(BaseData.uid + "_badge_icon");
		}
		if (PlayerPrefs.HasKey(BaseData.uid + "_mask_merc"))
		{
			BaseData.mask_merc = PlayerPrefs.GetInt(BaseData.uid + "_mask_merc");
		}
		if (PlayerPrefs.HasKey(BaseData.uid + "_mask_warcorp"))
		{
			BaseData.mask_warcorp = PlayerPrefs.GetInt(BaseData.uid + "_mask_warcorp");
		}
		for (int i = 0; i < 128; i++)
		{
			if (WeaponData.CheckWeapon(i))
			{
				string key = BaseData.uid + "_custom_" + WeaponData.GetData(i).wName.ToLower();
				if (PlayerPrefs.HasKey(key))
				{
					BaseData.profileWeapon[i] = PlayerPrefs.GetInt(key);
				}
				BaseData.currentWeapon[i] = BaseData.profileWeapon[i];
			}
		}
		if (PlayerPrefs.HasKey(BaseData.uid + "_invsig"))
		{
			BaseData.invsig = PlayerPrefs.GetString(BaseData.uid + "_invsig");
			for (int j = 0; j < 1024; j++)
			{
				if (PlayerPrefs.HasKey(BaseData.uid + "_item_" + j.ToString()))
				{
					BaseData.item[j] = 1;
				}
			}
			MenuPlayer.ChangePlayer(0, 8, 22);
		}
		else
		{
			WebHandler.get_inv();
		}
	}

	private void LoadAvatar()
	{
		if (!Steam.active || !Steam.logged)
		{
			Main.avatar = TEX.GetTextureByName("black");
			return;
		}
		byte[] array = new byte[16384];
		bool mediumAvatar = Steam.GetMediumAvatar(array, 16384);
		if (mediumAvatar)
		{
			Main.avatar = new Texture2D(64, 64, TextureFormat.RGBA32, false, true);
			Main.avatar.LoadRawTextureData(array);
			for (int i = 0; i < 64; i++)
			{
				for (int j = 0; j < 32; j++)
				{
					Color pixel = Main.avatar.GetPixel(i, j);
					Main.avatar.SetPixel(i, j, Main.avatar.GetPixel(i, 63 - j));
					Main.avatar.SetPixel(i, 63 - j, pixel);
				}
			}
			Main.avatar.Apply(false);
		}
	}

	public void OnResize()
	{
		this.rLoadingBackgr = new Rect((float)Screen.width / 2f - GUI2.YRES(324f) / 2f, (float)Screen.height / 2f - GUI2.YRES(106f) / 2f, GUI2.YRES(324f), GUI2.YRES(106f));
		this.rLoadingCooler = new Rect((float)Screen.width / 2f + GUI2.YRES(77f), (float)Screen.height / 2f - GUI2.YRES(20.5f), GUI2.YRES(40f), GUI2.YRES(40f));
		this.rLoadingProgr = new Rect((float)Screen.width / 2f - GUI2.YRES(118f), (float)Screen.height / 2f + GUI2.YRES(4f), GUI2.YRES(200f), GUI2.YRES(18f));
		this.rLoadingText = new Rect((float)Screen.width / 2f - GUI2.YRES(58f), (float)Screen.height / 2f - GUI2.YRES(22f), GUI2.YRES(100f), GUI2.YRES(20f));
	}

	private void OnGUI()
	{
		Rect r = new Rect((float)Screen.width / 2f - GUIM.YRES(160f), (float)Screen.height / 2f - GUIM.YRES(30f), GUIM.YRES(400f), GUIM.YRES(80f));
		if (GameData.gSteam)
		{
			if (!Steam.active)
			{
				GUIM.DrawBox(r, Main.tBlack);
				GUIM.DrawText(r, Lang.Get("_STEAM_NOT_LAUNCHED"), TextAnchor.MiddleCenter, BaseColor.White, 1, 12, true);
				return;
			}
			if (!Steam.logged)
			{
				GUIM.DrawBox(r, Main.tBlack);
				GUIM.DrawText(r, Lang.Get("_STEAM_NOT_LOGGEDON"), TextAnchor.MiddleCenter, BaseColor.White, 1, 12, true);
				return;
			}
		}
		if (!ContentLoader_.proceed)
		{
			UIManager.SetLoadingActive(true);
			return;
		}
		UIManager.SetLoadingActive(false);
		if (BaseData.banCost != string.Empty)
		{
			GUIM.DrawBox(r, Main.tBlack);
			Rect r2 = new Rect(r.x, r.y, r.width, r.height - GUIM.YRES(36f));
			string str = "$" + BaseData.banCost;
			if (GameData.gVK)
			{
				str = BaseData.banCost + " г.";
			}
			if (GameData.gFB)
			{
				int num = Convert.ToInt32(BaseData.banCost);
				str = string.Format("{0:C}", (float)num * 0.12f);
			}
			GUIM.DrawText(r2, Lang.Get("_YOU_BANNED!_UNBAN_COST") + " - " + str, TextAnchor.MiddleCenter, BaseColor.White, 1, 16, true);
			Rect r3 = new Rect(r.x + GUIM.YRES(84f), r.y + GUIM.YRES(44f), GUIM.YRES(100f), GUIM.YRES(22f));
			if (GameData.gSocial)
			{
				r3 = new Rect(r.x + GUIM.YRES(150f), r.y + GUIM.YRES(44f), GUIM.YRES(100f), GUIM.YRES(22f));
			}
			if (Main.inbuySteam || Main.inbuySocial)
			{
				GUIM.Button(r3, BaseColor.Gray, Lang.Get("_UNBAN"), TextAnchor.MiddleCenter, BaseColor.White, 1, 12, true);
			}
			else
			{
				if (GameData.gSteam && GUIM.Button(r3, BaseColor.Green, Lang.Get("_UNBAN"), TextAnchor.MiddleCenter, BaseColor.White, 1, 12, true))
				{
					WebHandler.get_buy("&itemid=10000");
					Main.inbuySteam = true;
				}
				if (GameData.gVK && GUIM.Button(r3, BaseColor.Green, Lang.Get("_UNBAN"), TextAnchor.MiddleCenter, BaseColor.White, 1, 12, true))
				{
					Main.inbuySocial = true;
					Application.ExternalCall("order", new object[]
					{
						"item10000"
					});
				}
				if (GameData.gFB && GUIM.Button(r3, BaseColor.Green, Lang.Get("_UNBAN"), TextAnchor.MiddleCenter, BaseColor.White, 1, 12, true))
				{
					Main.inbuySocial = true;
					FBManager.BuyUnbun(Convert.ToInt32(BaseData.banCost));
				}
			}
			if (GameData.gSteam)
			{
				Rect r4 = new Rect(r.x + GUIM.YRES(216f), r.y + GUIM.YRES(44f), GUIM.YRES(100f), GUIM.YRES(22f));
				if (GUIM.Button(r4, BaseColor.Red, Lang.Get("_EXIT"), TextAnchor.MiddleCenter, BaseColor.White, 1, 12, true))
				{
					Application.Quit();
				}
			}
			return;
		}
		if (!Client.actualVersion)
		{
			GUIM.DrawBox(r, Main.tBlack);
			Rect r5 = new Rect(r.x, r.y, r.width, r.height - GUIM.YRES(36f));
			if (GameData.gSteam)
			{
				GUIM.DrawText(r5, Lang.Get("_UPDATE_VERSION"), TextAnchor.MiddleCenter, BaseColor.White, 1, 16, true);
				Rect r6 = new Rect(r.x + GUIM.YRES(150f), r.y + GUIM.YRES(44f), GUIM.YRES(100f), GUIM.YRES(22f));
				if (GUIM.Button(r6, BaseColor.Red, Lang.Get("_EXIT"), TextAnchor.MiddleCenter, BaseColor.White, 1, 12, true))
				{
					Application.Quit();
				}
			}
			if (GameData.gSocial)
			{
				GUIM.DrawText(r, Lang.Get("_UPDATE_VERSION"), TextAnchor.MiddleCenter, BaseColor.White, 1, 16, true);
			}
			return;
		}
		if (!BaseData.Auth)
		{
			GUIM.DrawBox(r, Main.tBlack);
			GUIM.DrawText(r, Lang.Get("_AUTHORIZATION"), TextAnchor.MiddleCenter, BaseColor.White, 1, 12, true);
			return;
		}
		if (Main.tVig)
		{
			GUI.DrawTexture(new Rect(0f, 0f, (float)Screen.width, (float)Screen.height), Main.tVig);
		}
		TopBar.Draw();
		BottomBar.Draw();
		Profile.Draw();
		MenuPlayer.Draw();
		MenuGold.Draw();
		MenuServers.Draw();
		MenuShop.Draw();
		MenuOptions.Draw();
		MenuInventory.Draw();
		MenuPreview.Draw();
		if (GameData.gSteam)
		{
			MenuFriends.Draw();
		}
	}

	private void DrawLoadingProgress(float progress)
	{
		GUI.DrawTexture(this.rLoadingBackgr, Main.tLoadingBackgr);
		GUIM.DrawText(this.rLoadingText, string.Concat(new object[]
		{
			Lang.Get("_LOADING"),
			" ",
			(int)(ContentLoader2_.progress * 100f),
			"%"
		}), TextAnchor.MiddleLeft, BaseColor.White, 0, 22, true);
		if (progress < 15f)
		{
			GUI.DrawTexture(this.rLoadingProgr, Main.tLoadingProgr1);
		}
		else if (progress >= 15f && progress < 31f)
		{
			GUI.DrawTexture(this.rLoadingProgr, Main.tLoadingProgr2);
		}
		else if (progress >= 31f && progress < 47f)
		{
			GUI.DrawTexture(this.rLoadingProgr, Main.tLoadingProgr3);
		}
		else if (progress >= 47f && progress < 63f)
		{
			GUI.DrawTexture(this.rLoadingProgr, Main.tLoadingProgr4);
		}
		else if (progress >= 63f && progress < 79f)
		{
			GUI.DrawTexture(this.rLoadingProgr, Main.tLoadingProgr5);
		}
		else if (progress >= 79f && progress < 95f)
		{
			GUI.DrawTexture(this.rLoadingProgr, Main.tLoadingProgr6);
		}
		else if (progress >= 95f)
		{
			GUI.DrawTexture(this.rLoadingProgr, Main.tLoadingProgr7);
		}
		Matrix4x4 matrix = GUI.matrix;
		GUIUtility.RotateAroundPivot(this.coolerAngle, new Vector2((float)Screen.width / 2f + GUI2.YRES(97f), (float)Screen.height / 2f + GUI2.YRES(0f)));
		GUI.DrawTexture(this.rLoadingCooler, Main.tLoadingCooler);
		GUI.matrix = matrix;
	}

	public static void HideAll()
	{
		MenuPlayer.SetActive(false);
		MenuGold.SetActive(false);
		MenuServers.SetActive(false);
		MenuShop.SetActive(false);
		MenuOptions.SetActive(false);
		MenuInventory.SetActive(false);
		MenuPreview.SetActive(false);
	}

	private void LoadEnd()
	{
		GameObject gameObject = GameObject.Find("Camera");
		if (gameObject && gameObject.GetComponent<AudioSource>())
		{
			gameObject.GetComponent<AudioSource>().clip = ContentLoader_.LoadAudio("maintheme");
			gameObject.GetComponent<AudioSource>().Play();
			gameObject.GetComponent<AudioSource>().volume = Options.menuvol;
		}
		Main.tVig = TEX.GetTextureByName("vig");
		MonoBehaviour.print("MAIN.LOADEND");
	}

	private void FixedUpdate()
	{
		this.coolerAngle += 1f;
		if (this.coolerAngle >= 360f)
		{
			this.coolerAngle = 0f;
		}
	}

	private void Update()
	{
		if (Main.lastwidth != (float)Screen.width)
		{
			Main.lastwidth = (float)Screen.width;
			base.gameObject.BroadcastMessage("OnResize", SendMessageOptions.DontRequireReceiver);
		}
		if (UIManager.canvasActive)
		{
			UIManager.SetLoadingProgress((int)(ContentLoader2_.progress * 100f));
			UIManager.SetCoolerAngle(this.coolerAngle);
		}
		if (!Main.inbuySteam)
		{
			return;
		}
		Steam.RunCallBacks();
		ulong num = Steam.s_get_tnx_orderid();
		if (num != 0uL)
		{
			Main.inbuySteam = false;
			WebHandler.get_buyfin("&orderid=" + num.ToString());
		}
	}
}
