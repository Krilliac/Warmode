using System;
using UnityEngine;

public class ScoreBoard : MonoBehaviour
{
	private static Texture2D tWhite = null;

	private static Texture2D tBlack = null;

	private static Texture2D tGray0 = null;

	private static Texture2D tGray1 = null;

	private static Texture2D tGray2 = null;

	private static Texture2D tRed = null;

	private static Texture2D tBlue = null;

	private static Texture2D tClan = null;

	private static Texture2D tKills = null;

	private static Texture2D tDeaths = null;

	private static Texture2D tPoints = null;

	private static Texture2D tWeapon = null;

	private static Texture2D tMoney = null;

	private static Texture2D tBomb = null;

	private static Texture2D tLine = null;

	private static Rect[] rTeamBack = new Rect[2];

	private static Rect[] rTeamHeader = new Rect[2];

	private static Rect[] rTeamHeaderName = new Rect[2];

	private static Rect[] rTeamHeaderClan = new Rect[2];

	private static Rect[] rTeamHeaderKills = new Rect[2];

	private static Rect[] rTeamHeaderDeaths = new Rect[2];

	private static Rect[] rTeamHeaderPoints = new Rect[2];

	private static Rect rBonusHeader;

	private static Rect rBonusBody;

	private static Rect rWinnerHeader;

	private static Rect rWinnerBody;

	private static Texture2D tGP = null;

	private static Texture2D tExp = null;

	public static int winteam = 0;

	public static int wingp = 0;

	public static int winexp = 0;

	private static string[] teamname = new string[]
	{
		"MERCS",
		"CORPS"
	};

	private static int[] teamcolor = new int[2];

	private static bool show = false;

	private static bool showforce = false;

	private static int c = 0;

	private static string sGameMode = string.Empty;

	public static string sMap = string.Empty;

	public static int gamemode = 0;

	private static int[] teamscore = new int[2];

	private static string[] sTeamScore = new string[]
	{
		"0",
		"0"
	};

	private static Color ca = new Color(1f, 1f, 1f, 0.75f);

	private static int[] sorted = new int[16];

	public void PostAwake()
	{
		ScoreBoard.tWhite = TEX.GetTextureByName("white");
		ScoreBoard.tBlack = TEX.GetTextureByName("black");
		ScoreBoard.tGray0 = TEX.GetTextureByName("gray0");
		ScoreBoard.tGray1 = TEX.GetTextureByName("gray1");
		ScoreBoard.tGray2 = TEX.GetTextureByName("gray2");
		ScoreBoard.tRed = TEX.GetTextureByName("red");
		ScoreBoard.tBlue = TEX.GetTextureByName("blue");
		ScoreBoard.tClan = TEX.GetTextureByName("GUI/wartab_clan");
		ScoreBoard.tKills = TEX.GetTextureByName("GUI/wartab_kills");
		ScoreBoard.tDeaths = TEX.GetTextureByName("GUI/wartab_deaths");
		ScoreBoard.tPoints = TEX.GetTextureByName("GUI/wartab_points");
		ScoreBoard.tWeapon = TEX.GetTextureByName("GUI/wartab_guns");
		ScoreBoard.tMoney = TEX.GetTextureByName("GUI/wartab_money");
		ScoreBoard.tBomb = TEX.GetTextureByName("GUI/c4_orange");
		ScoreBoard.tGP = TEX.GetTextureByName("GUI/gp_64");
		ScoreBoard.tExp = TEX.GetTextureByName("GUI/wartab_exp");
		ScoreBoard.tLine = TEX.GetTextureByName("GUI/warline");
		ScoreBoard.teamcolor[0] = 2;
		ScoreBoard.teamcolor[1] = 4;
		ScoreBoard.show = false;
		ScoreBoard.showforce = false;
		this.OnResize();
	}

	private void OnResize()
	{
		ScoreBoard.ResizeRects();
	}

	private static void ResizeRects()
	{
		float num = GUI2.YRES(240f);
		float num2 = GUI2.YRES(180f);
		float num3 = GUI2.YRES(4f);
		float height = GUI2.YRES(20f);
		if (ScoreBoard.gamemode == 3)
		{
			num = GUI2.YRES(240f);
			num2 = GUI2.YRES(261f);
			num3 = GUI2.YRES(4f);
			height = GUI2.YRES(20f);
		}
		ScoreBoard.rTeamBack[0] = new Rect((float)Screen.width / 2f - num - num3, ((float)Screen.height - num2) / 2f, num, num2);
		ScoreBoard.rTeamBack[1] = new Rect((float)Screen.width / 2f + num3, ((float)Screen.height - num2) / 2f, num, num2);
		ScoreBoard.rTeamHeader[0] = new Rect(ScoreBoard.rTeamBack[0].x, ((float)Screen.height - num2) / 2f, num, height);
		ScoreBoard.rTeamHeader[1] = new Rect(ScoreBoard.rTeamBack[1].x, ((float)Screen.height - num2) / 2f, num, height);
		ScoreBoard.rTeamHeaderName[0] = new Rect(ScoreBoard.rTeamBack[0].x + num3, ((float)Screen.height - num2) / 2f, num, height);
		ScoreBoard.rTeamHeaderName[1] = new Rect(ScoreBoard.rTeamBack[1].x + num3, ((float)Screen.height - num2) / 2f, num, height);
		ScoreBoard.rTeamHeaderClan[0] = new Rect(ScoreBoard.rTeamBack[0].x + GUI2.YRES(90f), ((float)Screen.height - num2) / 2f + GUI2.YRES(3f), GUI2.YRES(14f), GUI2.YRES(14f));
		ScoreBoard.rTeamHeaderClan[1] = new Rect(ScoreBoard.rTeamBack[1].x + GUI2.YRES(90f), ((float)Screen.height - num2) / 2f + GUI2.YRES(3f), GUI2.YRES(14f), GUI2.YRES(14f));
		ScoreBoard.rTeamHeaderKills[0] = new Rect(ScoreBoard.rTeamBack[0].x + GUI2.YRES(140f), ((float)Screen.height - num2) / 2f + GUI2.YRES(4f), GUI2.YRES(12f), GUI2.YRES(12f));
		ScoreBoard.rTeamHeaderKills[1] = new Rect(ScoreBoard.rTeamBack[1].x + GUI2.YRES(140f), ((float)Screen.height - num2) / 2f + GUI2.YRES(4f), GUI2.YRES(12f), GUI2.YRES(12f));
		ScoreBoard.rTeamHeaderDeaths[0] = new Rect(ScoreBoard.rTeamBack[0].x + GUI2.YRES(175f), ((float)Screen.height - num2) / 2f + GUI2.YRES(4f), GUI2.YRES(12f), GUI2.YRES(12f));
		ScoreBoard.rTeamHeaderDeaths[1] = new Rect(ScoreBoard.rTeamBack[1].x + GUI2.YRES(175f), ((float)Screen.height - num2) / 2f + GUI2.YRES(4f), GUI2.YRES(12f), GUI2.YRES(12f));
		ScoreBoard.rTeamHeaderPoints[0] = new Rect(ScoreBoard.rTeamBack[0].x + GUI2.YRES(210f), ((float)Screen.height - num2) / 2f + GUI2.YRES(4f), GUI2.YRES(12f), GUI2.YRES(12f));
		ScoreBoard.rTeamHeaderPoints[1] = new Rect(ScoreBoard.rTeamBack[1].x + GUI2.YRES(210f), ((float)Screen.height - num2) / 2f + GUI2.YRES(4f), GUI2.YRES(12f), GUI2.YRES(12f));
		ScoreBoard.rBonusHeader = new Rect(((float)Screen.width - GUI2.YRES(240f)) / 2f, (float)Screen.height / 2f + GUI2.YRES(118f), num, height);
		ScoreBoard.rBonusBody = new Rect(ScoreBoard.rBonusHeader.x, ScoreBoard.rBonusHeader.y + GUI2.YRES(20f), num, GUI2.YRES(64f));
		if (ScoreBoard.gamemode == 3)
		{
			ScoreBoard.rBonusHeader = new Rect(((float)Screen.width - GUI2.YRES(240f)) / 2f, (float)Screen.height / 2f + GUI2.YRES(140f), num, height);
			ScoreBoard.rBonusBody = new Rect(ScoreBoard.rBonusHeader.x, ScoreBoard.rBonusHeader.y + GUI2.YRES(20f), num, GUI2.YRES(64f));
		}
		ScoreBoard.rWinnerHeader = new Rect(((float)Screen.width - GUI2.YRES(240f)) / 2f, (float)Screen.height / 2f - GUI2.YRES(180f), num, height);
		ScoreBoard.rWinnerBody = new Rect(ScoreBoard.rWinnerHeader.x, ScoreBoard.rWinnerHeader.y + GUI2.YRES(20f), num, GUI2.YRES(40f));
	}

	public static void Draw()
	{
		if (!ScoreBoard.show && !ScoreBoard.showforce)
		{
			return;
		}
		ScoreBoard.DrawTeam(0);
		ScoreBoard.DrawTeam(1);
		ScoreBoard.DrawMapData();
		if (ScoreBoard.gamemode > 0)
		{
			ScoreBoard.DrawRoundData();
		}
		int num = 0;
		GUI.color = ScoreBoard.ca;
		for (int i = 0; i < 16; i++)
		{
			if (PlayerControll.Player[i] != null)
			{
				if (PlayerControll.Player[i].Team == 255)
				{
					GUI2.DrawTextRes(new Rect(GUI2.YRES(8f), GUI2.YRES(380f) + (float)num * GUI2.YRES(15f), GUI2.YRES(200f), GUI2.YRES(14f)), PlayerControll.Player[i].Name, TextAnchor.LowerLeft, _Color.White, 0, 14, true);
					num++;
				}
			}
		}
		if (num > 0)
		{
			GUI2.DrawTextRes(new Rect(GUI2.YRES(8f), GUI2.YRES(380f) - GUI2.YRES(15f), GUI2.YRES(200f), GUI2.YRES(14f)), Lang.Get("_SPECTATORS") + ":", TextAnchor.LowerLeft, _Color.White, 0, 14, true);
		}
		ScoreBoard.DrawPlayerReward();
		GUI.color = Color.white;
	}

	private static void DrawTeam(int team)
	{
		float num = GUI2.YRES(240f);
		float num2 = GUI2.YRES(240f);
		float num3 = GUI2.YRES(4f);
		float num4 = GUI2.YRES(20f);
		if (ScoreBoard.gamemode == 3)
		{
			num = GUI2.YRES(240f);
			num2 = GUI2.YRES(256f);
			num3 = GUI2.YRES(4f);
			num4 = GUI2.YRES(15f);
		}
		GUI.color = new Color(1f, 1f, 1f, 0.75f);
		GUI.DrawTexture(ScoreBoard.rTeamBack[team], ScoreBoard.tGray1);
		GUI.color = Color.white;
		GUI.DrawTexture(ScoreBoard.rTeamHeader[team], ScoreBoard.tGray0);
		GUI2.DrawTextRes(ScoreBoard.rTeamHeaderName[team], ScoreBoard.teamname[team], TextAnchor.MiddleLeft, (_Color)ScoreBoard.teamcolor[team], 0, 12, true);
		GUI.DrawTexture(ScoreBoard.rTeamHeaderClan[team], ScoreBoard.tClan);
		GUI.DrawTexture(ScoreBoard.rTeamHeaderKills[team], ScoreBoard.tKills);
		GUI.DrawTexture(ScoreBoard.rTeamHeaderDeaths[team], ScoreBoard.tDeaths);
		GUI.DrawTexture(ScoreBoard.rTeamHeaderPoints[team], ScoreBoard.tPoints);
		float x = ScoreBoard.rTeamBack[team].x;
		float num5 = ScoreBoard.rTeamBack[team].y + num4;
		if (ScoreBoard.gamemode == 3)
		{
			num5 += GUI2.YRES(5f);
		}
		for (int i = 0; i < ScoreBoard.c; i++)
		{
			int num6 = ScoreBoard.sorted[i];
			if (PlayerControll.Player[num6] != null)
			{
				if (PlayerControll.Player[num6].Team == team)
				{
					ScoreBoard.DrawPlayerData(x, num5, num6);
					num5 += num4;
				}
			}
		}
	}

	private void DrawTeamAddon(int team)
	{
		float num = GUI2.YRES(79f);
		float num2 = GUI2.YRES(240f);
		float num3 = (float)team * ((float)Screen.width - num);
		float height = GUI2.YRES(20f);
		if (ScoreBoard.gamemode == 3)
		{
			height = GUI2.YRES(15f);
		}
		GUI.color = new Color(1f, 1f, 1f, 0.75f);
		GUI.DrawTexture(new Rect(num3, ((float)Screen.height - num2) / 2f, num, num2), ScoreBoard.tGray1);
		GUI.color = Color.white;
		GUI.DrawTexture(new Rect(num3, ((float)Screen.height - num2) / 2f, num, height), ScoreBoard.tGray0);
		GUI.DrawTexture(new Rect(num3 + GUI2.YRES(13f), ((float)Screen.height - num2) / 2f + GUI2.YRES(3f), GUI2.YRES(14f), GUI2.YRES(14f)), ScoreBoard.tMoney);
		GUI.DrawTexture(new Rect(num3 + GUI2.YRES(13f) + GUI2.YRES(40f), ((float)Screen.height - num2) / 2f + GUI2.YRES(3f), GUI2.YRES(14f), GUI2.YRES(14f)), ScoreBoard.tWeapon);
	}

	private static void DrawPlayerData(float x, float y, int id)
	{
		float height = GUI2.YRES(20f);
		if (ScoreBoard.gamemode == 3)
		{
			height = GUI2.YRES(15f);
		}
		if (id == Client.ID)
		{
			GUI.color = new Color(1f, 1f, 1f, 0.25f);
			GUI.DrawTexture(new Rect(x, y, GUI2.YRES(240f), height), ScoreBoard.tWhite);
			GUI.color = Color.white;
		}
		_Color fontcolor = _Color.White;
		if (PlayerControll.Player[id].DeadFlag != 0)
		{
			fontcolor = _Color.Gray;
		}
		GUI2.DrawTextRes(new Rect(x, y, GUI2.YRES(16f), height), PlayerControll.Player[id].sLevel, TextAnchor.MiddleCenter, _Color.Yellow, 0, 10, true);
		GUI2.DrawTextRes(new Rect(x + GUI2.YRES(18f), y, GUI2.YRES(70f), height), PlayerControll.Player[id].Name, TextAnchor.MiddleLeft, fontcolor, 0, 9, true);
		if (!PlayerControll.Player[id].bomb || BasePlayer.team == 0)
		{
		}
		GUI2.DrawTextRes(new Rect(x + GUI2.YRES(85f), y, GUI2.YRES(100f), height), PlayerControll.Player[id].ClanName, TextAnchor.MiddleLeft, _Color.Yellow, 0, 8, true);
		GUI2.DrawTextRes(new Rect(x + GUI2.YRES(92f), y, GUI2.YRES(100f), height), PlayerControll.Player[id].sFrags, TextAnchor.MiddleCenter, fontcolor, 0, 10, true);
		GUI2.DrawTextRes(new Rect(x + GUI2.YRES(126f), y, GUI2.YRES(100f), height), PlayerControll.Player[id].sDeaths, TextAnchor.MiddleCenter, fontcolor, 0, 10, true);
		GUI2.DrawTextRes(new Rect(x + GUI2.YRES(160f), y, GUI2.YRES(100f), height), PlayerControll.Player[id].sPoints, TextAnchor.MiddleCenter, fontcolor, 0, 10, true);
	}

	private void DrawPlayerDataAddon(int id)
	{
	}

	private static void DrawPlayerReward()
	{
		if (!ScoreBoard.showforce)
		{
			return;
		}
		GUI.DrawTexture(ScoreBoard.rBonusHeader, ScoreBoard.tGray0);
		GUI.color = new Color(1f, 1f, 1f, 0.75f);
		GUI.DrawTexture(ScoreBoard.rBonusBody, ScoreBoard.tGray1);
		GUI.color = Color.white;
		GUI2.DrawTextRes(ScoreBoard.rBonusHeader, Lang.Get("_YOU_REWARDS"), TextAnchor.MiddleCenter, _Color.White, 0, 12, true);
		GUI2.DrawTextRes(new Rect(ScoreBoard.rBonusBody.x + GUI2.YRES(140f), ScoreBoard.rBonusBody.y, GUI2.YRES(40f), GUI2.YRES(64f)), ScoreBoard.winexp.ToString(), TextAnchor.MiddleCenter, _Color.White, 0, 20, true);
		GUI.color = Color.black;
		GUI.DrawTexture(new Rect(ScoreBoard.rBonusBody.x + GUI2.YRES(180f) + 1f, ScoreBoard.rBonusBody.y + GUI2.YRES(16f) + 1f, GUI2.YRES(32f), GUI2.YRES(32f)), ScoreBoard.tExp);
		GUI.color = Color.white;
		GUI.DrawTexture(new Rect(ScoreBoard.rBonusBody.x + GUI2.YRES(180f), ScoreBoard.rBonusBody.y + GUI2.YRES(16f), GUI2.YRES(32f), GUI2.YRES(32f)), ScoreBoard.tExp);
		GUI.DrawTexture(ScoreBoard.rWinnerHeader, ScoreBoard.tGray0);
		GUI.color = new Color(1f, 1f, 1f, 0.75f);
		GUI.DrawTexture(ScoreBoard.rWinnerBody, ScoreBoard.tGray1);
		GUI.color = Color.white;
		GUI2.DrawTextRes(ScoreBoard.rWinnerHeader, Lang.Get("_WINNER"), TextAnchor.MiddleCenter, _Color.White, 0, 12, true);
		_Color fontcolor = _Color.Red;
		if (ScoreBoard.winteam == 1)
		{
			fontcolor = _Color.Blue;
		}
		GUI2.DrawTextRes(ScoreBoard.rWinnerBody, ScoreBoard.teamname[ScoreBoard.winteam], TextAnchor.MiddleCenter, fontcolor, 0, 18, true);
	}

	private static void DrawRoundData()
	{
		Rect position = new Rect(ScoreBoard.rTeamBack[0].x, ScoreBoard.rTeamBack[0].y - GUI2.YRES(20f) - GUI2.YRES(4f), GUI2.YRES(240f), GUI2.YRES(20f));
		Rect rect = new Rect(ScoreBoard.rTeamBack[0].x, ScoreBoard.rTeamBack[0].y - GUI2.YRES(20f) - GUI2.YRES(4f), GUI2.YRES(32f), GUI2.YRES(20f));
		GUI.DrawTexture(position, ScoreBoard.tGray0);
		GUI.DrawTexture(rect, ScoreBoard.tGray2);
		GUI2.DrawTextRes(rect, ScoreBoard.sTeamScore[0], TextAnchor.MiddleCenter, _Color.White, 0, 12, true);
		for (int i = 0; i < 10; i++)
		{
			if (i < ScoreBoard.teamscore[0])
			{
				GUI.color = GUI2.GetColor(_Color.Red);
			}
			else
			{
				GUI.color = Color.gray;
			}
			GUI.DrawTexture(new Rect(rect.x + GUI2.YRES(26f) + GUI2.YRES(20f) * (float)i, rect.y, GUI2.YRES(40f), GUI2.YRES(20f)), ScoreBoard.tLine);
			GUI.color = Color.white;
		}
		Rect position2 = new Rect(ScoreBoard.rTeamBack[0].x + GUI2.YRES(240f) + GUI2.YRES(4f) * 2f, ScoreBoard.rTeamBack[0].y - GUI2.YRES(20f) - GUI2.YRES(4f), GUI2.YRES(240f), GUI2.YRES(20f));
		Rect rect2 = new Rect(ScoreBoard.rTeamBack[0].x + GUI2.YRES(240f) + GUI2.YRES(4f) * 2f, ScoreBoard.rTeamBack[0].y - GUI2.YRES(20f) - GUI2.YRES(4f), GUI2.YRES(32f), GUI2.YRES(20f));
		GUI.DrawTexture(position2, ScoreBoard.tGray0);
		GUI.DrawTexture(rect2, ScoreBoard.tGray2);
		GUI2.DrawTextRes(rect2, ScoreBoard.sTeamScore[1], TextAnchor.MiddleCenter, _Color.White, 0, 12, true);
		for (int j = 0; j < 10; j++)
		{
			if (j < ScoreBoard.teamscore[1])
			{
				GUI.color = GUI2.GetColor(_Color.Blue);
			}
			else
			{
				GUI.color = Color.gray;
			}
			GUI.DrawTexture(new Rect(rect2.x + GUI2.YRES(26f) + GUI2.YRES(20f) * (float)j, rect2.y, GUI2.YRES(40f), GUI2.YRES(20f)), ScoreBoard.tLine);
			GUI.color = Color.white;
		}
	}

	private static void DrawMapData()
	{
		Rect position = new Rect(ScoreBoard.rTeamBack[0].x, ScoreBoard.rTeamBack[0].y + ScoreBoard.rTeamBack[0].height + GUI2.YRES(4f), GUI2.YRES(240f) * 2f + GUI2.YRES(4f) * 2f, GUI2.YRES(20f));
		GUI.DrawTexture(position, ScoreBoard.tGray0);
		GUI2.DrawTextRes(new Rect(position.x + GUI2.YRES(4f), position.y, position.width, position.height), ScoreBoard.sMap, TextAnchor.MiddleLeft, _Color.Gray, 0, 12, true);
		GUI2.DrawTextRes(new Rect(position.x, position.y, position.width - GUI2.YRES(4f), position.height), ScoreBoard.sGameMode, TextAnchor.MiddleRight, _Color.Gray, 0, 12, true);
	}

	public static void SetData(int gamemode, string mapname)
	{
		Options.xLightmapShadow = 0;
		ScoreBoard.sGameMode = GameMode.GetName(gamemode);
		if (gamemode == 3)
		{
			ScoreBoard.teamname = new string[]
			{
				Lang.Get("_ZOMBIE"),
				Lang.Get("_PEOPLE")
			};
		}
		else
		{
			ScoreBoard.teamname = new string[]
			{
				Lang.Get("_MERCS"),
				Lang.Get("_CORPS")
			};
		}
		ScoreBoard.sMap = mapname;
		string[] array = mapname.Split(new char[]
		{
			'v'
		});
		ScoreBoard.sMap = MapData.GetName(array[0]);
		ScoreBoard.gamemode = gamemode;
	}

	public static void SetRound0(int score0)
	{
		ScoreBoard.teamscore[0] = score0;
		ScoreBoard.sTeamScore[0] = ScoreBoard.teamscore[0].ToString();
	}

	public static void SetRound1(int score1)
	{
		ScoreBoard.teamscore[1] = score1;
		ScoreBoard.sTeamScore[1] = ScoreBoard.teamscore[1].ToString();
	}

	public static void SetActive(bool val)
	{
		ScoreBoard.show = val;
		if (ScoreBoard.show)
		{
			ScoreBoard.ResizeRects();
			ScoreBoard.SortPlayers();
		}
	}

	public static void SetActiveForce(bool val)
	{
		ScoreBoard.showforce = val;
		if (ScoreBoard.show)
		{
			ScoreBoard.SortPlayers();
		}
	}

	public static void SortPlayers()
	{
		ScoreBoard.c = 0;
		for (int i = 0; i < 16; i++)
		{
			if (PlayerControll.Player[i] != null)
			{
				ScoreBoard.sorted[ScoreBoard.c] = i;
				ScoreBoard.c++;
			}
		}
		if (ScoreBoard.c <= 1)
		{
			return;
		}
		bool flag = false;
		while (!flag)
		{
			flag = true;
			for (int j = 1; j < ScoreBoard.c; j++)
			{
				int num = ScoreBoard.sorted[j];
				int num2 = ScoreBoard.sorted[j - 1];
				if (PlayerControll.Player[num].Points > PlayerControll.Player[num2].Points)
				{
					ScoreBoard.sorted[j - 1] = num;
					ScoreBoard.sorted[j] = num2;
					flag = false;
				}
			}
		}
	}
}
