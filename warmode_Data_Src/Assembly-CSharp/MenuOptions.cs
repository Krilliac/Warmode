using System;
using UnityEngine;

public class MenuOptions : MonoBehaviour
{
	public static MenuOptions cs;

	private static bool show = false;

	private static float showtime = 0f;

	public static bool ingame = false;

	private static Rect rBack;

	private static Rect rBackSave;

	private static Rect rButtonSave;

	private static Rect rBackSaved;

	private static Texture2D tBlack;

	private static Texture2D tWhite;

	private static Texture2D tGray;

	public static int currCat = 0;

	public static bool saved = false;

	private static string[] restring;

	private static string[] defopt = new string[4];

	private static string[] sname = new string[3];

	private static string[] presetname = new string[4];

	private static string[] aaname = new string[4];

	private static string[] trigger = new string[2];

	private static bool WaitKey = false;

	private static Rect rWaitKey;

	public void LoadEnd()
	{
		MenuOptions.tBlack = TEX.GetTextureByName("black");
		MenuOptions.tWhite = TEX.GetTextureByName("white");
		MenuOptions.tGray = TEX.GetTextureByName("gray");
	}

	public void PostAwake()
	{
		MenuOptions.cs = this;
		MenuOptions.show = false;
		Resolution[] resolutions = Screen.resolutions;
		MenuOptions.restring = new string[resolutions.Length];
		for (int i = 0; i < resolutions.Length; i++)
		{
			MenuOptions.restring[i] = resolutions[i].width + "x" + resolutions[i].height;
		}
		MenuOptions.defopt[0] = Lang.Get("_LOW");
		MenuOptions.defopt[1] = Lang.Get("_MEDIUM");
		MenuOptions.defopt[2] = Lang.Get("_HIGH");
		MenuOptions.defopt[3] = Lang.Get("_VERY_HIGH");
		MenuOptions.sname[0] = Lang.Get("_OFF");
		MenuOptions.sname[1] = Lang.Get("_MEDIUM");
		MenuOptions.sname[2] = Lang.Get("_HIGH");
		MenuOptions.presetname[0] = Lang.Get("_LOW");
		MenuOptions.presetname[1] = Lang.Get("_MEDIUM");
		MenuOptions.presetname[2] = Lang.Get("_HIGH");
		MenuOptions.presetname[3] = Lang.Get("_CUSTOM");
		MenuOptions.aaname[0] = Lang.Get("_OFF");
		MenuOptions.aaname[1] = "2x";
		MenuOptions.aaname[2] = "4x";
		MenuOptions.aaname[3] = "8x";
		MenuOptions.trigger[0] = Lang.Get("_OFF");
		MenuOptions.trigger[1] = Lang.Get("_ON");
		this.OnResize();
	}

	public static void ForceCenter()
	{
		MenuOptions.ingame = true;
		MenuOptions.rBack = new Rect((float)Screen.width / 2f - GUIM.YRES(165f), GUIM.YRES(80f), GUIM.YRES(632f), GUIM.YRES(525f));
		if (MenuOptions.ingame)
		{
			MenuOptions.rBack.center = new Vector2((float)Screen.width / 2f, (float)Screen.height / 2f);
		}
		MenuOptions.rBackSave = new Rect(MenuOptions.rBack.x + MenuOptions.rBack.width - GUIM.YRES(200f), MenuOptions.rBack.y + MenuOptions.rBack.height + GUIM.YRES(8f), GUIM.YRES(200f), GUIM.YRES(32f));
		MenuOptions.rButtonSave = new Rect(MenuOptions.rBackSave.x + GUIM.YRES(4f), MenuOptions.rBackSave.y + GUIM.YRES(4f), GUIM.YRES(192f), GUIM.YRES(24f));
		MenuOptions.rBackSaved = new Rect(MenuOptions.rBack.x, MenuOptions.rBack.y + MenuOptions.rBack.height + GUIM.YRES(8f), GUIM.YRES(200f), GUIM.YRES(32f));
	}

	public static void SetActive(bool val)
	{
		MenuOptions.show = val;
		if (MenuOptions.show)
		{
			Options.Load();
			if (Options.resolution >= Screen.resolutions.Length)
			{
				Options.resolution = Screen.resolutions.Length - 1;
			}
			MenuOptions.saved = false;
			MenuOptions.showtime = Time.time;
		}
		MenuOptions.WaitKey = false;
	}

	public void OnResize()
	{
		MenuOptions.rBack = new Rect((float)Screen.width / 2f - GUIM.YRES(165f), GUIM.YRES(80f), GUIM.YRES(632f), GUIM.YRES(525f));
		if (MenuOptions.ingame)
		{
			MenuOptions.rBack.center = new Vector2((float)Screen.width / 2f, (float)Screen.height / 2f);
		}
		MenuOptions.rBackSave = new Rect(MenuOptions.rBack.x + MenuOptions.rBack.width - GUIM.YRES(200f), MenuOptions.rBack.y + MenuOptions.rBack.height + GUIM.YRES(8f), GUIM.YRES(200f), GUIM.YRES(32f));
		MenuOptions.rButtonSave = new Rect(MenuOptions.rBackSave.x + GUIM.YRES(4f), MenuOptions.rBackSave.y + GUIM.YRES(4f), GUIM.YRES(192f), GUIM.YRES(24f));
		MenuOptions.rBackSaved = new Rect(MenuOptions.rBack.x, MenuOptions.rBack.y + MenuOptions.rBack.height + GUIM.YRES(8f), GUIM.YRES(200f), GUIM.YRES(32f));
	}

	public static void Draw()
	{
		if (!MenuOptions.show)
		{
			return;
		}
		float num = Time.time - MenuOptions.showtime + 0.001f;
		if (num > 0.05f)
		{
			num = 0.05f;
		}
		num *= 20f;
		Matrix4x4 matrix = GUI.matrix;
		Vector3 s = new Vector3(num, num, 1f);
		Vector3 pos = new Vector3(MenuOptions.rBack.center.x - MenuOptions.rBack.center.x * num, MenuOptions.rBack.center.y - MenuOptions.rBack.center.y * num, 1f);
		GUI.matrix = Matrix4x4.TRS(pos, Quaternion.identity, s);
		GUIM.DrawBox(MenuOptions.rBack, MenuOptions.tBlack);
		GUIM.DrawBox(MenuOptions.rBackSave, MenuOptions.tBlack);
		MenuOptions.DrawButtonOption(0, new Rect(MenuOptions.rBack.x + GUIM.YRES(4f), MenuOptions.rBack.y + GUIM.YRES(4f), GUIM.YRES(120f), GUIM.YRES(24f)), Lang.Get("_VIDEO"));
		MenuOptions.DrawButtonOption(1, new Rect(MenuOptions.rBack.x + GUIM.YRES(4f) + GUIM.YRES(124f) * 1f, MenuOptions.rBack.y + GUIM.YRES(4f), GUIM.YRES(120f), GUIM.YRES(24f)), Lang.Get("_AUDIO"));
		MenuOptions.DrawButtonOption(2, new Rect(MenuOptions.rBack.x + GUIM.YRES(4f) + GUIM.YRES(124f) * 2f, MenuOptions.rBack.y + GUIM.YRES(4f), GUIM.YRES(120f), GUIM.YRES(24f)), Lang.Get("_GAME"));
		MenuOptions.DrawButtonOption(3, new Rect(MenuOptions.rBack.x + GUIM.YRES(4f) + GUIM.YRES(124f) * 3f, MenuOptions.rBack.y + GUIM.YRES(4f), GUIM.YRES(120f), GUIM.YRES(24f)), Lang.Get("_CONTROL"));
		if (MenuOptions.currCat == 0)
		{
			MenuOptions.DrawVideo();
		}
		else if (MenuOptions.currCat == 1)
		{
			MenuOptions.DrawAudio();
		}
		else if (MenuOptions.currCat == 2)
		{
			MenuOptions.DrawGame();
		}
		else if (MenuOptions.currCat == 3)
		{
			MenuOptions.DrawControl();
		}
		if (GUIM.Button(MenuOptions.rButtonSave, BaseColor.Blue, Lang.Get("_SAVE"), TextAnchor.MiddleCenter, BaseColor.White, 1, 12, true))
		{
			Options.Save();
			Options.Apply();
			Options.ApplyResolution();
			MenuOptions.saved = true;
			if (MenuOptions.ingame)
			{
				Options.ApplyInGame();
			}
			Main.lastwidth = 0f;
			HUD.lastwidth = 0f;
		}
		if (MenuOptions.saved)
		{
			GUIM.DrawBox(MenuOptions.rBackSaved, MenuOptions.tBlack);
			GUIM.DrawText(MenuOptions.rBackSaved, Lang.Get("_OPTIONS_SAVED"), TextAnchor.MiddleCenter, BaseColor.Gray, 1, 12, false);
		}
		GUI.matrix = matrix;
	}

	private static void DrawButtonOption(int cat, Rect r, string name)
	{
		bool flag;
		if (MenuOptions.currCat == cat)
		{
			flag = GUIM.Button(r, BaseColor.White, name, TextAnchor.MiddleCenter, BaseColor.Blue, 1, 12, false);
		}
		else
		{
			flag = GUIM.Button(r, BaseColor.Gray, name, TextAnchor.MiddleCenter, BaseColor.White, 1, 12, false);
		}
		if (flag)
		{
			MenuOptions.currCat = cat;
			MenuOptions.WaitKey = false;
		}
	}

	private static void DrawVideo()
	{
		MenuOptions.DrawParamInt(new Rect(MenuOptions.rBack.x + GUIM.YRES(32f), MenuOptions.rBack.y + GUIM.YRES(48f) + GUIM.YRES(28f) * 0f, MenuOptions.rBack.width - GUIM.YRES(64f), GUIM.YRES(24f)), Lang.Get("_DISPLAY_RESOLUTION"), ref Options.resolution, MenuOptions.restring);
		if (MenuOptions.DrawParamInt(new Rect(MenuOptions.rBack.x + GUIM.YRES(32f), MenuOptions.rBack.y + GUIM.YRES(48f) + GUIM.YRES(28f) * 1f, MenuOptions.rBack.width - GUIM.YRES(64f), GUIM.YRES(24f)), Lang.Get("_PRESET"), ref Options.preset, MenuOptions.presetname))
		{
			if (Options.preset == 3)
			{
				Options.preset = 2;
			}
			if (Options.preset == 0)
			{
				Options.shadows = 0;
				Options.antialiasing = 0;
				Options.colorcorrection = 1;
				Options.posteffects = 0;
				Options.ssao = 0;
				Options.sharpness = 0;
				Options.noise = 0;
				Options.tone = 0;
				Options.vig = 0;
				Options.vsync = 0;
			}
			if (Options.preset == 1)
			{
				Options.shadows = 1;
				Options.antialiasing = 0;
				Options.colorcorrection = 1;
				Options.posteffects = 0;
				Options.ssao = 0;
				Options.sharpness = 0;
				Options.noise = 0;
				Options.tone = 0;
				Options.vig = 0;
				Options.vsync = 0;
			}
			if (Options.preset == 2)
			{
				Options.shadows = 2;
				Options.antialiasing = 1;
				Options.colorcorrection = 1;
				Options.posteffects = 1;
				Options.ssao = 1;
				Options.sharpness = 1;
				Options.noise = 1;
				Options.tone = 1;
				Options.vig = 1;
				Options.vsync = 1;
			}
		}
		bool flag = false;
		if (MenuOptions.DrawParamFloat(new Rect(MenuOptions.rBack.x + GUIM.YRES(32f), MenuOptions.rBack.y + GUIM.YRES(48f) + GUIM.YRES(28f) * 2f, MenuOptions.rBack.width - GUIM.YRES(64f), GUIM.YRES(24f)), Lang.Get("_BRIGHTNESS"), ref Options.brightness, 0.5f, 2f))
		{
			flag = true;
		}
		if (MenuOptions.DrawParamFloat(new Rect(MenuOptions.rBack.x + GUIM.YRES(32f), MenuOptions.rBack.y + GUIM.YRES(48f) + GUIM.YRES(28f) * 3f, MenuOptions.rBack.width - GUIM.YRES(64f), GUIM.YRES(24f)), Lang.Get("_GAMMA"), ref Options.gamma, 0.5f, 2f))
		{
			flag = true;
		}
		if (flag)
		{
			Options.ApplyBrightness();
		}
		if (MenuOptions.DrawParamInt(new Rect(MenuOptions.rBack.x + GUIM.YRES(32f), MenuOptions.rBack.y + GUIM.YRES(64f) + GUIM.YRES(28f) * 5f, MenuOptions.rBack.width - GUIM.YRES(64f), GUIM.YRES(24f)), Lang.Get("_SHADOWS_QUALITY"), ref Options.shadows, MenuOptions.sname))
		{
			Options.preset = 3;
		}
		if (MenuOptions.DrawParamInt(new Rect(MenuOptions.rBack.x + GUIM.YRES(32f), MenuOptions.rBack.y + GUIM.YRES(64f) + GUIM.YRES(28f) * 6f, MenuOptions.rBack.width - GUIM.YRES(64f), GUIM.YRES(24f)), Lang.Get("_ANTI_ALIASING"), ref Options.antialiasing, MenuOptions.aaname))
		{
			Options.preset = 3;
		}
		if (MenuOptions.DrawParamInt(new Rect(MenuOptions.rBack.x + GUIM.YRES(32f), MenuOptions.rBack.y + GUIM.YRES(64f) + GUIM.YRES(28f) * 7f, MenuOptions.rBack.width - GUIM.YRES(64f), GUIM.YRES(24f)), Lang.Get("_VSYNC"), ref Options.vsync, null))
		{
			Options.preset = 3;
		}
		if (MenuOptions.DrawParamInt(new Rect(MenuOptions.rBack.x + GUIM.YRES(32f), MenuOptions.rBack.y + GUIM.YRES(64f) + GUIM.YRES(28f) * 8f, MenuOptions.rBack.width - GUIM.YRES(64f), GUIM.YRES(24f)), Lang.Get("_COLOR_CORRECTION"), ref Options.colorcorrection, null))
		{
			Options.preset = 3;
		}
		if (MenuOptions.DrawParamInt(new Rect(MenuOptions.rBack.x + GUIM.YRES(32f), MenuOptions.rBack.y + GUIM.YRES(64f) + GUIM.YRES(28f) * 9f, MenuOptions.rBack.width - GUIM.YRES(64f), GUIM.YRES(24f)), Lang.Get("_POSTEFFECTS"), ref Options.posteffects, null))
		{
			Options.preset = 3;
		}
		if (Options.posteffects == 1)
		{
			if (MenuOptions.DrawParamInt(new Rect(MenuOptions.rBack.x + GUIM.YRES(32f), MenuOptions.rBack.y + GUIM.YRES(64f) + GUIM.YRES(28f) * 10f, MenuOptions.rBack.width - GUIM.YRES(64f), GUIM.YRES(24f)), Lang.Get("_SSAO"), ref Options.ssao, null))
			{
				Options.preset = 3;
			}
			if (MenuOptions.DrawParamInt(new Rect(MenuOptions.rBack.x + GUIM.YRES(32f), MenuOptions.rBack.y + GUIM.YRES(64f) + GUIM.YRES(28f) * 11f, MenuOptions.rBack.width - GUIM.YRES(64f), GUIM.YRES(24f)), Lang.Get("_SHARPNESS"), ref Options.sharpness, null))
			{
				Options.preset = 3;
			}
			if (MenuOptions.DrawParamInt(new Rect(MenuOptions.rBack.x + GUIM.YRES(32f), MenuOptions.rBack.y + GUIM.YRES(64f) + GUIM.YRES(28f) * 12f, MenuOptions.rBack.width - GUIM.YRES(64f), GUIM.YRES(24f)), Lang.Get("_NOISE"), ref Options.noise, null))
			{
				Options.preset = 3;
			}
			if (MenuOptions.DrawParamInt(new Rect(MenuOptions.rBack.x + GUIM.YRES(32f), MenuOptions.rBack.y + GUIM.YRES(64f) + GUIM.YRES(28f) * 13f, MenuOptions.rBack.width - GUIM.YRES(64f), GUIM.YRES(24f)), Lang.Get("_TONE"), ref Options.tone, null))
			{
				Options.preset = 3;
			}
			if (MenuOptions.DrawParamInt(new Rect(MenuOptions.rBack.x + GUIM.YRES(32f), MenuOptions.rBack.y + GUIM.YRES(64f) + GUIM.YRES(28f) * 14f, MenuOptions.rBack.width - GUIM.YRES(64f), GUIM.YRES(24f)), Lang.Get("_VIG"), ref Options.vig, null))
			{
				Options.preset = 3;
			}
		}
	}

	private static void DrawAudio()
	{
		MenuOptions.DrawParamFloat(new Rect(MenuOptions.rBack.x + GUIM.YRES(32f), MenuOptions.rBack.y + GUIM.YRES(48f) + GUIM.YRES(28f) * 0f, MenuOptions.rBack.width - GUIM.YRES(64f), GUIM.YRES(24f)), Lang.Get("_GAME_VOLUME"), ref Options.gamevol, 0f, 1f);
		MenuOptions.DrawParamFloat(new Rect(MenuOptions.rBack.x + GUIM.YRES(32f), MenuOptions.rBack.y + GUIM.YRES(48f) + GUIM.YRES(28f) * 1f, MenuOptions.rBack.width - GUIM.YRES(64f), GUIM.YRES(24f)), Lang.Get("_MENU_VOLUME"), ref Options.menuvol, 0f, 1f);
	}

	private static void DrawGame()
	{
		MenuOptions.DrawParamFloat(new Rect(MenuOptions.rBack.x + GUIM.YRES(32f), MenuOptions.rBack.y + GUIM.YRES(48f) + GUIM.YRES(28f) * 0f, MenuOptions.rBack.width - GUIM.YRES(64f), GUIM.YRES(24f)), Lang.Get("_SENS"), ref Options.sens, 0.1f, 25f);
		MenuOptions.DrawParamFloat(new Rect(MenuOptions.rBack.x + GUIM.YRES(32f), MenuOptions.rBack.y + GUIM.YRES(48f) + GUIM.YRES(28f) * 1f, MenuOptions.rBack.width - GUIM.YRES(64f), GUIM.YRES(24f)), Lang.Get("_ZOOM_SENS"), ref Options.zoomsens, 0.1f, 25f);
		MenuOptions.DrawParamInt(new Rect(MenuOptions.rBack.x + GUIM.YRES(32f), MenuOptions.rBack.y + GUIM.YRES(48f) + GUIM.YRES(28f) * 2f, MenuOptions.rBack.width - GUIM.YRES(64f), GUIM.YRES(24f)), Lang.Get("_ZOOM_LOCK"), ref Options.zoomlock, null);
		MenuOptions.DrawParamInt(new Rect(MenuOptions.rBack.x + GUIM.YRES(32f), MenuOptions.rBack.y + GUIM.YRES(48f) + GUIM.YRES(28f) * 3f, MenuOptions.rBack.width - GUIM.YRES(64f), GUIM.YRES(24f)), Lang.Get("_INVERT_MOUSE"), ref Options.invertmouse, null);
		MenuOptions.DrawParamInt(new Rect(MenuOptions.rBack.x + GUIM.YRES(32f), MenuOptions.rBack.y + GUIM.YRES(48f) + GUIM.YRES(28f) * 4f, MenuOptions.rBack.width - GUIM.YRES(64f), GUIM.YRES(24f)), Lang.Get("_MOUSE_RAW_INPUT"), ref Options.rawinput, null);
		MenuOptions.DrawParamInt(new Rect(MenuOptions.rBack.x + GUIM.YRES(32f), MenuOptions.rBack.y + GUIM.YRES(48f) + GUIM.YRES(28f) * 5f, MenuOptions.rBack.width - GUIM.YRES(64f), GUIM.YRES(24f)), Lang.Get("_LIGHT_GAME_BUY"), ref Options.gamebuy, null);
		MenuOptions.DrawParamInt(new Rect(MenuOptions.rBack.x + GUIM.YRES(32f), MenuOptions.rBack.y + GUIM.YRES(48f) + GUIM.YRES(28f) * 6f, MenuOptions.rBack.width - GUIM.YRES(64f), GUIM.YRES(24f)), Lang.Get("_DYNAMIC_CROSSHAIR"), ref Options.dynamiccrosshair, null);
	}

	private static void DrawControl()
	{
		MenuOptions.DrawParamKey(new Rect(MenuOptions.rBack.x + GUIM.YRES(32f), MenuOptions.rBack.y + GUIM.YRES(48f) + GUIM.YRES(28f) * 0f, MenuOptions.rBack.width / 2f - GUIM.YRES(80f), GUIM.YRES(24f)), "W", ref vp_FPInput.control[0]);
		MenuOptions.DrawParamKey(new Rect(MenuOptions.rBack.x + GUIM.YRES(32f), MenuOptions.rBack.y + GUIM.YRES(48f) + GUIM.YRES(28f) * 1f, MenuOptions.rBack.width / 2f - GUIM.YRES(80f), GUIM.YRES(24f)), "A", ref vp_FPInput.control[1]);
		MenuOptions.DrawParamKey(new Rect(MenuOptions.rBack.x + GUIM.YRES(32f), MenuOptions.rBack.y + GUIM.YRES(48f) + GUIM.YRES(28f) * 2f, MenuOptions.rBack.width / 2f - GUIM.YRES(80f), GUIM.YRES(24f)), "S", ref vp_FPInput.control[2]);
		MenuOptions.DrawParamKey(new Rect(MenuOptions.rBack.x + GUIM.YRES(32f), MenuOptions.rBack.y + GUIM.YRES(48f) + GUIM.YRES(28f) * 3f, MenuOptions.rBack.width / 2f - GUIM.YRES(80f), GUIM.YRES(24f)), "D", ref vp_FPInput.control[3]);
		MenuOptions.DrawParamKey(new Rect(MenuOptions.rBack.x + GUIM.YRES(32f), MenuOptions.rBack.y + GUIM.YRES(48f) + GUIM.YRES(28f) * 4f, MenuOptions.rBack.width / 2f - GUIM.YRES(80f), GUIM.YRES(24f)), Lang.Get("_SPRINT"), ref vp_FPInput.control[4]);
		MenuOptions.DrawParamKey(new Rect(MenuOptions.rBack.x + GUIM.YRES(32f), MenuOptions.rBack.y + GUIM.YRES(48f) + GUIM.YRES(28f) * 5f, MenuOptions.rBack.width / 2f - GUIM.YRES(80f), GUIM.YRES(24f)), Lang.Get("_CREEP"), ref vp_FPInput.control[20]);
		MenuOptions.DrawParamKey(new Rect(MenuOptions.rBack.x + GUIM.YRES(32f), MenuOptions.rBack.y + GUIM.YRES(48f) + GUIM.YRES(28f) * 6f, MenuOptions.rBack.width / 2f - GUIM.YRES(80f), GUIM.YRES(24f)), Lang.Get("_JUMP"), ref vp_FPInput.control[5]);
		MenuOptions.DrawParamKey(new Rect(MenuOptions.rBack.x + GUIM.YRES(32f), MenuOptions.rBack.y + GUIM.YRES(48f) + GUIM.YRES(28f) * 7f, MenuOptions.rBack.width / 2f - GUIM.YRES(80f), GUIM.YRES(24f)), Lang.Get("_CROUCH"), ref vp_FPInput.control[6]);
		MenuOptions.DrawParamKey(new Rect(MenuOptions.rBack.x + GUIM.YRES(32f), MenuOptions.rBack.y + GUIM.YRES(48f) + GUIM.YRES(28f) * 8f, MenuOptions.rBack.width / 2f - GUIM.YRES(80f), GUIM.YRES(24f)), Lang.Get("_RELOAD"), ref vp_FPInput.control[7]);
		MenuOptions.DrawParamKey(new Rect(MenuOptions.rBack.x + GUIM.YRES(32f), MenuOptions.rBack.y + GUIM.YRES(48f) + GUIM.YRES(28f) * 9f, MenuOptions.rBack.width / 2f - GUIM.YRES(80f), GUIM.YRES(24f)), Lang.Get("_LAST_WEAPON"), ref vp_FPInput.control[8]);
		MenuOptions.DrawParamKey(new Rect(MenuOptions.rBack.x + GUIM.YRES(32f), MenuOptions.rBack.y + GUIM.YRES(48f) + GUIM.YRES(28f) * 10f, MenuOptions.rBack.width / 2f - GUIM.YRES(80f), GUIM.YRES(24f)), Lang.Get("_DROP_WEAPON"), ref vp_FPInput.control[11]);
		MenuOptions.DrawParamKey(new Rect(MenuOptions.rBack.x + GUIM.YRES(32f), MenuOptions.rBack.y + GUIM.YRES(48f) + GUIM.YRES(28f) * 11f, MenuOptions.rBack.width / 2f - GUIM.YRES(80f), GUIM.YRES(24f)), Lang.Get("_FAST_KNIFE"), ref vp_FPInput.control[9]);
		MenuOptions.DrawParamKey(new Rect(MenuOptions.rBack.x + GUIM.YRES(32f), MenuOptions.rBack.y + GUIM.YRES(48f) + GUIM.YRES(28f) * 12f, MenuOptions.rBack.width / 2f - GUIM.YRES(80f), GUIM.YRES(24f)), Lang.Get("_THROW_GRENADE"), ref vp_FPInput.control[10]);
		MenuOptions.DrawParamKey(new Rect(MenuOptions.rBack.x + GUIM.YRES(360f), MenuOptions.rBack.y + GUIM.YRES(48f) + GUIM.YRES(28f) * 0f, MenuOptions.rBack.width / 2f - GUIM.YRES(76f), GUIM.YRES(24f)), Lang.Get("_CHOOSE_TEAM"), ref vp_FPInput.control[16]);
		MenuOptions.DrawParamKey(new Rect(MenuOptions.rBack.x + GUIM.YRES(360f), MenuOptions.rBack.y + GUIM.YRES(48f) + GUIM.YRES(28f) * 1f, MenuOptions.rBack.width / 2f - GUIM.YRES(76f), GUIM.YRES(24f)), Lang.Get("_SCOREBOARD"), ref vp_FPInput.control[17]);
		MenuOptions.DrawParamKey(new Rect(MenuOptions.rBack.x + GUIM.YRES(360f), MenuOptions.rBack.y + GUIM.YRES(48f) + GUIM.YRES(28f) * 2f, MenuOptions.rBack.width / 2f - GUIM.YRES(76f), GUIM.YRES(24f)), Lang.Get("_TEAM_CHAT"), ref vp_FPInput.control[18]);
		MenuOptions.DrawParamKey(new Rect(MenuOptions.rBack.x + GUIM.YRES(360f), MenuOptions.rBack.y + GUIM.YRES(48f) + GUIM.YRES(28f) * 3f, MenuOptions.rBack.width / 2f - GUIM.YRES(76f), GUIM.YRES(24f)), Lang.Get("_BUY_MENU"), ref vp_FPInput.control[19]);
		MenuOptions.DrawParamKey(new Rect(MenuOptions.rBack.x + GUIM.YRES(360f), MenuOptions.rBack.y + GUIM.YRES(48f) + GUIM.YRES(28f) * 4f, MenuOptions.rBack.width / 2f - GUIM.YRES(76f), GUIM.YRES(24f)), Lang.Get("_AMMUNITION_BUY"), ref vp_FPInput.control[21]);
		if (GUIM.Button(new Rect(MenuOptions.rBack.x + GUIM.YRES(32f), MenuOptions.rBack.y + GUIM.YRES(48f) + GUIM.YRES(28f) * 15f, GUIM.YRES(120f), GUIM.YRES(24f)), BaseColor.Gray, Lang.Get("_RESET"), TextAnchor.MiddleCenter, BaseColor.White, 0, 14, false))
		{
			PlayerPrefs.DeleteKey("control");
			Options.LoadControl();
		}
	}

	private static bool DrawParamInt(Rect r, string text, ref int param, string[] options)
	{
		bool result = false;
		BaseColor fontcolor = BaseColor.White;
		if (options == null && param == 0)
		{
			fontcolor = BaseColor.Gray;
		}
		GUIM.DrawText(r, text, TextAnchor.MiddleLeft, fontcolor, 1, 12, true);
		if (options == null)
		{
			Rect r2 = new Rect(r.x + r.width - GUIM.YRES(68f), r.y, GUIM.YRES(32f), GUIM.YRES(24f));
			bool flag;
			if (param == 1)
			{
				flag = GUIM.Button(r2, BaseColor.White, MenuOptions.trigger[1], TextAnchor.MiddleCenter, BaseColor.Blue, 1, 10, false);
			}
			else
			{
				flag = GUIM.Button(r2, BaseColor.Gray, MenuOptions.trigger[1], TextAnchor.MiddleCenter, BaseColor.White, 1, 10, false);
			}
			if (flag)
			{
				param = 1;
				result = true;
			}
			Rect r3 = new Rect(r.x + r.width - GUIM.YRES(32f), r.y, GUIM.YRES(32f), GUIM.YRES(24f));
			if (param == 0)
			{
				flag = GUIM.Button(r3, BaseColor.White, MenuOptions.trigger[0], TextAnchor.MiddleCenter, BaseColor.Blue, 1, 10, false);
			}
			else
			{
				flag = GUIM.Button(r3, BaseColor.Gray, MenuOptions.trigger[0], TextAnchor.MiddleCenter, BaseColor.White, 1, 10, false);
			}
			if (flag)
			{
				param = 0;
				result = true;
			}
		}
		else if (options.Length >= 1)
		{
			Rect rect = new Rect(r.x + r.width - GUIM.YRES(32f) - GUIM.YRES(4f) - GUIM.YRES(200f), r.y, GUIM.YRES(200f), GUIM.YRES(24f));
			GUI.DrawTexture(rect, MenuOptions.tGray);
			GUIM.DrawText(rect, options[param], TextAnchor.MiddleCenter, BaseColor.White, 1, 12, true);
			Rect rect2 = new Rect(rect.x - GUIM.YRES(4f) - GUIM.YRES(32f), r.y, GUIM.YRES(32f), GUIM.YRES(24f));
			GUI.DrawTexture(rect2, MenuOptions.tGray);
			bool flag = GUIM.Button(rect2, BaseColor.Gray, "-", TextAnchor.MiddleCenter, BaseColor.White, 1, 16, false);
			if (flag)
			{
				param--;
				result = true;
			}
			Rect r4 = new Rect(r.x + r.width - GUIM.YRES(32f), r.y, GUIM.YRES(32f), GUIM.YRES(24f));
			flag = GUIM.Button(r4, BaseColor.Gray, "+", TextAnchor.MiddleCenter, BaseColor.White, 1, 16, false);
			if (flag)
			{
				param++;
				result = true;
			}
			if (param < 0)
			{
				param = 0;
				result = false;
			}
			if (param >= options.Length)
			{
				param = options.Length - 1;
				result = false;
			}
		}
		return result;
	}

	private static bool DrawParamFloat(Rect r, string text, ref float param, float min, float max)
	{
		float num = param;
		GUIM.DrawText(r, text, TextAnchor.MiddleLeft, BaseColor.White, 1, 12, true);
		param = GUIM.DrawSlider(new Rect(r.x + r.width - GUIM.YRES(240f), r.y + GUIM.YRES(6f), GUIM.YRES(200f), GUIM.YRES(24f)), (int)GUIM.YRES(200f), min, max, param);
		param = ((float)((int)(param * 10f)) + 0.05f) / 10f;
		GUI.DrawTexture(new Rect(r.x + r.width - GUIM.YRES(32f), r.y, GUIM.YRES(32f), GUIM.YRES(24f)), MenuOptions.tGray);
		GUIM.DrawText(new Rect(r.x + r.width - GUIM.YRES(32f), r.y, GUIM.YRES(32f), GUIM.YRES(24f)), param.ToString("0.0"), TextAnchor.MiddleCenter, BaseColor.White, 1, 12, true);
		return param == num;
	}

	private static void DrawParamKey(Rect r, string text, ref KeyCode param)
	{
		GUIM.DrawText(r, text, TextAnchor.MiddleLeft, BaseColor.White, 1, 12, true);
		Rect r2 = new Rect(r.x + r.width - GUIM.YRES(80f), r.y, GUIM.YRES(80f), GUIM.YRES(24f));
		bool flag = GUIM.Button(r2, BaseColor.Gray, param.ToString(), TextAnchor.MiddleCenter, BaseColor.White, 1, 14, false);
		if (flag)
		{
			MenuOptions.WaitKey = true;
			MenuOptions.rWaitKey = r;
		}
		if (MenuOptions.WaitKey && MenuOptions.rWaitKey == r)
		{
			GUIM.Button(r2, BaseColor.Red, "_", TextAnchor.MiddleCenter, BaseColor.White, 1, 14, false);
			if (Input.GetKeyDown(KeyCode.Escape))
			{
				MenuOptions.WaitKey = false;
			}
			for (int i = 0; i < 32; i++)
			{
				if (vp_FPInput.control[i] == Event.current.keyCode)
				{
					return;
				}
			}
			if (Event.current.isKey)
			{
				param = Event.current.keyCode;
				MenuOptions.WaitKey = false;
			}
			if (Event.current.shift)
			{
				param = KeyCode.LeftShift;
				MenuOptions.WaitKey = false;
			}
		}
	}
}
