using System;
using UnityEngine;

public class BottomBar : MonoBehaviour
{
	private static Texture2D tBlack = null;

	private static Texture2D tWhite = null;

	private static Texture2D tVolume = null;

	private static Texture2D tScreen = null;

	private static Texture2D tWid = null;

	private static Texture2D tLang = null;

	private static Rect[] rButton = new Rect[3];

	private static Vector2 mpos = Vector3.zero;

	private static float[] buttontime = new float[3];

	private static float[] clicktime = new float[3];

	private static float[] button2time = new float[3];

	private static float[] click2time = new float[3];

	private void LoadEnd()
	{
		BottomBar.tBlack = TEX.GetTextureByName("black");
		BottomBar.tWhite = TEX.GetTextureByName("white");
		BottomBar.tVolume = TEX.GetTextureByName("volume");
		BottomBar.tScreen = TEX.GetTextureByName("widescreen");
		BottomBar.tWid = TEX.GetTextureByName("wid");
		BottomBar.tLang = TEX.GetTextureByName("lang");
	}

	public void PostAwake()
	{
		this.OnResize();
	}

	private void OnResize()
	{
		int num = (int)GUIM.YRES(80f);
		BottomBar.rButton[0] = new Rect(GUIM.YRES(16f), (float)Screen.height - GUIM.YRES(32f), (float)num, GUIM.YRES(32f));
		BottomBar.rButton[1] = new Rect((float)(Screen.width - num * 2) - GUIM.YRES(8f) - GUIM.YRES(16f), (float)Screen.height - GUIM.YRES(32f), (float)num, GUIM.YRES(32f));
		BottomBar.rButton[2] = new Rect((float)(Screen.width - num) - GUIM.YRES(16f), (float)Screen.height - GUIM.YRES(32f), (float)num, GUIM.YRES(32f));
	}

	public static void Draw()
	{
		GUI.DrawTexture(new Rect(0f, (float)Screen.height - GUIM.YRES(32f), (float)Screen.width, GUIM.YRES(32f)), BottomBar.tBlack);
		BottomBar.DrawButton(0, BottomBar.rButton[0], Lang.Get("_LANGUAGE"), BottomBar.tLang);
		BottomBar.DrawButton(1, BottomBar.rButton[1], Lang.Get("_MUTE"), BottomBar.tVolume);
		BottomBar.DrawButton(2, BottomBar.rButton[2], Lang.Get("_FULLSCREEN_F9"), BottomBar.tScreen);
	}

	public static void DrawButton(int state, Rect r, string text, Texture2D icon)
	{
		BottomBar.mpos = new Vector2(Input.mousePosition.x, (float)Screen.height - Input.mousePosition.y);
		if (BottomBar.click2time[state] + 0.05f > Time.time)
		{
			GUI.DrawTexture(r, BottomBar.tWhite);
			GUI.color = Color.black;
			GUI.DrawTexture(new Rect(r.x + (r.width - r.height) / 2f, r.y, r.height, r.height), icon);
			GUI.color = Color.white;
			GUI.DrawTexture(new Rect(r.x, r.y - GUIM.YRES(14f), r.width, GUIM.YRES(16f)), BottomBar.tWhite);
			GUIM.DrawText(new Rect(r.x, r.y - GUIM.YRES(14f), r.width, GUIM.YRES(16f)), text, TextAnchor.MiddleCenter, BaseColor.Black, 0, 10, false);
		}
		else if (r.Contains(BottomBar.mpos))
		{
			BottomBar.button2time[state] = Time.time;
			GUI.DrawTexture(new Rect(r.x + (r.width - r.height) / 2f, r.y, r.height, r.height), icon);
			GUI.DrawTexture(new Rect(r.x, r.y - GUIM.YRES(14f), r.width, GUIM.YRES(16f)), BottomBar.tWhite);
			GUIM.DrawText(new Rect(r.x, r.y - GUIM.YRES(14f), r.width, GUIM.YRES(16f)), text, TextAnchor.MiddleCenter, BaseColor.Black, 0, 10, false);
			if (Input.GetMouseButton(0))
			{
				BottomBar.click2time[state] = Time.time;
			}
		}
		else
		{
			float num = Time.time - BottomBar.button2time[state];
			float num2 = 16f - num * 100f;
			if (num2 < 2f)
			{
				num2 = 2f;
			}
			GUI.DrawTexture(new Rect(r.x + (r.width - r.height) / 2f, r.y, r.height, r.height), icon);
			GUI.DrawTexture(new Rect(r.x, r.y + GUIM.YRES(2f - num2), r.width, GUIM.YRES(num2)), BottomBar.tWhite);
		}
		if (GUIM.HideButton(r))
		{
			if (state == 0)
			{
				Lang.ChangeLanguage();
			}
			if (state == 1)
			{
				if (Options.menuvol != 0f)
				{
					Options.menuvol = 0f;
				}
				else
				{
					Options.menuvol = 0.25f;
				}
				PlayerPrefs.SetFloat("menuvol", Options.menuvol);
				GameObject gameObject = GameObject.Find("Camera");
				if (gameObject && gameObject.GetComponent<AudioSource>())
				{
					gameObject.GetComponent<AudioSource>().volume = Options.menuvol;
				}
			}
			if (state == 2)
			{
				Options.ToggleFullScreen();
			}
		}
	}
}
