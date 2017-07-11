using System;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.ImageEffects;

public class Console : MonoBehaviour
{
	public static global::Console cs;

	private bool show;

	private Texture2D tblack;

	private Rect rblack;

	private string command = string.Empty;

	private string inputstore = string.Empty;

	private static List<string> log = new List<string>();

	private static bool showfps = false;

	private static ScreenSpaceAmbientOcclusion ssao = null;

	private static ContrastEnhance sharpness = null;

	private static NoiseAndGrain noise = null;

	private static Tonemapping tone = null;

	private static VignetteAndChromaticAberration vig = null;

	private static GameObject dl = null;

	private float updateInterval = 0.5f;

	private float accum;

	private int frames;

	private float timeleft;

	private string fps_text = string.Empty;

	private _Color fps_color = _Color.Green;

	public void PostAwake()
	{
		global::Console.cs = this;
		this.tblack = TEX.GetTextureByName("black");
		this.OnResize();
	}

	public void SetActive(bool val)
	{
		this.show = val;
	}

	public void ToggleActive()
	{
		this.show = !this.show;
	}

	public void OnResize()
	{
		this.rblack = new Rect(0f, 0f, (float)Screen.width, (float)Screen.height / 2f);
	}

	private void OnGUI()
	{
		if (global::Console.showfps)
		{
			this.DrawFPS();
		}
		if (!this.show)
		{
			return;
		}
		GUI.color = new Color(1f, 1f, 1f, 0.75f);
		if (this.tblack)
		{
			GUI.DrawTexture(this.rblack, this.tblack);
		}
		GUI.color = Color.white;
		float num = (float)Screen.height / 2f - 46f;
		for (int i = global::Console.log.Count - 1; i >= 0; i--)
		{
			GUI2.DrawText(new Rect(4f, num, (float)(Screen.width - 8), 24f), global::Console.log[i], TextAnchor.MiddleLeft, _Color.Gray, 0, 12, false);
			num -= 14f;
		}
		GUI2.DrawText(new Rect(0f, 0f, (float)(Screen.width - 4), 24f), "0.01a", TextAnchor.MiddleRight, _Color.White, 0, 16, true);
		GUI2.DrawEdit(new Rect(4f, (float)Screen.height / 2f - 28f, (float)(Screen.width - 8), 24f), ref this.command, TextAnchor.MiddleLeft, _Color.White, 0, 16, true);
		char character = Event.current.character;
		if ((character < 'a' || character > 'z') && (character < 'A' || character > 'Z') && (character < '0' || character > '9') && character != ' ' && character != '_' && character != '.')
		{
			Event.current.character = '\0';
		}
		if (Event.current.isKey)
		{
			KeyCode keyCode = Event.current.keyCode;
			switch (keyCode)
			{
			case KeyCode.KeypadEnter:
				goto IL_207;
			case KeyCode.KeypadEquals:
				IL_1C3:
				if (keyCode == KeyCode.Return)
				{
					goto IL_207;
				}
				if (keyCode != KeyCode.BackQuote && keyCode != KeyCode.F10)
				{
					return;
				}
				this.command = string.Empty;
				this.SetActive(false);
				Event.current.Use();
				return;
			case KeyCode.UpArrow:
			{
				this.command = this.inputstore;
				TextEditor textEditor = (TextEditor)GUIUtility.GetStateObject(typeof(TextEditor), GUIUtility.keyboardControl);
				textEditor.selectIndex = this.command.Length + 1;
				textEditor.cursorIndex = this.command.Length + 1;
				return;
			}
			}
			goto IL_1C3;
			IL_207:
			this.ParsingCommand(this.command);
			this.command = string.Empty;
			Event.current.Use();
		}
	}

	public void Command(string cmd)
	{
		this.ParsingCommand(cmd);
	}

	public void AddLogString(string logStr)
	{
		global::Console.log.Add(logStr);
		if (global::Console.log.Count > 100)
		{
			global::Console.log.RemoveAt(0);
		}
	}

	private void ParsingCommand(string cmd)
	{
		this.AddLogString(cmd);
		this.inputstore = cmd;
		string[] array = cmd.Split(new char[]
		{
			' '
		});
		if (array[0] == "map")
		{
			if (array.Length != 2)
			{
				return;
			}
			MapLoader mapLoader = (MapLoader)UnityEngine.Object.FindObjectOfType(typeof(MapLoader));
			PlayerPrefs.SetInt("localplay", 1);
			mapLoader.Load(array[1]);
		}
		else if (array[0] == "devmap")
		{
			if (array.Length != 2)
			{
				return;
			}
			MapLoader mapLoader2 = (MapLoader)UnityEngine.Object.FindObjectOfType(typeof(MapLoader));
			PlayerPrefs.SetInt("localplay", 2);
			mapLoader2.DevLoad(array[1]);
		}
		else if (array[0] == "team")
		{
			if (array.Length != 3)
			{
				return;
			}
			int num = 0;
			int num2 = 0;
			if (!int.TryParse(array[1], out num))
			{
				return;
			}
			if (!int.TryParse(array[2], out num2))
			{
				return;
			}
			if (num < 0 || num >= 2)
			{
				return;
			}
			if (num2 < 0 || num2 >= 8)
			{
				return;
			}
			UnityEngine.Object[] array2 = UnityEngine.Object.FindObjectsOfType(typeof(Spawn));
			List<Spawn> list = new List<Spawn>();
			List<Spawn> list2 = new List<Spawn>();
			UnityEngine.Object[] array3 = array2;
			for (int i = 0; i < array3.Length; i++)
			{
				Spawn spawn = (Spawn)array3[i];
				if (spawn.Team == 0)
				{
					list.Add(spawn);
				}
				else if (spawn.Team == 1)
				{
					list2.Add(spawn);
				}
			}
			GameObject gameObject = null;
			if (num == 0)
			{
				if (num2 >= list.Count)
				{
					return;
				}
				gameObject = list[num2].gameObject;
			}
			else if (num == 1)
			{
				if (num2 >= list2.Count)
				{
					return;
				}
				gameObject = list2[num2].gameObject;
			}
			MonoBehaviour.print("spawn");
			GameObject gameObject2 = GameObject.Find("LocalPlayer");
			vp_FPCamera vp_FPCamera = (vp_FPCamera)UnityEngine.Object.FindObjectOfType(typeof(vp_FPCamera));
			gameObject2.transform.position = gameObject.transform.position;
			vp_FPCamera.SetRotation(new Vector2(0f, gameObject.transform.eulerAngles.y), true, true);
		}
		else if (array[0] == "sky")
		{
			if (array.Length != 2)
			{
				return;
			}
		}
		else if (array[0] == "sun_shadowbias" || array[0] == "sun_shadows")
		{
			if (array.Length != 2)
			{
				return;
			}
			UnityEngine.Object[] array4 = UnityEngine.Object.FindObjectsOfType(typeof(Light));
			Light light = null;
			UnityEngine.Object[] array5 = array4;
			for (int j = 0; j < array5.Length; j++)
			{
				Light light2 = (Light)array5[j];
				if (light2.type == LightType.Directional)
				{
					light = light2;
					break;
				}
			}
			if (light == null)
			{
				return;
			}
			if (array[0] == "sun_shadowbias")
			{
				float shadowBias = 0f;
				if (!float.TryParse(array[1], out shadowBias))
				{
					return;
				}
				light.shadowBias = shadowBias;
			}
			if (array[0] == "sun_shadows")
			{
				int num3 = 0;
				if (!int.TryParse(array[1], out num3))
				{
					return;
				}
				switch (num3)
				{
				case 0:
					light.shadows = LightShadows.None;
					break;
				case 1:
					light.shadows = LightShadows.Hard;
					break;
				case 2:
					light.shadows = LightShadows.Soft;
					break;
				}
			}
		}
		if (array[0] == "shadow_cascades")
		{
			if (array.Length != 2)
			{
				return;
			}
			int num4 = 0;
			if (!int.TryParse(array[1], out num4))
			{
				return;
			}
			switch (num4)
			{
			case 0:
				QualitySettings.shadowCascades = 0;
				break;
			case 1:
				QualitySettings.shadowCascades = 2;
				break;
			case 2:
				QualitySettings.shadowCascades = 4;
				break;
			}
		}
		if (array[0] == "shadow_distance")
		{
			if (array.Length != 2)
			{
				return;
			}
			int num5 = 0;
			if (!int.TryParse(array[1], out num5))
			{
				return;
			}
			QualitySettings.shadowDistance = (float)num5;
		}
		if (array[0] == "shadow_projection")
		{
			if (array.Length != 2)
			{
				return;
			}
			int num6 = 0;
			if (!int.TryParse(array[1], out num6))
			{
				return;
			}
			int num7 = num6;
			if (num7 != 0)
			{
				if (num7 == 1)
				{
					QualitySettings.shadowProjection = ShadowProjection.StableFit;
				}
			}
			else
			{
				QualitySettings.shadowProjection = ShadowProjection.CloseFit;
			}
		}
		if (array[0] == "fx_ssao")
		{
			if (array.Length != 2)
			{
				return;
			}
			if (global::Console.ssao == null)
			{
				global::Console.ssao = (ScreenSpaceAmbientOcclusion)UnityEngine.Object.FindObjectOfType(typeof(ScreenSpaceAmbientOcclusion));
			}
			if (global::Console.ssao == null)
			{
				return;
			}
			int num8 = 0;
			if (!int.TryParse(array[1], out num8))
			{
				return;
			}
			if (num8 > 0)
			{
				global::Console.ssao.enabled = true;
			}
			else
			{
				global::Console.ssao.enabled = false;
			}
		}
		if (array[0] == "fx_sharpness")
		{
			if (array.Length != 2)
			{
				return;
			}
			if (global::Console.sharpness == null)
			{
				global::Console.sharpness = (ContrastEnhance)UnityEngine.Object.FindObjectOfType(typeof(ContrastEnhance));
			}
			if (global::Console.sharpness == null)
			{
				return;
			}
			int num9 = 0;
			if (!int.TryParse(array[1], out num9))
			{
				return;
			}
			if (num9 > 0)
			{
				global::Console.sharpness.enabled = true;
			}
			else
			{
				global::Console.sharpness.enabled = false;
			}
		}
		if (array[0] == "fx_noise")
		{
			if (array.Length != 2)
			{
				return;
			}
			if (global::Console.noise == null)
			{
				global::Console.noise = (NoiseAndGrain)UnityEngine.Object.FindObjectOfType(typeof(NoiseAndGrain));
			}
			if (global::Console.noise == null)
			{
				return;
			}
			int num10 = 0;
			if (!int.TryParse(array[1], out num10))
			{
				return;
			}
			if (num10 > 0)
			{
				global::Console.noise.enabled = true;
			}
			else
			{
				global::Console.noise.enabled = false;
			}
		}
		if (array[0] == "fx_tone")
		{
			if (array.Length != 2)
			{
				return;
			}
			if (global::Console.tone == null)
			{
				global::Console.tone = (Tonemapping)UnityEngine.Object.FindObjectOfType(typeof(Tonemapping));
			}
			if (global::Console.tone == null)
			{
				return;
			}
			int num11 = 0;
			if (!int.TryParse(array[1], out num11))
			{
				return;
			}
			if (num11 > 0)
			{
				global::Console.tone.enabled = true;
			}
			else
			{
				global::Console.tone.enabled = false;
			}
		}
		if (array[0] == "fx_vig")
		{
			if (array.Length != 2)
			{
				return;
			}
			if (global::Console.vig == null)
			{
				global::Console.vig = (VignetteAndChromaticAberration)UnityEngine.Object.FindObjectOfType(typeof(VignetteAndChromaticAberration));
			}
			if (global::Console.vig == null)
			{
				return;
			}
			int num12 = 0;
			if (!int.TryParse(array[1], out num12))
			{
				return;
			}
			if (num12 > 0)
			{
				global::Console.vig.enabled = true;
			}
			else
			{
				global::Console.vig.enabled = false;
			}
		}
		if (array[0] == "fx_dl")
		{
			if (array.Length != 2)
			{
				return;
			}
			if (global::Console.dl == null)
			{
				global::Console.dl = GameObject.Find("Directional light");
			}
			if (global::Console.dl == null)
			{
				return;
			}
			int num13 = 0;
			if (!int.TryParse(array[1], out num13))
			{
				return;
			}
			if (num13 > 0)
			{
				global::Console.dl.SetActive(true);
			}
			else
			{
				global::Console.dl.SetActive(false);
			}
		}
		if (array[0] == "fx_shadowweapon")
		{
			if (array.Length != 2)
			{
				return;
			}
			GameObject gameObject3 = GameObject.Find("maplight");
			if (gameObject3)
			{
				gameObject3.SetActive(false);
			}
		}
		if (array[0] == "fx_lightweapon")
		{
			if (array.Length != 2)
			{
				return;
			}
			GameObject gameObject4 = GameObject.Find("lights");
			if (gameObject4)
			{
				gameObject4.SetActive(false);
			}
		}
		if (array[0] == "devconnect")
		{
			PlayerPrefs.SetInt("localplay", 0);
			array[0] = "connect";
		}
		if (array[0] == "connect")
		{
			if (array.Length < 3)
			{
				Client.cs.Connect();
				return;
			}
			Client.IP = array[1];
			int.TryParse(array[2], out Client.PORT);
			if (array.Length == 4)
			{
				Client.PASSWORD = array[3];
			}
			else
			{
				Client.PASSWORD = string.Empty;
			}
			Client.cs.Connect();
		}
		if (array[0] == "deadcam")
		{
			DeadCam.SetActive(true);
		}
		if (array[0] == "test")
		{
			Award.SetPoints(100);
			Award.SetCustomPoints(23, 0);
		}
		if (array[0] == "sens")
		{
			if (array.Length != 2)
			{
				return;
			}
			float num14;
			if (!float.TryParse(array[1], out num14))
			{
				return;
			}
			vp_FPCamera.SetMouseSensitivity(num14);
			Options.sens = num14;
			PlayerPrefs.SetFloat("sens", num14);
		}
		if (array[0] == "zoomsens")
		{
			if (array.Length != 2)
			{
				return;
			}
			float num15;
			if (!float.TryParse(array[1], out num15))
			{
				return;
			}
			vp_FPCamera.SetZoomSensitivity(num15);
			Options.zoomsens = num15;
			PlayerPrefs.SetFloat("zoomsens", num15);
		}
		if (array[0] == "disconnect")
		{
			GameObject gameObject5 = GameObject.Find("LocalPlayer");
			GameObject gameObject6 = GameObject.Find("Sky");
			GameObject gameObject7 = GameObject.Find("Sky Manager");
			GameObject gameObject8 = GameObject.Find("GUI");
			GameObject gameObject9 = GameObject.Find("Core");
			if (gameObject5)
			{
				UnityEngine.Object.Destroy(gameObject5);
			}
			if (gameObject6)
			{
				UnityEngine.Object.Destroy(gameObject6);
			}
			if (gameObject7)
			{
				UnityEngine.Object.Destroy(gameObject7);
			}
			if (gameObject8)
			{
				UnityEngine.Object.Destroy(gameObject8);
			}
			if (gameObject9)
			{
				UnityEngine.Object.Destroy(gameObject9);
			}
			Client.cs.CloseClient();
			Application.LoadLevel("main");
			Screen.lockCursor = false;
			Cursor.visible = true;
		}
		if (array[0] == "crosshair_size")
		{
			if (array.Length != 2)
			{
				return;
			}
			int crosshairSize;
			if (!int.TryParse(array[1], out crosshairSize))
			{
				return;
			}
			Crosshair.SetCrosshairSize(crosshairSize);
		}
		if (array[0] == "crosshair_color")
		{
			if (array.Length != 4)
			{
				return;
			}
			int r;
			if (!int.TryParse(array[1], out r))
			{
				return;
			}
			int g;
			if (!int.TryParse(array[2], out g))
			{
				return;
			}
			int b;
			if (!int.TryParse(array[3], out b))
			{
				return;
			}
			Crosshair.SetCrosshairColor(r, g, b);
		}
		if (array[0] == "disableskymanager")
		{
		}
		if (array[0] == "atest")
		{
			Award.SetDeath(26, 0);
		}
		if (array[0] == "chat")
		{
			if (array.Length != 2)
			{
				return;
			}
			int num16;
			if (!int.TryParse(array[1], out num16))
			{
				return;
			}
			if (num16 == 0)
			{
				Message.blockchat = true;
			}
			else
			{
				Message.blockchat = false;
			}
		}
		if (array[0] == "hud")
		{
			if (array.Length != 2)
			{
				return;
			}
			int num17 = 0;
			if (!int.TryParse(array[1], out num17))
			{
				return;
			}
			if (num17 == 0 || num17 == 2)
			{
				HUD.ForceHide(true);
				Crosshair.ForceHide(true);
				Message.ForceHide(true);
				ScoreTop.SetActive(false);
				PlayerNames.SetActive(false);
				GameObject gameObject10 = GameObject.Find("WeaponCamera");
				if (num17 == 2)
				{
					gameObject10.GetComponent<Camera>().cullingMask = -2147483648;
					Crosshair.ForceHide(false);
				}
				else
				{
					gameObject10.GetComponent<Camera>().cullingMask = 0;
				}
			}
			else if (num17 == 3)
			{
				HUD.ForceHide(true);
				Crosshair.ForceHide(true);
				Message.ForceHide(false);
				ScoreTop.SetActive(true);
				PlayerNames.SetActive(true);
				GameObject gameObject11 = GameObject.Find("WeaponCamera");
				gameObject11.GetComponent<Camera>().cullingMask = 0;
			}
			else
			{
				HUD.ForceHide(false);
				Crosshair.ForceHide(false);
				Message.ForceHide(false);
				ScoreTop.SetActive(true);
				PlayerNames.SetActive(true);
				GameObject gameObject12 = GameObject.Find("WeaponCamera");
				gameObject12.GetComponent<Camera>().cullingMask = -2147483648;
			}
		}
		if (array[0] == "spectator")
		{
			if (array.Length != 2)
			{
				return;
			}
			int num18 = 0;
			if (!int.TryParse(array[1], out num18))
			{
				return;
			}
			if (num18 == 1)
			{
				SpecCam.show = true;
				Client.cs.send_chooseteam(255);
				GameObject gameObject13 = GameObject.Find("WeaponCamera");
				gameObject13.GetComponent<Camera>().cullingMask = 0;
			}
			else
			{
				SpecCam.show = false;
				Client.cs.send_chooseteam(3);
				GameObject gameObject14 = GameObject.Find("WeaponCamera");
				gameObject14.GetComponent<Camera>().cullingMask = -2147483648;
			}
		}
		if (array[0] == "m_steps")
		{
			if (array.Length != 2)
			{
				return;
			}
			int num19 = 0;
			if (!int.TryParse(array[1], out num19))
			{
				return;
			}
			if (num19 > 20)
			{
				num19 = 20;
			}
			if (num19 < 1)
			{
				num19 = 1;
			}
			vp_FPCamera.MouseSmoothSteps = num19;
		}
		if (array[0] == "m_weight")
		{
			if (array.Length != 2)
			{
				return;
			}
			float num20 = 0f;
			if (!float.TryParse(array[1], out num20))
			{
				return;
			}
			if (num20 > 1f)
			{
				num20 = 1f;
			}
			if (num20 < 0f)
			{
				num20 = 0f;
			}
			vp_FPCamera.MouseSmoothWeight = num20;
		}
		if (array[0] == "extralod")
		{
			if (array.Length != 2)
			{
				return;
			}
			int num21 = 0;
			if (!int.TryParse(array[1], out num21))
			{
				return;
			}
			if (num21 == 0)
			{
				PlayerControll.extralod = false;
			}
			else
			{
				PlayerControll.extralod = true;
			}
		}
		if (array[0] == "showfps")
		{
			if (array.Length != 2)
			{
				return;
			}
			int num22 = 0;
			if (!int.TryParse(array[1], out num22))
			{
				return;
			}
			if (num22 == 0)
			{
				global::Console.showfps = false;
			}
			else
			{
				global::Console.showfps = true;
			}
		}
		if (array[0] == "ping")
		{
			if (PlayerPrefs.GetString("devpass") != "6f66a0d73e9894e62fe0ee48c209911b")
			{
				return;
			}
			if (array.Length != 2)
			{
				return;
			}
			int num23 = 0;
			if (!int.TryParse(array[1], out num23))
			{
				return;
			}
			PlayerPrefs.SetInt("ShowPing", num23);
			if (num23 == 1)
			{
				global::Ping.cs.ShowPing(true);
			}
			else
			{
				global::Ping.cs.ShowPing(false);
			}
		}
		if (array[0] == "nord")
		{
			if (array.Length != 2)
			{
				return;
			}
			int num24 = 0;
			if (!int.TryParse(array[1], out num24))
			{
				return;
			}
			if (num24 == 0)
			{
				PlayerControll.nord = false;
			}
			else
			{
				PlayerControll.nord = true;
			}
		}
		if (array[0] == "devunlock")
		{
		}
		if (array[0] == "devnext")
		{
		}
		if (array[0] == "forcewin")
		{
			Client.cs.send_consolecmd(array[0], string.Empty);
		}
		if (array[0] == "name")
		{
			if (array.Length != 2)
			{
				return;
			}
			MonoBehaviour.print(array[0] + array[1]);
			Client.cs.send_consolecmd(array[0], array[1]);
		}
		else if (array[0] == "kill")
		{
			Client.cs.send_consolecmd(array[0], string.Empty);
		}
		else if (array[0] == "rcon")
		{
			if (array.Length != 2)
			{
				return;
			}
			Client.cs.send_consolecmd(array[0], array[1]);
		}
		else if (array[0] == "kick")
		{
			if (array.Length != 2)
			{
				return;
			}
			Client.cs.send_consolecmd(array[0], array[1]);
		}
		else if (array[0] == "banlist")
		{
			Client.cs.send_consolecmd(array[0], string.Empty);
		}
		else if (array[0] == "ban")
		{
			if (array.Length != 2)
			{
				return;
			}
			Client.cs.send_consolecmd(array[0], array[1]);
		}
		else if (array[0] == "password")
		{
			if (array.Length != 2)
			{
				return;
			}
			Client.cs.send_consolecmd(array[0], array[1]);
		}
		else if (array[0] == "startmoney")
		{
			if (array.Length != 2)
			{
				return;
			}
			int num25;
			if (!int.TryParse(array[1], out num25))
			{
				return;
			}
			Client.cs.send_consolecmd(array[0], array[1]);
		}
		else if (array[0] == "fraglimit")
		{
			if (array.Length != 2)
			{
				return;
			}
			int num26;
			if (!int.TryParse(array[1], out num26))
			{
				return;
			}
			Client.cs.send_consolecmd(array[0], array[1]);
		}
		else if (array[0] == "rr" || array[0] == "restart")
		{
			Client.cs.send_consolecmd(array[0], string.Empty);
		}
		else if (array[0] == "live")
		{
			Client.cs.send_consolecmd(array[0], string.Empty);
		}
		else if (array[0] == "skin_test")
		{
			if (array.Length != 2)
			{
				return;
			}
			Texture2D textureByName = TEX.GetTextureByName(array[1]);
			vp_FPWeapon vp_FPWeapon = (vp_FPWeapon)UnityEngine.Object.FindObjectOfType(typeof(vp_FPWeapon));
			if (vp_FPWeapon.m_WeaponCoreModel != null && textureByName != null)
			{
				Component[] componentsInChildren = vp_FPWeapon.m_WeaponCoreModel.GetComponentsInChildren(typeof(Renderer));
				for (int k = 0; k < componentsInChildren.Length; k++)
				{
					Renderer renderer = (Renderer)componentsInChildren[k];
					if (!(renderer.name == "merc_hands"))
					{
						if (!(renderer.name == "warcorp_hands"))
						{
							renderer.materials[0].SetTexture(0, textureByName);
						}
					}
				}
			}
		}
	}

	private void Update()
	{
		if (!global::Console.showfps)
		{
			return;
		}
		this.timeleft -= Time.deltaTime;
		this.accum += 1f / Time.deltaTime;
		this.frames++;
		if ((double)this.timeleft <= 0.0)
		{
			float num = this.accum / (float)this.frames;
			this.fps_text = string.Format("FPS {0:0}", num);
			if (num < 25f)
			{
				this.fps_color = _Color.Red;
			}
			else if (num < 50f)
			{
				this.fps_color = _Color.Yellow;
			}
			else
			{
				this.fps_color = _Color.Green;
			}
			this.timeleft = this.updateInterval;
			this.accum = 0f;
			this.frames = 0;
		}
	}

	private void DrawFPS()
	{
		GUI2.DrawTextColorRes(new Rect((float)Screen.width - GUI2.YRES(50f), 0f, GUI2.YRES(50f), GUI2.YRES(14f)), this.fps_text, TextAnchor.MiddleLeft, this.fps_color, 0, 12, true);
	}
}
