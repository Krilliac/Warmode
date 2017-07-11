using System;
using UnityEngine;

public class GUI2 : MonoBehaviour
{
	private static Color[] colorlist = new Color[10];

	private static GUIStyle guistyle = new GUIStyle();

	private static Font[] fontlist = new Font[4];

	private static int nextelement = 0;

	private static string hackControlName = "matrix";

	public static void Init()
	{
		GUI2.colorlist[0] = new Color(0f, 0f, 0f, 1f);
		GUI2.colorlist[1] = new Color(1f, 1f, 1f, 1f);
		GUI2.colorlist[3] = new Color(0.25f, 1f, 0.25f, 1f);
		GUI2.colorlist[2] = new Color(1f, 0.2f, 0f, 1f);
		GUI2.colorlist[4] = new Color(0f, 0.5f, 1f, 1f);
		GUI2.colorlist[5] = new Color(1f, 0.2f, 0f, 1f);
		GUI2.colorlist[6] = new Color(1f, 1f, 0f, 1f);
		GUI2.colorlist[7] = new Color(0.5f, 0.5f, 0.5f, 1f);
		GUI2.fontlist[0] = ContentLoader_.LoadFont("Archangelsk");
		GUI2.fontlist[1] = ContentLoader_.LoadFont("Kelson Sans Regular RU");
		GUI2.fontlist[2] = ContentLoader_.LoadFont("Kelson Sans Bold RU");
	}

	public static void ResetGUI()
	{
		GUI2.nextelement = 0;
	}

	public static void DrawTextRes(Rect r, string text, TextAnchor pos, _Color fontcolor, int fontpos, int fontsize, bool fontshadow)
	{
		int fontsize2 = (int)GUI2.YRES((float)fontsize);
		GUI2.DrawText(r, text, pos, fontcolor, fontpos, fontsize2, fontshadow);
	}

	public static void DrawTextColorRes(Rect r, string text, TextAnchor pos, _Color fontcolor, int fontpos, int fontsize, bool fontshadow)
	{
		int fontsize2 = (int)GUI2.YRES((float)fontsize);
		GUI2.DrawTextColor(r, text, pos, fontcolor, fontpos, fontsize2, fontshadow);
	}

	public static void DrawText(Rect r, string text, TextAnchor pos, _Color fontcolor, int fontpos, int fontsize, bool fontshadow)
	{
		GUI2.guistyle.font = GUI2.fontlist[fontpos];
		GUI2.guistyle.alignment = pos;
		GUI2.guistyle.fontSize = fontsize;
		if (fontshadow)
		{
			GUI2.guistyle.normal.textColor = GUI2.colorlist[0];
			GUI.Label(new Rect(r.x + 1f, r.y + 1f, r.width, r.height), text, GUI2.guistyle);
		}
		GUI2.guistyle.normal.textColor = GUI2.colorlist[(int)fontcolor];
		GUI.Label(r, text, GUI2.guistyle);
	}

	public static void DrawTextColor(Rect r, string text, TextAnchor pos, _Color fontcolor, int fontpos, int fontsize, bool fontshadow)
	{
		GUI2.guistyle.font = GUI2.fontlist[fontpos];
		GUI2.guistyle.alignment = pos;
		GUI2.guistyle.fontSize = fontsize;
		if (fontshadow)
		{
			string text2 = text.Replace("<color=", string.Empty);
			text2 = text2.Replace("</color>", string.Empty);
			text2 = text2.Replace("#F20>", string.Empty);
			text2 = text2.Replace("#04F>", string.Empty);
			text2 = text2.Replace("#FFF>", string.Empty);
			text2 = text2.Replace("#F60>", string.Empty);
			GUI2.guistyle.normal.textColor = GUI2.colorlist[0];
			GUI.Label(new Rect(r.x + 1f, r.y + 1f, r.width, r.height), text2, GUI2.guistyle);
		}
		GUI2.guistyle.normal.textColor = GUI2.colorlist[(int)fontcolor];
		GUI.Label(r, text, GUI2.guistyle);
	}

	public static void DrawEdit(Rect r, ref string text, TextAnchor pos, _Color fontcolor, int fontpos, int fontsize, bool fontshadow)
	{
		GUI.SetNextControlName("123");
		GUI2.guistyle.wordWrap = false;
		GUI2.guistyle.clipping = TextClipping.Clip;
		GUI2.guistyle.font = GUI2.fontlist[fontpos];
		GUI2.guistyle.alignment = pos;
		GUI2.guistyle.fontSize = fontsize;
		GUI2.guistyle.normal.textColor = GUI2.colorlist[(int)fontcolor];
		text = GUI.TextField(r, text, GUI2.guistyle);
		GUI.FocusControl("123");
	}

	public static void DrawEditRes(Rect r, ref string text, TextAnchor pos, _Color fontcolor, int fontpos, int fontsize, bool fontshadow)
	{
		GUI.SetNextControlName("123");
		GUI2.guistyle.wordWrap = false;
		GUI2.guistyle.clipping = TextClipping.Clip;
		GUI2.guistyle.font = GUI2.fontlist[fontpos];
		GUI2.guistyle.alignment = pos;
		GUI2.guistyle.fontSize = (int)GUI2.YRES((float)fontsize);
		GUI2.guistyle.normal.textColor = GUI2.colorlist[(int)fontcolor];
		text = GUI.TextField(r, text, GUI2.guistyle);
		GUI.FocusControl("123");
	}

	public static bool HideButton(Rect r)
	{
		return GUI.Button(r, string.Empty, GUI2.guistyle);
	}

	public void SetFocus()
	{
		GUI.FocusControl("element" + (GUI2.nextelement - 1).ToString());
	}

	public void AutoSetName()
	{
		GUI.SetNextControlName("element" + GUI2.nextelement.ToString());
		GUI2.nextelement++;
	}

	public void SetFocusHack()
	{
		GUI.SetNextControlName(GUI2.hackControlName);
		GUI.Button(new Rect(-10000f, -10000f, 0f, 0f), GUIContent.none);
	}

	public static float YRES(float val)
	{
		return val * ((float)Screen.height / 480f);
	}

	public static float CalcSizeRes(string text, int fontpos, int fontsize)
	{
		GUI2.guistyle.font = GUI2.fontlist[fontpos];
		GUI2.guistyle.fontSize = (int)GUI2.YRES((float)fontsize);
		return GUI2.guistyle.CalcSize(new GUIContent(text)).x;
	}

	public static Color GetColor(_Color fontcolor)
	{
		return GUI2.colorlist[(int)fontcolor];
	}
}
