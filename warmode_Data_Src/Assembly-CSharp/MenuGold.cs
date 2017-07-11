using System;
using UnityEngine;

public class MenuGold : MonoBehaviour
{
	private static bool show;

	private static Texture2D tBlack;

	private static Texture2D tWhite;

	private static Texture2D tBuy;

	private static Texture2D tOrange;

	private static Texture2D tGray;

	private static Texture2D tVK;

	private static Texture2D tGold;

	private static Rect rBack;

	private static Rect rBackName;

	public void LoadEnd()
	{
		MenuGold.tBlack = TEX.GetTextureByName("black");
		MenuGold.tWhite = TEX.GetTextureByName("white");
		MenuGold.tBuy = TEX.GetTextureByName("lightblue");
		MenuGold.tOrange = TEX.GetTextureByName("orange");
		MenuGold.tGray = TEX.GetTextureByName("gray");
		MenuGold.tGold = TEX.GetTextureByName("gold_64");
		MenuGold.tVK = TEX.GetTextureByName("vk");
	}

	public void PostAwake()
	{
		MenuGold.show = false;
		this.OnResize();
	}

	public static void SetActive(bool val)
	{
		MenuGold.show = val;
		if (MenuGold.show)
		{
		}
	}

	public void OnResize()
	{
		MenuGold.rBack = new Rect((float)Screen.width / 2f - GUIM.YRES(165f), GUIM.YRES(80f), GUIM.YRES(632f), GUIM.YRES(525f));
		MenuGold.rBackName = new Rect(MenuGold.rBack.x + GUIM.YRES(6f), MenuGold.rBack.y + GUIM.YRES(6f), MenuGold.rBack.width - GUIM.YRES(6f) * 2f, GUIM.YRES(32f));
	}

	public static void Draw()
	{
		if (!MenuGold.show)
		{
			return;
		}
		GUIM.DrawBox(MenuGold.rBack, MenuGold.tBlack);
		GUI.DrawTexture(MenuGold.rBackName, MenuGold.tOrange);
		GUIM.DrawText(MenuGold.rBackName, Lang.Get("_ADD_GOLD"), TextAnchor.MiddleCenter, BaseColor.White, 1, 16, false);
		if (GameData.gVK)
		{
			if (MenuGold.DrawPrice(new Rect(MenuGold.rBackName.x, MenuGold.rBackName.y + GUIM.YRES(128f) + GUIM.YRES(44f) * 0f, MenuGold.rBackName.width, GUIM.YRES(36f)), "1", "8", string.Empty))
			{
				Application.ExternalCall("order", new object[]
				{
					"item0"
				});
			}
			if (MenuGold.DrawPrice(new Rect(MenuGold.rBackName.x, MenuGold.rBackName.y + GUIM.YRES(128f) + GUIM.YRES(44f) * 1f, MenuGold.rBackName.width, GUIM.YRES(36f)), "5", "40", string.Empty))
			{
				Application.ExternalCall("order", new object[]
				{
					"item1"
				});
			}
			if (MenuGold.DrawPrice(new Rect(MenuGold.rBackName.x, MenuGold.rBackName.y + GUIM.YRES(128f) + GUIM.YRES(44f) * 2f, MenuGold.rBackName.width, GUIM.YRES(36f)), "10", "80", string.Empty))
			{
				Application.ExternalCall("order", new object[]
				{
					"item2"
				});
			}
			if (MenuGold.DrawPrice(new Rect(MenuGold.rBackName.x, MenuGold.rBackName.y + GUIM.YRES(128f) + GUIM.YRES(44f) * 3f, MenuGold.rBackName.width, GUIM.YRES(36f)), "50", "400", "+40"))
			{
				Application.ExternalCall("order", new object[]
				{
					"item3"
				});
			}
			if (MenuGold.DrawPrice(new Rect(MenuGold.rBackName.x, MenuGold.rBackName.y + GUIM.YRES(128f) + GUIM.YRES(44f) * 4f, MenuGold.rBackName.width, GUIM.YRES(36f)), "100", "800", "+100"))
			{
				Application.ExternalCall("order", new object[]
				{
					"item4"
				});
			}
			if (MenuGold.DrawPrice(new Rect(MenuGold.rBackName.x, MenuGold.rBackName.y + GUIM.YRES(128f) + GUIM.YRES(44f) * 5f, MenuGold.rBackName.width, GUIM.YRES(36f)), "200", "1600", "+240"))
			{
				Application.ExternalCall("order", new object[]
				{
					"item5"
				});
			}
		}
		if (GameData.gFB)
		{
			if (MenuGold.DrawPrice(new Rect(MenuGold.rBackName.x, MenuGold.rBackName.y + GUIM.YRES(128f) + GUIM.YRES(44f) * 0f, MenuGold.rBackName.width, GUIM.YRES(36f)), "$0.16", "8", string.Empty))
			{
				FBManager.BuyCoins(8);
			}
			if (MenuGold.DrawPrice(new Rect(MenuGold.rBackName.x, MenuGold.rBackName.y + GUIM.YRES(128f) + GUIM.YRES(44f) * 1f, MenuGold.rBackName.width, GUIM.YRES(36f)), "$0.80", "40", string.Empty))
			{
				FBManager.BuyCoins(40);
			}
			if (MenuGold.DrawPrice(new Rect(MenuGold.rBackName.x, MenuGold.rBackName.y + GUIM.YRES(128f) + GUIM.YRES(44f) * 2f, MenuGold.rBackName.width, GUIM.YRES(36f)), "$1.60", "80", string.Empty))
			{
				FBManager.BuyCoins(80);
			}
			if (MenuGold.DrawPrice(new Rect(MenuGold.rBackName.x, MenuGold.rBackName.y + GUIM.YRES(128f) + GUIM.YRES(44f) * 3f, MenuGold.rBackName.width, GUIM.YRES(36f)), "$8.00", "400", "+40"))
			{
				FBManager.BuyCoins(400);
			}
			if (MenuGold.DrawPrice(new Rect(MenuGold.rBackName.x, MenuGold.rBackName.y + GUIM.YRES(128f) + GUIM.YRES(44f) * 4f, MenuGold.rBackName.width, GUIM.YRES(36f)), "$16.00", "800", "+100"))
			{
				FBManager.BuyCoins(800);
			}
			if (MenuGold.DrawPrice(new Rect(MenuGold.rBackName.x, MenuGold.rBackName.y + GUIM.YRES(128f) + GUIM.YRES(44f) * 5f, MenuGold.rBackName.width, GUIM.YRES(36f)), "$32.00", "1600", "+240"))
			{
				FBManager.BuyCoins(1600);
			}
		}
	}

	private static bool DrawPrice(Rect r, string cost, string gold, string goldbonus)
	{
		GUI.DrawTexture(r, MenuGold.tGray);
		Rect rect = r;
		r = new Rect(r.x, r.y, GUIM.YRES(128f), rect.height - GUIM.YRES(4f));
		GUIM.DrawText(r, gold, TextAnchor.MiddleRight, BaseColor.White, 1, 20, true);
		r = new Rect(r.x + GUIM.YRES(130f), r.y, rect.height, rect.height);
		GUI.DrawTexture(new Rect(r.x + GUIM.YRES(4f), r.y + GUIM.YRES(4f), r.width - GUIM.YRES(8f), r.height - GUIM.YRES(8f)), MenuGold.tGold);
		if (goldbonus != string.Empty)
		{
			r = new Rect(r.x + r.height, r.y, GUIM.YRES(96f), rect.height - GUIM.YRES(4f));
			GUIM.DrawText(r, goldbonus, TextAnchor.MiddleRight, BaseColor.White, 1, 20, true);
			r = new Rect(r.x + GUIM.YRES(98f), r.y, rect.height, rect.height);
			GUI.DrawTexture(new Rect(r.x + GUIM.YRES(4f), r.y + GUIM.YRES(4f), r.width - GUIM.YRES(8f), r.height - GUIM.YRES(8f)), MenuGold.tGold);
			r = new Rect(r.x + r.height + GUIM.YRES(4f), r.y, GUIM.YRES(96f), rect.height - GUIM.YRES(4f));
			GUIM.DrawText(r, Lang.Get("_BONUS"), TextAnchor.MiddleLeft, BaseColor.White, 1, 20, true);
		}
		r = new Rect(rect.x + rect.width - GUIM.YRES(140f), r.y, GUIM.YRES(140f), rect.height);
		GUI.DrawTexture(r, MenuGold.tBuy);
		if (GameData.gVK)
		{
			r = new Rect(r.x, r.y, r.width, r.height - GUIM.YRES(4f));
			GUIM.DrawText(new Rect(r.x, r.y, r.width - GUIM.YRES(72f), r.height), cost, TextAnchor.MiddleRight, BaseColor.White, 1, 20, true);
			GUI.DrawTexture(new Rect(r.x + GUIM.YRES(74f), r.y + GUIM.YRES(6f), GUIM.YRES(24f), GUIM.YRES(24f)), MenuGold.tVK);
		}
		if (GameData.gFB)
		{
			GUIM.DrawText(r, cost, TextAnchor.MiddleCenter, BaseColor.White, 1, 20, true);
		}
		return GUIM.HideButton(r);
	}
}
