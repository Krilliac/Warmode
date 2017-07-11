using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public class MenuFriends : MonoBehaviour
{
	public class FriendData
	{
		public int index;

		public string name;

		public Texture2D avatar;

		public int state;

		public bool ingame;

		public FriendData(int index, string name, Texture2D avatar, bool ingame, int state)
		{
			this.index = index;
			this.name = name;
			this.avatar = avatar;
			this.state = state;
			this.ingame = ingame;
		}

		~FriendData()
		{
		}
	}

	public static bool show = false;

	public static List<MenuFriends.FriendData> friendlist = new List<MenuFriends.FriendData>();

	public static List<MenuFriends.FriendData> friendlistorder = new List<MenuFriends.FriendData>();

	private static Texture2D tBlack = null;

	private static Texture2D tWhite = null;

	private static Texture2D tGray = null;

	private static int currpage = 0;

	public static void SetActive(bool val)
	{
		MenuFriends.show = val;
	}

	public void LoadEnd()
	{
		MenuFriends.tBlack = TEX.GetTextureByName("black");
		MenuFriends.tWhite = TEX.GetTextureByName("white");
		MenuFriends.tGray = TEX.GetTextureByName("gray");
	}

	public static void Load()
	{
		if (!Steam.active || !Steam.logged)
		{
			return;
		}
		MenuFriends.friendlist.Clear();
		MenuFriends.friendlistorder.Clear();
		int num = 0;
		IntPtr zero = IntPtr.Zero;
		bool ingame = false;
		byte[] array = new byte[16384];
		int state = 0;
		while (Steam.GetFriend(ref zero, ref ingame, array, ref state))
		{
			string name = Marshal.PtrToStringAnsi(zero);
			Texture2D texture2D = new Texture2D(64, 64, TextureFormat.RGBA32, false, true);
			texture2D.LoadRawTextureData(array);
			for (int i = 0; i < 64; i++)
			{
				for (int j = 0; j < 32; j++)
				{
					Color pixel = texture2D.GetPixel(i, j);
					texture2D.SetPixel(i, j, texture2D.GetPixel(i, 63 - j));
					texture2D.SetPixel(i, 63 - j, pixel);
				}
			}
			texture2D.Apply(false);
			MenuFriends.friendlist.Add(new MenuFriends.FriendData(num, name, texture2D, ingame, state));
			num++;
		}
		for (int k = 0; k < MenuFriends.friendlist.Count; k++)
		{
			if (MenuFriends.friendlist[k].ingame)
			{
				MenuFriends.friendlistorder.Add(MenuFriends.friendlist[k]);
			}
		}
		for (int l = 1; l < 7; l++)
		{
			for (int m = 0; m < MenuFriends.friendlist.Count; m++)
			{
				if (!MenuFriends.friendlist[m].ingame)
				{
					if (MenuFriends.friendlist[m].state == l)
					{
						MenuFriends.friendlistorder.Add(MenuFriends.friendlist[m]);
					}
				}
			}
		}
		for (int n = 0; n < MenuFriends.friendlist.Count; n++)
		{
			if (!MenuFriends.friendlist[n].ingame)
			{
				if (MenuFriends.friendlist[n].state == 0)
				{
					MenuFriends.friendlistorder.Add(MenuFriends.friendlist[n]);
				}
			}
		}
	}

	public static void Draw()
	{
		if (!MenuFriends.show)
		{
			return;
		}
		GUIM.DrawText(new Rect((float)Screen.width / 2f + GUIM.YRES(240f), GUIM.YRES(110f) - GUIM.YRES(18f), GUIM.YRES(200f), GUIM.YRES(18f)), Lang.Get("_FRIENDS"), TextAnchor.MiddleLeft, BaseColor.White, 1, 12, false);
		int num = (int)GUIM.YRES(36f);
		int num2 = (int)GUIM.YRES(4f);
		int num3 = 0;
		int num4 = MenuFriends.currpage * 14;
		int num5 = (MenuFriends.currpage + 1) * 14;
		if (num5 > MenuFriends.friendlistorder.Count)
		{
			num5 = MenuFriends.friendlistorder.Count;
		}
		for (int i = num4; i < num5; i++)
		{
			MenuFriends.DrawFriend(new Rect((float)Screen.width / 2f + GUIM.YRES(240f), GUIM.YRES(110f) + (float)((num + num2) * num3), GUIM.YRES(200f), (float)num), MenuFriends.friendlistorder[i]);
			num3++;
		}
		if (GUIM.Button(new Rect((float)Screen.width / 2f + GUIM.YRES(396f), GUIM.YRES(110f) - GUIM.YRES(18f), GUIM.YRES(20f), GUIM.YRES(16f)), BaseColor.Black, "<", TextAnchor.MiddleCenter, BaseColor.Gray, 1, 16, false))
		{
			MenuFriends.currpage--;
			if (MenuFriends.currpage < 0)
			{
				MenuFriends.currpage = 0;
			}
		}
		if (GUIM.Button(new Rect((float)Screen.width / 2f + GUIM.YRES(396f) + GUIM.YRES(24f), GUIM.YRES(110f) - GUIM.YRES(18f), GUIM.YRES(20f), GUIM.YRES(16f)), BaseColor.Black, ">", TextAnchor.MiddleCenter, BaseColor.Gray, 1, 16, false))
		{
			MenuFriends.currpage++;
			if (MenuFriends.currpage > MenuFriends.friendlistorder.Count / 14)
			{
				MenuFriends.currpage = MenuFriends.friendlistorder.Count / 14;
			}
		}
	}

	private static void DrawFriend(Rect r, MenuFriends.FriendData f)
	{
		if (f == null)
		{
			return;
		}
		int num = (int)GUIM.YRES(2f);
		GUIM.DrawBox(r, MenuFriends.tBlack);
		if (f.avatar != null)
		{
			GUI.DrawTexture(new Rect(r.x + (float)num, r.y + (float)num, GUIM.YRES(32f), GUIM.YRES(32f)), f.avatar);
		}
		GUIM.DrawText(new Rect(r.x + GUIM.YRES(40f), r.y + (float)num, GUIM.YRES(140f), GUIM.YRES(20f)), f.name, TextAnchor.MiddleLeft, BaseColor.White, 1, 14, false);
		if (f.ingame)
		{
			GUIM.DrawText(new Rect(r.x + GUIM.YRES(40f), r.y + GUIM.YRES(16f), GUIM.YRES(140f), GUIM.YRES(20f)), "WARMODE", TextAnchor.MiddleLeft, BaseColor.Yellow, 0, 12, false);
		}
		else
		{
			string state = MenuFriends.GetState(f.state);
			GUIM.DrawText(new Rect(r.x + GUIM.YRES(40f), r.y + GUIM.YRES(16f), GUIM.YRES(140f), GUIM.YRES(20f)), state, TextAnchor.MiddleLeft, BaseColor.Gray, 0, 12, false);
		}
	}

	private static string GetState(int state)
	{
		if (state == 0)
		{
			return "OFFLINE";
		}
		if (state == 1)
		{
			return "ONLINE";
		}
		if (state == 2)
		{
			return "BUSY";
		}
		if (state == 3)
		{
			return "AWAY";
		}
		if (state == 4)
		{
			return "LONG AWAY";
		}
		if (state == 5)
		{
			return "ONLINE/TRADING";
		}
		if (state == 6)
		{
			return "WAITING OT PLAY";
		}
		return "N/A";
	}
}
