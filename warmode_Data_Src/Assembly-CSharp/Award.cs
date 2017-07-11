using System;
using UnityEngine;

public class Award : MonoBehaviour
{
	private static Texture2D tAwardBack = null;

	private static Texture2D[] tAwardTeam0 = new Texture2D[8];

	private static Texture2D[] tAwardTeam1 = new Texture2D[8];

	private static Texture2D tWhite = null;

	private static Rect rPoints;

	private static Rect rCustomPoints;

	private static Rect rMoney;

	private static Rect rAwardBack;

	private static Rect rFlashScreen;

	private static bool showpoints = false;

	private static bool showcustompoints = false;

	private static bool showmoney = false;

	private static bool showaward = false;

	private static string sPoints = string.Empty;

	private static string sMoney = string.Empty;

	private static string sCustomPoints = string.Empty;

	private static float pointstime;

	private static float custompointstime;

	private static float moneytime;

	private static float awardtime;

	private static float lasttime = -10f;

	private static int money;

	private static int award;

	public static int lastaward = 0;

	private static Color c = new Color(1f, 1f, 1f, 0.9f);

	private static string sAssist = string.Empty;

	public void PostAwake()
	{
		Award.tWhite = TEX.GetTextureByName("white");
		Award.tAwardBack = TEX.GetTextureByName("kill_spray");
		Award.tAwardTeam0[0] = TEX.GetTextureByName("kill_headshot_orange");
		Award.tAwardTeam1[0] = TEX.GetTextureByName("kill_headshot_blue");
		Award.tAwardTeam0[1] = TEX.GetTextureByName("kill_bloodsteel_orange");
		Award.tAwardTeam1[1] = TEX.GetTextureByName("kill_bloodsteel_blue");
		Award.tAwardTeam0[2] = TEX.GetTextureByName("kill_doublekill_orange");
		Award.tAwardTeam1[2] = TEX.GetTextureByName("kill_doublekill_blue");
		Award.tAwardTeam0[3] = TEX.GetTextureByName("kill_triplekill_orange");
		Award.tAwardTeam1[3] = TEX.GetTextureByName("kill_triplekill_blue");
		Award.tAwardTeam0[4] = TEX.GetTextureByName("kill_megakill_orange");
		Award.tAwardTeam1[4] = TEX.GetTextureByName("kill_megakill_blue");
		Award.tAwardTeam0[5] = TEX.GetTextureByName("kill_niceone_orange");
		Award.tAwardTeam1[5] = TEX.GetTextureByName("kill_niceone_blue");
		Award.sAssist = Lang.Get("_ASSIST");
		Award.lastaward = 0;
		this.OnResize();
	}

	public void OnResize()
	{
		Award.rPoints = new Rect(0f, 0f, (float)Screen.width, (float)Screen.height);
		Award.rPoints.center = new Vector2((float)Screen.width / 2f, (float)Screen.height / 2f - GUI2.YRES(64f));
		Award.rCustomPoints = new Rect(Award.rPoints.x, Award.rPoints.y + GUI2.YRES(16f), Award.rPoints.width, Award.rPoints.height);
		Award.rMoney = new Rect(GUI2.YRES(5f), (float)Screen.height - GUI2.YRES(68f), GUI2.YRES(64f), GUI2.YRES(20f));
		Award.rAwardBack = new Rect((float)Screen.width / 2f, GUI2.YRES(100f), GUI2.YRES(192f), GUI2.YRES(96f));
		Award.rFlashScreen = new Rect(0f, 0f, (float)Screen.width, (float)Screen.height);
	}

	public static void Draw()
	{
		Award.DrawPoints();
		Award.DrawCustomPoints();
		Award.DrawMoney();
		Award.DrawAward();
	}

	private static void DrawPoints()
	{
		if (!Award.showpoints)
		{
			return;
		}
		if (Time.time > Award.pointstime)
		{
			Award.showpoints = false;
			return;
		}
		float num = 2f - (Award.pointstime - Time.time);
		int fontsize = 12;
		if (num < 0.1f)
		{
			fontsize = 112 - (int)(num / 0.001f);
		}
		GUI2.DrawTextColorRes(Award.rPoints, Award.sPoints, TextAnchor.MiddleCenter, _Color.Yellow, 0, fontsize, true);
	}

	private static void DrawMoney()
	{
		if (!Award.showmoney)
		{
			return;
		}
		if (Time.time > Award.moneytime)
		{
			Award.showmoney = false;
			HUD.SetMoneyInc(Award.money);
			return;
		}
		float num = 2f - (Award.moneytime - Time.time);
		float num2 = 0f;
		int fontsize = 16;
		if (num < 0.1f)
		{
			fontsize = 21 - (int)(num / 0.02f);
		}
		else if (num > 1.9f)
		{
			num2 = GUI2.YRES((num - 1.9f) * 8f * 10f);
		}
		GUI2.DrawTextColorRes(new Rect(Award.rMoney.x, Award.rMoney.y + num2, Award.rMoney.width, Award.rMoney.height), Award.sMoney, TextAnchor.MiddleCenter, _Color.Green, 0, fontsize, true);
	}

	private static void DrawAward()
	{
		if (!Award.showaward)
		{
			return;
		}
		if (Time.time > Award.awardtime + 2f)
		{
			Award.showaward = false;
			return;
		}
		float num = Time.time - Award.awardtime;
		num *= 20f;
		if (num > 1.01f && num < 2f)
		{
			num = 1f - (num - 1.01f) * 2f + 1f;
		}
		else if (num >= 2f)
		{
			num = 1f;
		}
		float num2 = Award.rAwardBack.width * num;
		float num3 = Award.rAwardBack.height * num;
		Rect position = new Rect(Award.rAwardBack.x - num2 / 2f, Award.rAwardBack.y - num3 / 2f, num2, num3);
		GUI.color = Award.c;
		GUI.DrawTexture(position, Award.tAwardBack);
		GUI.color = Color.white;
		if (BasePlayer.team == 0)
		{
			GUI.DrawTexture(position, Award.tAwardTeam0[Award.award]);
		}
		else if (BasePlayer.team == 1)
		{
			GUI.DrawTexture(position, Award.tAwardTeam1[Award.award]);
		}
	}

	public static void SetPoints(int val)
	{
		Award.sPoints = "+" + val.ToString();
		Award.pointstime = Time.time + 2f;
		Award.showpoints = true;
	}

	public static void SetPoinsFake()
	{
		Award.sPoints = "-1";
		Award.pointstime = Time.time + 2f;
		Award.showpoints = true;
	}

	public static void SetMoney(int val)
	{
		int num = HUD.GetMoney();
		if (num + val > 15000)
		{
			val = 15000 - num;
		}
		if (val <= 0)
		{
			return;
		}
		if (Award.showmoney)
		{
			HUD.SetMoneyInc(Award.money);
		}
		Award.money = val;
		if (val >= 0)
		{
			Award.sMoney = "+" + val.ToString();
		}
		else
		{
			Award.sMoney = val.ToString();
		}
		Award.moneytime = Time.time + 2f;
		Award.showmoney = true;
	}

	public static void SetAward(int val)
	{
		Award.showaward = true;
		Award.awardtime = Time.time;
		Award.award = val;
	}

	public static void SetDeath(int wid, int hs)
	{
		Award.lastaward = Award.award;
		if (Time.time < Award.lasttime + 3f)
		{
			if (Award.lastaward >= 2)
			{
				int num = Award.lastaward + 1;
				if (num > 5)
				{
					num = 5;
				}
				Award.SetAward(num);
			}
			else
			{
				Award.SetAward(2);
			}
		}
		else if (wid == 26)
		{
			Award.SetAward(1);
		}
		else if (hs == 1)
		{
			Award.SetAward(0);
		}
		Award.lasttime = Time.time;
	}

	public static void SetCustomPoints(int val, int cpid)
	{
		Award.sCustomPoints = "+" + val.ToString();
		if (cpid == 0)
		{
			Award.sCustomPoints = Award.sCustomPoints + " " + Award.sAssist;
		}
		Award.custompointstime = Time.time + 1.5f;
		Award.showcustompoints = true;
		HUD.SetMoneyInc(val * 2);
	}

	private static void DrawCustomPoints()
	{
		if (!Award.showcustompoints)
		{
			return;
		}
		if (Time.time > Award.custompointstime)
		{
			Award.showcustompoints = false;
			return;
		}
		float num = 1.5f - (Award.custompointstime - Time.time);
		int fontsize = 12;
		if (num < 0.075f)
		{
			fontsize = 87 - (int)(num / 0.001f);
		}
		GUI2.DrawTextColorRes(Award.rCustomPoints, Award.sCustomPoints, TextAnchor.MiddleCenter, _Color.Green, 0, fontsize, true);
	}
}
