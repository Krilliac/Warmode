using System;
using UnityEngine;

public class GUIM : MonoBehaviour
{
	private static Color[] colorlist = new Color[10];

	public static GUIStyle guistyle = new GUIStyle();

	private static Font[] fontlist = new Font[2];

	private static int nextelement = 0;

	private static bool debug = false;

	private static Texture2D tDebug = null;

	private static Texture2D[] tBar = new Texture2D[6];

	private static float offsetx = 0f;

	private static float offsety = 0f;

	private static float offsety2 = 0f;

	private static Texture2D tButton = null;

	private static string hackControlName = "matrix";

	private static Rect rBar;

	public static void Init()
	{
		GUIM.colorlist[0] = new Color(0f, 0f, 0f, 1f);
		GUIM.colorlist[1] = new Color(1f, 1f, 1f, 1f);
		GUIM.colorlist[2] = new Color(1f, 0f, 0f, 1f);
		GUIM.colorlist[3] = new Color(0f, 1f, 0f, 1f);
		GUIM.colorlist[4] = new Color(0f, 0.4f, 1f, 1f);
		GUIM.colorlist[5] = new Color(1f, 0.8f, 0f, 1f);
		GUIM.colorlist[6] = new Color(0.5f, 0.5f, 0.5f, 1f);
		GUIM.colorlist[7] = new Color(1f, 0.2f, 0f, 1f);
		GUIM.colorlist[8] = new Color(1f, 0.4f, 0.35f, 1f);
		GUIM.colorlist[9] = new Color(0.75f, 0.75f, 0.75f, 1f);
	}

	private void LoadEnd()
	{
		GUIM.fontlist[0] = ContentLoader_.LoadFont("Play-Regular");
		GUIM.fontlist[1] = ContentLoader_.LoadFont("Play-Bold");
		GUIM.tDebug = TEX.GetTextureByName("red");
		GUIM.tBar[0] = TEX.GetTextureByName("tr");
		GUIM.tBar[1] = TEX.GetTextureByName("scroll_middle");
		GUIM.tBar[2] = TEX.GetTextureByName("tr");
		GUIM.tBar[3] = TEX.GetTextureByName("slider_normal");
		GUIM.tBar[4] = TEX.GetTextureByName("slider_active");
		GUIM.tBar[5] = TEX.GetTextureByName("slider_back");
		GUIM.tButton = TEX.GetTextureByName("button");
	}

	public static void ResetGUI()
	{
		GUIM.nextelement = 0;
	}

	public static void DrawTextureX(Rect r, Texture2D tex)
	{
		Rect position = new Rect(GUIM.YRES(r.x), GUIM.YRES(r.y), GUIM.YRES(r.width), GUIM.YRES(r.height));
		GUI.DrawTexture(position, tex);
	}

	public static void DrawText(Rect r, string text, TextAnchor pos, BaseColor fontcolor, int fontpos, int fontsize, bool fontshadow)
	{
		fontsize = (int)GUIM.YRES((float)fontsize);
		GUIM.guistyle.font = GUIM.fontlist[fontpos];
		GUIM.guistyle.alignment = pos;
		GUIM.guistyle.fontSize = fontsize;
		if (fontshadow)
		{
			GUIM.guistyle.normal.textColor = GUIM.colorlist[0];
			GUI.Label(new Rect(r.x + 1f, r.y + 1f, r.width, r.height), text, GUIM.guistyle);
		}
		GUIM.guistyle.normal.textColor = GUIM.colorlist[(int)fontcolor];
		GUI.Label(r, text, GUIM.guistyle);
	}

	public static void DrawEdit(Rect r, ref string text, TextAnchor pos, BaseColor fontcolor, int fontpos, int fontsize, bool fontshadow)
	{
		GUI.SetNextControlName("123");
		GUIM.guistyle.wordWrap = false;
		GUIM.guistyle.clipping = TextClipping.Clip;
		GUIM.guistyle.font = GUIM.fontlist[fontpos];
		GUIM.guistyle.alignment = pos;
		GUIM.guistyle.fontSize = fontsize;
		GUIM.guistyle.normal.textColor = GUIM.colorlist[(int)fontcolor];
		text = GUI.TextField(r, text, GUIM.guistyle);
		GUIM.guistyle.clipping = TextClipping.Overflow;
		GUI.FocusControl("123");
	}

	public void SetFocus()
	{
		GUI.FocusControl("element" + (GUIM.nextelement - 1).ToString());
	}

	public void AutoSetName()
	{
		GUI.SetNextControlName("element" + GUIM.nextelement.ToString());
		GUIM.nextelement++;
	}

	public void SetFocusHack()
	{
		GUI.SetNextControlName(GUIM.hackControlName);
		GUI.Button(new Rect(-10000f, -10000f, 0f, 0f), GUIContent.none);
	}

	public static float YRES(float val)
	{
		return val * ((float)Screen.height / 720f);
	}

	public static Vector2 CalcSize(string text, int fontpos, int fontsize)
	{
		GUIM.guistyle.font = GUIM.fontlist[fontpos];
		GUIM.guistyle.fontSize = fontsize;
		return GUIM.guistyle.CalcSize(new GUIContent(text));
	}

	public static Vector2 BeginScrollViewORG(Rect viewzone, Vector2 scrollViewVector, Rect scrollzone)
	{
		GUI.skin.verticalScrollbar.normal.background = null;
		GUI.skin.verticalScrollbarThumb.normal.background = null;
		scrollViewVector = GUI.BeginScrollView(viewzone, scrollViewVector, scrollzone);
		float height = viewzone.height / scrollzone.height * viewzone.height;
		float num = scrollViewVector.y / scrollzone.height * viewzone.height;
		if (scrollzone.height <= viewzone.height)
		{
			GUIM.rBar.height = 0f;
		}
		else
		{
			GUIM.rBar = new Rect(viewzone.x + viewzone.width - 14f, viewzone.y + num, 14f, height);
		}
		return scrollViewVector;
	}

	public static void EndScrollViewORG()
	{
		GUI.EndScrollView();
	}

	public static void DrawBar(Texture2D top, Texture2D middle, Texture2D bottom)
	{
		if (GUIM.rBar.height == 0f)
		{
			return;
		}
		GUI.DrawTexture(new Rect(GUIM.rBar.x, GUIM.rBar.y, GUIM.rBar.width, 4f), top);
		if (GUIM.rBar.height - 8f > 0f)
		{
			GUI.DrawTexture(new Rect(GUIM.rBar.x, GUIM.rBar.y, GUIM.rBar.width, GUIM.rBar.height + 2f), middle);
		}
		GUI.DrawTexture(new Rect(GUIM.rBar.x, GUIM.rBar.y + GUIM.rBar.height - 4f, GUIM.rBar.width, 4f), bottom);
	}

	public static bool HideButton(Rect r)
	{
		return GUI.Button(r, string.Empty, GUIM.guistyle);
	}

	public static Vector2 BeginScrollView(Rect viewzone, Vector2 scrollViewVector, Rect scrollzone)
	{
		GUIM.offsetx = viewzone.x;
		GUIM.offsety = viewzone.y;
		Vector2 result = GUIM.BeginScrollViewORG(viewzone, scrollViewVector, scrollzone);
		GUIM.offsety2 = result.y;
		return result;
	}

	public static void EndScrollView()
	{
		GUIM.EndScrollViewORG();
		GUIM.DrawBar(GUIM.tBar[0], GUIM.tBar[1], GUIM.tBar[2]);
		GUIM.offsetx = 0f;
		GUIM.offsety = 0f;
		GUIM.offsety2 = 0f;
	}

	public static bool Contains(Rect r, Vector2 mpos)
	{
		return r.Contains(new Vector2(mpos.x - GUIM.offsetx, mpos.y - GUIM.offsety + GUIM.offsety2));
	}

	public static void DrawBox(Rect r, Texture2D t)
	{
		int num = (int)GUIM.YRES(1f);
		if (num < 0)
		{
			num = 1;
		}
		GUI.color = new Color(1f, 1f, 1f, 0.4f);
		GUI.DrawTexture(new Rect(r.x - (float)num, r.y - (float)num, r.width + (float)(num * 2), r.height + (float)(num * 2)), t);
		GUI.color = new Color(1f, 1f, 1f, 0.4f);
		GUI.DrawTexture(r, t);
		GUI.color = new Color(1f, 1f, 1f, 0.2f);
		GUI.DrawTexture(new Rect(r.x, r.y - (float)(num * 2), r.width, (float)num), t);
		GUI.DrawTexture(new Rect(r.x, r.y + r.height + (float)num, r.width, (float)num), t);
		GUI.DrawTexture(new Rect(r.x - (float)(num * 2), r.y, (float)num, r.height), t);
		GUI.DrawTexture(new Rect(r.x + r.width + (float)num, r.y, (float)num, r.height), t);
		GUI.color = Color.white;
	}

	public static float DrawSlider(Rect r, int size, float start, float end, float val)
	{
		GUI.skin.horizontalSliderThumb.normal.background = GUIM.tBar[3];
		GUI.skin.horizontalSliderThumb.hover.background = GUIM.tBar[4];
		GUI.skin.horizontalSliderThumb.active.background = GUIM.tBar[4];
		GUI.skin.horizontalSlider.normal.background = GUIM.tBar[1];
		GUI.skin.horizontalSliderThumb.fixedHeight = 16f;
		GUI.skin.horizontalSliderThumb.fixedWidth = 16f;
		GUI.skin.horizontalSlider.fixedHeight = 16f;
		GUI.skin.horizontalSlider.fixedWidth = (float)size;
		GUILayout.BeginArea(r);
		val = GUILayout.HorizontalSlider(val, start, end, new GUILayoutOption[]
		{
			GUILayout.Width((float)size)
		});
		GUILayout.EndArea();
		return val;
	}

	public static bool Button(Rect r, BaseColor c, string text, TextAnchor anchor, BaseColor tc, int fp, int size, bool shadow)
	{
		GUI.color = GUIM.colorlist[(int)c];
		GUI.DrawTexture(r, GUIM.tButton);
		GUI.color = Color.white;
		GUIM.DrawText(r, text, anchor, tc, fp, size, shadow);
		return GUIM.HideButton(r);
	}
}
