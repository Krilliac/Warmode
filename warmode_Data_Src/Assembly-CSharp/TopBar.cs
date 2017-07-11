using System;
using UnityEngine;

public class TopBar : MonoBehaviour
{
	private static Texture2D tBlack = null;

	private static Texture2D tOrange = null;

	private static Texture2D tBlue = null;

	private static Texture2D tGreen = null;

	private static Texture2D tHeader_logo = null;

	private static Texture2D[] tPlay = new Texture2D[2];

	private static Texture2D[] tShop = new Texture2D[2];

	private static Texture2D[] tOptions = new Texture2D[2];

	private static Texture2D tClose;

	private static Rect rHeader_logo;

	private static Rect[] rButton = new Rect[3];

	private static Rect rClose;

	private static Vector2 mpos = Vector3.zero;

	private static float[] clicktime = new float[3];

	private static float[] buttontime = new float[3];

	public void PostAwake()
	{
		TopBar.tBlack = TEX.GetTextureByName("black");
		TopBar.tOrange = TEX.GetTextureByName("orange");
		TopBar.tBlue = TEX.GetTextureByName("lightblue");
		TopBar.tGreen = TEX.GetTextureByName("green");
		this.OnResize();
	}

	public void LoadEnd()
	{
		TopBar.tHeader_logo = TEX.GetTextureByName("warlogo");
		TopBar.tPlay[0] = TEX.GetTextureByName("waricon_play");
		TopBar.tPlay[1] = TEX.GetTextureByName("waricon_play_hover");
		TopBar.tShop[0] = TEX.GetTextureByName("waricon_shop");
		TopBar.tShop[1] = TEX.GetTextureByName("waricon_shop_hover");
		TopBar.tOptions[0] = TEX.GetTextureByName("waricon_options");
		TopBar.tOptions[1] = TEX.GetTextureByName("waricon_options_hover");
		TopBar.tClose = TEX.GetTextureByName("shutdown");
	}

	public void OnResize()
	{
		TopBar.rButton[0] = new Rect((float)Screen.width / 2f - GUIM.YRES(10f) + GUIM.YRES(0f), 0f, GUIM.YRES(96f), GUIM.YRES(40f));
		TopBar.rButton[1] = new Rect((float)Screen.width / 2f - GUIM.YRES(10f) + GUIM.YRES(104f), 0f, GUIM.YRES(96f), GUIM.YRES(40f));
		TopBar.rButton[2] = new Rect((float)Screen.width / 2f - GUIM.YRES(10f) + GUIM.YRES(208f), 0f, GUIM.YRES(96f), GUIM.YRES(40f));
		TopBar.rClose = new Rect((float)Screen.width - GUIM.YRES(32f), GUIM.YRES(11f), GUIM.YRES(18f), GUIM.YRES(18f));
	}

	public static void Draw()
	{
		GUI.DrawTexture(new Rect(0f, 0f, (float)Screen.width, GUIM.YRES(40f)), TopBar.tBlack);
		TopBar.rHeader_logo = new Rect(0f, 4f, 256f, 32f);
		GUIM.DrawTextureX(TopBar.rHeader_logo, TopBar.tHeader_logo);
		TopBar.DrawButton(0, Lang.Get("_PLAY"), TopBar.rButton[0], TopBar.tPlay[0], TopBar.tPlay[1], TopBar.tOrange);
		TopBar.DrawButton(1, Lang.Get("_SHOP"), TopBar.rButton[1], TopBar.tShop[0], TopBar.tShop[1], TopBar.tGreen);
		TopBar.DrawButton(2, Lang.Get("_OPTIONS"), TopBar.rButton[2], TopBar.tOptions[0], TopBar.tOptions[1], TopBar.tBlue);
		if (GameData.gSteam)
		{
			TopBar.mpos = new Vector2(Input.mousePosition.x, (float)Screen.height - Input.mousePosition.y);
			if (TopBar.rClose.Contains(TopBar.mpos))
			{
				GUI.color = Color.red;
			}
			GUI.DrawTexture(TopBar.rClose, TopBar.tClose);
			GUI.color = Color.white;
			if (GUIM.HideButton(TopBar.rClose))
			{
				Application.Quit();
			}
		}
	}

	public static void DrawButton(int state, string text, Rect r, Texture2D normal, Texture2D hover, Texture2D line)
	{
		TopBar.mpos = new Vector2(Input.mousePosition.x, (float)Screen.height - Input.mousePosition.y);
		Rect position = new Rect(r.x + (r.width - GUIM.YRES(50f)) / 2f, r.y - GUIM.YRES(8f), GUIM.YRES(50f), GUIM.YRES(50f));
		if (TopBar.clicktime[state] + 0.05f > Time.time)
		{
			GUI.DrawTexture(r, line);
			GUI.color = Color.black;
			GUI.DrawTexture(position, normal);
			GUI.color = Color.white;
			GUI.DrawTexture(new Rect(r.x, r.y + GUIM.YRES(38f), r.width, GUIM.YRES(16f)), line);
			GUIM.DrawText(new Rect(r.x, r.y + GUIM.YRES(38f), r.width, GUIM.YRES(16f)), text, TextAnchor.MiddleCenter, BaseColor.White, 1, 14, true);
		}
		else if (r.Contains(TopBar.mpos))
		{
			TopBar.buttontime[state] = Time.time;
			GUI.DrawTexture(position, hover);
			GUI.DrawTexture(new Rect(r.x, r.y + GUIM.YRES(38f), r.width, GUIM.YRES(16f)), line);
			GUIM.DrawText(new Rect(r.x, r.y + GUIM.YRES(38f), r.width, GUIM.YRES(16f)), text, TextAnchor.MiddleCenter, BaseColor.White, 1, 14, true);
			if (Input.GetMouseButton(0))
			{
				TopBar.clicktime[state] = Time.time;
			}
		}
		else
		{
			float num = Time.time - TopBar.buttontime[state];
			float num2 = 16f - num * 100f;
			if (num2 < 2f)
			{
				num2 = 2f;
			}
			GUI.DrawTexture(position, normal);
			GUI.DrawTexture(new Rect(r.x, r.y + GUIM.YRES(38f), r.width, GUIM.YRES(num2)), line);
		}
		if (GUIM.HideButton(r))
		{
			Main.HideAll();
			if (state == 0)
			{
				MenuServers.SetActive(true);
			}
			if (state == 1)
			{
				MenuShop.SetActive(true);
			}
			if (state == 2)
			{
				MenuOptions.SetActive(true);
			}
		}
	}
}
