using System;
using System.Collections.Generic;
using UnityEngine;

public static class WeaponData
{
	public const int maxWeapons = 128;

	public const int PISTOLS = 0;

	public const int SHOTGUNS = 1;

	public const int SUBMACHINEGUNS = 2;

	public const int RIFLES = 3;

	public const int SNIPERS = 4;

	public const int HEAVY = 5;

	public const int GRENADE = 6;

	public const int AMMO = 7;

	public const int KNIFE = 8;

	public const int BOMB = 9;

	public const int ZOMBIEHAND = 10;

	public const int WORLD = 255;

	public const int LEFT = 0;

	public const int RIGHT = 1;

	private static Dictionary<int, CWeaponData> weapons;

	static WeaponData()
	{
		WeaponData.weapons = new Dictionary<int, CWeaponData>();
		WeaponData.weapons.Clear();
		WeaponData.weapons.Add(0, new CWeaponData(0, "WEAPON", 0, 0, 0, 255, 0, 300, 0f, 8, false, 0.18f, 0.22f, 0, 0, 0, 1, false));
		WeaponData.weapons.Add(1, new CWeaponData(1, "GLOCK17", 17, 90, 0, 0, 400, 300, 0.3f, 1, true, 0.2f, 0.24f, 1, 1, 0, 1, true));
		WeaponData.weapons.Add(2, new CWeaponData(2, "BERETTA", 15, 90, 0, 0, 500, 300, 0.3f, 1, true, 0.2f, 0.24f, 1, 1, 0, 1, true));
		WeaponData.weapons.Add(3, new CWeaponData(3, "COLT", 7, 42, 2, 0, 600, 300, 0.3f, 1, true, 0.2f, 0.24f, 1, 1, 0, 1, true));
		WeaponData.weapons.Add(4, new CWeaponData(4, "DEAGLE", 7, 42, 1, 0, 750, 300, 0.5f, 1, true, 0.2f, 0.24f, 1, 1, 0, 1, true));
		WeaponData.weapons.Add(5, new CWeaponData(5, "REMINGTON", 8, 48, 0, 0, 350, 400, 0.3f, 1, true, 0.2f, 0.24f, 1, 1, 0, 1, true));
		WeaponData.weapons.Add(6, new CWeaponData(6, "AK47", 30, 90, 3, 3, 2700, 300, 0.3f, 0, true, 0.18f, 0.22f, 1, 4, 0, 1, true));
		WeaponData.weapons.Add(7, new CWeaponData(7, "AKS74U", 30, 90, 4, 3, 2250, 300, 0.95f, 0, true, 0.18f, 0.22f, 1, 4, 0, 1, true));
		WeaponData.weapons.Add(8, new CWeaponData(8, "ASVAL", 20, 80, 5, 3, 3500, 300, 0.25f, 0, true, 0.18f, 0.22f, 1, 4, 0, 1, true));
		WeaponData.weapons.Add(9, new CWeaponData(9, "BM4", 6, 24, 6, 1, 2500, 400, 0.5f, 0, true, 0.18f, 0.22f, 1, 2, 32, 1, true));
		WeaponData.weapons.Add(10, new CWeaponData(10, "FAMAS", 25, 100, 7, 3, 2300, 300, 0.3f, 0, true, 0.18f, 0.22f, 1, 4, 0, 1, true));
		WeaponData.weapons.Add(11, new CWeaponData(11, "M4A1", 30, 90, 7, 3, 3200, 300, 0.85f, 0, true, 0.18f, 0.22f, 1, 4, 0, 1, true));
		WeaponData.weapons.Add(12, new CWeaponData(12, "MP5", 30, 90, 0, 2, 1500, 400, 0.3f, 0, true, 0.19f, 0.23f, 1, 3, 0, 1, true));
		WeaponData.weapons.Add(13, new CWeaponData(13, "MP7", 20, 100, 0, 2, 1700, 600, 0.3f, 0, true, 0.19f, 0.23f, 1, 3, 0, 1, true));
		WeaponData.weapons.Add(14, new CWeaponData(14, "P90", 50, 100, 2, 2, 2350, 400, 0.3f, 0, true, 0.19f, 0.23f, 1, 3, 0, 1, true));
		WeaponData.weapons.Add(15, new CWeaponData(15, "QBZ95", 30, 90, 7, 3, 2350, 300, 0.3f, 0, true, 0.19f, 0.23f, 1, 4, 0, 1, true));
		WeaponData.weapons.Add(16, new CWeaponData(16, "SPAS12", 8, 32, 6, 1, 2000, 300, 0.5f, 0, true, 0.18f, 0.22f, 1, 2, 32, 1, true));
		WeaponData.weapons.Add(17, new CWeaponData(17, "UMP45", 30, 90, 2, 2, 1450, 600, 0.3f, 0, true, 0.18f, 0.22f, 1, 3, 0, 1, true));
		WeaponData.weapons.Add(18, new CWeaponData(18, "AUG", 30, 90, 7, 3, 4200, 300, 0.5f, 0, true, 0.18f, 0.22f, 2, 4, 0, 1, true));
		WeaponData.weapons.Add(19, new CWeaponData(19, "SVD", 10, 30, 3, 4, 3900, 300, 0.6f, 0, true, 0.18f, 0.22f, 2, 5, 64, 1, true));
		WeaponData.weapons.Add(20, new CWeaponData(20, "M110", 10, 30, 3, 4, 4550, 300, 0.6f, 0, true, 0.18f, 0.22f, 2, 5, 64, 1, true));
		WeaponData.weapons.Add(21, new CWeaponData(21, "M24", 10, 30, 8, 4, 4500, 300, 0.4f, 0, true, 0.17f, 0.21f, 2, 5, 64, 1, true));
		WeaponData.weapons.Add(22, new CWeaponData(22, "AWP", 10, 30, 8, 4, 5600, 100, 0.4f, 0, true, 0.16f, 0.2f, 2, 5, 64, 1, true));
		WeaponData.weapons.Add(23, new CWeaponData(23, "M90", 10, 30, 9, 4, 6000, 100, 0.5f, 0, true, 0.16f, 0.2f, 2, 5, 64, 1, true));
		WeaponData.weapons.Add(24, new CWeaponData(24, "PKP", 100, 300, 3, 5, 8500, 300, 1f, 0, true, 0.16f, 0.2f, 1, 6, 0, 1, true));
		WeaponData.weapons.Add(25, new CWeaponData(25, "M249", 100, 300, 7, 5, 8000, 300, 1f, 0, true, 0.16f, 0.2f, 1, 6, 0, 1, true));
		WeaponData.weapons.Add(26, new CWeaponData(26, "KNIFE", 0, 0, 0, 8, 99999, 1000, 0.3f, 9, false, 0.2f, 0.24f, 0, 0, 64, 0, false));
		WeaponData.weapons.Add(27, new CWeaponData(27, "FG", 0, 0, 0, 6, 300, 300, 0.3f, 3, false, 0.2f, 0.24f, 0, 7, 32, 0, false));
		WeaponData.weapons.Add(28, new CWeaponData(28, "KNIFERUN", 0, 0, 0, 8, 99999, 1000, 0.3f, 2, false, 0.2f, 0.24f, 0, 0, 0, 0, false));
		WeaponData.weapons.Add(29, new CWeaponData(29, "FB", 0, 0, 0, 6, 300, 300, 0.3f, 5, false, 0.2f, 0.24f, 0, 7, 32, 0, false));
		WeaponData.weapons.Add(30, new CWeaponData(30, "SG", 0, 0, 0, 6, 200, 300, 0.3f, 6, false, 0.2f, 0.24f, 0, 7, 32, 0, false));
		WeaponData.weapons.Add(31, new CWeaponData(31, "ZOMBIEHAND", 0, 0, 0, 10, 99999, 300, 0.3f, 7, false, 0.2f, 0.24f, 0, 0, 64, 1, false));
		WeaponData.weapons.Add(49, new CWeaponData(49, "DEFUSE", 0, 0, 0, 7, 200, 0, 0f, 8, false, 0.18f, 0.22f, 0, 7, 0, 1, false));
		WeaponData.weapons.Add(50, new CWeaponData(50, "C4", 0, 0, 0, 9, 99999, 0, 0.3f, 4, true, 0.18f, 0.22f, 0, 0, 64, 1, false));
		WeaponData.weapons.Add(120, new CWeaponData(120, "ARMORHELMET", 0, 0, 0, 7, 650, 0, 0f, 8, false, 0.18f, 0.22f, 0, 7, 0, 1, false));
		WeaponData.weapons.Add(121, new CWeaponData(121, "ARMORFULL", 0, 0, 0, 7, 1000, 0, 0f, 8, false, 0.18f, 0.22f, 0, 7, 0, 1, false));
	}

	public static void Init()
	{
		foreach (KeyValuePair<int, CWeaponData> current in WeaponData.weapons)
		{
			current.Value.fire1 = SND.GetSoundByName("weapon/" + current.Value.wName.ToLower() + "_fire1");
			current.Value.icon = TEX.GetTextureByName("weapon_" + current.Value.wName.ToLower());
			if (current.Value.icon == null)
			{
				Debug.Log("[TEX] can't load: weapon_" + current.Value.wName.ToLower());
			}
			if (current.Value.icon)
			{
				int width = current.Value.icon.width;
				int height = current.Value.icon.height;
				current.Value.icon2_inverted = new Texture2D(width, height, TextureFormat.RGBA32, false);
				current.Value.icon2 = new Texture2D(width, height, TextureFormat.RGBA32, false);
				for (int i = 0; i < width; i++)
				{
					for (int j = 0; j < height; j++)
					{
						current.Value.icon2.SetPixel(width - i, j, new Color(1f, 1f, 1f, current.Value.icon.GetPixel(i, j).a));
						current.Value.icon2_inverted.SetPixel(i, j, new Color(1f, 1f, 1f, current.Value.icon.GetPixel(i, j).a));
					}
				}
				current.Value.icon2.Apply(true);
				current.Value.icon2_inverted.Apply(true);
			}
		}
	}

	public static bool CheckWeapon(int wid)
	{
		return WeaponData.weapons.ContainsKey(wid);
	}

	public static bool CheckCustomSkin(int wid)
	{
		return WeaponData.CheckWeapon(wid) && WeaponData.weapons[wid].customSkin;
	}

	public static CWeaponData GetData(int wid)
	{
		return WeaponData.weapons[wid];
	}

	public static int GetId(string wName)
	{
		foreach (KeyValuePair<int, CWeaponData> current in WeaponData.weapons)
		{
			if (current.Value.wName.ToLower() == wName.ToLower())
			{
				return current.Key;
			}
		}
		return -1;
	}
}
