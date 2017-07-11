using System;
using UnityEngine;

public class Profile : MonoBehaviour
{
	public static Vector2 mpos = Vector2.zero;

	private static Texture2D tBlack;

	private static Texture2D tWhite;

	private static Texture2D tGray;

	private static Texture2D tOrange;

	private static Texture2D tBlue;

	private static Texture2D tGreen;

	private static Texture2D tGold = null;

	private static Texture2D tBadgeBack = null;

	private static Texture2D tBadgeIcon = null;

	private static Texture2D tMercMask = null;

	private static Texture2D tCorpMask = null;

	private static Texture2D[] tPlayer = new Texture2D[2];

	private static Texture2D[] tInventory = new Texture2D[2];

	private static Rect rBackMenu;

	private static Rect rBackProfile;

	private static Rect rBackGold;

	private static Rect rBackName;

	private static Rect rAvatar;

	private static bool inEdit = false;

	private static string newname = string.Empty;

	private static Rect rBackBadge;

	private static Rect rBackMask;

	private void LoadEnd()
	{
		Profile.tGold = TEX.GetTextureByName("gold_64");
		Profile.tBadgeBack = TEX.GetTextureByName("warbadge_hor_empty");
		Profile.tBadgeIcon = TEX.GetTextureByName("warbadge_icon_empty");
		Profile.tPlayer[0] = TEX.GetTextureByName("player");
		Profile.tPlayer[1] = TEX.GetTextureByName("player_hover");
		Profile.tInventory[0] = TEX.GetTextureByName("inventory");
		Profile.tInventory[1] = TEX.GetTextureByName("inventory_hover");
		Profile.tMercMask = TEX.GetTextureByName("merc_default");
		Profile.tCorpMask = TEX.GetTextureByName("warcorp_default");
	}

	public void PostAwake()
	{
		Profile.tBlack = TEX.GetTextureByName("black");
		Profile.tWhite = TEX.GetTextureByName("white");
		Profile.tGray = TEX.GetTextureByName("gray");
		Profile.tBlue = TEX.GetTextureByName("lightblue");
		Profile.tOrange = TEX.GetTextureByName("orange");
		Profile.tGreen = TEX.GetTextureByName("green");
		this.OnResize();
	}

	private void OnResize()
	{
		Profile.rBackMenu = new Rect((float)Screen.width / 2f - GUIM.YRES(460f), GUIM.YRES(110f), GUIM.YRES(256f), GUIM.YRES(40f));
		Profile.rBackProfile = new Rect((float)Screen.width / 2f - GUIM.YRES(460f), GUIM.YRES(170f), GUIM.YRES(256f), GUIM.YRES(112f));
		if (GameData.gSteam)
		{
			Profile.rBackGold = new Rect(0f, 0f, 0f, 0f);
		}
		else
		{
			Profile.rBackGold = new Rect((float)Screen.width / 2f - GUIM.YRES(460f), GUIM.YRES(286f), GUIM.YRES(256f), GUIM.YRES(42f));
		}
		Profile.rBackBadge = new Rect((float)Screen.width / 2f - GUIM.YRES(460f), GUIM.YRES(302f) + Profile.rBackGold.height + GUIM.YRES(4f), GUIM.YRES(256f), GUIM.YRES(76f));
		Profile.rBackMask = new Rect((float)Screen.width / 2f - GUIM.YRES(460f), GUIM.YRES(398f) + Profile.rBackGold.height + GUIM.YRES(4f), GUIM.YRES(256f), GUIM.YRES(76f));
		Profile.rBackName = new Rect(Profile.rBackProfile.x + GUIM.YRES(80f), Profile.rBackProfile.y + GUIM.YRES(32f), GUIM.YRES(168f), GUIM.YRES(24f));
		Profile.rAvatar = new Rect(Profile.rBackProfile.x + GUIM.YRES(8f), Profile.rBackProfile.y + GUIM.YRES(8f), GUIM.YRES(64f), GUIM.YRES(64f));
	}

	public static void Draw()
	{
		Profile.DrawMenu();
		Profile.DrawProfile();
		Profile.DrawBadge();
		Profile.DrawMask();
		if (Event.current.isKey)
		{
			KeyCode keyCode = Event.current.keyCode;
			if (keyCode == KeyCode.Return || keyCode == KeyCode.KeypadEnter)
			{
				if (Profile.inEdit)
				{
					BaseData.Name = Profile.newname;
					Profile.inEdit = false;
					WebHandler.set_profile();
				}
				Event.current.Use();
			}
		}
		if (Event.current.isMouse && Profile.inEdit)
		{
			BaseData.Name = Profile.newname;
			Profile.inEdit = false;
			WebHandler.set_profile();
		}
	}

	public static void DrawProfile()
	{
		GUIM.DrawText(new Rect(Profile.rBackProfile.x, Profile.rBackProfile.y - GUIM.YRES(18f), Profile.rBackProfile.width, GUIM.YRES(18f)), Lang.Get("_PROFILE"), TextAnchor.MiddleLeft, BaseColor.White, 1, 12, false);
		Profile.mpos = new Vector2(Input.mousePosition.x, (float)Screen.height - Input.mousePosition.y);
		BaseColor fontcolor = BaseColor.White;
		GUIM.DrawBox(Profile.rBackProfile, Profile.tBlack);
		if (Main.avatar == null)
		{
			GUI.DrawTexture(Profile.rAvatar, Profile.tBlack);
		}
		else
		{
			GUI.DrawTexture(Profile.rAvatar, Main.avatar);
		}
		if (GUIM.HideButton(Profile.rBackName) && !GameData.gSteam)
		{
			Profile.inEdit = true;
			Profile.newname = BaseData.Name;
		}
		if (Profile.inEdit)
		{
			GUI.DrawTexture(Profile.rBackName, Profile.tGray);
			GUIM.DrawEdit(Profile.rBackName, ref Profile.newname, TextAnchor.MiddleCenter, BaseColor.White, 1, 14, true);
		}
		else
		{
			if (Profile.rBackName.Contains(Profile.mpos) && !GameData.gSteam)
			{
				GUI.DrawTexture(Profile.rBackName, Profile.tOrange);
			}
			else
			{
				GUI.DrawTexture(Profile.rBackName, Profile.tBlue);
			}
			GUIM.DrawText(Profile.rBackName, BaseData.Name, TextAnchor.MiddleCenter, fontcolor, 1, 14, true);
		}
		GUIM.DrawText(new Rect(Profile.rBackName.x, Profile.rBackProfile.y + GUIM.YRES(8f), Profile.rBackName.width, Profile.rBackName.height), Lang.Get("_NICKNAME"), TextAnchor.MiddleLeft, BaseColor.White, 1, 14, false);
		GUI.DrawTexture(new Rect(Profile.rBackProfile.x + GUIM.YRES(8f), Profile.rBackProfile.y + GUIM.YRES(80f), GUIM.YRES(48f), GUIM.YRES(24f)), Profile.tWhite);
		GUIM.DrawText(new Rect(Profile.rBackProfile.x + GUIM.YRES(8f), Profile.rBackProfile.y + GUIM.YRES(80f), GUIM.YRES(48f), GUIM.YRES(24f)), BaseData.Level, TextAnchor.MiddleCenter, BaseColor.Blue, 1, 14, false);
		GUI.DrawTexture(new Rect(Profile.rBackProfile.x + GUIM.YRES(58f), Profile.rBackProfile.y + GUIM.YRES(94f), GUIM.YRES(190f), GUIM.YRES(10f)), Profile.tGray);
		GUI.DrawTexture(new Rect(Profile.rBackProfile.x + GUIM.YRES(58f), Profile.rBackProfile.y + GUIM.YRES(94f), (float)BaseData.iProgress * 0.01f * GUIM.YRES(190f), GUIM.YRES(10f)), Profile.tOrange);
		GUIM.DrawText(new Rect(Profile.rBackProfile.x + GUIM.YRES(58f), Profile.rBackProfile.y + GUIM.YRES(80f), GUIM.YRES(190f), GUIM.YRES(12f)), BaseData.Progress, TextAnchor.MiddleRight, BaseColor.Gray, 1, 14, false);
		GUIM.DrawText(new Rect(Profile.rBackProfile.x + GUIM.YRES(58f), Profile.rBackProfile.y + GUIM.YRES(80f), GUIM.YRES(190f), GUIM.YRES(12f)), BaseData.EXPData, TextAnchor.MiddleLeft, BaseColor.Gray, 1, 14, false);
		if (GameData.gSteam)
		{
			return;
		}
		GUIM.DrawBox(Profile.rBackGold, Profile.tBlack);
		GUI.DrawTexture(new Rect(Profile.rBackGold.x + GUIM.YRES(14f), Profile.rBackGold.y + GUIM.YRES(6f), GUIM.YRES(30f), GUIM.YRES(30f)), Profile.tGold);
		GUIM.DrawText(new Rect(Profile.rBackGold.x + GUIM.YRES(50f), Profile.rBackGold.y + GUIM.YRES(1f), GUIM.YRES(110f), Profile.rBackGold.height - GUIM.YRES(4f)), BaseData.Gold, TextAnchor.MiddleLeft, BaseColor.White, 1, 20, false);
		Profile.DrawMenuButton(2, new Rect(Profile.rBackGold.x + GUIM.YRES(120f), Profile.rBackGold.y + GUIM.YRES(6f), GUIM.YRES(126f), GUIM.YRES(30f)), Profile.tGreen, Profile.tOrange);
		GUIM.DrawText(new Rect(Profile.rBackGold.x + GUIM.YRES(120f), Profile.rBackGold.y + GUIM.YRES(5f), GUIM.YRES(126f), GUIM.YRES(30f)), Lang.Get("_ADD_GOLD"), TextAnchor.MiddleCenter, BaseColor.White, 1, 14, true);
	}

	public static void DrawMenu()
	{
		GUIM.DrawText(new Rect(Profile.rBackMenu.x, Profile.rBackMenu.y - GUIM.YRES(18f), Profile.rBackMenu.width, GUIM.YRES(18f)), Lang.Get("_PLAYER_MENU"), TextAnchor.MiddleLeft, BaseColor.White, 1, 12, false);
		GUIM.DrawBox(Profile.rBackMenu, Profile.tBlack);
		Profile.DrawMenuButton(0, new Rect(Profile.rBackMenu.x + GUIM.YRES(2f), Profile.rBackMenu.y + GUIM.YRES(0f), GUIM.YRES(40f), GUIM.YRES(40f)), Profile.tPlayer[0], Profile.tPlayer[1]);
		Profile.DrawMenuButton(1, new Rect(Profile.rBackMenu.x + GUIM.YRES(2f) + GUIM.YRES(40f), Profile.rBackMenu.y + GUIM.YRES(0f), GUIM.YRES(40f), GUIM.YRES(40f)), Profile.tInventory[0], Profile.tInventory[1]);
		GUI.color = new Color(1f, 1f, 1f, 0.4f);
		GUI.DrawTexture(new Rect(Profile.rBackMenu.x + (GUIM.YRES(2f) + GUIM.YRES(40f)) * 2f, Profile.rBackMenu.y + GUIM.YRES(4f), GUIM.YRES(40f), GUIM.YRES(32f)), Profile.tBlack);
		GUI.DrawTexture(new Rect(Profile.rBackMenu.x + (GUIM.YRES(2f) + GUIM.YRES(40f)) * 3f, Profile.rBackMenu.y + GUIM.YRES(4f), GUIM.YRES(40f), GUIM.YRES(32f)), Profile.tBlack);
		GUI.DrawTexture(new Rect(Profile.rBackMenu.x + (GUIM.YRES(2f) + GUIM.YRES(40f)) * 4f, Profile.rBackMenu.y + GUIM.YRES(4f), GUIM.YRES(40f), GUIM.YRES(32f)), Profile.tBlack);
		GUI.DrawTexture(new Rect(Profile.rBackMenu.x + (GUIM.YRES(2f) + GUIM.YRES(40f)) * 5f, Profile.rBackMenu.y + GUIM.YRES(4f), GUIM.YRES(40f), GUIM.YRES(32f)), Profile.tBlack);
		GUI.color = Color.white;
	}

	private static void DrawMenuButton(int state, Rect r, Texture2D normal, Texture2D hover)
	{
		Profile.mpos = new Vector2(Input.mousePosition.x, (float)Screen.height - Input.mousePosition.y);
		if (r.Contains(Profile.mpos))
		{
			GUI.DrawTexture(r, hover);
		}
		else
		{
			GUI.DrawTexture(r, normal);
		}
		if (GUIM.HideButton(r))
		{
			Main.HideAll();
			if (state == 0)
			{
				MenuPlayer.SetActive(true);
			}
			if (state == 1)
			{
				MenuInventory.SetActive(true);
			}
			if (state == 2)
			{
				MenuGold.SetActive(true);
			}
		}
	}

	private static void DrawBadge()
	{
		GUIM.DrawText(new Rect(Profile.rBackBadge.x, Profile.rBackBadge.y - GUIM.YRES(18f), Profile.rBackBadge.width, GUIM.YRES(18f)), Lang.Get("_BADGE"), TextAnchor.MiddleLeft, BaseColor.White, 1, 12, false);
		Profile.mpos = new Vector2(Input.mousePosition.x, (float)Screen.height - Input.mousePosition.y);
		GUIM.DrawBox(Profile.rBackBadge, Profile.tBlack);
		Rect position = new Rect(Profile.rBackBadge.x + GUIM.YRES(8f), Profile.rBackBadge.y + GUIM.YRES(8f), GUIM.YRES(240f), GUIM.YRES(60f));
		Rect position2 = new Rect(Profile.rBackBadge.x + GUIM.YRES(4f) + GUIM.YRES(12f), Profile.rBackBadge.y + GUIM.YRES(8f), GUIM.YRES(60f), GUIM.YRES(60f));
		if (BaseData.badge_back == 0)
		{
			GUI.DrawTexture(position, Profile.tBadgeBack);
		}
		else
		{
			GUI.DrawTexture(position, MenuShop.shopdata[BaseData.badge_back].icon);
		}
		if (BaseData.badge_icon == 0)
		{
			GUI.color = new Color(0f, 0f, 0f, 0.5f);
			GUI.DrawTexture(position2, Profile.tBadgeIcon);
			GUI.color = Color.white;
		}
		else
		{
			GUI.DrawTexture(position2, MenuShop.shopdata[BaseData.badge_icon].icon);
		}
	}

	private static void DrawMask()
	{
		GUIM.DrawText(new Rect(Profile.rBackMask.x, Profile.rBackMask.y - GUIM.YRES(18f), Profile.rBackMask.width, GUIM.YRES(18f)), Lang.Get("_MASKS"), TextAnchor.MiddleLeft, BaseColor.White, 1, 12, false);
		GUIM.DrawBox(Profile.rBackMask, Profile.tBlack);
		if (BaseData.mask_merc == 0)
		{
			GUI.DrawTexture(new Rect(Profile.rBackMask.x + GUIM.YRES(32f), Profile.rBackMask.y + GUIM.YRES(8f), GUIM.YRES(60f), GUIM.YRES(60f)), Profile.tMercMask);
		}
		else
		{
			GUI.DrawTexture(new Rect(Profile.rBackMask.x + GUIM.YRES(32f), Profile.rBackMask.y + GUIM.YRES(8f), GUIM.YRES(60f), GUIM.YRES(60f)), MenuShop.shopdata[BaseData.mask_merc].icon);
		}
		if (BaseData.mask_warcorp == 0)
		{
			GUI.DrawTexture(new Rect(Profile.rBackMask.x + Profile.rBackMask.width - GUIM.YRES(92f), Profile.rBackMask.y + GUIM.YRES(8f), GUIM.YRES(60f), GUIM.YRES(60f)), Profile.tCorpMask);
		}
		else
		{
			GUI.DrawTexture(new Rect(Profile.rBackMask.x + Profile.rBackMask.width - GUIM.YRES(92f), Profile.rBackMask.y + GUIM.YRES(8f), GUIM.YRES(60f), GUIM.YRES(60f)), MenuShop.shopdata[BaseData.mask_warcorp].icon);
		}
	}
}
