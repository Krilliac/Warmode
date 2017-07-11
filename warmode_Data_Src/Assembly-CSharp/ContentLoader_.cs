using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class ContentLoader_
{
	public class LoadBundle
	{
		public string name;

		public bool loaded;

		public bool dontdestroy;

		public int version;

		public int size;

		public bool crypted;

		public LoadBundle(string name, int version, bool dontdestroy, bool crypted, int size = 1)
		{
			this.name = name;
			this.loaded = false;
			this.version = version;
			this.size = size;
			this.dontdestroy = dontdestroy;
			this.crypted = crypted;
		}
	}

	private const int HEADER_SKIP = 26;

	public static bool proceed = false;

	public static string CURRPATH = string.Empty;

	public static List<UnityEngine.Object> materialList = new List<UnityEngine.Object>();

	public static List<UnityEngine.Object> gameobjectList = new List<UnityEngine.Object>();

	public static List<UnityEngine.Object> textureList = new List<UnityEngine.Object>();

	public static List<UnityEngine.Object> audioList = new List<UnityEngine.Object>();

	public static List<UnityEngine.Object> fontList = new List<UnityEngine.Object>();

	public static List<ContentLoader_.LoadBundle> LoadList = new List<ContentLoader_.LoadBundle>();

	public static ContentLoader_.LoadBundle currBundle = null;

	public static bool inDownload = false;

	public static int maxcontentcount = 0;

	public static WWW www = null;

	public static string[] assetdir = null;

	private static int gv = 5;

	public static void Init()
	{
		if (ContentLoader_.proceed)
		{
			ContentLoader_.BroadcastAll("LoadEnd", string.Empty);
			return;
		}
		ContentLoader_.materialList.Clear();
		ContentLoader_.gameobjectList.Clear();
		ContentLoader_.textureList.Clear();
		ContentLoader_.audioList.Clear();
		ContentLoader_.AddPack();
		if (ContentLoader_.LoadList.Count == 0)
		{
			ContentLoader_.BroadcastAll("LoadEnd", string.Empty);
			ContentLoader_.proceed = true;
			UnityEngine.Debug.Log("ContentLoader.Editor.LoadEnd");
		}
	}

	public static void AddPack()
	{
		ContentLoader_.LoadList.Clear();
		ContentLoader_.LoadList.Add(new ContentLoader_.LoadBundle("Pack_weapon_ak47", ContentLoader_.gv, false, true, 1));
		ContentLoader_.LoadList.Add(new ContentLoader_.LoadBundle("Pack_weapon_aks74u", ContentLoader_.gv, false, true, 1));
		ContentLoader_.LoadList.Add(new ContentLoader_.LoadBundle("Pack_weapon_asval", ContentLoader_.gv, false, true, 1));
		ContentLoader_.LoadList.Add(new ContentLoader_.LoadBundle("Pack_weapon_aug", ContentLoader_.gv, false, true, 1));
		ContentLoader_.LoadList.Add(new ContentLoader_.LoadBundle("Pack_weapon_awp", ContentLoader_.gv, false, true, 1));
		ContentLoader_.LoadList.Add(new ContentLoader_.LoadBundle("Pack_weapon_bm4", ContentLoader_.gv, false, true, 1));
		ContentLoader_.LoadList.Add(new ContentLoader_.LoadBundle("Pack_weapon_c4", ContentLoader_.gv, false, true, 1));
		ContentLoader_.LoadList.Add(new ContentLoader_.LoadBundle("Pack_weapon_famas", ContentLoader_.gv, false, true, 1));
		ContentLoader_.LoadList.Add(new ContentLoader_.LoadBundle("Pack_weapon_grenades", ContentLoader_.gv, false, true, 1));
		ContentLoader_.LoadList.Add(new ContentLoader_.LoadBundle("Pack_weapon_knife", ContentLoader_.gv, false, true, 1));
		ContentLoader_.LoadList.Add(new ContentLoader_.LoadBundle("Pack_weapon_kniferun", ContentLoader_.gv, false, true, 1));
		ContentLoader_.LoadList.Add(new ContentLoader_.LoadBundle("Pack_weapon_m4a1", ContentLoader_.gv, false, true, 1));
		ContentLoader_.LoadList.Add(new ContentLoader_.LoadBundle("Pack_weapon_m24", ContentLoader_.gv, false, true, 1));
		ContentLoader_.LoadList.Add(new ContentLoader_.LoadBundle("Pack_weapon_m90", ContentLoader_.gv, false, true, 1));
		ContentLoader_.LoadList.Add(new ContentLoader_.LoadBundle("Pack_weapon_m110", ContentLoader_.gv, false, true, 1));
		ContentLoader_.LoadList.Add(new ContentLoader_.LoadBundle("Pack_weapon_m249", ContentLoader_.gv, false, true, 1));
		ContentLoader_.LoadList.Add(new ContentLoader_.LoadBundle("Pack_weapon_mp5", ContentLoader_.gv, false, true, 1));
		ContentLoader_.LoadList.Add(new ContentLoader_.LoadBundle("Pack_weapon_mp7", ContentLoader_.gv, false, true, 1));
		ContentLoader_.LoadList.Add(new ContentLoader_.LoadBundle("Pack_weapon_p90", ContentLoader_.gv, false, true, 1));
		ContentLoader_.LoadList.Add(new ContentLoader_.LoadBundle("Pack_weapon_pistols", ContentLoader_.gv, false, true, 1));
		ContentLoader_.LoadList.Add(new ContentLoader_.LoadBundle("Pack_weapon_pkp", ContentLoader_.gv, false, true, 1));
		ContentLoader_.LoadList.Add(new ContentLoader_.LoadBundle("Pack_weapon_qbz95", ContentLoader_.gv, false, true, 1));
		ContentLoader_.LoadList.Add(new ContentLoader_.LoadBundle("Pack_weapon_spas12", ContentLoader_.gv, false, true, 1));
		ContentLoader_.LoadList.Add(new ContentLoader_.LoadBundle("Pack_weapon_svd", ContentLoader_.gv, false, true, 1));
		ContentLoader_.LoadList.Add(new ContentLoader_.LoadBundle("Pack_weapon_ump45", ContentLoader_.gv, false, true, 1));
		ContentLoader_.LoadList.Add(new ContentLoader_.LoadBundle("Pack_weapon_zombiehand", ContentLoader_.gv, false, true, 1));
		ContentLoader_.LoadList.Add(new ContentLoader_.LoadBundle("PackAwards", ContentLoader_.gv, false, true, 1));
		ContentLoader_.LoadList.Add(new ContentLoader_.LoadBundle("PackBadges", ContentLoader_.gv, false, true, 1));
		ContentLoader_.LoadList.Add(new ContentLoader_.LoadBundle("PackDetonator", ContentLoader_.gv, false, true, 1));
		ContentLoader_.LoadList.Add(new ContentLoader_.LoadBundle("PackGame", ContentLoader_.gv, false, true, 1));
		ContentLoader_.LoadList.Add(new ContentLoader_.LoadBundle("PackLoader", ContentLoader_.gv, false, true, 1));
		ContentLoader_.LoadList.Add(new ContentLoader_.LoadBundle("PackMainMenu", ContentLoader_.gv, false, true, 1));
		ContentLoader_.LoadList.Add(new ContentLoader_.LoadBundle("PackMaskCorp", ContentLoader_.gv, false, true, 1));
		ContentLoader_.LoadList.Add(new ContentLoader_.LoadBundle("PackMaskMerc", ContentLoader_.gv, false, true, 1));
		ContentLoader_.LoadList.Add(new ContentLoader_.LoadBundle("PackMusic", ContentLoader_.gv, false, true, 1));
		ContentLoader_.LoadList.Add(new ContentLoader_.LoadBundle("PackPlayer", ContentLoader_.gv, false, true, 1));
		ContentLoader_.LoadList.Add(new ContentLoader_.LoadBundle("PackPlayerSounds", ContentLoader_.gv, false, true, 1));
		ContentLoader_.LoadList.Add(new ContentLoader_.LoadBundle("PackWeaponSounds", ContentLoader_.gv, false, true, 1));
		ContentLoader_.LoadList.Add(new ContentLoader_.LoadBundle("PackSkyboxes", ContentLoader_.gv + 1, false, true, 1));
		ContentLoader_.LoadList.Add(new ContentLoader_.LoadBundle("PackZombie", ContentLoader_.gv, false, true, 1));
		ContentLoader_.LoadList.Add(new ContentLoader_.LoadBundle("PackSkin_ak47", ContentLoader_.gv, false, true, 1));
		ContentLoader_.LoadList.Add(new ContentLoader_.LoadBundle("PackSkin_aks74u", ContentLoader_.gv, false, true, 1));
		ContentLoader_.LoadList.Add(new ContentLoader_.LoadBundle("PackSkin_asval", ContentLoader_.gv, false, true, 1));
		ContentLoader_.LoadList.Add(new ContentLoader_.LoadBundle("PackSkin_aug", ContentLoader_.gv, false, true, 1));
		ContentLoader_.LoadList.Add(new ContentLoader_.LoadBundle("PackSkin_awp", ContentLoader_.gv, false, true, 1));
		ContentLoader_.LoadList.Add(new ContentLoader_.LoadBundle("PackSkin_beretta", ContentLoader_.gv, false, true, 1));
		ContentLoader_.LoadList.Add(new ContentLoader_.LoadBundle("PackSkin_bm4", ContentLoader_.gv, false, true, 1));
		ContentLoader_.LoadList.Add(new ContentLoader_.LoadBundle("PackSkin_colt", ContentLoader_.gv, false, true, 1));
		ContentLoader_.LoadList.Add(new ContentLoader_.LoadBundle("PackSkin_deagle", ContentLoader_.gv, false, true, 1));
		ContentLoader_.LoadList.Add(new ContentLoader_.LoadBundle("PackSkin_famas", ContentLoader_.gv, false, true, 1));
		ContentLoader_.LoadList.Add(new ContentLoader_.LoadBundle("PackSkin_glock17", ContentLoader_.gv, false, true, 1));
		ContentLoader_.LoadList.Add(new ContentLoader_.LoadBundle("PackSkin_m4a1", ContentLoader_.gv, false, true, 1));
		ContentLoader_.LoadList.Add(new ContentLoader_.LoadBundle("PackSkin_m24", ContentLoader_.gv, false, true, 1));
		ContentLoader_.LoadList.Add(new ContentLoader_.LoadBundle("PackSkin_m90", ContentLoader_.gv, false, true, 1));
		ContentLoader_.LoadList.Add(new ContentLoader_.LoadBundle("PackSkin_m110", ContentLoader_.gv, false, true, 1));
		ContentLoader_.LoadList.Add(new ContentLoader_.LoadBundle("PackSkin_m249", ContentLoader_.gv, false, true, 1));
		ContentLoader_.LoadList.Add(new ContentLoader_.LoadBundle("PackSkin_mp5", ContentLoader_.gv, false, true, 1));
		ContentLoader_.LoadList.Add(new ContentLoader_.LoadBundle("PackSkin_mp7", ContentLoader_.gv, false, true, 1));
		ContentLoader_.LoadList.Add(new ContentLoader_.LoadBundle("PackSkin_p90", ContentLoader_.gv, false, true, 1));
		ContentLoader_.LoadList.Add(new ContentLoader_.LoadBundle("PackSkin_pkp", ContentLoader_.gv, false, true, 1));
		ContentLoader_.LoadList.Add(new ContentLoader_.LoadBundle("PackSkin_qbz95", ContentLoader_.gv, false, true, 1));
		ContentLoader_.LoadList.Add(new ContentLoader_.LoadBundle("PackSkin_remington", ContentLoader_.gv, false, true, 1));
		ContentLoader_.LoadList.Add(new ContentLoader_.LoadBundle("PackSkin_spas12", ContentLoader_.gv, false, true, 1));
		ContentLoader_.LoadList.Add(new ContentLoader_.LoadBundle("PackSkin_svd", ContentLoader_.gv, false, true, 1));
		ContentLoader_.LoadList.Add(new ContentLoader_.LoadBundle("PackSkin_ump45", ContentLoader_.gv, false, true, 1));
		ContentLoader_.maxcontentcount = ContentLoader_.LoadList.Count;
	}

	[DebuggerHidden]
	public static IEnumerator cLoad(string svalue, int version)
	{
		ContentLoader_.<cLoad>c__Iterator13 <cLoad>c__Iterator = new ContentLoader_.<cLoad>c__Iterator13();
		<cLoad>c__Iterator.svalue = svalue;
		<cLoad>c__Iterator.<$>svalue = svalue;
		return <cLoad>c__Iterator;
	}

	public static void BroadcastAll(string fun, object msg)
	{
		GameObject[] array = (GameObject[])UnityEngine.Object.FindObjectsOfType(typeof(GameObject));
		GameObject[] array2 = array;
		for (int i = 0; i < array2.Length; i++)
		{
			GameObject gameObject = array2[i];
			if (gameObject && gameObject.transform.parent == null)
			{
				gameObject.gameObject.BroadcastMessage(fun, msg, SendMessageOptions.DontRequireReceiver);
			}
		}
	}

	public static Material LoadMaterial(string name)
	{
		using (List<UnityEngine.Object>.Enumerator enumerator = ContentLoader_.materialList.GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				Material material = (Material)enumerator.Current;
				if (material.name == name)
				{
					return material;
				}
			}
		}
		return null;
	}

	public static void ReplaceMaterialsFromRes(ref MeshRenderer mr)
	{
	}

	public static GameObject LoadGameObject(string name)
	{
		using (List<UnityEngine.Object>.Enumerator enumerator = ContentLoader_.gameobjectList.GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				GameObject gameObject = (GameObject)enumerator.Current;
				if (gameObject.name == name)
				{
					return gameObject;
				}
			}
		}
		return null;
	}

	public static GameObject LoadGameObjectPref(string name)
	{
		return Resources.Load<GameObject>("Prefabs/" + name);
	}

	public static Texture LoadTexture(string name)
	{
		using (List<UnityEngine.Object>.Enumerator enumerator = ContentLoader_.textureList.GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				Texture texture = (Texture)enumerator.Current;
				if (texture.name == name)
				{
					return texture;
				}
			}
		}
		return null;
	}

	public static AudioClip LoadAudio(string name)
	{
		using (List<UnityEngine.Object>.Enumerator enumerator = ContentLoader_.audioList.GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				AudioClip audioClip = (AudioClip)enumerator.Current;
				if (audioClip.name == name)
				{
					return audioClip;
				}
			}
		}
		return null;
	}

	public static Font LoadFont(string name)
	{
		using (List<UnityEngine.Object>.Enumerator enumerator = ContentLoader_.fontList.GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				Font font = (Font)enumerator.Current;
				if (font.name == name)
				{
					return font;
				}
			}
		}
		return null;
	}

	private static byte[] UnLoad(byte[] data)
	{
		if (data.Length < 26)
		{
			return null;
		}
		int num = data.Length / 2 * 2;
		for (int i = 0; i < num; i++)
		{
			if (i >= 26)
			{
				byte b = data[i];
				data[i] = data[i + 1];
				data[i + 1] = b;
				i++;
			}
		}
		return data;
	}
}
