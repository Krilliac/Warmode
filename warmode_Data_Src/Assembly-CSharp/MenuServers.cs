using System;
using System.Collections.Generic;
using UnityEngine;

public class MenuServers : MonoBehaviour
{
	public class CServerData
	{
		public int idx;

		public int players;

		public int port;

		public int gamemode;

		public int rate;

		public int privatemode;

		public int channel;

		public string mapname;

		public string ip;

		public string ping;

		public static int maxchannel;

		public CServerData(string ip, int port, int channel = -1, int priv = 0, string ping = "")
		{
			this.ip = ip;
			this.players = 0;
			this.port = port;
			this.privatemode = priv;
			this.channel = channel;
			this.idx = MenuServers.server.Count;
			this.ping = ping;
			if (channel > MenuServers.CServerData.maxchannel)
			{
				MenuServers.CServerData.maxchannel = channel;
			}
			if (MenuServers.CServerData.maxchannel > 32)
			{
				MenuServers.CServerData.maxchannel = 32;
			}
		}

		~CServerData()
		{
		}
	}

	private static bool show = false;

	private static float showtime = 0f;

	public static List<MenuServers.CServerData> server = new List<MenuServers.CServerData>();

	private static MenuServers.CServerData currServer = null;

	private static float[] RefreshTime = new float[33];

	private static Texture2D tBlack;

	private static Texture2D tOrange;

	private static Texture2D tGray;

	private static Texture2D tWhite;

	private static Texture2D tBlue;

	private static Rect rBack;

	private static Rect rBackChannel;

	private static Rect rBackPlay;

	private static Rect rButtonPlay;

	private static Rect rButtonRefresh;

	private static Rect rBackMode;

	private static Rect[] rButtonMode = new Rect[4];

	private static int currMode = -1;

	private static int currChannel = 0;

	private static Vector2 scroll = Vector2.zero;

	public void LoadEnd()
	{
		MenuServers.tBlack = TEX.GetTextureByName("black");
		MenuServers.tOrange = TEX.GetTextureByName("orange");
		MenuServers.tGray = TEX.GetTextureByName("gray");
		MenuServers.tWhite = TEX.GetTextureByName("white");
		MenuServers.tBlue = TEX.GetTextureByName("lightblue");
	}

	public void PostAwake()
	{
		MenuServers.show = false;
		MenuServers.RefreshTime[0] = -10f;
		MenuServers.RefreshTime[1] = -10f;
		MenuServers.RefreshTime[2] = -10f;
		MenuServers.server.Clear();
		MenuServers.CServerData.maxchannel = -1;
	}

	public void OnResize()
	{
		MenuServers.rBack = new Rect((float)Screen.width / 2f - GUIM.YRES(185f), GUIM.YRES(80f), GUIM.YRES(420f), GUIM.YRES(565f));
		MenuServers.rBackChannel = new Rect(MenuServers.rBack.x + MenuServers.rBack.width + GUIM.YRES(12f), MenuServers.rBack.y, GUIM.YRES(200f), GUIM.YRES(28f) * (float)(MenuServers.CServerData.maxchannel + 1) + GUIM.YRES(4f));
		MenuServers.rBackPlay = new Rect(MenuServers.rBack.x + MenuServers.rBack.width + GUIM.YRES(12f), MenuServers.rBack.y + MenuServers.rBack.height - GUIM.YRES(60f), GUIM.YRES(200f), GUIM.YRES(60f));
		MenuServers.rButtonPlay = new Rect(MenuServers.rBackPlay.x + GUIM.YRES(4f), MenuServers.rBackPlay.y + GUIM.YRES(32f), MenuServers.rBackPlay.width - GUIM.YRES(8f), GUIM.YRES(24f));
		MenuServers.rButtonRefresh = new Rect(MenuServers.rBackPlay.x + GUIM.YRES(4f), MenuServers.rBackPlay.y + GUIM.YRES(4f), MenuServers.rBackPlay.width - GUIM.YRES(8f), GUIM.YRES(24f));
		MenuServers.rBackMode = new Rect(MenuServers.rBackChannel.x, MenuServers.rBackChannel.y + MenuServers.rBackChannel.height + GUIM.YRES(28f), MenuServers.rBackChannel.width, GUIM.YRES(28f) * 3f + GUIM.YRES(4f));
		MenuServers.rButtonMode[0] = new Rect(MenuServers.rBackChannel.x + GUIM.YRES(4f), MenuServers.rBackChannel.y + MenuServers.rBackChannel.height + GUIM.YRES(28f) + GUIM.YRES(4f), MenuServers.rBackChannel.width - GUIM.YRES(8f), GUIM.YRES(24f));
		MenuServers.rButtonMode[1] = new Rect(MenuServers.rBackChannel.x + GUIM.YRES(4f), MenuServers.rBackChannel.y + MenuServers.rBackChannel.height + GUIM.YRES(28f) * 2f + GUIM.YRES(4f), MenuServers.rBackChannel.width - GUIM.YRES(8f), GUIM.YRES(24f));
		MenuServers.rButtonMode[2] = new Rect(MenuServers.rBackChannel.x + GUIM.YRES(4f), MenuServers.rBackChannel.y + MenuServers.rBackChannel.height + GUIM.YRES(28f) * 3f + GUIM.YRES(4f), MenuServers.rBackChannel.width - GUIM.YRES(8f), GUIM.YRES(24f));
		MenuServers.rButtonMode[3] = new Rect(MenuServers.rBackChannel.x + GUIM.YRES(4f), MenuServers.rBackChannel.y + MenuServers.rBackChannel.height + GUIM.YRES(28f) * 4f + GUIM.YRES(4f), MenuServers.rBackChannel.width - GUIM.YRES(8f), GUIM.YRES(24f));
	}

	public static void SetActive(bool val)
	{
		MenuServers.show = val;
		if (val)
		{
			MenuServers.showtime = Time.time;
			MenuServers.currChannel = 0;
			MenuShop.GenerateCustomIcons();
			MenuServers.currMode = -1;
			MenuServers.Refresh(MenuServers.currChannel);
		}
	}

	public static void Draw()
	{
		if (!MenuServers.show)
		{
			return;
		}
		float num = Time.time - MenuServers.showtime + 0.001f;
		if (num > 0.05f)
		{
			num = 0.05f;
		}
		num *= 20f;
		Matrix4x4 matrix = GUI.matrix;
		Vector3 s = new Vector3(num, num, 1f);
		Vector3 pos = new Vector3(MenuServers.rBack.center.x - MenuServers.rBack.center.x * num, MenuServers.rBack.center.y - MenuServers.rBack.center.y * num, 1f);
		GUI.matrix = Matrix4x4.TRS(pos, Quaternion.identity, s);
		GUIM.DrawBox(MenuServers.rBack, MenuServers.tBlack);
		Rect rect = new Rect(MenuServers.rBack.x + GUIM.YRES(4f), MenuServers.rBack.y + GUIM.YRES(4f), GUIM.YRES(40f), GUIM.YRES(24f));
		GUI.DrawTexture(rect, MenuServers.tGray);
		GUIM.DrawText(rect, "#", TextAnchor.MiddleCenter, BaseColor.White, 1, 12, false);
		rect.x = rect.x + GUIM.YRES(2f) + rect.width;
		rect.width = GUIM.YRES(120f);
		GUI.DrawTexture(rect, MenuServers.tGray);
		GUIM.DrawText(rect, Lang.Get("_MODE"), TextAnchor.MiddleCenter, BaseColor.White, 1, 12, false);
		rect.x = rect.x + GUIM.YRES(2f) + rect.width;
		rect.width = GUIM.YRES(104f);
		GUI.DrawTexture(rect, MenuServers.tGray);
		GUIM.DrawText(rect, Lang.Get("_MAP"), TextAnchor.MiddleCenter, BaseColor.White, 1, 12, false);
		rect.x = rect.x + GUIM.YRES(2f) + rect.width;
		rect.width = GUIM.YRES(60f);
		GUI.DrawTexture(rect, MenuServers.tGray);
		GUIM.DrawText(rect, Lang.Get("_RATE"), TextAnchor.MiddleCenter, BaseColor.White, 1, 12, false);
		rect.x = rect.x + GUIM.YRES(2f) + rect.width;
		rect.width = GUIM.YRES(80f);
		GUI.DrawTexture(rect, MenuServers.tGray);
		GUIM.DrawText(rect, Lang.Get("_PLAYERS"), TextAnchor.MiddleCenter, BaseColor.White, 1, 12, false);
		int num2 = 0;
		for (int i = 0; i < MenuServers.server.Count; i++)
		{
			if (MenuServers.server[i].channel == MenuServers.currChannel)
			{
				num2++;
			}
		}
		MenuServers.scroll = GUIM.BeginScrollView(new Rect(MenuServers.rBack.x + GUIM.YRES(4f), MenuServers.rBack.y + GUIM.YRES(32f), MenuServers.rBack.width - GUIM.YRES(8f), MenuServers.rBack.height - GUIM.YRES(40f)), MenuServers.scroll, new Rect(0f, 0f, 0f, (float)num2 * GUIM.YRES(26f)));
		int num3 = 0;
		for (int j = 16; j >= 0; j--)
		{
			for (int k = 0; k < MenuServers.server.Count; k++)
			{
				if (MenuServers.currMode != 0 || MenuServers.server[k].gamemode == 0)
				{
					if (MenuServers.currMode != 1 || MenuServers.server[k].gamemode == 1)
					{
						if (MenuServers.currMode != 2 || MenuServers.server[k].gamemode == 2)
						{
							if (MenuServers.currMode != 3 || MenuServers.server[k].gamemode == 3)
							{
								if (MenuServers.server[k].channel == MenuServers.currChannel)
								{
									if (MenuServers.server[k].players == j)
									{
										if (MenuServers.DrawButtonServer(MenuServers.server[k], num3))
										{
											MenuServers.currServer = MenuServers.server[k];
										}
										num3++;
									}
								}
							}
						}
					}
				}
			}
		}
		GUIM.EndScrollView();
		MenuServers.rBackChannel = new Rect(MenuServers.rBack.x + MenuServers.rBack.width + GUIM.YRES(12f), MenuServers.rBack.y, GUIM.YRES(200f), GUIM.YRES(28f) * (float)(MenuServers.CServerData.maxchannel + 1) + GUIM.YRES(4f));
		GUIM.DrawBox(MenuServers.rBackChannel, MenuServers.tBlack);
		GUIM.DrawBox(MenuServers.rBackPlay, MenuServers.tBlack);
		for (int l = 0; l < MenuServers.CServerData.maxchannel + 1; l++)
		{
			MenuServers.DrawButtonChannel(l, new Rect(MenuServers.rBackChannel.x + GUIM.YRES(4f), MenuServers.rBackChannel.y + GUIM.YRES(4f) + (float)l * GUIM.YRES(28f), MenuServers.rBackChannel.width - GUIM.YRES(8f), GUIM.YRES(24f)), Lang.Get("_CHANNEL") + " " + l.ToString("00"));
		}
		MenuServers.rBackMode = new Rect(MenuServers.rBackChannel.x, MenuServers.rBackChannel.y + MenuServers.rBackChannel.height + GUIM.YRES(28f), MenuServers.rBackChannel.width, GUIM.YRES(28f) * 4f + GUIM.YRES(4f));
		MenuServers.rButtonMode[0] = new Rect(MenuServers.rBackChannel.x + GUIM.YRES(4f), MenuServers.rBackChannel.y + MenuServers.rBackChannel.height + GUIM.YRES(28f) + GUIM.YRES(4f), MenuServers.rBackChannel.width - GUIM.YRES(8f), GUIM.YRES(24f));
		MenuServers.rButtonMode[1] = new Rect(MenuServers.rBackChannel.x + GUIM.YRES(4f), MenuServers.rBackChannel.y + MenuServers.rBackChannel.height + GUIM.YRES(28f) * 2f + GUIM.YRES(4f), MenuServers.rBackChannel.width - GUIM.YRES(8f), GUIM.YRES(24f));
		MenuServers.rButtonMode[2] = new Rect(MenuServers.rBackChannel.x + GUIM.YRES(4f), MenuServers.rBackChannel.y + MenuServers.rBackChannel.height + GUIM.YRES(28f) * 3f + GUIM.YRES(4f), MenuServers.rBackChannel.width - GUIM.YRES(8f), GUIM.YRES(24f));
		MenuServers.rButtonMode[3] = new Rect(MenuServers.rBackChannel.x + GUIM.YRES(4f), MenuServers.rBackChannel.y + MenuServers.rBackChannel.height + GUIM.YRES(28f) * 4f + GUIM.YRES(4f), MenuServers.rBackChannel.width - GUIM.YRES(8f), GUIM.YRES(24f));
		GUIM.DrawBox(MenuServers.rBackMode, MenuServers.tBlack);
		MenuServers.DrawButtonMode(0, MenuServers.rButtonMode[0], Lang.Get("_DEATHMATCH"));
		MenuServers.DrawButtonMode(1, MenuServers.rButtonMode[1], Lang.Get("_CONFRONTATION"));
		MenuServers.DrawButtonMode(2, MenuServers.rButtonMode[2], Lang.Get("_DETONATION"));
		MenuServers.DrawButtonMode(3, MenuServers.rButtonMode[3], Lang.Get("_ZOMBIEMATCH"));
		GUI.DrawTexture(MenuServers.rButtonRefresh, MenuServers.tGray);
		float num4 = MenuServers.RefreshTime[MenuServers.currChannel] + 5f - Time.time;
		string str = string.Empty;
		if (num4 >= 0f)
		{
			str = " " + num4.ToString("0.00");
		}
		if (GUIM.Button(MenuServers.rButtonRefresh, BaseColor.Gray, Lang.Get("_REFRESH") + str, TextAnchor.MiddleCenter, BaseColor.White, 1, 12, false))
		{
			MenuServers.Refresh(MenuServers.currChannel);
		}
		bool flag = false;
		if (MenuServers.currServer != null)
		{
			flag = GUIM.Button(MenuServers.rButtonPlay, BaseColor.Orange, Lang.Get("_CONNECT"), TextAnchor.MiddleCenter, BaseColor.White, 1, 12, true);
		}
		else
		{
			GUIM.Button(MenuServers.rButtonPlay, BaseColor.Black, Lang.Get("_CONNECT"), TextAnchor.MiddleCenter, BaseColor.White, 1, 12, true);
		}
		if (flag)
		{
			PlayerPrefs.SetInt("localplay", 0);
			PlayerPrefs.SetString("autostart", "connect " + MenuServers.currServer.ip + " " + MenuServers.currServer.port.ToString());
			Application.LoadLevel("game");
		}
		GUI.matrix = matrix;
	}

	private static void DrawButtonChannel(int channel, Rect r, string name)
	{
		bool flag;
		if (MenuServers.currChannel == channel)
		{
			flag = GUIM.Button(r, BaseColor.White, name, TextAnchor.MiddleCenter, BaseColor.Blue, 1, 12, false);
		}
		else
		{
			flag = GUIM.Button(r, BaseColor.Gray, name, TextAnchor.MiddleCenter, BaseColor.White, 1, 12, false);
		}
		if (flag)
		{
			MenuServers.currChannel = channel;
			MenuServers.currMode = -1;
			MenuServers.Refresh(channel);
		}
	}

	private static void DrawButtonMode(int mode, Rect r, string name)
	{
		bool flag;
		if (MenuServers.currMode == mode)
		{
			flag = GUIM.Button(r, BaseColor.White, name, TextAnchor.MiddleCenter, BaseColor.Orange, 1, 12, false);
		}
		else
		{
			flag = GUIM.Button(r, BaseColor.Gray, name, TextAnchor.MiddleCenter, BaseColor.White, 1, 12, false);
		}
		if (flag)
		{
			MenuServers.currMode = mode;
		}
	}

	public static void Refresh(int channel)
	{
		if (Time.time < MenuServers.RefreshTime[channel] + 5f)
		{
			return;
		}
		MenuServers.RefreshTime[channel] = Time.time;
		for (int i = 0; i < MenuServers.server.Count; i++)
		{
			if (MenuServers.server[i].channel == channel)
			{
				UDPClient.cs.send_serverinfo(MenuServers.server[i].ip, MenuServers.server[i].port);
			}
		}
	}

	public static void UpdateServer(int port, int players, int gamemode, string mapname, int rate, float pingTime)
	{
		for (int i = 0; i < MenuServers.server.Count; i++)
		{
			if (MenuServers.server[i].port == port)
			{
				MenuServers.server[i].players = players;
				MenuServers.server[i].gamemode = gamemode;
				MenuServers.server[i].mapname = mapname;
				MenuServers.server[i].rate = rate;
				MenuServers.server[i].ping = (pingTime * 1000f).ToString("#");
				break;
			}
		}
	}

	private static bool DrawButtonServer(MenuServers.CServerData s, int pos)
	{
		Texture2D image = MenuServers.tGray;
		BaseColor fontcolor = BaseColor.LightGray;
		if (s.players == 16)
		{
			fontcolor = BaseColor.Red;
		}
		else if (s.players > 0)
		{
			fontcolor = BaseColor.Yellow;
		}
		if (s == MenuServers.currServer)
		{
			image = MenuServers.tWhite;
			if (s.players != 16)
			{
				fontcolor = BaseColor.Blue;
			}
		}
		string name = GameMode.GetName(s.gamemode);
		string text = s.mapname;
		string clearName = s.mapname;
		if (text != null)
		{
			string[] array = text.Split(new char[]
			{
				'v'
			});
			clearName = array[0];
		}
		text = MapData.GetName(clearName);
		Rect rect = new Rect(0f, GUIM.YRES((float)(1 * pos * 26)), GUIM.YRES(40f), GUIM.YRES(24f));
		Color white = new Color(1f, 1f, 1f, 0.5f);
		if (pos % 2 == 0)
		{
			white = new Color(1f, 1f, 1f, 0.35f);
		}
		if (s == MenuServers.currServer)
		{
			white = Color.white;
		}
		GUI.color = white;
		GUI.DrawTexture(rect, image);
		GUI.color = Color.white;
		GUIM.DrawText(rect, (s.idx + 1).ToString(), TextAnchor.MiddleCenter, fontcolor, 1, 12, false);
		rect.x = rect.x + rect.width + GUIM.YRES(2f);
		rect.width = GUIM.YRES(120f);
		GUI.color = white;
		GUI.DrawTexture(rect, image);
		GUI.color = Color.white;
		GUIM.DrawText(rect, name, TextAnchor.MiddleCenter, fontcolor, 1, 12, false);
		rect.x = rect.x + rect.width + GUIM.YRES(2f);
		rect.width = GUIM.YRES(104f);
		GUI.color = white;
		GUI.DrawTexture(rect, image);
		GUI.color = Color.white;
		GUIM.DrawText(rect, text, TextAnchor.MiddleCenter, fontcolor, 1, 10, false);
		rect.x = rect.x + rect.width + GUIM.YRES(2f);
		rect.width = GUIM.YRES(60f);
		GUI.color = white;
		GUI.DrawTexture(rect, image);
		GUI.color = Color.white;
		GUIM.DrawText(rect, "x" + s.rate.ToString(), TextAnchor.MiddleCenter, fontcolor, 1, 12, false);
		rect.x = rect.x + rect.width + GUIM.YRES(2f);
		rect.width = GUIM.YRES(80f);
		GUI.color = white;
		GUI.DrawTexture(rect, image);
		GUI.color = Color.white;
		GUIM.DrawText(rect, s.players.ToString() + "/16", TextAnchor.MiddleCenter, fontcolor, 1, 12, false);
		rect.x = 0f;
		rect.width = GUIM.YRES(404f);
		return GUIM.HideButton(rect);
	}
}
