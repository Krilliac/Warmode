using System;
using UnityEngine;

public class MenuBanList : MonoBehaviour
{
	public struct BanData
	{
		public string playerName;

		public string playerSID;

		public BanData(string playerName, string playerSID)
		{
			this.playerName = playerName;
			this.playerSID = playerSID;
		}
	}

	public static bool show = false;

	private static bool admin = false;

	private static Texture2D tBlack;

	private static Texture2D tGray;

	private static Texture2D tRed;

	private static Texture2D tOrange;

	private static Rect rBack;

	private static Rect[] rPlayerBack = new Rect[16];

	private static Rect[] rPlayerID = new Rect[16];

	private static Rect[] rPlayerName = new Rect[16];

	private static Rect[] rPlayerSID = new Rect[16];

	private static Rect rKickButton;

	private static Rect rBanButton;

	private static int selected = -1;

	private static MenuBanList.BanData[] banList = new MenuBanList.BanData[16];

	public void PostAwake()
	{
		MenuBanList.tBlack = TEX.GetTextureByName("black");
		MenuBanList.tGray = TEX.GetTextureByName("gray");
		MenuBanList.tRed = TEX.GetTextureByName("red");
		MenuBanList.tOrange = TEX.GetTextureByName("orange");
		this.OnResize();
		MenuBanList.show = false;
	}

	public void OnResize()
	{
		MenuBanList.rBack = new Rect((float)Screen.width / 2f - GUI2.YRES(200f), (float)Screen.height / 2f - GUI2.YRES(210f), GUI2.YRES(400f), GUI2.YRES(360f));
		float num = GUI2.YRES(10f);
		float num2 = GUI2.YRES(10f);
		float num3 = GUI2.YRES(19f);
		float num4 = GUI2.YRES(6f);
		float num5 = GUI2.YRES(1f);
		for (int i = 0; i < 16; i++)
		{
			MenuBanList.rPlayerID[i] = new Rect(num + MenuBanList.rBack.x, num2 + MenuBanList.rBack.y + num3 * (float)i, GUIM.YRES(40f), GUIM.YRES(20f));
			MenuBanList.rPlayerName[i] = new Rect(num + MenuBanList.rPlayerID[i].xMax + num4, num2 + MenuBanList.rBack.y + num3 * (float)i, GUIM.YRES(280f), GUIM.YRES(20f));
			MenuBanList.rPlayerSID[i] = new Rect(num + MenuBanList.rPlayerName[i].xMax + num4, num2 + MenuBanList.rBack.y + num3 * (float)i, GUIM.YRES(200f), GUIM.YRES(20f));
			MenuBanList.rPlayerBack[i] = new Rect(MenuBanList.rPlayerID[i].x - num5, MenuBanList.rPlayerID[i].y - num5, MenuBanList.rPlayerSID[i].xMax - MenuBanList.rPlayerID[i].x + num5 * 2f, MenuBanList.rPlayerName[i].yMax - MenuBanList.rPlayerID[i].y + num5 * 2f);
		}
		MenuBanList.rKickButton = new Rect((float)Screen.width / 2f - GUI2.YRES(90f) / 2f - GUI2.YRES(80f), (float)Screen.height - GUI2.YRES(125f), GUI2.YRES(90f), GUI2.YRES(20f));
		MenuBanList.rBanButton = new Rect((float)Screen.width / 2f - GUI2.YRES(90f) / 2f + GUI2.YRES(80f), (float)Screen.height - GUI2.YRES(125f), GUI2.YRES(90f), GUI2.YRES(20f));
	}

	public static void Draw()
	{
	}

	public static void SetActive(bool val)
	{
		MenuBanList.show = val;
		if (MenuBanList.show)
		{
			Crosshair.SetActive(false);
		}
		else
		{
			MenuBanList.selected = -1;
			Crosshair.SetActive(true);
		}
		vp_FPCamera.cs.SetMouseFreeze(val);
	}

	public static void SetAdmin(bool val)
	{
		MenuBanList.admin = val;
	}

	public static void SetBanData(int id, string playerName, string playerSID)
	{
		MenuBanList.banList[id] = new MenuBanList.BanData(playerName, playerSID);
	}

	private static void DrawPlayer(int id)
	{
		Vector2 mpos = new Vector2(Input.mousePosition.x, (float)Screen.height - Input.mousePosition.y);
		if (id == MenuBanList.selected)
		{
			GUI.DrawTexture(MenuBanList.rPlayerBack[id], MenuBanList.tRed);
		}
		else if (GUIM.Contains(MenuBanList.rPlayerBack[id], mpos))
		{
			GUI.DrawTexture(MenuBanList.rPlayerBack[id], MenuBanList.tRed);
			if (Input.GetMouseButtonDown(0))
			{
				MenuBanList.selected = id;
			}
		}
		else
		{
			GUI.DrawTexture(MenuBanList.rPlayerBack[id], MenuBanList.tGray);
		}
		GUI2.DrawTextRes(MenuBanList.rPlayerID[id], id.ToString(), TextAnchor.MiddleCenter, _Color.White, 0, 12, false);
		GUI2.DrawTextRes(MenuBanList.rPlayerName[id], MenuBanList.banList[id].playerName, TextAnchor.MiddleLeft, _Color.White, 0, 12, false);
		GUI2.DrawTextRes(MenuBanList.rPlayerSID[id], MenuBanList.banList[id].playerSID, TextAnchor.MiddleLeft, _Color.White, 0, 12, false);
	}

	public static void GetList()
	{
	}

	private void Update()
	{
	}
}
