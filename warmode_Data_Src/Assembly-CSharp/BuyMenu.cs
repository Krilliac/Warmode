using System;
using UnityEngine;

public class BuyMenu : MonoBehaviour
{
	private const int maxmenu = 7;

	public static bool show = false;

	private static Vector2 mpos = Vector2.zero;

	private static int team;

	private Texture2D tBlack;

	private Texture2D tWhite;

	private Texture2D tGray0;

	private Texture2D tGray2;

	private Texture2D tRed;

	private Texture2D tMoney;

	private Texture2D tYellow;

	private Texture2D tLock;

	private Texture2D tGradient;

	private Texture2D tKeyBuy;

	private Texture2D tLoad;

	private Rect rBuyMsg;

	private Rect rBuyBack;

	private Rect rBuyBack2;

	private Rect rBuyArmory;

	private Rect rMainBack;

	private Rect[] rMainMenu = new Rect[7];

	private Rect[] rActiveSlots = new Rect[7];

	private Rect[] rActiveSlotsNull = new Rect[2];

	private Rect[] rArmorySlots = new Rect[15];

	private Rect[] rArmorySlotsNull = new Rect[2];

	private string[] sCategoryName = new string[8];

	private static int catid = -1;

	private static int menuLevel = 0;

	private static int[] buySlotWeapon = new int[10];

	private Color[] teamcolor = new Color[2];

	public static float showtime = 0f;

	public static bool canbuy = false;

	public static bool inbuyzone = false;

	private static float[] spawnpos = new float[3];

	private Transform trPlayer;

	private float t0;

	public void PostAwake()
	{
		this.trPlayer = GameObject.Find("LocalPlayer").transform;
		this.tBlack = TEX.GetTextureByName("black");
		this.tWhite = TEX.GetTextureByName("white");
		this.tGray0 = TEX.GetTextureByName("gray0");
		this.tGray2 = TEX.GetTextureByName("gray2");
		this.tRed = TEX.GetTextureByName("red");
		this.tMoney = TEX.GetTextureByName("wartab_money");
		this.tYellow = TEX.GetTextureByName("yellow");
		this.tLock = TEX.GetTextureByName("lock");
		this.tGradient = TEX.GetTextureByName("GUI/left_gradient");
		this.tKeyBuy = TEX.GetTextureByName("GUI/keybuy");
		this.tLoad = TEX.GetTextureByName("wmload");
		this.sCategoryName[1] = Lang.Get("_PISTOLS");
		this.sCategoryName[2] = Lang.Get("_SHOTGUNS");
		this.sCategoryName[3] = Lang.Get("_SUBMACHUNE_GUNS");
		this.sCategoryName[4] = Lang.Get("_ASSAULT_RIFLES");
		this.sCategoryName[5] = Lang.Get("_SNIPER_RIFLES");
		this.sCategoryName[6] = Lang.Get("_HEAVY_RIFLES");
		this.sCategoryName[7] = Lang.Get("_AMMUNITION");
		this.teamcolor[0] = new Color(1f, 0.2f, 0f, 1f);
		this.teamcolor[1] = new Color(0f, 0.5f, 1f, 1f);
		this.OnResize();
		BuyMenu.show = false;
	}

	public void OnResize()
	{
		this.rBuyMsg = new Rect(((float)Screen.width - GUI2.YRES(200f)) / 2f, GUI2.YRES(400f), GUI2.YRES(200f), GUI2.YRES(20f));
		int num = (int)GUI2.YRES(2f);
		int num2 = (int)GUI2.YRES(19f);
		int num3 = (int)GUI2.YRES(110f);
		int num4 = (int)GUI2.YRES(80f);
		int num5 = (int)GUI2.YRES(22f);
		int num6 = (int)GUI2.YRES(2f);
		int num7 = (int)GUI2.YRES(60f);
		int num8 = (int)GUI2.YRES(230f);
		if (Options.gamebuy == 1)
		{
			num7 = (int)GUI2.YRES(40f);
			num8 = (int)GUI2.YRES(140f);
		}
		this.rMainBack = new Rect(((float)Screen.width - GUI2.YRES(300f)) / 2f, GUI2.YRES(158f), GUI2.YRES(300f), (float)num5);
		this.rMainMenu[0] = new Rect((float)num7, (float)(num8 + (num5 + num6) * 0), this.rMainBack.width, this.rMainBack.height);
		this.rMainMenu[1] = new Rect((float)num7, (float)(num8 + (num5 + num6) * 1), this.rMainBack.width, this.rMainBack.height);
		this.rMainMenu[2] = new Rect((float)num7, (float)(num8 + (num5 + num6) * 2), this.rMainBack.width, this.rMainBack.height);
		this.rMainMenu[3] = new Rect((float)num7, (float)(num8 + (num5 + num6) * 3), this.rMainBack.width, this.rMainBack.height);
		this.rMainMenu[4] = new Rect((float)num7, (float)(num8 + (num5 + num6) * 4), this.rMainBack.width, this.rMainBack.height);
		this.rMainMenu[5] = new Rect((float)num7, (float)(num8 + (num5 + num6) * 5), this.rMainBack.width, this.rMainBack.height);
		this.rMainMenu[6] = new Rect((float)num7, (float)(num8 + (num5 + num6) * 6), this.rMainBack.width, this.rMainBack.height);
		this.rBuyBack = new Rect(((float)Screen.width - GUI2.YRES(600f)) / 2f, GUI2.YRES(80f), (float)(num3 * 5 + num2 * 2 + num * 6), (float)num5);
		this.rBuyBack2 = new Rect(this.rBuyBack.x, this.rBuyBack.height + this.rBuyBack.y + (float)(num4 * 2) + (float)(num * 3), (float)(num3 * 5 + num2 * 2 + num * 6), (float)num5);
		this.rActiveSlotsNull[0] = new Rect(this.rBuyBack.x, this.rBuyBack.y + (float)(num5 + num), (float)num2, (float)num4);
		this.rActiveSlotsNull[1] = new Rect(this.rBuyBack.x + (float)(num3 * 5) + (float)num2 + (float)(num * 6), this.rBuyBack.y + (float)(num5 + num), (float)num2, (float)num4);
		this.rActiveSlots[0] = new Rect(this.rBuyBack.x + (float)num2 + (float)num + (float)((num + num3) * 0), GUI2.YRES(104f), (float)num3, (float)num4);
		this.rActiveSlots[1] = new Rect(this.rBuyBack.x + (float)num2 + (float)num + (float)((num + num3) * 1), GUI2.YRES(104f), (float)num3, (float)num4);
		this.rActiveSlots[2] = new Rect(this.rBuyBack.x + (float)num2 + (float)num + (float)((num + num3) * 2), GUI2.YRES(104f), (float)num3, (float)num4);
		this.rActiveSlots[3] = new Rect(this.rBuyBack.x + (float)num2 + (float)num + (float)((num + num3) * 3), GUI2.YRES(104f), (float)num3, (float)num4);
		this.rActiveSlots[4] = new Rect(this.rBuyBack.x + (float)num2 + (float)num + (float)((num + num3) * 4), GUI2.YRES(104f), (float)num3, (float)num4);
		this.rActiveSlots[5] = new Rect(this.rBuyBack.x + (float)num2 + (float)num + (float)((num + num3) * 5), GUI2.YRES(104f), (float)num3, (float)num4);
		this.rBuyArmory = new Rect(this.rBuyBack.x, this.rBuyBack.y, this.rBuyBack.width, this.rBuyBack.height);
		this.rArmorySlots[0] = new Rect(this.rActiveSlots[0].x, this.rBuyArmory.y + (float)num5 + (float)num, this.rActiveSlots[0].width, this.rActiveSlots[0].height);
		this.rArmorySlots[1] = new Rect(this.rActiveSlots[1].x, this.rBuyArmory.y + (float)num5 + (float)num, this.rActiveSlots[0].width, this.rActiveSlots[0].height);
		this.rArmorySlots[2] = new Rect(this.rActiveSlots[2].x, this.rBuyArmory.y + (float)num5 + (float)num, this.rActiveSlots[0].width, this.rActiveSlots[0].height);
		this.rArmorySlots[3] = new Rect(this.rActiveSlots[3].x, this.rBuyArmory.y + (float)num5 + (float)num, this.rActiveSlots[0].width, this.rActiveSlots[0].height);
		this.rArmorySlots[4] = new Rect(this.rActiveSlots[4].x, this.rBuyArmory.y + (float)num5 + (float)num, this.rActiveSlots[0].width, this.rActiveSlots[0].height);
		this.rArmorySlots[5] = new Rect(this.rActiveSlots[0].x, this.rBuyArmory.y + (float)num5 + (float)num + (float)(num + num4), this.rActiveSlots[0].width, this.rActiveSlots[0].height);
		this.rArmorySlots[6] = new Rect(this.rActiveSlots[1].x, this.rBuyArmory.y + (float)num5 + (float)num + (float)(num + num4), this.rActiveSlots[0].width, this.rActiveSlots[0].height);
		this.rArmorySlots[7] = new Rect(this.rActiveSlots[2].x, this.rBuyArmory.y + (float)num5 + (float)num + (float)(num + num4), this.rActiveSlots[0].width, this.rActiveSlots[0].height);
		this.rArmorySlots[8] = new Rect(this.rActiveSlots[3].x, this.rBuyArmory.y + (float)num5 + (float)num + (float)(num + num4), this.rActiveSlots[0].width, this.rActiveSlots[0].height);
		this.rArmorySlots[9] = new Rect(this.rActiveSlots[4].x, this.rBuyArmory.y + (float)num5 + (float)num + (float)(num + num4), this.rActiveSlots[0].width, this.rActiveSlots[0].height);
		this.rArmorySlots[10] = new Rect(this.rActiveSlots[0].x, this.rBuyArmory.y + (float)num5 + (float)num + (float)((num + num4) * 2), this.rActiveSlots[0].width, this.rActiveSlots[0].height);
		this.rArmorySlots[11] = new Rect(this.rActiveSlots[1].x, this.rBuyArmory.y + (float)num5 + (float)num + (float)((num + num4) * 2), this.rActiveSlots[0].width, this.rActiveSlots[0].height);
		this.rArmorySlots[12] = new Rect(this.rActiveSlots[2].x, this.rBuyArmory.y + (float)num5 + (float)num + (float)((num + num4) * 2), this.rActiveSlots[0].width, this.rActiveSlots[0].height);
		this.rArmorySlots[13] = new Rect(this.rActiveSlots[3].x, this.rBuyArmory.y + (float)num5 + (float)num + (float)((num + num4) * 2), this.rActiveSlots[0].width, this.rActiveSlots[0].height);
		this.rArmorySlots[14] = new Rect(this.rActiveSlots[4].x, this.rBuyArmory.y + (float)num5 + (float)num + (float)((num + num4) * 2), this.rActiveSlots[0].width, this.rActiveSlots[0].height);
		this.rArmorySlotsNull[0] = new Rect(this.rBuyBack.x, this.rArmorySlots[0].y, (float)num2, (float)(num4 * 2 + num * 1));
		this.rArmorySlotsNull[1] = new Rect(this.rBuyBack.x + this.rBuyBack.width - (float)num2, this.rArmorySlots[0].y, (float)num2, (float)(num4 * 2 + num * 1));
	}

	private void OnGUI()
	{
		if (!BuyMenu.inbuyzone)
		{
			return;
		}
		if (BasePlayer.deadflag == 1)
		{
			return;
		}
		if (BuyMenu.canbuy)
		{
			if (BuyMenu.showtime > Time.time || GameData.restartroundmode == 1)
			{
				this.DrawBuyMessage();
			}
			else
			{
				BuyMenu.canbuy = false;
				BuyMenu.SetActive(false);
			}
		}
		if (!BuyMenu.show)
		{
			return;
		}
		BuyMenu.team = BasePlayer.team;
		if (Options.gamebuy == 1)
		{
			if (BuyMenu.catid < 0)
			{
				this.DrawMenuLight();
			}
			else
			{
				this.DrawBuyLight();
			}
		}
		else if (BuyMenu.catid < 0)
		{
			this.DrawMenu();
		}
		else
		{
			this.DrawBuy();
		}
	}

	private void DrawBuyMessage()
	{
		GUI.DrawTexture(this.rBuyMsg, this.tGradient);
		Rect position = new Rect(this.rBuyMsg.x + GUI2.YRES(8f), this.rBuyMsg.y, GUI2.YRES(20f), GUI2.YRES(20f));
		int num = (int)GUI2.YRES(1f);
		GUI.DrawTexture(position, this.tKeyBuy);
		GUI2.DrawTextRes(this.rBuyMsg, Lang.Get("_WEAPON_BUY"), TextAnchor.MiddleCenter, _Color.White, 0, 12, true);
		if (GameData.restartroundmode != 1)
		{
			GUI2.DrawTextRes(new Rect(this.rBuyMsg.x + GUI2.YRES(170f), this.rBuyMsg.y, GUI2.YRES(20f), GUI2.YRES(20f)), (BuyMenu.showtime - Time.time).ToString("0") + " ", TextAnchor.MiddleCenter, _Color.White, 0, 12, true);
		}
	}

	private void Update()
	{
		if (!BuyMenu.canbuy || vp_FPInput.grenadeThrowStarting)
		{
			return;
		}
		if (BuyMenu.inbuyzone && BuyMenu.show)
		{
			if (BuyMenu.menuLevel == 0)
			{
				if (Input.GetKeyUp(KeyCode.Alpha1))
				{
					BuyMenu.catid = 1;
				}
				if (Input.GetKeyUp(KeyCode.Alpha2))
				{
					BuyMenu.catid = 2;
				}
				if (Input.GetKeyUp(KeyCode.Alpha3))
				{
					BuyMenu.catid = 3;
				}
				if (Input.GetKeyUp(KeyCode.Alpha4))
				{
					BuyMenu.catid = 4;
				}
				if (Input.GetKeyUp(KeyCode.Alpha5))
				{
					BuyMenu.catid = 5;
				}
				if (Input.GetKeyUp(KeyCode.Alpha6))
				{
					BuyMenu.catid = 6;
				}
				if (Input.GetKeyUp(KeyCode.Alpha7))
				{
					BuyMenu.catid = 7;
				}
			}
			if (BuyMenu.menuLevel == 1)
			{
				if (Input.GetKeyUp(KeyCode.Alpha0))
				{
					BuyMenu.catid = -1;
				}
				if (Input.GetKeyUp(KeyCode.Alpha1))
				{
					this.SendBuy(0);
				}
				if (Input.GetKeyUp(KeyCode.Alpha2))
				{
					this.SendBuy(1);
				}
				if (Input.GetKeyUp(KeyCode.Alpha3))
				{
					this.SendBuy(2);
				}
				if (Input.GetKeyUp(KeyCode.Alpha4))
				{
					this.SendBuy(3);
				}
				if (Input.GetKeyUp(KeyCode.Alpha5))
				{
					this.SendBuy(4);
				}
				if (Input.GetKeyUp(KeyCode.Alpha6))
				{
					this.SendBuy(5);
				}
				if (Input.GetKeyUp(KeyCode.Alpha7))
				{
					this.SendBuy(6);
				}
				if (Input.GetKeyUp(KeyCode.Alpha8))
				{
					this.SendBuy(7);
				}
				if (Input.GetKeyUp(KeyCode.Alpha9))
				{
					this.SendBuy(8);
				}
			}
		}
		if (this.t0 > Time.time)
		{
			return;
		}
		this.t0 = Time.time + 0.5f;
		if (ScoreBoard.gamemode == 3 && (GameData.restartroundmode == 1 || (GameData.restartroundmode == 0 && !GameData.infected)))
		{
			BuyMenu.inbuyzone = true;
		}
		else if (Mathf.Abs(this.trPlayer.position.x - BuyMenu.spawnpos[0]) < 12f && Mathf.Abs(this.trPlayer.position.y - BuyMenu.spawnpos[1]) < 4f && Mathf.Abs(this.trPlayer.position.z - BuyMenu.spawnpos[2]) < 12f)
		{
			BuyMenu.inbuyzone = true;
		}
		else
		{
			BuyMenu.inbuyzone = false;
			BuyMenu.SetActive(false);
		}
	}

	private void SendBuy(int slot)
	{
		if (BuyMenu.buySlotWeapon[slot] == 0)
		{
			return;
		}
		byte b = vp_FPWeapon.WeaponPlayerCollision(WeaponData.GetData(BuyMenu.buySlotWeapon[slot]).slot);
		Client.cs.send_buy(BuyMenu.buySlotWeapon[slot]);
		BuyMenu.Toggle();
	}

	public static void ShowBuy(float x, float y, float z)
	{
		if (ScoreBoard.gamemode == 3)
		{
			BuyMenu.showtime = Time.time + 20f - (float)(GameData.roundTimeLimit - ScoreTop.TimeLeft);
		}
		else
		{
			BuyMenu.showtime = Time.time + 30f;
		}
		BuyMenu.canbuy = true;
		BuyMenu.spawnpos[0] = x;
		BuyMenu.spawnpos[1] = y;
		BuyMenu.spawnpos[2] = z;
	}

	private void DrawMenu()
	{
		BuyMenu.menuLevel = 0;
		Rect position = new Rect(0f, 0f, (float)Screen.width, (float)Screen.height);
		GUI.DrawTexture(position, this.tLoad);
		GUI.DrawTexture(new Rect(0f, GUI2.YRES(80f), GUI2.YRES(200f), GUI2.YRES(20f)), this.tGradient);
		GUI2.DrawTextRes(new Rect(GUI2.YRES(60f), GUI2.YRES(80f), GUI2.YRES(200f), GUI2.YRES(26f)), Lang.Get("_ACTIVE_WEAPONS"), TextAnchor.MiddleLeft, _Color.White, 1, 14, true);
		BuyMenu.buySlotWeapon[0] = 4;
		BuyMenu.buySlotWeapon[1] = 17;
		BuyMenu.buySlotWeapon[2] = 27;
		BuyMenu.buySlotWeapon[3] = 120;
		BuyMenu.buySlotWeapon[4] = 121;
		this.DrawItem(this.rActiveSlots[0], BuyMenu.buySlotWeapon[0], 0);
		this.DrawItem(this.rActiveSlots[1], BuyMenu.buySlotWeapon[1], 1);
		this.DrawItem(this.rActiveSlots[2], BuyMenu.buySlotWeapon[2], 2);
		this.DrawItem(this.rActiveSlots[3], BuyMenu.buySlotWeapon[3], 3);
		this.DrawItem(this.rActiveSlots[4], BuyMenu.buySlotWeapon[4], 4);
		GUI.DrawTexture(new Rect(0f, GUI2.YRES(64f) + GUI2.YRES(138f), GUI2.YRES(200f), GUI2.YRES(20f)), this.tGradient);
		GUI2.DrawTextRes(new Rect(GUI2.YRES(60f), GUI2.YRES(64f) + GUI2.YRES(138f), GUI2.YRES(200f), GUI2.YRES(26f)), Lang.Get("_WEAPON_BUY"), TextAnchor.MiddleLeft, _Color.White, 1, 14, true);
		this.DrawMenuSlot(1, this.rMainMenu[0], this.sCategoryName[1]);
		this.DrawMenuSlot(2, this.rMainMenu[1], this.sCategoryName[2]);
		this.DrawMenuSlot(3, this.rMainMenu[2], this.sCategoryName[3]);
		this.DrawMenuSlot(4, this.rMainMenu[3], this.sCategoryName[4]);
		this.DrawMenuSlot(5, this.rMainMenu[4], this.sCategoryName[5]);
		this.DrawMenuSlot(6, this.rMainMenu[5], this.sCategoryName[6]);
		this.DrawMenuSlot(7, this.rMainMenu[6], this.sCategoryName[7]);
	}

	private void DrawMenuLight()
	{
		BuyMenu.menuLevel = 0;
		GUI2.DrawTextRes(new Rect(this.rMainMenu[0].x, this.rMainMenu[0].y - this.rMainMenu[0].height * 1.4f, GUI2.YRES(200f), GUI2.YRES(26f)), Lang.Get("_WEAPON_BUY"), TextAnchor.MiddleLeft, _Color.White, 1, 12, true);
		GUI2.DrawTextRes(this.rMainMenu[0], "1. " + this.sCategoryName[0], TextAnchor.MiddleLeft, _Color.White, 1, 12, true);
		GUI2.DrawTextRes(this.rMainMenu[1], "2. " + this.sCategoryName[1], TextAnchor.MiddleLeft, _Color.White, 1, 12, true);
		GUI2.DrawTextRes(this.rMainMenu[2], "3. " + this.sCategoryName[2], TextAnchor.MiddleLeft, _Color.White, 1, 12, true);
		GUI2.DrawTextRes(this.rMainMenu[3], "4. " + this.sCategoryName[3], TextAnchor.MiddleLeft, _Color.White, 1, 12, true);
		GUI2.DrawTextRes(this.rMainMenu[4], "5. " + this.sCategoryName[4], TextAnchor.MiddleLeft, _Color.White, 1, 12, true);
		GUI2.DrawTextRes(this.rMainMenu[5], "6. " + this.sCategoryName[5], TextAnchor.MiddleLeft, _Color.White, 1, 12, true);
		GUI2.DrawTextRes(this.rMainMenu[6], "7. " + this.sCategoryName[6], TextAnchor.MiddleLeft, _Color.White, 1, 12, true);
		GUI2.DrawTextRes(new Rect(this.rMainMenu[0].x, this.rMainMenu[6].y + this.rMainMenu[6].height * 1.2f, GUI2.YRES(200f), GUI2.YRES(26f)), "ESC. " + Lang.Get("_CLOSE"), TextAnchor.MiddleLeft, _Color.White, 1, 12, true);
	}

	private void DrawMenuSlot(int id, Rect r, string text)
	{
		BuyMenu.mpos = new Vector2(Input.mousePosition.x, (float)Screen.height - Input.mousePosition.y);
		GUI2.DrawTextRes(new Rect(r.x + GUI2.YRES(8f), r.y + GUI2.YRES(3f), GUI2.YRES(20f), GUI2.YRES(20f)), id.ToString(), TextAnchor.MiddleCenter, _Color.Yellow, 1, 14, true);
		if (r.Contains(BuyMenu.mpos))
		{
			GUI2.DrawTextRes(new Rect(r.x + GUI2.YRES(36f), r.y, r.width, r.height), text, TextAnchor.MiddleLeft, _Color.Red, 1, 14, true);
		}
		else
		{
			GUI2.DrawTextRes(new Rect(r.x + GUI2.YRES(36f), r.y, r.width, r.height), text, TextAnchor.MiddleLeft, _Color.White, 1, 14, true);
		}
		if (GUI2.HideButton(r))
		{
			BuyMenu.catid = id;
		}
	}

	private void DrawBuy()
	{
		BuyMenu.menuLevel = 1;
		GUI.DrawTexture(this.rBuyBack, this.tGray0);
		GUI2.DrawTextRes(this.rBuyBack, this.sCategoryName[BuyMenu.catid], TextAnchor.MiddleCenter, _Color.White, 0, 14, true);
		GUI.DrawTexture(this.rArmorySlotsNull[0], this.tGray2);
		GUI.DrawTexture(this.rArmorySlotsNull[1], this.tGray2);
		bool flag = false;
		int num = 0;
		for (int i = 1; i < 128; i++)
		{
			if (WeaponData.CheckWeapon(i))
			{
				if (WeaponData.GetData(i).buyMenuSlot == BuyMenu.catid)
				{
					if (WeaponData.GetData(i).wid == 49)
					{
						flag = true;
					}
					else
					{
						BuyMenu.buySlotWeapon[num] = i;
						this.DrawItem(this.rArmorySlots[num], i, num);
						num++;
					}
				}
			}
		}
		if (flag && ScoreTop.GameMode == 2 && BasePlayer.team == 1)
		{
			BuyMenu.buySlotWeapon[num] = 49;
			this.DrawItem(this.rArmorySlots[num], 49, num);
			num++;
		}
		for (int j = num; j < 10; j++)
		{
			BuyMenu.buySlotWeapon[j] = 0;
			this.DrawItem(this.rArmorySlots[j], 0, 0);
		}
		GUI.DrawTexture(this.rBuyBack2, this.tGray0);
		BuyMenu.mpos = new Vector2(Input.mousePosition.x, (float)Screen.height - Input.mousePosition.y);
		GUI2.DrawTextRes(new Rect(this.rBuyBack2.x + GUI2.YRES(6f), this.rBuyBack2.y, this.rBuyBack2.width, this.rBuyBack2.height), "0", TextAnchor.MiddleLeft, _Color.Yellow, 0, 12, true);
		if (this.rBuyBack2.Contains(BuyMenu.mpos))
		{
			GUI2.DrawTextRes(new Rect(this.rBuyBack2.x + GUI2.YRES(12f), this.rBuyBack2.y, this.rBuyBack2.width, this.rBuyBack2.height), " - " + Lang.Get("_BACK"), TextAnchor.MiddleLeft, _Color.Red, 0, 10, true);
		}
		else
		{
			GUI2.DrawTextRes(new Rect(this.rBuyBack2.x + GUI2.YRES(12f), this.rBuyBack2.y, this.rBuyBack2.width, this.rBuyBack2.height), " - " + Lang.Get("_BACK"), TextAnchor.MiddleLeft, _Color.White, 0, 10, true);
		}
		if (GUI2.HideButton(this.rBuyBack2))
		{
			BuyMenu.catid = -1;
		}
	}

	private void DrawBuyLight()
	{
		BuyMenu.menuLevel = 1;
		int num = (int)GUI2.YRES(22f);
		int num2 = (int)GUI2.YRES(2f);
		float y = this.rMainMenu[0].y;
		GUI2.DrawTextRes(new Rect(this.rMainMenu[0].x, this.rMainMenu[0].y - this.rMainMenu[0].height * 1.4f, GUI2.YRES(200f), GUI2.YRES(26f)), this.sCategoryName[BuyMenu.catid], TextAnchor.MiddleLeft, _Color.White, 1, 12, true);
		bool flag = false;
		int num3 = 0;
		for (int i = 1; i < 128; i++)
		{
			if (WeaponData.CheckWeapon(i))
			{
				if (WeaponData.GetData(i).buyMenuSlot == BuyMenu.catid)
				{
					if (WeaponData.GetData(i).wid == 49)
					{
						flag = true;
					}
					else
					{
						GUI2.DrawTextRes(new Rect(this.rMainMenu[0].x, y + (float)((num + num2) * num3), GUI2.YRES(140f), (float)num), (num3 + 1).ToString() + ". " + WeaponData.GetData(i).wName, TextAnchor.MiddleLeft, _Color.White, 1, 12, true);
						GUI2.DrawTextRes(new Rect(this.rMainMenu[0].x, y + (float)((num + num2) * num3), GUI2.YRES(140f), (float)num), WeaponData.GetData(i).cost.ToString(), TextAnchor.MiddleRight, _Color.White, 1, 12, true);
						BuyMenu.buySlotWeapon[num3] = i;
						num3++;
					}
				}
			}
		}
		if (flag && ScoreTop.GameMode == 2 && BasePlayer.team == 1)
		{
			BuyMenu.buySlotWeapon[num3] = 49;
			GUI2.DrawTextRes(new Rect(this.rMainMenu[0].x, y + (float)((num + num2) * num3), GUI2.YRES(140f), (float)num), (num3 + 1).ToString() + ". " + WeaponData.GetData(49).wName, TextAnchor.MiddleLeft, _Color.White, 1, 12, true);
			GUI2.DrawTextRes(new Rect(this.rMainMenu[0].x, y + (float)((num + num2) * num3), GUI2.YRES(140f), (float)num), WeaponData.GetData(49).cost.ToString(), TextAnchor.MiddleRight, _Color.White, 1, 12, true);
			num3++;
		}
		GUI2.DrawTextRes(new Rect(this.rMainMenu[0].x, y + (float)((num + num2) * num3) + this.rMainMenu[0].height * 0.2f, GUI2.YRES(200f), GUI2.YRES(22f)), "0. " + Lang.Get("_BACK"), TextAnchor.MiddleLeft, _Color.White, 1, 12, true);
		for (int j = num3; j < 10; j++)
		{
			BuyMenu.buySlotWeapon[j] = 0;
		}
	}

	private void DrawItem(Rect r, int wid = 0, int slot = 0)
	{
		BuyMenu.mpos = new Vector2(Input.mousePosition.x, (float)Screen.height - Input.mousePosition.y);
		if (r.Contains(BuyMenu.mpos))
		{
			GUI.color = this.teamcolor[BuyMenu.team];
		}
		else
		{
			GUI.color = new Color(0.35f, 0.35f, 0.35f, 0.75f);
		}
		if (wid == 0)
		{
			GUI.color = new Color(0.15f, 0.15f, 0.15f, 0.75f);
		}
		GUI.DrawTexture(r, this.tWhite);
		GUI.color = Color.white;
		if (wid > 0)
		{
			Rect rect = new Rect(r.x + GUI2.YRES(2f), r.y + GUI2.YRES(2f), GUI2.YRES(14f), GUI2.YRES(14f));
			GUI.DrawTexture(rect, this.tBlack);
			if (rect.Contains(BuyMenu.mpos))
			{
				GUI2.DrawTextRes(rect, ">>", TextAnchor.MiddleCenter, _Color.Yellow, 0, 12, false);
			}
			else if (BuyMenu.menuLevel == 0)
			{
				GUI2.DrawTextRes(rect, ">", TextAnchor.MiddleCenter, _Color.Yellow, 0, 12, false);
			}
			else if (BuyMenu.menuLevel == 1)
			{
				GUI2.DrawTextRes(rect, (slot + 1).ToString(), TextAnchor.MiddleCenter, _Color.Yellow, 0, 11, false);
			}
			GUI.color = new Color(1f, 1f, 1f, 0.25f);
			GUI.DrawTexture(new Rect(r.x + GUI2.YRES(17f), r.y + GUI2.YRES(2f), r.width - GUI2.YRES(19f), GUI2.YRES(14f)), this.tBlack);
			GUI.color = Color.white;
			GUI2.DrawTextRes(new Rect(r.x + GUI2.YRES(20f), r.y + GUI2.YRES(2f), r.width - GUI2.YRES(22f), GUI2.YRES(14f)), WeaponData.GetData(wid).wName, TextAnchor.MiddleLeft, _Color.White, 0, 12, true);
			int num = wid;
			if (WeaponData.CheckCustomSkin(wid) && BaseData.profileWeapon[wid] > 0)
			{
				num = BaseData.profileWeapon[wid];
			}
			Texture2D icon;
			if (num >= 128)
			{
				icon = MenuShop.shopdata[num].icon;
			}
			else
			{
				icon = WeaponData.GetData(wid).icon;
			}
			if (icon)
			{
				float num2 = r.x;
				float width = r.width;
				float height = r.width / 2f;
				float num3 = r.y + GUI2.YRES(13f);
				if (num >= 128)
				{
					height = r.width;
					num3 = r.y + GUI2.YRES(6f) - r.height / 4f;
					if (WeaponData.GetData(wid).buyMenuSlot == 1)
					{
						height = r.width * 0.75f;
						width = r.width * 0.75f;
						num3 += r.width * 0.1f;
						num2 += r.width * 0.125f;
					}
				}
				if (WeaponData.GetData(wid).buyMenuSlot == 1 && num < 128)
				{
					GUI.color = Color.black;
					GUI.DrawTexture(new Rect(r.x + 1f + GUI2.YRES(15f), r.y + GUI2.YRES(20f) + 1f, r.height, r.height / 2f), WeaponData.GetData(wid).icon);
					GUI.color = Color.white;
					GUI.DrawTexture(new Rect(r.x + GUI2.YRES(15f), r.y + GUI2.YRES(20f), r.height, r.height / 2f), WeaponData.GetData(wid).icon);
				}
				else
				{
					GUI.color = Color.black;
					GUI.DrawTexture(new Rect(num2 + 1f, num3 + 1f, width, height), icon);
					GUI.color = Color.white;
					GUI.DrawTexture(new Rect(num2, num3, width, height), icon);
				}
			}
			GUI.DrawTexture(new Rect(r.x, r.y + GUI2.YRES(66f), r.width, GUI2.YRES(14f)), this.tGray2);
			GUI.DrawTexture(new Rect(r.x + r.width - GUI2.YRES(17f), r.y + GUI2.YRES(66f), GUI2.YRES(14f), GUI2.YRES(14f)), this.tMoney);
			GUI2.DrawTextRes(new Rect(r.x, r.y + GUI2.YRES(66f), r.width - GUI2.YRES(20f), GUI2.YRES(14f)), WeaponData.GetData(wid).sCost, TextAnchor.MiddleRight, _Color.White, 0, 12, false);
			if (GUI2.HideButton(r))
			{
				this.SendBuy(slot);
			}
		}
	}

	public static void Toggle()
	{
		BuyMenu.SetActive(!BuyMenu.show);
	}

	public static void ToggleAmmunition()
	{
		BuyMenu.SetActive(!BuyMenu.show);
		BuyMenu.catid = 7;
		BuyMenu.menuLevel = 1;
	}

	public static void SetActive(bool val)
	{
		if (BuyMenu.show == val)
		{
			return;
		}
		BuyMenu.show = val;
		if (BuyMenu.show)
		{
			BuyMenu.catid = -1;
			Crosshair.SetActive(false);
			EscapeMenu.SetActive(false);
		}
		else
		{
			Crosshair.SetActive(true);
		}
		vp_FPCamera.cs.SetMouseFreeze(BuyMenu.show);
	}

	public static bool isActive()
	{
		return BuyMenu.show;
	}
}
