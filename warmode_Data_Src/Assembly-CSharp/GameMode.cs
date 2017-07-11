using System;
using System.Collections.Generic;
using UnityEngine;

public class GameMode : MonoBehaviour
{
	public class CGameMode
	{
		public string modeName;

		public int id;

		public CGameMode(int id, string modeName)
		{
			this.id = id;
			this.modeName = modeName;
		}
	}

	public const int DEATHMATCH = 0;

	public const int CONFRONTATION = 1;

	public const int DETONATION = 2;

	public const int ZOMBIEMATCH = 3;

	public static int gamemode = 0;

	public static List<GameMode.CGameMode> modes = new List<GameMode.CGameMode>
	{
		new GameMode.CGameMode(0, "_DEATHMATCH"),
		new GameMode.CGameMode(1, "_CONFRONTATION"),
		new GameMode.CGameMode(2, "_DETONATION"),
		new GameMode.CGameMode(3, "_ZOMBIEMATCH")
	};

	public static string GetName(int id)
	{
		foreach (GameMode.CGameMode current in GameMode.modes)
		{
			if (current.id == id)
			{
				return Lang.Get(current.modeName);
			}
		}
		return Lang.Get("_GAMEMODE") + id.ToString();
	}
}
