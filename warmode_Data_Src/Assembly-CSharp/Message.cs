using System;
using System.Collections.Generic;
using UnityEngine;

public class Message : MonoBehaviour
{
	public class ChatData
	{
		public string aname;

		public string msg;

		public int flag;

		public int ateam;

		public float time;

		public ChatData(int _flag, string _aname, string _msg, int _ateam)
		{
			this.aname = _aname;
			this.msg = _msg;
			this.flag = _flag;
			this.ateam = _ateam;
			this.time = Time.time;
		}

		~ChatData()
		{
		}
	}

	public class SystemData
	{
		public string msg;

		public SystemData(string _msg)
		{
			this.msg = _msg;
		}

		private SystemData()
		{
		}
	}

	public class DeathData
	{
		public string aname;

		public string vname;

		public string sStreak;

		public int wid;

		public int ateam;

		public int vteam;

		public int hitzone;

		public int streak;

		public int status;

		public Rect r;

		public Rect r2;

		public Rect rIcon;

		public Rect rIconBlack;

		public Rect rHS;

		public Rect rHSBlack;

		public Rect rBack;

		public string text1;

		public string text2;

		public DeathData(string _aname, string _vname, int _wid, int _ateam, int _vteam, int _hitzone, int _streak, int _status)
		{
			this.aname = _aname;
			this.vname = _vname;
			this.wid = _wid;
			this.ateam = _ateam;
			this.vteam = _vteam;
			this.hitzone = _hitzone;
			this.streak = _streak;
			this.sStreak = _streak.ToString();
			this.status = _status;
		}

		~DeathData()
		{
		}
	}

	private const float messagetime = 5f;

	private const float bombmessagetime = 4f;

	private static Texture2D tBlack;

	private static Texture2D tRed;

	private static Texture2D tHeadshot;

	private static Texture2D tGradient;

	private static Texture2D tWhite;

	private static Texture2D tEmptyBack;

	private static Texture2D tAttention;

	private static Rect rMessage;

	private static Rect rAttention;

	private static Rect rAttentionText;

	private static Rect rShortMessage;

	private static Rect rWarmupMessage;

	private static float tBombMessage;

	private static bool forcehide = false;

	public static int freezeTimeLeft = 0;

	private static List<Message.ChatData> chat = new List<Message.ChatData>();

	private static List<Message.SystemData> system = new List<Message.SystemData>();

	private static List<Message.DeathData> death = new List<Message.DeathData>();

	private static Color a = new Color(1f, 1f, 1f, 0.5f);

	private static Color ab = new Color(0f, 0f, 0f, 0.35f);

	private static Color ar = new Color(1f, 0f, 0f, 0.35f);

	private static string[] sColor = new string[]
	{
		"#F20",
		"#04F",
		"#FFF",
		"#F60"
	};

	private static string[] sMessage = new string[7];

	private static int currmessage = -1;

	public static bool blockchat = false;

	public static float lastcheck = 0f;

	public static float lastdeath = 0f;

	private static bool dead = false;

	public static string badge_name = string.Empty;

	private static string badge_lvl = string.Empty;

	private static string badge_clanname = string.Empty;

	private static int badge_back = 0;

	private static int badge_icon = 0;

	private static float badge_time = 0f;

	private static int badge_wid = 0;

	private static int badge_wid_custom = 0;

	public void PostAwake()
	{
		Message.tBlack = TEX.GetTextureByName("black");
		Message.tRed = TEX.GetTextureByName("red");
		Message.tWhite = TEX.GetTextureByName("white");
		Message.tHeadshot = TEX.GetTextureByName("headshot_icon");
		Message.tGradient = new Texture2D(64, 32);
		for (int i = 0; i < 64; i++)
		{
			for (int j = 0; j < 32; j++)
			{
				Message.tGradient.SetPixel(i, j, new Color(1f, 1f, 1f, (float)(64 - i) / 64f));
			}
		}
		Message.tGradient.filterMode = FilterMode.Point;
		Message.tGradient.Apply(true);
		Message.tEmptyBack = TEX.GetTextureByName("warbadge_hor_empty");
		this.OnResize();
		Message.chat.Clear();
		Message.system.Clear();
		Message.death.Clear();
		Message.sMessage[0] = Lang.Get("_FULLTEAM");
		Message.sMessage[1] = Lang.Get("_PREPARE");
		Message.sMessage[2] = Lang.Get("_МERCS_WIN");
		Message.sMessage[3] = Lang.Get("_WARCORPS_WIN");
		Message.sMessage[4] = Lang.Get("_DRAW");
		Message.sMessage[5] = Lang.Get("_TEAM_NOT_BALANCED");
		Message.sMessage[6] = Lang.Get("_WARMUP_ENDED");
		Message.HideBombPlantedMsg();
		Message.tAttention = TEX.GetTextureByName("BombMsg_01");
	}

	public void OnResize()
	{
		Message.rMessage = new Rect(40f, (float)(Screen.height - 100), (float)(Screen.width - 40), 20f);
		Message.rAttention = new Rect((float)Screen.width / 2f - GUI2.YRES(150f) / 2f, GUI2.YRES(60f), GUI2.YRES(150f), GUI2.YRES(50f));
		Message.rAttentionText = new Rect((float)Screen.width / 2f - GUI2.YRES(150f) / 2f + GUI2.YRES(52f), GUI2.YRES(60f), GUI2.YRES(150f), GUI2.YRES(50f));
		Message.rShortMessage = new Rect((float)Screen.width / 2f - GUI2.YRES(300f) / 2f, (float)Screen.height - GUI2.YRES(120f), GUI2.YRES(300f), GUI2.YRES(50f));
		Message.rWarmupMessage = new Rect((float)Screen.width / 2f - GUI2.YRES(300f) / 2f, GUI2.YRES(12f), GUI2.YRES(300f), GUI2.YRES(50f));
	}

	public static void DeathClear()
	{
		Message.death.Clear();
	}

	public static void Draw()
	{
		if (Message.forcehide)
		{
			return;
		}
		Message.DrawBadge();
		Message.DrawChat();
		Message.DrawSystem();
		Message.DrawDeath();
		Message.DrawMessage();
		Message.DrawBombMsg();
		Message.DrawFreezeTimeMsg();
		Message.DrawWarmupTimeMsg();
	}

	public static void ForceHide(bool val)
	{
		Message.forcehide = val;
	}

	public static void AddChat(int _flag, string _aname, string _msg, int _ateam)
	{
		if (Message.blockchat)
		{
			return;
		}
		Message.chat.Add(new Message.ChatData(_flag, _aname, _msg, _ateam));
		if (Message.chat.Count > 5)
		{
			Message.chat.RemoveAt(0);
		}
	}

	public static void AddSystem(string _msg)
	{
	}

	public static void AddDeath(int aid, int vid, string _aname, string _vname, int _wid, int _ateam, int _vteam, int _hitzone, int _streak, int _status)
	{
		if (vid == Client.ID)
		{
			Message.badge_lvl = PlayerControll.Player[aid].sLevel;
			Message.badge_name = PlayerControll.Player[aid].Name;
			Message.badge_clanname = PlayerControll.Player[aid].ClanName;
			Message.badge_back = PlayerControll.Player[aid].badge_back;
			Message.badge_icon = PlayerControll.Player[aid].badge_icon;
			Message.badge_wid = _wid;
			Message.badge_wid_custom = 0;
			if (PlayerControll.Player[aid] != null && PlayerControll.Player[aid].customWeapon[Message.badge_wid] > 0)
			{
				Message.badge_wid_custom = PlayerControll.Player[aid].currentWeapon[Message.badge_wid];
			}
		}
		if (SpecCam.show && SpecCam.mode == 1 && SpecCam.FID == aid)
		{
			Award.SetPoinsFake();
		}
		Message.death.Add(new Message.DeathData(_aname, _vname, _wid, _ateam, _vteam, _hitzone, _streak, _status));
		if (Message.death.Count > 5)
		{
			Message.death.RemoveAt(0);
		}
		Message.ResizeDeath();
		string text = string.Empty;
		string text2 = (_hitzone != 1) ? string.Empty : "a headshot from ";
		int num = _wid;
		if (num == 28)
		{
			num = 26;
		}
		if (num == 0)
		{
			text = text + _vname + " suicide";
		}
		else
		{
			string text3 = text;
			text = string.Concat(new string[]
			{
				text3,
				_aname,
				" killed ",
				_vname,
				" with ",
				text2,
				WeaponData.GetData(num).wName
			});
		}
		global::Console.cs.AddLogString(text);
	}

	public static void ResizeDeath()
	{
		if (Message.death.Count == 0)
		{
			return;
		}
		float x = GUI2.YRES(8f);
		float num = GUI2.YRES(4f) + GUI2.YRES(14f) * 4f;
		for (int i = Message.death.Count - 1; i >= 0; i--)
		{
			Message.death[i].r = new Rect(x, num, GUI2.YRES(300f), GUI2.YRES(20f));
			float width = GUI2.CalcSizeRes(Message.death[i].aname + Message.death[i].vname, 0, 12) + GUI2.YRES(70f);
			Message.death[i].rBack = new Rect(0f, Message.death[i].r.y + 6f, width, GUI2.YRES(12f));
			num -= GUI2.YRES(14f);
			Message.death[i].text1 = string.Concat(new string[]
			{
				"<color=",
				Message.sColor[Message.death[i].ateam],
				">",
				Message.death[i].aname,
				"</color>"
			});
			Message.death[i].text2 = string.Concat(new string[]
			{
				"<color=",
				Message.sColor[Message.death[i].vteam],
				">",
				Message.death[i].vname,
				"</color>"
			});
			float num2 = GUI2.CalcSizeRes(Message.death[i].text1, 0, 12);
			if ((Message.death[i].wid >= 0 && Message.death[i].wid <= 5) || Message.death[i].wid == 27)
			{
				Message.death[i].rIcon = new Rect(Message.death[i].r.x + num2, Message.death[i].r.y + GUI2.YRES(4f), GUI2.YRES(24f), GUI2.YRES(12f));
			}
			else
			{
				Message.death[i].rIcon = new Rect(Message.death[i].r.x + num2 + GUI2.YRES(2f), Message.death[i].r.y, GUI2.YRES(44f), GUI2.YRES(22f));
			}
			float num3 = GUI2.CalcSizeRes(Message.death[i].text2, 0, 12);
			Message.death[i].rIconBlack = new Rect(Message.death[i].rIcon.x + 1f, Message.death[i].rIcon.y + 1f, Message.death[i].rIcon.width, Message.death[i].rIcon.height);
			float num4 = 0f;
			if (Message.death[i].hitzone == 1)
			{
				Message.death[i].rHSBlack = new Rect(Message.death[i].rIcon.x + Message.death[i].rIcon.width + GUI2.YRES(4f) + 1f, Message.death[i].r.y + GUI2.YRES(2f) + 1f, GUI2.YRES(16f), GUI2.YRES(16f));
				Message.death[i].rHS = new Rect(Message.death[i].rIcon.x + Message.death[i].rIcon.width + GUI2.YRES(4f), Message.death[i].r.y + GUI2.YRES(2f), GUI2.YRES(16f), GUI2.YRES(16f));
				num4 = GUI2.YRES(20f);
			}
			Message.death[i].r2 = new Rect(Message.death[i].rIcon.x + Message.death[i].rIcon.width + GUI2.YRES(4f) + num4, Message.death[i].r.y, Message.death[i].r.width, Message.death[i].r.height);
		}
	}

	private static void DrawChat()
	{
		if (Message.chat.Count == 0)
		{
			return;
		}
		if (Time.time > Message.lastcheck)
		{
			Message.lastcheck = Time.time + 0.5f;
			for (int i = 0; i < Message.chat.Count; i++)
			{
				if (Time.time > Message.chat[i].time + 5f)
				{
					Message.chat.RemoveAt(0);
				}
			}
		}
		float x = GUI2.YRES(8f);
		float num = (float)Screen.height - GUI2.YRES(90f);
		for (int j = Message.chat.Count - 1; j >= 0; j--)
		{
			Rect r = new Rect(x, num, GUI2.YRES(460f), GUI2.YRES(20f));
			num -= GUI2.YRES(14f);
			if (Message.chat[j].flag == 0)
			{
				GUI2.DrawTextColorRes(r, string.Concat(new string[]
				{
					"<color=",
					Message.sColor[Message.chat[j].ateam],
					">",
					Message.chat[j].aname,
					"</color> : ",
					Message.chat[j].msg
				}), TextAnchor.MiddleLeft, _Color.White, 0, 12, true);
			}
			else if (Message.chat[j].flag == 1)
			{
				GUI2.DrawTextColorRes(r, string.Concat(new string[]
				{
					"<color=",
					Message.sColor[Message.chat[j].ateam],
					">",
					Message.chat[j].aname,
					"</color> (команде) : ",
					Message.chat[j].msg
				}), TextAnchor.MiddleLeft, _Color.White, 0, 12, true);
			}
			else if (Message.chat[j].flag == 2)
			{
				GUI2.DrawTextColorRes(r, "<color=" + Message.sColor[3] + ">SERVER</color> : " + Message.chat[j].msg, TextAnchor.MiddleLeft, _Color.White, 0, 12, true);
			}
		}
	}

	private static void DrawSystem()
	{
		if (Message.system.Count == 0)
		{
			return;
		}
		GUI.color = Message.a;
		float x = (float)Screen.width - GUI2.YRES(308f);
		float num = GUI2.YRES(4f) + GUI2.YRES(14f) * 4f;
		for (int i = 0; i < Message.system.Count; i++)
		{
			Rect r = new Rect(x, num, GUI2.YRES(300f), GUI2.YRES(20f));
			num -= GUI2.YRES(14f);
			GUI2.DrawTextRes(r, Message.system[i].msg, TextAnchor.MiddleRight, _Color.White, 0, 12, true);
		}
		GUI.color = Color.white;
	}

	private static void DrawDeath()
	{
		if (Message.death.Count == 0)
		{
			Message.lastdeath = Time.time + 3f;
			return;
		}
		if (Time.time > Message.lastdeath)
		{
			Message.lastdeath = Time.time + 5f;
			Message.death.RemoveAt(0);
		}
		for (int i = Message.death.Count - 1; i >= 0; i--)
		{
			if (Message.death[i].status == 1)
			{
				GUI.color = Message.ab;
			}
			else if (Message.death[i].status == 2)
			{
				GUI.color = Message.ar;
			}
			if (Message.death[i].status == 1 || Message.death[i].status == 2)
			{
				GUI.DrawTexture(Message.death[i].rBack, Message.tGradient);
			}
			GUI2.DrawTextColorRes(Message.death[i].r, Message.death[i].text1, TextAnchor.MiddleLeft, _Color.White, 0, 12, true);
			GUI.color = Color.black;
			GUI.DrawTexture(Message.death[i].rIconBlack, WeaponData.GetData(Message.death[i].wid).icon2);
			GUI.color = Color.white;
			GUI.DrawTexture(Message.death[i].rIcon, WeaponData.GetData(Message.death[i].wid).icon2);
			if (Message.death[i].wid != 0)
			{
				if (Message.death[i].hitzone == 1)
				{
					GUI.color = Color.black;
					GUI.DrawTexture(Message.death[i].rHSBlack, Message.tHeadshot);
					GUI.color = Color.white;
					GUI.DrawTexture(Message.death[i].rHS, Message.tHeadshot);
				}
				GUI2.DrawTextColorRes(Message.death[i].r2, Message.death[i].text2, TextAnchor.MiddleLeft, _Color.White, 0, 12, true);
			}
		}
	}

	private static void DrawMessage()
	{
		if (Message.currmessage < 0)
		{
			return;
		}
		Rect rect = new Rect(0f, (float)Screen.height - GUI2.YRES(124f), (float)Screen.width, GUI2.YRES(20f));
		GUI.color = new Color(1f, 1f, 1f, 0.75f);
		GUI.DrawTexture(rect, Message.tBlack);
		GUI.color = Color.white;
		string text = Message.sMessage[Message.currmessage];
		if (ScoreBoard.gamemode == 3)
		{
			if (Message.currmessage == 2)
			{
				text = Lang.Get("_ZOMBIE_WIN");
			}
			if (Message.currmessage == 3)
			{
				text = Lang.Get("_PEOPLE_WIN");
			}
		}
		GUI2.DrawTextRes(rect, text, TextAnchor.MiddleCenter, _Color.White, 0, 12, true);
	}

	public static void SetMessage(int msgid)
	{
		Message.currmessage = msgid;
		if (Message.currmessage == 0 || Message.currmessage == 5)
		{
			ChooseTeam.SetActive(true);
		}
	}

	public static void ResetMessage()
	{
		Message.currmessage = -1;
	}

	public static void SetDead(bool val)
	{
		Message.dead = val;
		if (!val)
		{
			Message.badge_name = string.Empty;
		}
		if (val)
		{
			Message.badge_time = Time.time;
		}
	}

	public static void DrawBadge()
	{
		if (!Message.dead)
		{
			return;
		}
		if (Message.badge_name == string.Empty)
		{
			return;
		}
		GUI.color = new Color(0f, 0f, 0f, 0.75f);
		GUI.DrawTexture(new Rect(0f, 0f, (float)Screen.width, GUI2.YRES(80f)), Message.tBlack);
		GUI.DrawTexture(new Rect(0f, (float)Screen.height - GUI2.YRES(102f), (float)Screen.width, GUI2.YRES(102f)), Message.tBlack);
		GUI.color = Color.white;
		int num = Message.badge_back;
		int num2 = Message.badge_icon;
		Rect r = new Rect(((float)Screen.width - GUI2.YRES(256f)) / 2f, GUI2.YRES(380f), GUI2.YRES(256f), GUI2.YRES(20f));
		float num3 = (Time.time - Message.badge_time) * 10f * GUI2.YRES(64f);
		if (Message.badge_wid > 0)
		{
			if (num3 < GUI2.YRES(63f))
			{
				GUI.color = Color.red;
			}
			Texture2D icon;
			if (Message.badge_wid_custom > 0)
			{
				icon = MenuShop.shopdata[Message.badge_wid_custom].icon;
			}
			else
			{
				icon = WeaponData.GetData(Message.badge_wid).icon;
			}
			if (icon)
			{
				if (icon.width == icon.height)
				{
					GUI.DrawTexture(new Rect(((float)Screen.width - GUI2.YRES(256f)) / 2f + GUI2.YRES(272f), GUI2.YRES(400f) - GUI2.YRES(25f), GUI2.YRES(100f), GUI2.YRES(100f)), icon);
				}
				else
				{
					GUI.DrawTexture(new Rect(((float)Screen.width - GUI2.YRES(256f)) / 2f + GUI2.YRES(272f), GUI2.YRES(400f), GUI2.YRES(100f), GUI2.YRES(50f)), icon);
				}
				Rect position = new Rect(((float)Screen.width - GUI2.YRES(256f)) / 2f + GUI2.YRES(272f), GUI2.YRES(400f) + GUI2.YRES(50f), GUI2.YRES(128f), GUI2.YRES(14f));
				GUI.color = Color.black;
				GUI.DrawTexture(position, Message.tGradient);
				GUI.color = Color.white;
				if (Message.badge_wid_custom > 0)
				{
					GUI2.DrawTextRes(new Rect(position.x + GUI2.YRES(4f), position.y, position.width, position.height), MenuShop.shopdata[Message.badge_wid_custom].name, TextAnchor.MiddleLeft, _Color.White, 0, 10, true);
				}
				else
				{
					GUI2.DrawTextRes(new Rect(position.x + GUI2.YRES(4f), position.y, position.width, position.height), WeaponData.GetData(Message.badge_wid).wName, TextAnchor.MiddleLeft, _Color.White, 0, 10, true);
				}
			}
			GUI.color = Color.white;
		}
		GUI2.DrawTextRes(r, Lang.Get("_YOU_KILLED_BY"), TextAnchor.MiddleCenter, _Color.White, 0, 14, true);
		if (num3 > GUI2.YRES(64f))
		{
			num3 = GUI2.YRES(64f);
		}
		if (num3 < GUI2.YRES(63f))
		{
			GUI.DrawTexture(new Rect(((float)Screen.width - GUI2.YRES(256f)) / 2f, GUI2.YRES(400f) + (GUI2.YRES(64f) - num3) / 2f, GUI2.YRES(256f), num3), Message.tWhite);
			return;
		}
		Rect position2 = new Rect(((float)Screen.width - GUI2.YRES(256f)) / 2f, GUI2.YRES(400f), GUI2.YRES(256f), GUI2.YRES(64f));
		Rect position3 = new Rect(((float)Screen.width - GUI2.YRES(256f)) / 2f + GUI2.YRES(8f), GUI2.YRES(400f), GUI2.YRES(64f), GUI2.YRES(64f));
		if (num > 0)
		{
			GUI.DrawTexture(position2, MenuShop.shopdata[num].icon);
		}
		else
		{
			GUI.DrawTexture(position2, Message.tEmptyBack);
		}
		if (num2 > 0)
		{
			GUI.DrawTexture(position3, MenuShop.shopdata[num2].icon);
		}
		Rect rect = new Rect(position3.x + GUI2.YRES(68f), position3.y + GUI2.YRES(14f), GUI2.YRES(32f), GUI2.YRES(16f));
		Rect position4 = new Rect(position3.x + GUI2.YRES(68f) + GUI2.YRES(32f), position3.y + GUI2.YRES(14f), GUI2.YRES(128f), GUI2.YRES(16f));
		Rect r2 = new Rect(position3.x + GUI2.YRES(68f) + GUI2.YRES(32f) + GUI2.YRES(4f), position3.y + GUI2.YRES(14f), GUI2.YRES(128f), GUI2.YRES(16f));
		Rect rect2 = new Rect(rect.x, rect.y + GUI2.YRES(20f), GUI2.YRES(160f), GUI2.YRES(16f));
		GUI.DrawTexture(rect, Message.tWhite);
		GUI.color = new Color(0f, 0f, 0f, 0.75f);
		GUI.DrawTexture(position4, Message.tBlack);
		GUI.color = Color.white;
		GUI2.DrawTextRes(rect, "Lv." + Message.badge_lvl, TextAnchor.MiddleCenter, _Color.Blue, 0, 12, false);
		GUI2.DrawTextRes(r2, Message.badge_name, TextAnchor.MiddleLeft, _Color.White, 0, 12, true);
		if (Message.badge_clanname != string.Empty)
		{
			GUI.color = new Color(0f, 0f, 0f, 0.75f);
			GUI.DrawTexture(rect2, Message.tBlack);
			GUI.color = Color.white;
			GUI2.DrawTextRes(rect2, Message.badge_clanname, TextAnchor.MiddleCenter, _Color.White, 0, 12, true);
		}
	}

	private static void DrawFreezeTimeMsg()
	{
		if (BuyMenu.isActive() || ChooseTeam.show)
		{
			return;
		}
		if (Message.freezeTimeLeft == 0)
		{
			return;
		}
		GUI.color = Color.white;
		GUI2.DrawTextRes(Message.rShortMessage, "Freeze time (sec): " + Message.freezeTimeLeft + "...", TextAnchor.MiddleCenter, _Color.White, 0, 12, true);
	}

	private static void DrawWarmupTimeMsg()
	{
		if (GameData.restartroundmode != 1)
		{
			return;
		}
		GUI.color = Color.white;
		GUI2.DrawTextRes(Message.rWarmupMessage, Lang.Get("_WARMUP_TIME"), TextAnchor.MiddleCenter, _Color.White, 0, 12, true);
	}

	public static void ShowBombPlantedMsg()
	{
		Message.tBombMessage = Time.time;
	}

	public static void HideBombPlantedMsg()
	{
		Message.tBombMessage = Time.time - 4f - 1f;
	}

	private static void DrawBombMsg()
	{
		if (Message.tBombMessage + 4f < Time.time)
		{
			return;
		}
		GUI.color = new Color(1f, 1f, 1f, 0.8f);
		GUI.DrawTexture(Message.rAttention, Message.tAttention);
		GUI.color = Color.white;
		GUI2.DrawTextRes(Message.rAttentionText, "Бомба заложена.\r\nВзрыв через 40 сек.", TextAnchor.MiddleLeft, _Color.White, 0, 9, true);
	}

	private void Update()
	{
		Message.freezeTimeLeft = GameData.freezeTime - (GameData.roundTimeLimit - ScoreTop.TimeLeft);
		if (Message.freezeTimeLeft < 0)
		{
			Message.freezeTimeLeft = 0;
		}
		if (C4.isplanting || C4.isdiffusing)
		{
			return;
		}
		if (Message.freezeTimeLeft == 0)
		{
			BasePlayer.FreezePosition(false);
		}
		else
		{
			BasePlayer.FreezePosition(true);
		}
	}
}
