using System;
using UnityEngine;

public class Chat : MonoBehaviour
{
	private static bool show;

	private Texture2D tBlack;

	private Texture2D[] tTeam = new Texture2D[2];

	private string message = string.Empty;

	private static int flag;

	private Rect rBack;

	private Rect rEdit;

	private Color c = new Color(1f, 1f, 1f, 0.5f);

	public void PostAwake()
	{
		this.tBlack = TEX.GetTextureByName("black");
		this.tTeam[0] = TEX.GetTextureByName("red");
		this.tTeam[1] = TEX.GetTextureByName("blue");
		this.OnResize();
	}

	public void OnResize()
	{
		this.rBack = new Rect(0f, (float)Screen.height - GUI2.YRES(200f), (float)Screen.width, GUI2.YRES(20f));
		this.rEdit = new Rect((float)Screen.width / 2f - GUI2.YRES(150f), (float)Screen.height - GUI2.YRES(200f), GUI2.YRES(300f), GUI2.YRES(20f));
	}

	public static void Toggle()
	{
		Chat.show = !Chat.show;
		Chat.flag = 0;
	}

	public static void ToggleTeam()
	{
		Chat.show = !Chat.show;
		Chat.flag = 1;
	}

	private void OnGUI()
	{
		if (!Chat.show)
		{
			return;
		}
		GUI.color = this.c;
		if (Chat.flag == 1)
		{
			GUI.DrawTexture(this.rBack, this.tTeam[BasePlayer.team]);
		}
		else
		{
			GUI.DrawTexture(this.rBack, this.tBlack);
		}
		GUI.color = Color.white;
		GUI2.DrawEditRes(this.rEdit, ref this.message, TextAnchor.MiddleLeft, _Color.White, 0, 12, true);
		char character = Event.current.character;
		if ((character < 'а' || character > 'я') && (character < 'А' || character > 'Я') && (character < 'a' || character > 'z') && (character < 'A' || character > 'Z') && (character < '0' || character > '9') && character != ' ' && character != '_' && character != '.' && character != '?' && character != '!' && character != '(' && character != ')' && character != ':' && character != '.' && character != ',')
		{
			Event.current.character = '\0';
		}
		if (Event.current.isKey)
		{
			KeyCode keyCode = Event.current.keyCode;
			if (keyCode == KeyCode.Return || keyCode == KeyCode.KeypadEnter)
			{
				if (this.message != string.Empty)
				{
					Client.cs.send_chat(Chat.flag, this.message);
				}
				Screen.lockCursor = true;
				this.message = string.Empty;
				Event.current.Use();
				Chat.Toggle();
			}
		}
	}
}
