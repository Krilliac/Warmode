using System;
using UnityEngine;

public class Crosshair : MonoBehaviour
{
	private Texture2D tblack;

	private Texture2D twhite;

	private Texture2D[] tCrosshair = new Texture2D[4];

	private Texture2D tCrosshairHit;

	private static Rect[] rCrosshair = new Rect[4];

	private static Rect[] rCrosshair2 = new Rect[4];

	private Rect rCrosshaitHit;

	public static bool forceLockCursor = false;

	public static bool show = false;

	private static bool forcehide = false;

	private static int offset = 0;

	private static int offsetinc = 0;

	private static int lastoffset = -1;

	private static int lastoffsetinc = -1;

	private static float offsettime = 0f;

	private static float hittime = 0f;

	private static int crosshair_size = 6;

	private static int next_crosshair_dynamic = 0;

	private static int crosshair_dynamic = 0;

	private static Color[] c = new Color[3];

	public void PostAwake()
	{
		this.tblack = TEX.GetTextureByName("black");
		this.twhite = TEX.GetTextureByName("white");
		this.tCrosshairHit = TEX.GetTextureByName("GUI/crosshair_hit");
		Crosshair.c[0] = new Color(1f, 1f, 1f, 0.75f);
		Crosshair.c[1] = new Color(1f, 1f, 1f, 0.5f);
		Crosshair.c[2] = new Color(1f, 1f, 1f, 0.25f);
		this.OnResize();
		Crosshair.offsettime = 0f;
		Crosshair.SetActive(false);
	}

	private void OnResize()
	{
		Vector2 vector = new Vector2((float)Screen.width, (float)Screen.height) / 2f;
		int num = (int)GUI2.YRES(32f);
		this.rCrosshaitHit = new Rect(vector.x - (float)num / 2f, vector.y - (float)num / 2f, (float)num, (float)num);
		Crosshair.CalcResize();
	}

	public static void SetCrosshairColor(int r, int g, int b)
	{
		Options.ccx = r;
		Options.ccy = g;
		Options.ccz = b;
		PlayerPrefs.SetInt("ccx", r);
		PlayerPrefs.SetInt("ccy", g);
		PlayerPrefs.SetInt("ccz", b);
		Crosshair.c[0] = new Color((float)r / 255f, (float)g / 255f, (float)b / 255f, Crosshair.c[0].a);
		Crosshair.c[1] = new Color((float)r / 255f, (float)g / 255f, (float)b / 255f, Crosshair.c[1].a);
		Crosshair.c[2] = new Color((float)r / 255f, (float)g / 255f, (float)b / 255f, Crosshair.c[2].a);
	}

	private static void CalcResize()
	{
		int num = 1;
		if (Screen.height > 600)
		{
			num = 2;
		}
		int num2 = (int)GUI2.YRES((float)Crosshair.crosshair_size);
		int num3 = 4 * num;
		int num4 = 1 * num;
		Vector2 vector = new Vector2((float)Screen.width, (float)Screen.height) / 2f;
		int num5 = (int)GUI2.YRES((float)Crosshair.offset);
		int num6 = (int)GUI2.YRES((float)Crosshair.offsetinc) + (int)GUI2.YRES((float)Crosshair.crosshair_dynamic);
		Crosshair.rCrosshair[0] = new Rect(vector.x - (float)num4, vector.y - (float)num3 - (float)num5 - (float)num6 - (float)num2, (float)num4, (float)num3);
		Crosshair.rCrosshair[2] = new Rect(vector.x - (float)num4, vector.y + (float)num5 + (float)num6 + (float)num2 - (float)num4, (float)num4, (float)num3);
		Crosshair.rCrosshair[3] = new Rect(vector.x - (float)num3 - (float)num5 - (float)num6 - (float)num2, vector.y - (float)num4, (float)num3, (float)num4);
		Crosshair.rCrosshair[1] = new Rect(vector.x + (float)num5 + (float)num6 + (float)num2 - (float)num4, vector.y - (float)num4, (float)num3, (float)num4);
		Crosshair.rCrosshair2[0] = new Rect(Crosshair.rCrosshair[0].x - 1f, Crosshair.rCrosshair[0].y - 1f, (float)(num4 + 2), (float)(num3 + 1));
		Crosshair.rCrosshair2[2] = new Rect(Crosshair.rCrosshair[2].x - 1f, Crosshair.rCrosshair[2].y, (float)(num4 + 2), (float)(num3 + 1));
		Crosshair.rCrosshair2[3] = new Rect(Crosshair.rCrosshair[3].x - 1f, Crosshair.rCrosshair[3].y - 1f, (float)(num3 + 1), (float)(num4 + 2));
		Crosshair.rCrosshair2[1] = new Rect(Crosshair.rCrosshair[1].x, Crosshair.rCrosshair[1].y - 1f, (float)(num3 + 1), (float)(num4 + 2));
	}

	private void OnGUI()
	{
		if (Crosshair.forcehide)
		{
			return;
		}
		if (Crosshair.hittime > Time.time)
		{
			GUI.DrawTexture(this.rCrosshaitHit, this.tCrosshairHit);
		}
		if (!Crosshair.show)
		{
			return;
		}
		if (Crosshair.offset != Crosshair.lastoffset || Crosshair.offsetinc != Crosshair.lastoffsetinc || Crosshair.offset != 0 || Crosshair.offsetinc != 0 || Crosshair.crosshair_dynamic != Crosshair.next_crosshair_dynamic)
		{
			Crosshair.crosshair_dynamic = (int)Mathf.Lerp((float)Crosshair.crosshair_dynamic, (float)Crosshair.next_crosshair_dynamic, Time.deltaTime * 10f);
			if (Crosshair.offset < 0)
			{
				Crosshair.offset = 0;
			}
			float time = Time.time;
			if (time >= Crosshair.offsettime)
			{
				int num = (int)((time - Crosshair.offsettime) / 0.01f) + 1;
				Crosshair.offsettime = time + 0.01f;
				Crosshair.offsetinc -= num;
				if (Crosshair.offsetinc < 0)
				{
					Crosshair.offsetinc = 0;
				}
			}
			Crosshair.CalcResize();
			Crosshair.lastoffset = Crosshair.offset;
			Crosshair.lastoffsetinc = Crosshair.offsetinc;
		}
		if (Crosshair.offset + Crosshair.crosshair_dynamic > 50)
		{
			GUI.color = Crosshair.c[2];
		}
		else if (Crosshair.offset + Crosshair.crosshair_dynamic > 10)
		{
			GUI.color = Crosshair.c[1];
		}
		else
		{
			GUI.color = Crosshair.c[0];
		}
		GUI.DrawTexture(Crosshair.rCrosshair2[0], this.tblack);
		GUI.DrawTexture(Crosshair.rCrosshair2[1], this.tblack);
		GUI.DrawTexture(Crosshair.rCrosshair2[2], this.tblack);
		GUI.DrawTexture(Crosshair.rCrosshair2[3], this.tblack);
		GUI.DrawTexture(Crosshair.rCrosshair[0], this.twhite);
		GUI.DrawTexture(Crosshair.rCrosshair[1], this.twhite);
		GUI.DrawTexture(Crosshair.rCrosshair[2], this.twhite);
		GUI.DrawTexture(Crosshair.rCrosshair[3], this.twhite);
		GUI.color = Color.white;
	}

	public static void SetActive(bool val)
	{
		Crosshair.show = val;
	}

	public static void ForceHide(bool val)
	{
		Crosshair.forcehide = val;
	}

	public static void SetOffset(int _offset)
	{
		if (Options.dynamiccrosshair == 0)
		{
			Crosshair.offset = 0;
		}
		else
		{
			Crosshair.offset = (int)Mathf.Lerp((float)Crosshair.offset, (float)_offset, Time.deltaTime * 10f);
		}
	}

	public static void SetDynamic(int size)
	{
		Crosshair.next_crosshair_dynamic = size;
		Crosshair.CalcResize();
	}

	public static void SetOffsetNull()
	{
		Crosshair.offsetinc = 0;
		Crosshair.offset = 0;
	}

	public static void SetOffsetInc(int _offset)
	{
		if (Options.dynamiccrosshair == 0)
		{
			Crosshair.offsetinc = 0;
		}
		else if (Crosshair.offsetinc == 0)
		{
			Crosshair.offsetinc = _offset;
		}
		else
		{
			Crosshair.offsetinc += _offset;
		}
		Crosshair.offsettime = Time.time;
	}

	public static void SetHit()
	{
		Crosshair.hittime = Time.time + 0.5f;
	}

	public static void SetCrosshairSize(int size)
	{
		Crosshair.crosshair_size = 6 + size;
		if (Crosshair.crosshair_size < 4)
		{
			Crosshair.crosshair_size = 4;
		}
		Crosshair.CalcResize();
	}
}
