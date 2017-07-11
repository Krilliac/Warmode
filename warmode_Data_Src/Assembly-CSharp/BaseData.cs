using System;
using System.Collections;
using System.Diagnostics;
using UnityEngine;

public class BaseData : MonoBehaviour
{
	public static BaseData cs = null;

	public static bool Auth = false;

	public static bool Profile = false;

	public static string Name = string.Empty;

	public static string Level = "Lv.1";

	public static string Gold = "-";

	public static string GP = "-";

	public static string EXP = "-";

	public static string Progress = "0%";

	public static string EXPData = "0/0";

	public static string ClanName = string.Empty;

	public static int iProgress = 1;

	public static int iLevel = 1;

	public static int iGold = 0;

	public static int iGP = 0;

	public static int iEXP = 0;

	public static int iWarid = 0;

	public static string banCost = string.Empty;

	public static string uid = string.Empty;

	public static string key = string.Empty;

	public static string session = string.Empty;

	public static string warid = string.Empty;

	public static string warsession = string.Empty;

	public static string invsig = string.Empty;

	public static float AuthTime = -1f;

	public static bool Update = false;

	public static int[] item = new int[1024];

	public static int badge_back = 0;

	public static int badge_icon = 0;

	public static int mask_merc = 0;

	public static int mask_warcorp = 0;

	public static int[] profileWeapon = new int[128];

	public static int[] currentWeapon = new int[128];

	public static int Error = 0;

	public void PostAwake()
	{
		BaseData.cs = this;
	}

	public static void Init()
	{
		if (GameData.gSteam)
		{
			BaseData.uid = string.Empty;
			BaseData.key = string.Empty;
			BaseData.session = "_";
			BaseData.Auth = false;
		}
		if (GameData.gVK && !PlayerPrefs.HasKey("reconnect"))
		{
			BaseData.uid = string.Empty;
			BaseData.key = string.Empty;
			BaseData.session = "_";
			BaseData.Auth = false;
		}
	}

	public void SetID(string uid)
	{
		BaseData.uid = uid;
	}

	public void SetKey(string key)
	{
		BaseData.key = key;
	}

	public void SetSession(string session)
	{
		BaseData.session = session;
		if (GameData.gVK)
		{
			BaseData.Profile = false;
			WebHandler.get_profile("&version=" + Client.version);
		}
	}

	public static void Refresh()
	{
		BaseData.Profile = false;
		BaseData.banCost = string.Empty;
		WebHandler.get_profile("&version=" + Client.version);
	}

	public void Refresh2SecDelay()
	{
		base.StartCoroutine("RefreshDelay");
	}

	[DebuggerHidden]
	public IEnumerator RefreshDelay()
	{
		return new BaseData.<RefreshDelay>c__Iterator14();
	}

	public static void StartAuth()
	{
		if (GameData.gVK)
		{
			if (Time.time < BaseData.AuthTime + 1f)
			{
				return;
			}
			Application.ExternalCall("get_auth", new object[]
			{
				string.Empty
			});
			BaseData.AuthTime = Time.time;
		}
		if (GameData.gFB)
		{
			if (Time.time < BaseData.AuthTime + 1f)
			{
				return;
			}
			FBManager.StartAuth();
			BaseData.AuthTime = Time.time;
		}
	}

	public static void FullCalcLevel()
	{
		int num;
		int num2;
		int num3;
		int num4;
		BaseData.CalcLevel(BaseData.iEXP, out num, out num2, out num3, out num4);
		BaseData.Level = "Lv." + num.ToString();
		BaseData.iLevel = num;
		BaseData.Progress = num4.ToString() + "%";
		BaseData.iProgress = num4;
		BaseData.EXPData = BaseData.iEXP.ToString() + "/" + num3.ToString();
	}

	public static int CalcLevel(int exp)
	{
		int num = 1;
		int num2 = 1;
		while (exp >= (num * (num + 1) * (num + 2) + 15 * num) * 10)
		{
			num++;
			num2++;
		}
		return num2;
	}

	public static void CalcLevel(int exp, out int _level, out int _currexp, out int _nextexp, out int _progress)
	{
		int num = 1;
		int num2 = 1;
		while (exp >= (num * (num + 1) * (num + 2) + 15 * num) * 10)
		{
			num++;
			num2++;
		}
		num = num2 - 1;
		int num3 = (num * (num + 1) * (num + 2) + 15 * num) * 10;
		num = num2;
		int num4 = (num * (num + 1) * (num + 2) + 15 * num) * 10;
		float num5 = (float)((exp - num3) * 100 / (num4 - num3));
		_progress = (int)num5;
		_level = num2;
		_currexp = num3;
		_nextexp = num4;
	}

	public static DateTime ConvertFromUnixTimestamp(double timestamp)
	{
		DateTime dateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0);
		return dateTime.AddSeconds(timestamp);
	}
}
