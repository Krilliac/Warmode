using System;
using UnityEngine;

public class MenuInventory : MonoBehaviour
{
	private static bool show = false;

	private static float showtime = 0f;

	private static Rect rBack;

	private static Rect rBackBody;

	private static Texture2D tBlack;

	private static Texture2D tWhite;

	private static Texture2D tGray;

	public static int currCat = 0;

	private static Vector2 scroll = Vector2.zero;

	private static int hcount = 0;

	public void LoadEnd()
	{
		MenuInventory.tBlack = TEX.GetTextureByName("black");
		MenuInventory.tWhite = TEX.GetTextureByName("white");
		MenuInventory.tGray = TEX.GetTextureByName("gray");
	}

	public void PostAwake()
	{
		MenuInventory.show = false;
		this.OnResize();
	}

	public static bool isActive()
	{
		return MenuInventory.show;
	}

	public static void SetActive(bool val)
	{
		MenuInventory.show = val;
		MenuInventory.currCat = 0;
		if (MenuInventory.show)
		{
			MenuInventory.showtime = Time.time;
			MenuShop.GenerateCustomIcons();
		}
	}

	public void OnResize()
	{
		MenuInventory.rBack = new Rect((float)Screen.width / 2f - GUIM.YRES(165f), GUIM.YRES(80f), GUIM.YRES(632f), GUIM.YRES(525f));
		MenuInventory.rBackBody = MenuInventory.rBack;
	}

	public static void Draw()
	{
		if (!MenuInventory.show)
		{
			return;
		}
		float num = Time.time - MenuInventory.showtime + 0.001f;
		if (num > 0.05f)
		{
			num = 0.05f;
		}
		num *= 20f;
		Matrix4x4 matrix = GUI.matrix;
		Vector3 s = new Vector3(num, num, 1f);
		Vector3 pos = new Vector3(MenuInventory.rBack.center.x - MenuInventory.rBack.center.x * num, MenuInventory.rBack.center.y - MenuInventory.rBack.center.y * num, 1f);
		GUI.matrix = Matrix4x4.TRS(pos, Quaternion.identity, s);
		GUIM.DrawBox(MenuInventory.rBack, MenuInventory.tBlack);
		MenuInventory.DrawButtonCategory(0, new Rect(MenuInventory.rBackBody.x + GUIM.YRES(4f) + GUIM.YRES(84f) * 0f, MenuInventory.rBackBody.y + GUIM.YRES(4f), GUIM.YRES(80f), GUIM.YRES(24f)), Lang.Get("_ALL"), false);
		MenuInventory.DrawButtonCategory(1, new Rect(MenuInventory.rBackBody.x + GUIM.YRES(4f) + GUIM.YRES(84f) * 1f, MenuInventory.rBackBody.y + GUIM.YRES(4f), GUIM.YRES(80f), GUIM.YRES(24f)), Lang.Get("_STOCK"), false);
		MenuInventory.DrawButtonCategory(2, new Rect(MenuInventory.rBackBody.x + GUIM.YRES(4f) + GUIM.YRES(84f) * 2f, MenuInventory.rBackBody.y + GUIM.YRES(4f), GUIM.YRES(80f), GUIM.YRES(24f)), Lang.Get("_BADGES"), false);
		MenuInventory.DrawButtonCategory(3, new Rect(MenuInventory.rBackBody.x + GUIM.YRES(4f) + GUIM.YRES(84f) * 3f, MenuInventory.rBackBody.y + GUIM.YRES(4f), GUIM.YRES(80f), GUIM.YRES(24f)), Lang.Get("_MASKS"), false);
		MenuInventory.DrawButtonCategory(4, new Rect(MenuInventory.rBackBody.x + GUIM.YRES(4f) + GUIM.YRES(84f) * 4f, MenuInventory.rBackBody.y + GUIM.YRES(4f), GUIM.YRES(80f), GUIM.YRES(24f)), Lang.Get("_WEAPONS"), false);
		MenuInventory.scroll = GUIM.BeginScrollView(new Rect(MenuInventory.rBackBody.x + GUIM.YRES(4f), MenuInventory.rBackBody.y + GUIM.YRES(32f), MenuInventory.rBackBody.width - GUIM.YRES(8f), MenuInventory.rBackBody.height - GUIM.YRES(40f)), MenuInventory.scroll, new Rect(0f, 0f, 0f, (float)MenuInventory.hcount * GUIM.YRES(100f) - GUIM.YRES(4f)));
		int num2 = 0;
		int num3 = 0;
		for (int i = 0; i < 1024; i++)
		{
			if (BaseData.item[i] != 0 || i <= 127)
			{
				if (MenuShop.shopdata[i] != null)
				{
					if (MenuInventory.currCat != 0)
					{
						if (MenuInventory.currCat == 1 && MenuShop.shopdata[i].section != 0)
						{
							goto IL_602;
						}
						if (MenuInventory.currCat == 2 && MenuShop.shopdata[i].section != 1 && MenuShop.shopdata[i].section != 2)
						{
							goto IL_602;
						}
						if (MenuInventory.currCat == 3 && MenuShop.shopdata[i].section != 3 && MenuShop.shopdata[i].section != 4)
						{
							goto IL_602;
						}
						if (MenuInventory.currCat == 4 && MenuShop.shopdata[i].section != 5)
						{
							goto IL_602;
						}
					}
					if (MenuShop.DrawItem(new Rect((GUIM.YRES(96f) + GUIM.YRES(4f)) * (float)num2, (GUIM.YRES(96f) + GUIM.YRES(4f)) * (float)num3, GUIM.YRES(96f), GUIM.YRES(96f)), MenuShop.shopdata[i]))
					{
						if (MenuShop.shopdata[i].section == 1)
						{
							BaseData.badge_back = i;
							PlayerPrefs.SetInt(BaseData.uid + "_badge_back", i);
						}
						else if (MenuShop.shopdata[i].section == 2)
						{
							BaseData.badge_icon = i;
							PlayerPrefs.SetInt(BaseData.uid + "_badge_icon", i);
						}
						else if (MenuShop.shopdata[i].section == 3)
						{
							BaseData.mask_merc = i;
							PlayerPrefs.SetInt(BaseData.uid + "_mask_merc", i);
						}
						else if (MenuShop.shopdata[i].section == 4)
						{
							BaseData.mask_warcorp = i;
							PlayerPrefs.SetInt(BaseData.uid + "_mask_warcorp", i);
						}
						else if (MenuShop.shopdata[i].section == 5)
						{
							int id = WeaponData.GetId(MenuShop.shopdata[i].name2);
							if (id > 0)
							{
								BaseData.profileWeapon[id] = i;
								BaseData.currentWeapon[id] = i;
								PlayerPrefs.SetInt(BaseData.uid + "_custom_" + MenuShop.shopdata[i].name2, i);
							}
						}
					}
					num2++;
					if (num2 >= 6)
					{
						num2 = 0;
						num3++;
					}
				}
			}
			IL_602:;
		}
		MenuInventory.hcount = num3 + 1;
		GUIM.EndScrollView();
		GUI.matrix = matrix;
	}

	private static void DrawButtonCategory(int cat, Rect r, string name, bool block = false)
	{
		bool flag;
		if (block)
		{
			flag = GUIM.Button(r, BaseColor.Orange, name, TextAnchor.MiddleCenter, BaseColor.White, 1, 12, false);
			return;
		}
		if (MenuInventory.currCat == cat)
		{
			flag = GUIM.Button(r, BaseColor.White, name, TextAnchor.MiddleCenter, BaseColor.Blue, 1, 12, false);
		}
		else
		{
			flag = GUIM.Button(r, BaseColor.Gray, name, TextAnchor.MiddleCenter, BaseColor.White, 1, 12, false);
		}
		if (flag)
		{
			MenuInventory.currCat = cat;
		}
	}
}
