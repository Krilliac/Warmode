using BestHTTP.Extensions;
using System;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuConsole : MonoBehaviour
{
	public static MainMenuConsole cs;

	private bool show;

	private Texture2D tBlack;

	private Rect rBlack;

	private static string command = string.Empty;

	private static string inputstore = string.Empty;

	private static List<string> log = new List<string>();

	private bool debugplayer;

	private float debugx;

	private int debugstate;

	public void PostAwake()
	{
		MainMenuConsole.cs = this;
		this.tBlack = TEX.GetTextureByName("black");
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
		this.rBlack = new Rect(0f, 0f, (float)Screen.width, (float)Screen.height / 2f);
	}

	private void OnGUI()
	{
		if (!this.show)
		{
			return;
		}
		GUI.depth = -1;
		GUI.color = new Color(1f, 1f, 1f, 0.8f);
		if (this.tBlack)
		{
			GUI.DrawTexture(this.rBlack, this.tBlack);
		}
		GUI.color = Color.white;
		float num = (float)Screen.height / 2f - 46f;
		for (int i = MainMenuConsole.log.Count - 1; i >= 0; i--)
		{
			GUIM.DrawText(new Rect(4f, num, (float)(Screen.width - 8), 24f), MainMenuConsole.log[i], TextAnchor.MiddleLeft, BaseColor.Gray, 0, 12, false);
			num -= 14f;
		}
		GUIM.DrawText(new Rect(0f, 0f, (float)(Screen.width - 4), 24f), "[MENU CONSOLE]", TextAnchor.MiddleRight, BaseColor.White, 0, 16, true);
		GUIM.DrawEdit(new Rect(4f, (float)Screen.height / 2f - 28f, (float)(Screen.width - 8), 24f), ref MainMenuConsole.command, TextAnchor.MiddleLeft, BaseColor.White, 0, 16, true);
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
				goto IL_1FB;
			case KeyCode.KeypadEquals:
				IL_1B8:
				if (keyCode == KeyCode.Return)
				{
					goto IL_1FB;
				}
				if (keyCode != KeyCode.BackQuote && keyCode != KeyCode.F10)
				{
					return;
				}
				MainMenuConsole.command = string.Empty;
				this.SetActive(false);
				Event.current.Use();
				return;
			case KeyCode.UpArrow:
			{
				MainMenuConsole.command = MainMenuConsole.inputstore;
				TextEditor textEditor = (TextEditor)GUIUtility.GetStateObject(typeof(TextEditor), GUIUtility.keyboardControl);
				textEditor.selectIndex = MainMenuConsole.command.Length + 1;
				textEditor.cursorIndex = MainMenuConsole.command.Length + 1;
				return;
			}
			}
			goto IL_1B8;
			IL_1FB:
			MainMenuConsole.ParsingCommand(MainMenuConsole.command);
			MainMenuConsole.command = string.Empty;
			Event.current.Use();
		}
	}

	public static void Command(string cmd)
	{
		MainMenuConsole.ParsingCommand(cmd);
	}

	private static void ParsingCommand(string cmd)
	{
		MainMenuConsole.log.Add(cmd);
		if (MainMenuConsole.log.Count > 100)
		{
			MainMenuConsole.log.RemoveAt(0);
		}
		MainMenuConsole.inputstore = cmd;
		string[] array = cmd.Split(new char[]
		{
			' '
		});
		if (array[0] == "connect")
		{
			if (array.Length < 3)
			{
				return;
			}
			MenuShop.GenerateCustomIcons();
			PlayerPrefs.SetInt("localplay", 0);
			if (array.Length == 4)
			{
				PlayerPrefs.SetString("autostart", string.Concat(new string[]
				{
					"connect ",
					array[1],
					" ",
					array[2],
					" ",
					array[3]
				}));
			}
			else
			{
				PlayerPrefs.SetString("autostart", "connect " + array[1] + " " + array[2]);
			}
			Application.LoadLevel("game");
		}
		else if (array[0] == "reload_inv")
		{
			PlayerPrefs.DeleteKey(BaseData.uid + "_invsig");
			for (int i = 0; i < 1024; i++)
			{
				BaseData.item[i] = 0;
				if (PlayerPrefs.HasKey(BaseData.uid + "_item_" + i.ToString()))
				{
					PlayerPrefs.DeleteKey(BaseData.uid + "_item_" + i.ToString());
				}
			}
			BaseData.badge_back = 0;
			BaseData.badge_icon = 0;
			BaseData.mask_merc = 0;
			BaseData.mask_warcorp = 0;
			for (int j = 0; j < 128; j++)
			{
				BaseData.profileWeapon[j] = 0;
				BaseData.currentWeapon[j] = 0;
			}
			if (PlayerPrefs.HasKey(BaseData.uid + "_badge_back"))
			{
				PlayerPrefs.DeleteKey(BaseData.uid + "_badge_back");
			}
			if (PlayerPrefs.HasKey(BaseData.uid + "_badge_icon"))
			{
				PlayerPrefs.DeleteKey(BaseData.uid + "_badge_icon");
			}
			if (PlayerPrefs.HasKey(BaseData.uid + "_mask_merc"))
			{
				PlayerPrefs.DeleteKey(BaseData.uid + "_mask_merc");
			}
			if (PlayerPrefs.HasKey(BaseData.uid + "_mask_warcorp"))
			{
				PlayerPrefs.DeleteKey(BaseData.uid + "_mask_warcorp");
			}
			for (int k = 0; k < 128; k++)
			{
				if (WeaponData.CheckWeapon(k))
				{
					string key = BaseData.uid + "_custom_" + WeaponData.GetData(k).wName.ToLower();
					if (PlayerPrefs.HasKey(key))
					{
						PlayerPrefs.DeleteKey(key);
					}
				}
			}
			Main.MainInventory();
		}
		else if (array[0] == "reload_auth")
		{
			PlayerPrefs.DeleteKey(BaseData.uid + "_key");
			BaseData.key = string.Empty;
			BaseData.Auth = false;
			if (GameData.gSteam)
			{
				Main.MainAuthSteam();
			}
			if (GameData.gSocial)
			{
				BaseData.StartAuth();
			}
		}
		else if (array[0] == "devpass")
		{
			if (array.Length != 2)
			{
				return;
			}
			string value = array[1].GetASCIIBytes().CalculateMD5Hash();
			PlayerPrefs.SetString("devpass", value);
		}
		else if (array[0] == "localweb")
		{
			if (PlayerPrefs.GetString("devpass") != "6f66a0d73e9894e62fe0ee48c209911b")
			{
				return;
			}
			WebHandler.handlerSteam = "192.168.1.201:80/warmode_steam";
			WebHandler.handlerVK = "192.168.1.201:80/warmode_core";
		}
		else if (array[0] == "devconnect")
		{
			if (PlayerPrefs.GetString("devpass") != "6f66a0d73e9894e62fe0ee48c209911b")
			{
				return;
			}
			MenuShop.GenerateCustomIcons();
			PlayerPrefs.SetInt("localplay", 0);
			PlayerPrefs.SetString("autostart", "connect 192.168.1.201 5555");
			Application.LoadLevel("game");
		}
		else if (array[0] == "map")
		{
			if (PlayerPrefs.GetString("devpass") != "6f66a0d73e9894e62fe0ee48c209911b")
			{
				return;
			}
			if (array.Length != 2)
			{
				return;
			}
			MapLoader mapLoader = (MapLoader)UnityEngine.Object.FindObjectOfType(typeof(MapLoader));
			PlayerPrefs.SetInt("localplay", 1);
			PlayerPrefs.SetString("map", array[1]);
			Application.LoadLevel("game");
		}
		else if (array[0] == "devmap")
		{
			if (PlayerPrefs.GetString("devpass") != "6f66a0d73e9894e62fe0ee48c209911b")
			{
				return;
			}
			if (array.Length != 2)
			{
				return;
			}
			MapLoader mapLoader2 = (MapLoader)UnityEngine.Object.FindObjectOfType(typeof(MapLoader));
			PlayerPrefs.SetInt("localplay", 2);
			PlayerPrefs.SetString("map", array[1]);
			Application.LoadLevel("game");
		}
	}

	private void Update()
	{
		if (Input.GetKeyUp(KeyCode.F10) || Input.GetKeyUp(KeyCode.BackQuote))
		{
			this.ToggleActive();
		}
		if (Input.GetKeyUp(KeyCode.F9))
		{
			Options.ToggleFullScreen();
		}
	}
}
