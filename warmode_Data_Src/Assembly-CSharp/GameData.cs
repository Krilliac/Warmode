using System;
using UnityEngine;

public class GameData : MonoBehaviour
{
	public const string devPass = "6f66a0d73e9894e62fe0ee48c209911b";

	public static bool gSteam;

	public static bool gSocial;

	public static bool gVK;

	public static bool gFB;

	public static int roundTimeLimit;

	public static float roundTimeStart;

	public static int freezeTime;

	public static int restartroundmode;

	public static bool infected;

	public static void SetRoundTimeLimit(int val)
	{
		GameData.roundTimeLimit = val;
	}

	public static void SetRoundTimeStart(float val)
	{
		GameData.roundTimeStart = val;
	}

	public static void SetFreezeTime(int val)
	{
		GameData.freezeTime = val;
	}

	public static void SetRestartRoundMode(int val)
	{
		GameData.restartroundmode = val;
	}
}
