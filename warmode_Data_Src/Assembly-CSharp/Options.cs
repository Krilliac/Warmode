using System;
using UnityEngine;
using UnityStandardAssets.ImageEffects;

public class Options : MonoBehaviour
{
	public static int preset = 1;

	public static int resolution;

	public static float brightness = 1f;

	public static float gamma = 1f;

	public static int shadows = 1;

	public static int antialiasing;

	public static int vsync;

	public static int colorcorrection = 1;

	public static int posteffects;

	public static int ssao;

	public static int sharpness;

	public static int noise;

	public static int tone;

	public static int vig;

	public static float gamevol = 0.25f;

	public static float menuvol = 0.25f;

	public static float sens = 3f;

	public static float zoomsens = 2f;

	public static int zoomlock = 1;

	public static int invertmouse;

	public static int rawinput;

	public static int gamebuy;

	public static int dynamiccrosshair = 1;

	public static int xWeaponLight;

	public static int xWeaponShadow;

	public static int xGrenade;

	public static int xLightmapShadow;

	public static int ccx = 255;

	public static int ccy = 255;

	public static int ccz = 255;

	private static bool resonce;

	public static void Load()
	{
		if (PlayerPrefs.HasKey("preset"))
		{
			Options.preset = PlayerPrefs.GetInt("preset");
		}
		if (PlayerPrefs.HasKey("resolution"))
		{
			Options.resolution = PlayerPrefs.GetInt("resolution");
		}
		else
		{
			int num = Screen.resolutions.Length;
			float num2 = (float)Screen.resolutions[num - 1].width;
			float num3 = (float)Screen.resolutions[num - 1].height;
			float num4 = (float)((int)(num2 / num3 * 10f)) / 10f;
			for (int i = 0; i < num; i++)
			{
				if (num4 == 1.3f && Screen.resolutions[i].width == 1024 && Screen.resolutions[i].height == 768)
				{
					Options.resolution = i;
					break;
				}
				if (num4 == 1.7f && Screen.resolutions[i].width == 1280 && Screen.resolutions[i].height == 720)
				{
					Options.resolution = i;
					break;
				}
				if (num4 == 1.6f && Screen.resolutions[i].width == 1280 && Screen.resolutions[i].height == 800)
				{
					Options.resolution = i;
					break;
				}
			}
		}
		if (PlayerPrefs.HasKey("shadows"))
		{
			Options.shadows = PlayerPrefs.GetInt("shadows");
		}
		if (PlayerPrefs.HasKey("antialiasing"))
		{
			Options.antialiasing = PlayerPrefs.GetInt("antialiasing");
		}
		if (PlayerPrefs.HasKey("vsync"))
		{
			Options.vsync = PlayerPrefs.GetInt("vsync");
		}
		if (PlayerPrefs.HasKey("colorcorrection"))
		{
			Options.colorcorrection = PlayerPrefs.GetInt("colorcorrection");
		}
		if (PlayerPrefs.HasKey("posteffects"))
		{
			Options.posteffects = PlayerPrefs.GetInt("posteffects");
		}
		if (PlayerPrefs.HasKey("brightness"))
		{
			Options.brightness = PlayerPrefs.GetFloat("brightness");
		}
		if (PlayerPrefs.HasKey("gamma"))
		{
			Options.gamma = PlayerPrefs.GetFloat("gamma");
		}
		if (PlayerPrefs.HasKey("ssao"))
		{
			Options.ssao = PlayerPrefs.GetInt("ssao");
		}
		if (PlayerPrefs.HasKey("sharpness"))
		{
			Options.sharpness = PlayerPrefs.GetInt("sharpness");
		}
		if (PlayerPrefs.HasKey("noise"))
		{
			Options.noise = PlayerPrefs.GetInt("noise");
		}
		if (PlayerPrefs.HasKey("tone"))
		{
			Options.tone = PlayerPrefs.GetInt("tone");
		}
		if (PlayerPrefs.HasKey("vig"))
		{
			Options.vig = PlayerPrefs.GetInt("vig");
		}
		if (PlayerPrefs.HasKey("ccx") && PlayerPrefs.HasKey("ccy") && PlayerPrefs.HasKey("ccz"))
		{
			Options.ccx = PlayerPrefs.GetInt("ccx");
			Options.ccy = PlayerPrefs.GetInt("ccy");
			Options.ccz = PlayerPrefs.GetInt("ccz");
		}
		else
		{
			Options.ccx = 255;
			Options.ccy = 255;
			Options.ccz = 255;
		}
		if (PlayerPrefs.HasKey("gamevol"))
		{
			Options.gamevol = PlayerPrefs.GetFloat("gamevol");
		}
		if (PlayerPrefs.HasKey("menuvol"))
		{
			Options.menuvol = PlayerPrefs.GetFloat("menuvol");
		}
		if (PlayerPrefs.HasKey("sens"))
		{
			Options.sens = PlayerPrefs.GetFloat("sens");
		}
		if (PlayerPrefs.HasKey("zoomsens"))
		{
			Options.zoomsens = PlayerPrefs.GetFloat("zoomsens");
		}
		if (PlayerPrefs.HasKey("zoomlock"))
		{
			Options.zoomlock = PlayerPrefs.GetInt("zoomlock");
		}
		if (PlayerPrefs.HasKey("invertmouse"))
		{
			Options.invertmouse = PlayerPrefs.GetInt("invertmouse");
		}
		if (PlayerPrefs.HasKey("rawinput"))
		{
			Options.rawinput = PlayerPrefs.GetInt("rawinput");
		}
		if (PlayerPrefs.HasKey("gamebuy"))
		{
			Options.gamebuy = PlayerPrefs.GetInt("gamebuy");
		}
		if (PlayerPrefs.HasKey("dynamiccrosshair"))
		{
			Options.dynamiccrosshair = PlayerPrefs.GetInt("dynamiccrosshair");
		}
		Options.LoadControl();
	}

	public static void Save()
	{
		PlayerPrefs.SetInt("preset", Options.preset);
		PlayerPrefs.SetInt("resolution", Options.resolution);
		PlayerPrefs.SetInt("shadows", Options.shadows);
		PlayerPrefs.SetInt("antialiasing", Options.antialiasing);
		PlayerPrefs.SetInt("vsync", Options.vsync);
		PlayerPrefs.SetInt("colorcorrection", Options.colorcorrection);
		PlayerPrefs.SetInt("posteffects", Options.posteffects);
		PlayerPrefs.SetFloat("brightness", Options.brightness);
		PlayerPrefs.SetFloat("gamma", Options.gamma);
		PlayerPrefs.SetInt("ssao", Options.ssao);
		PlayerPrefs.SetInt("sharpness", Options.sharpness);
		PlayerPrefs.SetInt("noise", Options.noise);
		PlayerPrefs.SetInt("tone", Options.tone);
		PlayerPrefs.SetInt("vig", Options.vig);
		PlayerPrefs.SetInt("ccx", Options.ccx);
		PlayerPrefs.SetInt("ccy", Options.ccy);
		PlayerPrefs.SetInt("ccz", Options.ccz);
		PlayerPrefs.SetFloat("gamevol", Options.gamevol);
		PlayerPrefs.SetFloat("menuvol", Options.menuvol);
		PlayerPrefs.SetFloat("sens", Options.sens);
		PlayerPrefs.SetFloat("zoomsens", Options.zoomsens);
		PlayerPrefs.SetInt("zoomlock", Options.zoomlock);
		PlayerPrefs.SetInt("invertmouse", Options.invertmouse);
		PlayerPrefs.SetInt("rawinput", Options.rawinput);
		PlayerPrefs.SetInt("gamebuy", Options.gamebuy);
		PlayerPrefs.SetInt("dynamiccrosshair", Options.dynamiccrosshair);
		PlayerPrefs.SetInt("bind_0", (int)vp_FPInput.control[0]);
		PlayerPrefs.SetInt("bind_1", (int)vp_FPInput.control[1]);
		PlayerPrefs.SetInt("bind_2", (int)vp_FPInput.control[2]);
		PlayerPrefs.SetInt("bind_3", (int)vp_FPInput.control[3]);
		PlayerPrefs.SetInt("bind_4", (int)vp_FPInput.control[4]);
		PlayerPrefs.SetInt("bind_5", (int)vp_FPInput.control[5]);
		PlayerPrefs.SetInt("bind_6", (int)vp_FPInput.control[6]);
		PlayerPrefs.SetInt("bind_7", (int)vp_FPInput.control[7]);
		PlayerPrefs.SetInt("bind_8", (int)vp_FPInput.control[8]);
		PlayerPrefs.SetInt("bind_9", (int)vp_FPInput.control[9]);
		PlayerPrefs.SetInt("bind_10", (int)vp_FPInput.control[10]);
		PlayerPrefs.SetInt("bind_11", (int)vp_FPInput.control[11]);
		PlayerPrefs.SetInt("bind_16", (int)vp_FPInput.control[16]);
		PlayerPrefs.SetInt("bind_17", (int)vp_FPInput.control[17]);
		PlayerPrefs.SetInt("bind_18", (int)vp_FPInput.control[18]);
		PlayerPrefs.SetInt("bind_19", (int)vp_FPInput.control[19]);
		PlayerPrefs.SetInt("bind_20", (int)vp_FPInput.control[20]);
		PlayerPrefs.SetInt("bind_21", (int)vp_FPInput.control[21]);
		PlayerPrefs.SetInt("control", 1);
	}

	public static void Apply()
	{
		QualitySettings.SetQualityLevel(Options.shadows);
		QualitySettings.antiAliasing = Options.antialiasing;
		QualitySettings.vSyncCount = Options.vsync;
		GameObject gameObject = GameObject.Find("Camera");
		if (gameObject && gameObject.GetComponent<AudioSource>())
		{
			gameObject.GetComponent<AudioSource>().volume = Options.menuvol;
		}
		Options.ApplyBrightness();
	}

	public static void ApplyResolution()
	{
		if (Options.resolution < Screen.resolutions.Length)
		{
			Screen.SetResolution(Screen.resolutions[Options.resolution].width, Screen.resolutions[Options.resolution].height, true);
			Debug.Log(string.Concat(new object[]
			{
				"Apply resolution: ",
				Options.resolution,
				" ",
				Screen.resolutions[Options.resolution].width,
				"x",
				Screen.resolutions[Options.resolution].height
			}));
			Options.resonce = true;
		}
	}

	public static void ApplyResolutionOnce()
	{
		if (Options.resonce)
		{
			return;
		}
		if (Options.resolution < Screen.resolutions.Length)
		{
			Screen.SetResolution(Screen.resolutions[Options.resolution].width, Screen.resolutions[Options.resolution].height, true);
			Debug.Log(string.Concat(new object[]
			{
				"Apply resolution: ",
				Options.resolution,
				" ",
				Screen.resolutions[Options.resolution].width,
				"x",
				Screen.resolutions[Options.resolution].height
			}));
			Options.resonce = true;
		}
	}

	public static void ToggleFullScreen()
	{
		FS fS = (FS)UnityEngine.Object.FindObjectOfType(typeof(FS));
		if (fS)
		{
			fS.ToggleFullscreen();
		}
	}

	public static void ApplyBrightness()
	{
		Texture2D temp2DTex;
		if (Options.colorcorrection == 1)
		{
			temp2DTex = (Resources.Load("wmlut") as Texture2D);
		}
		else
		{
			temp2DTex = (Resources.Load("deflut") as Texture2D);
		}
		ColorCorrectionLookup colorCorrectionLookup = (ColorCorrectionLookup)UnityEngine.Object.FindObjectOfType(typeof(ColorCorrectionLookup));
		if (colorCorrectionLookup)
		{
			colorCorrectionLookup.Convert(temp2DTex, Options.brightness, Options.gamma);
		}
	}

	public static void ApplyInGame()
	{
		if (Options.posteffects == 0)
		{
			global::Console.cs.Command("fx_ssao 0");
			global::Console.cs.Command("fx_sharpness 0");
			global::Console.cs.Command("fx_noise 0");
			global::Console.cs.Command("fx_tone 0");
			global::Console.cs.Command("fx_vig 0");
		}
		else
		{
			global::Console.cs.Command("fx_ssao " + Options.ssao);
			global::Console.cs.Command("fx_sharpness " + Options.sharpness);
			global::Console.cs.Command("fx_noise " + Options.noise);
			global::Console.cs.Command("fx_tone " + Options.tone);
			global::Console.cs.Command("fx_vig " + Options.vig);
		}
		if (Options.shadows == 0)
		{
			global::Console.cs.Command("fx_dl 0");
		}
		else
		{
			global::Console.cs.Command("fx_dl 1");
		}
		if (Options.xWeaponLight == 0)
		{
			global::Console.cs.Command("fx_lightweapon 0");
		}
		if (Options.xWeaponShadow == 0)
		{
			global::Console.cs.Command("fx_shadowweapon 0");
		}
		vp_FPCamera.SetMouseSensitivity(Options.sens);
		vp_FPCamera.SetZoomSensitivity(Options.zoomsens);
		AudioSource[] array = UnityEngine.Object.FindObjectsOfType(typeof(AudioSource)) as AudioSource[];
		AudioSource[] array2 = array;
		for (int i = 0; i < array2.Length; i++)
		{
			AudioSource audioSource = array2[i];
			audioSource.volume = Options.gamevol;
		}
		Options.ApplyBrightness();
	}

	public static void LoadControl()
	{
		if (!PlayerPrefs.HasKey("control"))
		{
			vp_FPInput.control[0] = KeyCode.W;
			vp_FPInput.control[1] = KeyCode.A;
			vp_FPInput.control[2] = KeyCode.S;
			vp_FPInput.control[3] = KeyCode.D;
			vp_FPInput.control[4] = KeyCode.LeftShift;
			vp_FPInput.control[5] = KeyCode.Space;
			vp_FPInput.control[6] = KeyCode.LeftControl;
			vp_FPInput.control[7] = KeyCode.R;
			vp_FPInput.control[8] = KeyCode.Q;
			vp_FPInput.control[9] = KeyCode.F;
			vp_FPInput.control[10] = KeyCode.G;
			vp_FPInput.control[11] = KeyCode.V;
			vp_FPInput.control[16] = KeyCode.M;
			vp_FPInput.control[17] = KeyCode.Tab;
			vp_FPInput.control[18] = KeyCode.T;
			vp_FPInput.control[19] = KeyCode.B;
			vp_FPInput.control[20] = KeyCode.C;
			vp_FPInput.control[21] = KeyCode.X;
		}
		else
		{
			vp_FPInput.control[0] = (KeyCode)PlayerPrefs.GetInt("bind_0", 119);
			vp_FPInput.control[1] = (KeyCode)PlayerPrefs.GetInt("bind_1", 97);
			vp_FPInput.control[2] = (KeyCode)PlayerPrefs.GetInt("bind_2", 115);
			vp_FPInput.control[3] = (KeyCode)PlayerPrefs.GetInt("bind_3", 100);
			vp_FPInput.control[4] = (KeyCode)PlayerPrefs.GetInt("bind_4", 304);
			vp_FPInput.control[5] = (KeyCode)PlayerPrefs.GetInt("bind_5", 32);
			vp_FPInput.control[6] = (KeyCode)PlayerPrefs.GetInt("bind_6", 306);
			vp_FPInput.control[7] = (KeyCode)PlayerPrefs.GetInt("bind_7", 114);
			vp_FPInput.control[8] = (KeyCode)PlayerPrefs.GetInt("bind_8", 113);
			vp_FPInput.control[9] = (KeyCode)PlayerPrefs.GetInt("bind_9", 102);
			vp_FPInput.control[10] = (KeyCode)PlayerPrefs.GetInt("bind_10", 103);
			vp_FPInput.control[11] = (KeyCode)PlayerPrefs.GetInt("bind_11", 118);
			vp_FPInput.control[16] = (KeyCode)PlayerPrefs.GetInt("bind_16", 109);
			vp_FPInput.control[17] = (KeyCode)PlayerPrefs.GetInt("bind_17", 9);
			vp_FPInput.control[18] = (KeyCode)PlayerPrefs.GetInt("bind_18", 116);
			vp_FPInput.control[19] = (KeyCode)PlayerPrefs.GetInt("bind_19", 98);
			vp_FPInput.control[20] = (KeyCode)PlayerPrefs.GetInt("bind_20", 99);
			vp_FPInput.control[21] = (KeyCode)PlayerPrefs.GetInt("bind_21", 120);
		}
	}
}
