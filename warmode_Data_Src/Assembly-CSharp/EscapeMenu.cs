using System;
using UnityEngine;

public class EscapeMenu : MonoBehaviour
{
	public static bool show;

	private static int state;

	private Texture2D tBlack;

	private Texture2D tGray0;

	private Texture2D tLoad;

	private Texture2D tKey0;

	private Texture2D tKey1;

	private Texture2D tLogo;

	private Rect[] rButton = new Rect[7];

	private Rect rCloseButton;

	private Vector2 mpos = Vector2.zero;

	public void PostAwake()
	{
		this.tBlack = TEX.GetTextureByName("black");
		this.tGray0 = TEX.GetTextureByName("gray0");
		this.tLoad = TEX.GetTextureByName("wmload");
		this.tLogo = TEX.GetTextureByName("warlogo");
		this.tKey0 = TEX.GetTextureByName("wmkey0_" + Lang.GetLanguage());
		this.tKey1 = TEX.GetTextureByName("wmkey1_" + Lang.GetLanguage());
		if (this.tKey0 == null)
		{
			this.tKey0 = TEX.GetTextureByName("wmkey0");
		}
		if (this.tKey1 == null)
		{
			this.tKey1 = TEX.GetTextureByName("wmkey1");
		}
		this.OnResize();
		EscapeMenu.show = false;
	}

	public void OnResize()
	{
		int num = (int)GUI2.YRES(8f);
		int num2 = (int)GUI2.YRES(20f);
		int num3 = (int)GUI2.YRES(96f);
		this.rButton[0] = new Rect((float)num3, (float)Screen.height / 2f - GUI2.YRES(50f) + (float)((num2 + num) * 0), GUI2.YRES(200f), GUI2.YRES(20f));
		this.rButton[1] = new Rect((float)num3, (float)Screen.height / 2f - GUI2.YRES(50f) + (float)((num2 + num) * 1), GUI2.YRES(200f), GUI2.YRES(20f));
		this.rButton[2] = new Rect((float)num3, (float)Screen.height / 2f - GUI2.YRES(50f) + (float)((num2 + num) * 2), GUI2.YRES(200f), GUI2.YRES(20f));
		this.rButton[3] = new Rect((float)num3, (float)Screen.height / 2f - GUI2.YRES(50f) + (float)((num2 + num) * 3), GUI2.YRES(200f), GUI2.YRES(20f));
		this.rButton[4] = new Rect((float)num3, (float)Screen.height / 2f - GUI2.YRES(50f) + (float)((num2 + num) * 4), GUI2.YRES(200f), GUI2.YRES(20f));
		this.rButton[5] = new Rect((float)num3, (float)Screen.height / 2f - GUI2.YRES(50f) + (float)((num2 + num) * 5), GUI2.YRES(200f), GUI2.YRES(20f));
		this.rButton[6] = new Rect((float)num3, (float)Screen.height / 2f - GUI2.YRES(50f) + (float)((num2 + num) * 6), GUI2.YRES(200f), GUI2.YRES(20f));
		this.rCloseButton = new Rect((float)Screen.width / 2f - GUI2.YRES(100f), GUI2.YRES(400f), GUI2.YRES(200f), GUI2.YRES(20f));
	}

	private void OnGUI()
	{
		if (!EscapeMenu.show)
		{
			return;
		}
		Rect position = new Rect(0f, 0f, (float)Screen.width, (float)Screen.height);
		GUI.DrawTexture(position, this.tLoad);
		if (EscapeMenu.state == 0)
		{
			GUI.DrawTexture(new Rect(GUI2.YRES(60f), GUI2.YRES(128f), GUI2.YRES(256f), GUI2.YRES(32f)), this.tLogo);
			if (this.DrawButton(this.rButton[0], Lang.Get("_RESUME"), Lang.Get("_resume_to_game"), _Color.Gray))
			{
				EscapeMenu.Toggle();
			}
			if (this.DrawButton(this.rButton[1], Lang.Get("_VOTE"), Lang.Get("_vote_menu"), _Color.Gray))
			{
				EscapeMenu.Toggle();
				Vote.SetActive(true);
			}
			if (this.DrawButton(this.rButton[2], Lang.Get("_HELP"), Lang.Get("_tip_control"), _Color.Gray))
			{
				EscapeMenu.state = 1;
			}
			if (ScoreBoard.gamemode != 3)
			{
				if (this.DrawButton(this.rButton[3], Lang.Get("_CHOOSE_TEAM"), Lang.Get("_change_team_ingame"), _Color.Gray))
				{
					EscapeMenu.Toggle();
					ChooseTeam.Toggle();
				}
			}
			else
			{
				GUI2.DrawTextRes(this.rButton[3], Lang.Get("_CHOOSE_TEAM"), TextAnchor.MiddleRight, _Color.Gray, 1, 14, true);
			}
			if (this.DrawButton(this.rButton[4], Lang.Get("_FULLSCREEN"), Lang.Get("_open_game_in_fullscreen"), _Color.Gray))
			{
				EscapeMenu.Toggle();
				Options.ToggleFullScreen();
			}
			if (this.DrawButton(this.rButton[5], Lang.Get("_OPTIONS"), string.Empty, _Color.Gray))
			{
				EscapeMenu.state = 2;
				MenuOptions.ForceCenter();
				MenuOptions.SetActive(true);
			}
			if (this.DrawButton(this.rButton[6], Lang.Get("_EXIT_MENU"), Lang.Get("_progress_not_saved"), _Color.Yellow))
			{
				EscapeMenu.Toggle();
				global::Console.cs.Command("disconnect");
			}
		}
		else if (EscapeMenu.state == 1)
		{
			this.DrawHelp();
		}
		else if (EscapeMenu.state == 2)
		{
			MenuOptions.Draw();
		}
		if (this.DrawButton(new Rect(GUI2.YRES(60f), GUI2.YRES(400f), GUI2.YRES(64f), GUI2.YRES(32f)), Lang.Get("_BACK"), string.Empty, _Color.Gray))
		{
			EscapeMenu.Toggle();
		}
		GUI2.DrawTextRes(new Rect(GUI2.YRES(60f), GUI2.YRES(420f), GUI2.YRES(64f), GUI2.YRES(32f)), "ESC", TextAnchor.MiddleRight, _Color.Red, 2, 23, true);
	}

	public static void Toggle()
	{
		EscapeMenu.SetActive(!EscapeMenu.show);
	}

	public static void SetActive(bool val)
	{
		EscapeMenu.state = 0;
		EscapeMenu.show = val;
		if (EscapeMenu.show)
		{
			Crosshair.SetActive(false);
			BuyMenu.SetActive(false);
			Vote.SetActive(false);
		}
		else
		{
			Crosshair.SetActive(true);
		}
		vp_FPCamera.cs.SetMouseFreeze(val);
	}

	private bool DrawButton(Rect r, string text, string text2, _Color color2 = _Color.Gray)
	{
		this.mpos = new Vector2(Input.mousePosition.x, (float)Screen.height - Input.mousePosition.y);
		if (r.Contains(this.mpos))
		{
			GUI2.DrawTextRes(r, text, TextAnchor.MiddleRight, _Color.Red, 1, 14, true);
			GUI2.DrawTextRes(new Rect(r.x + r.width + GUI2.YRES(20f), r.y, GUI2.YRES(256f), r.height), text2, TextAnchor.MiddleLeft, color2, 1, 10, false);
		}
		else
		{
			GUI2.DrawTextRes(r, text, TextAnchor.MiddleRight, _Color.White, 1, 14, true);
		}
		return GUI2.HideButton(r);
	}

	private void DrawHelp()
	{
		GUI.color = new Color(1f, 1f, 1f, 0.25f);
		GUI.DrawTexture(new Rect(0f, 0f, (float)Screen.width, (float)Screen.height), this.tBlack);
		GUI.color = Color.white;
		Rect position = new Rect(0f, 0f, (float)Screen.width * 0.8f, (float)Screen.width / 2.5f);
		position.center = new Vector2((float)Screen.width, (float)Screen.width / 2.5f) / 2f;
		GUI.DrawTexture(position, this.tKey0);
		GUI.color = new Color(1f, 0.2f, 0f, 1f);
		GUI.DrawTexture(position, this.tKey1);
		GUI.color = Color.white;
	}
}
