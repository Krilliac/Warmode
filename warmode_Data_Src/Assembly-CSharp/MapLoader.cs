using System;
using System.Collections;
using System.Diagnostics;
using UnityEngine;

public class MapLoader : MonoBehaviour
{
	private const string PATH = "http://warmodegame.com/core/content/maps/";

	private const int HEADER_SKIP = 26;

	public static MapLoader cs;

	private Texture2D tKey0;

	private Texture2D tKey1;

	private Texture2D tLoad;

	private Texture2D tGray;

	private Texture2D MapShot;

	private Texture2D[] tNumbers = new Texture2D[9];

	private string MAPNAME = string.Empty;

	private static string url = string.Empty;

	private static int version = 6;

	private static AssetBundle bundle;

	public static AssetBundle wwwbundle;

	public static WWW www;

	private static string clearmapname = string.Empty;

	private bool inload;

	private void Start()
	{
		MapLoader.cs = this;
		this.LoadEnd();
		this.tKey0 = TEX.GetTextureByName("wmkey0_" + Lang.GetLanguage());
		this.tKey1 = TEX.GetTextureByName("wmkey1_" + Lang.GetLanguage());
		this.tLoad = TEX.GetTextureByName("wmload");
		this.tGray = TEX.GetTextureByName("gray0");
		if (this.tKey0 == null)
		{
			this.tKey0 = TEX.GetTextureByName("wmkey0");
		}
		if (this.tKey1 == null)
		{
			this.tKey1 = TEX.GetTextureByName("wmkey1");
		}
		for (int i = 0; i < 9; i++)
		{
			this.tNumbers[i] = TEX.GetTextureByName("progress" + i.ToString());
		}
	}

	public void Load(string mapname)
	{
		this.LoadMap(mapname, true);
	}

	public void DevLoad(string mapname)
	{
		MonoBehaviour.print("DEVLOAD " + mapname);
		this.LoadMap(mapname, false);
	}

	private void LoadMap(string mapname, bool crypted)
	{
		Crosshair.forceLockCursor = false;
		string empty = string.Empty;
		this.MAPNAME = mapname;
		HUD.PlayBriefing();
		base.StartCoroutine(this.rLoad(mapname, crypted));
	}

	[DebuggerHidden]
	private IEnumerator rLoad(string svalue, bool crypted)
	{
		MapLoader.<rLoad>c__Iterator9 <rLoad>c__Iterator = new MapLoader.<rLoad>c__Iterator9();
		<rLoad>c__Iterator.svalue = svalue;
		<rLoad>c__Iterator.crypted = crypted;
		<rLoad>c__Iterator.<$>svalue = svalue;
		<rLoad>c__Iterator.<$>crypted = crypted;
		<rLoad>c__Iterator.<>f__this = this;
		return <rLoad>c__Iterator;
	}

	private byte[] DecryptMap(byte[] data)
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

	[DebuggerHidden]
	private IEnumerator SpectatorSpawn()
	{
		return new MapLoader.<SpectatorSpawn>c__IteratorA();
	}

	public static void UpdateReflectionProbe()
	{
		GameObject gameObject = GameObject.Find("map");
		if (gameObject == null)
		{
			return;
		}
		ReflectionProbe component = gameObject.GetComponent<ReflectionProbe>();
		if (component == null)
		{
			return;
		}
		component.RenderProbe();
	}

	[DebuggerHidden]
	private IEnumerator ParsMapData()
	{
		return new MapLoader.<ParsMapData>c__IteratorB();
	}

	public void LoadEnd()
	{
		int @int = PlayerPrefs.GetInt("localplay");
		if (PlayerPrefs.GetInt("localplay") > 0)
		{
			string @string = PlayerPrefs.GetString("map");
			if (@string != string.Empty)
			{
				if (@int == 1)
				{
					global::Console.cs.Command("map " + @string);
				}
				if (@int == 2)
				{
					global::Console.cs.Command("devmap " + @string);
				}
			}
		}
		PlayerPrefs.SetString("map", string.Empty);
	}

	private void OnGUI()
	{
		if (this.inload)
		{
			this.DrawLoad();
		}
	}

	private void DrawLoad()
	{
		Rect position = new Rect(0f, 0f, (float)Screen.width, (float)Screen.height);
		GUI.DrawTexture(position, this.tGray);
		Rect position2 = new Rect(0f, 0f, GUI2.YRES(480f) * 2f, GUI2.YRES(480f));
		position2.center = new Vector2((float)Screen.width, (float)Screen.height) / 2f;
		if (this.MapShot != null)
		{
			GUI.DrawTexture(position2, this.MapShot);
		}
		GUI.DrawTexture(position, this.tLoad);
		Rect position3 = new Rect(0f, 0f, (float)Screen.width * 0.8f, (float)Screen.width / 2.5f);
		position3.center = new Vector2((float)Screen.width, (float)Screen.width / 2.5f) / 2f;
		if (this.tKey0)
		{
			GUI.DrawTexture(position3, this.tKey0);
		}
		GUI.color = new Color(1f, 0.2f, 0f, 1f);
		if (this.tKey1)
		{
			GUI.DrawTexture(position3, this.tKey1);
		}
		GUI.color = Color.white;
		int num = 100;
		Rect position4 = new Rect((float)Screen.width * 0.15f, (float)Screen.height - GUI2.YRES(128f), GUI2.YRES(96f), GUI2.YRES(96f));
		int num2;
		if (num < 5)
		{
			num2 = 0;
		}
		else if (num < 13)
		{
			num2 = 1;
		}
		else if (num < 26)
		{
			num2 = 2;
		}
		else if (num < 39)
		{
			num2 = 3;
		}
		else if (num < 52)
		{
			num2 = 4;
		}
		else if (num < 65)
		{
			num2 = 5;
		}
		else if (num < 78)
		{
			num2 = 6;
		}
		else if (num < 91)
		{
			num2 = 7;
		}
		else
		{
			num2 = 8;
		}
		GUI.DrawTexture(position4, this.tNumbers[num2]);
		GUI2.DrawTextRes(new Rect(position4.x, position4.y + GUI2.YRES(4f), position4.width, position4.height), num.ToString() + "%", TextAnchor.MiddleCenter, _Color.Orange, 2, 22, false);
		Rect r = new Rect(position4.x + position4.width + GUI2.YRES(8f), position4.y, GUI2.YRES(280f), GUI2.YRES(64f));
		Rect r2 = new Rect(position4.x + position4.width + GUI2.YRES(8f), position4.y + GUI2.YRES(48f), GUI2.YRES(280f), GUI2.YRES(64f));
		GUI2.DrawTextRes(r, Lang.Get("_MAP"), TextAnchor.MiddleLeft, _Color.White, 2, 38, false);
		GUI2.DrawTextRes(r2, ScoreBoard.sMap, TextAnchor.MiddleLeft, _Color.Orange, 1, 38, false);
	}
}
