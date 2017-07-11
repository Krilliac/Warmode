using System;
using UnityEngine;

public class ScoreTop : MonoBehaviour
{
	private static bool show = false;

	private Texture2D tBlack;

	private Texture2D tBlue;

	private Texture2D tRed;

	private Texture2D tBombIconActive;

	private Rect[] rDotRed = new Rect[16];

	private Rect[] rDotBlue = new Rect[16];

	private Rect[] rDotRedBack = new Rect[16];

	private Rect[] rDotBlueBack = new Rect[16];

	private Rect[] rTeam = new Rect[2];

	private static int[] Count = new int[2];

	private Rect rScore;

	private Rect rBombIcon;

	private Color c = new Color(0f, 0f, 0f, 0.5f);

	private static int FragLimit = 0;

	public static int[] Score = new int[2];

	private static string sFragLimit = "n/a";

	public static string[] sScore = new string[]
	{
		"0",
		"0"
	};

	public static int TimeLeft = 0;

	public static float detonationTime = 40f;

	private static float TimeTimer = 0f;

	private static string sTimeLeft = "n/a";

	public static int GameMode = 0;

	public static int FriendlyFire = 0;

	private static float tBombIndic = 0f;

	private static float tBombMinSpeed = 0.1f;

	private static float tBombMaxSpeed = 2f;

	private static bool bombIndicator = false;

	public void PostAwake()
	{
		this.tBlack = TEX.GetTextureByName("black");
		this.tBlue = TEX.GetTextureByName("blue");
		this.tRed = TEX.GetTextureByName("red");
		this.tBombIconActive = TEX.GetTextureByName("GUI/c4_red");
		this.OnResize();
		ScoreTop.show = false;
	}

	public void OnResize()
	{
		int num = (int)GUI2.YRES(50f);
		int num2 = (int)GUI2.YRES(1f);
		int num3 = (int)GUI2.YRES(8f);
		int num4 = num3 + (int)GUI2.YRES(2f);
		int num5 = (int)GUI2.YRES(8f);
		for (int i = 0; i < 16; i++)
		{
			float num6 = (float)Screen.width / 2f - (float)(num + num4 * 16);
			this.rDotRed[15 - i] = new Rect(num6 + (float)(num4 * i), (float)num5, (float)num3, (float)num3);
			this.rDotRedBack[15 - i] = new Rect(num6 + (float)(num4 * i) - (float)num2, (float)(num5 - num2), (float)num3, (float)num3);
		}
		for (int j = 0; j < 16; j++)
		{
			float num7 = (float)Screen.width / 2f + (float)num + (float)((int)GUI2.YRES(3f));
			this.rDotBlue[j] = new Rect(num7 + (float)(num4 * j), (float)num5, (float)num3, (float)num3);
			this.rDotBlueBack[j] = new Rect(num7 + (float)(num4 * j) - (float)num2, (float)(num5 - num2), (float)num3, (float)num3);
		}
		this.rTeam[0] = new Rect((float)Screen.width / 2f - GUI2.YRES(50f), (float)num2, GUI2.YRES(32f), GUI2.YRES(22f));
		this.rTeam[1] = new Rect((float)Screen.width / 2f + GUI2.YRES(18f), (float)num2, GUI2.YRES(32f), GUI2.YRES(22f));
		this.rScore = new Rect((float)Screen.width / 2f - GUI2.YRES(36f) / 2f, (float)num2, GUI2.YRES(36f), GUI2.YRES(22f));
		this.rBombIcon = new Rect((float)Screen.width / 2f - GUI2.YRES(32f) / 2f, (float)num2 + GUI2.YRES(4f), GUI2.YRES(32f), GUI2.YRES(16f));
	}

	private void OnGUI()
	{
		if (!ScoreTop.show)
		{
			return;
		}
		for (int i = 0; i < ScoreTop.Count[0]; i++)
		{
			this.DrawPlayer(i, 0);
		}
		for (int j = 0; j < ScoreTop.Count[1]; j++)
		{
			this.DrawPlayer(j, 1);
		}
		this.DrawScore(ScoreTop.sScore[0], 0);
		this.DrawScore(ScoreTop.sScore[1], 1);
		if (ScoreTop.GameMode == 0)
		{
			this.DrawFrags(ScoreTop.sFragLimit);
		}
		else if (ScoreTop.GameMode == 1)
		{
			this.DrawTimer();
		}
		else if (ScoreTop.GameMode == 2 && !C4.bombdiffused)
		{
			this.DrawTimer();
		}
		else if (ScoreTop.GameMode == 3)
		{
			this.DrawTimer();
		}
		if (ScoreTop.bombIndicator)
		{
			this.DrawBombInd();
		}
	}

	public static void SetActive(bool val)
	{
		ScoreTop.show = val;
	}

	private void DrawPlayer(int id, int team)
	{
		if (team == 0)
		{
			GUI.DrawTexture(this.rDotRed[id], this.tBlack);
			GUI.DrawTexture(this.rDotRedBack[id], this.tRed);
		}
		else
		{
			GUI.DrawTexture(this.rDotBlue[id], this.tBlack);
			GUI.DrawTexture(this.rDotBlueBack[id], this.tBlue);
		}
	}

	private void DrawScore(string score, int team)
	{
		GUI.color = this.c;
		GUI.DrawTexture(this.rTeam[team], this.tBlack);
		GUI.color = Color.white;
		if (team == 0)
		{
			GUI2.DrawTextRes(this.rTeam[team], score, TextAnchor.MiddleCenter, _Color.Red, 0, 14, true);
		}
		else
		{
			GUI2.DrawTextRes(this.rTeam[team], score, TextAnchor.MiddleCenter, _Color.Blue, 0, 14, true);
		}
	}

	private void DrawFrags(string score)
	{
		GUI2.DrawTextRes(this.rScore, score, TextAnchor.MiddleCenter, _Color.White, 0, 15, true);
	}

	private void DrawTimer()
	{
		int num = (int)(ScoreTop.TimeTimer - Time.time);
		if (num != ScoreTop.TimeLeft)
		{
			ScoreTop.TimeLeft = num;
			ScoreTop.UpdateTimeLeft();
		}
		if (!ScoreTop.bombIndicator)
		{
			GUI2.DrawTextRes(this.rScore, ScoreTop.sTimeLeft, TextAnchor.MiddleCenter, _Color.White, 0, 12, true);
		}
	}

	private void DrawBombInd()
	{
		GUI.color = new Color(1f, 1f, 1f, ScoreTop.tBombIndic);
		GUI.DrawTexture(this.rBombIcon, this.tBombIconActive);
		GUI.color = Color.white;
	}

	public static void SetScore0(int val)
	{
		ScoreTop.Score[0] = val;
		ScoreTop.sScore[0] = ScoreTop.Score[0].ToString();
	}

	public static void SetScore1(int val)
	{
		ScoreTop.Score[1] = val;
		ScoreTop.sScore[1] = ScoreTop.Score[1].ToString();
	}

	public static void SetBombIndicator(bool val)
	{
		ScoreTop.bombIndicator = val;
	}

	public static void SetData(int gamemode, int fraglimit, int timeleft, int friendlyfire)
	{
		ScoreTop.GameMode = gamemode;
		ScoreTop.FragLimit = fraglimit;
		ScoreTop.FriendlyFire = friendlyfire;
		ScoreTop.sFragLimit = fraglimit.ToString();
		ScoreTop.TimeTimer = Time.time + (float)timeleft;
		ScoreTop.TimeLeft = timeleft;
		ScoreTop.UpdateTimeLeft();
	}

	private static void UpdateTimeLeft()
	{
		int num = ScoreTop.TimeLeft / 60;
		int num2 = ScoreTop.TimeLeft - num * 60;
		if (ScoreTop.TimeLeft < 0)
		{
			ScoreTop.sTimeLeft = "0:00";
		}
		else
		{
			ScoreTop.sTimeLeft = num.ToString() + ":" + num2.ToString("00");
		}
	}

	public static void SetTimeLeft(int val)
	{
		ScoreTop.TimeTimer = Time.time + (float)val;
		ScoreTop.TimeLeft = val;
		ScoreTop.detonationTime = (float)val;
		ScoreTop.UpdateTimeLeft();
	}

	public static void SetScore0Inc(int val)
	{
		ScoreTop.Score[0] += val;
		ScoreTop.sScore[0] = ScoreTop.Score[0].ToString();
	}

	public static void SetScore1Inc(int val)
	{
		ScoreTop.Score[1] += val;
		ScoreTop.sScore[1] = ScoreTop.Score[1].ToString();
	}

	public static void UpdateData()
	{
		if (Client.ID == -1)
		{
			return;
		}
		ScoreTop.show = true;
		ScoreTop.Count[0] = 0;
		ScoreTop.Count[1] = 0;
		for (int i = 0; i < 16; i++)
		{
			if (PlayerControll.Player[i] != null)
			{
				if (PlayerControll.Player[i].Team <= 1)
				{
					if (PlayerControll.Player[i].DeadFlag != 1)
					{
						ScoreTop.Count[PlayerControll.Player[i].Team]++;
					}
				}
			}
		}
	}

	private void Update()
	{
		if (ScoreTop.bombIndicator)
		{
			float num = (float)ScoreTop.TimeLeft / ScoreTop.detonationTime;
			float num2 = (num / (num + 0.2f) * 1f + 0.001f) * 6.8f;
			ScoreTop.tBombIndic += (6f - num2) * Time.deltaTime;
			if (ScoreTop.tBombIndic > 1f)
			{
				ScoreTop.tBombIndic = 0.1f;
			}
		}
		else
		{
			ScoreTop.tBombIndic = 0.1f;
		}
	}
}
