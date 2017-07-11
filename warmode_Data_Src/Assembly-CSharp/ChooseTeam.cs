using System;
using System.Collections;
using System.Diagnostics;
using UnityEngine;

public class ChooseTeam : MonoBehaviour
{
	public static bool show = false;

	private static Vector2 mpos = Vector2.zero;

	private static Texture2D tAngle0 = null;

	private static Texture2D tAngle1 = null;

	private static Texture2D tWhite = null;

	private static Texture2D tBlack = null;

	private static Rect rLineTeam0;

	private static Rect rAnlgeTeam0;

	private static Rect rLineTeam1;

	private static Rect rAnlgeTeam1;

	private static Rect rBox0;

	private static Rect rBox1;

	private static Rect rTextTeam0;

	private static Rect rTextTeam1;

	private static Rect rMsgTeam;

	private static bool chooseTeamDelay = false;

	public void PostAwake()
	{
		ChooseTeam.tAngle0 = TEX.GetTextureByName("GUI/menu_angle0");
		ChooseTeam.tAngle1 = TEX.GetTextureByName("GUI/menu_angle1");
		ChooseTeam.tWhite = TEX.GetTextureByName("white");
		ChooseTeam.tBlack = TEX.GetTextureByName("black");
		ChooseTeam.show = false;
		this.OnResize();
	}

	private void OnResize()
	{
		float num = GUI2.YRES(40f);
		float num2 = GUI2.YRES(24f);
		float num3 = GUI2.YRES(28f);
		ChooseTeam.rLineTeam0 = new Rect(0f, ((float)Screen.height - num) / 2f, (float)Screen.width / 2f - num2, num);
		ChooseTeam.rLineTeam1 = new Rect((float)Screen.width / 2f + num2, ((float)Screen.height - num) / 2f, (float)Screen.width / 2f - num2, num);
		ChooseTeam.rAnlgeTeam0 = new Rect(ChooseTeam.rLineTeam0.width, ChooseTeam.rLineTeam0.y, num, num);
		ChooseTeam.rAnlgeTeam1 = new Rect(ChooseTeam.rLineTeam1.x - num, ChooseTeam.rLineTeam1.y, num, num);
		ChooseTeam.rBox0 = new Rect(ChooseTeam.rLineTeam0.width / 4f, ChooseTeam.rLineTeam0.y + (ChooseTeam.rLineTeam0.height - num3) / 2f, num3, num3);
		ChooseTeam.rBox1 = new Rect(ChooseTeam.rLineTeam1.x + ChooseTeam.rLineTeam1.width / 4f, ChooseTeam.rLineTeam1.y + (ChooseTeam.rLineTeam1.height - num3) / 2f, num3, num3);
		ChooseTeam.rTextTeam0 = new Rect(ChooseTeam.rBox0.x + num3 + GUI2.YRES(4f), ((float)Screen.height - num) / 2f, (float)Screen.width / 2f - num2, num);
		ChooseTeam.rTextTeam1 = new Rect(ChooseTeam.rBox1.x + num3 + GUI2.YRES(4f), ((float)Screen.height - num) / 2f, (float)Screen.width / 2f - num2, num);
		ChooseTeam.rMsgTeam = new Rect(0f, ChooseTeam.rLineTeam0.y - GUI2.YRES(64f), (float)Screen.width, num3);
	}

	public static void Draw()
	{
		if (!ChooseTeam.show)
		{
			return;
		}
		ChooseTeam.mpos = new Vector2(Input.mousePosition.x, (float)Screen.height - Input.mousePosition.y);
		if (ChooseTeam.rLineTeam0.Contains(ChooseTeam.mpos))
		{
			GUI.color = new Color(1f, 0.2f, 0f, 1f);
		}
		else
		{
			GUI.color = new Color(1f, 0.2f, 0f, 0.5f);
		}
		GUI.DrawTexture(ChooseTeam.rLineTeam0, ChooseTeam.tWhite);
		GUI.DrawTexture(ChooseTeam.rAnlgeTeam0, ChooseTeam.tAngle0);
		if (ChooseTeam.rLineTeam1.Contains(ChooseTeam.mpos))
		{
			GUI.color = new Color(0f, 0.5f, 1f, 1f);
		}
		else
		{
			Color color = new Color(0f, 0.5f, 1f, 0.5f);
			GUI.color = color;
			GUI.color = color;
		}
		GUI.DrawTexture(ChooseTeam.rLineTeam1, ChooseTeam.tWhite);
		GUI.DrawTexture(ChooseTeam.rAnlgeTeam1, ChooseTeam.tAngle1);
		GUI.color = new Color(1f, 1f, 1f, 0.5f);
		GUI.DrawTexture(ChooseTeam.rBox0, ChooseTeam.tBlack);
		GUI.DrawTexture(ChooseTeam.rBox1, ChooseTeam.tBlack);
		GUI.DrawTexture(ChooseTeam.rMsgTeam, ChooseTeam.tBlack);
		GUI.color = Color.white;
		GUI2.DrawTextRes(ChooseTeam.rTextTeam0, "MERCS", TextAnchor.MiddleLeft, _Color.White, 0, 24, true);
		GUI2.DrawTextRes(ChooseTeam.rBox0, "1", TextAnchor.MiddleCenter, _Color.White, 0, 24, true);
		GUI2.DrawTextRes(ChooseTeam.rTextTeam1, "WARCORPS", TextAnchor.MiddleLeft, _Color.White, 0, 24, true);
		GUI2.DrawTextRes(ChooseTeam.rBox1, "2", TextAnchor.MiddleCenter, _Color.White, 0, 24, true);
		GUI2.DrawTextRes(ChooseTeam.rMsgTeam, Lang.Get("_CHOOSE_TEAM"), TextAnchor.MiddleCenter, _Color.White, 0, 18, true);
		if (GUI2.HideButton(ChooseTeam.rLineTeam0))
		{
			ChooseTeam.Choose(0);
		}
		if (GUI2.HideButton(ChooseTeam.rLineTeam1))
		{
			ChooseTeam.Choose(1);
		}
	}

	public static void Toggle()
	{
		ChooseTeam.SetActive(!ChooseTeam.show);
	}

	public static void SetActive(bool val)
	{
		if (ScoreBoard.gamemode == 3 && val)
		{
			ChooseTeam.chooseTeamDelay = true;
			return;
		}
		ChooseTeam.show = val;
		if (ChooseTeam.show)
		{
			Crosshair.SetActive(false);
		}
		else
		{
			Crosshair.SetActive(true);
		}
		vp_FPCamera.cs.SetMouseFreeze(val);
	}

	[DebuggerHidden]
	private IEnumerator ChooseTeamDelay()
	{
		return new ChooseTeam.<ChooseTeamDelay>c__Iterator8();
	}

	private void Update()
	{
		if (ScoreBoard.gamemode == 3 && ChooseTeam.chooseTeamDelay)
		{
			ChooseTeam.chooseTeamDelay = false;
			base.StartCoroutine(this.ChooseTeamDelay());
		}
		if (!ChooseTeam.show)
		{
			return;
		}
		if (Input.GetKeyUp(KeyCode.Alpha1))
		{
			ChooseTeam.Choose(0);
		}
		else if (Input.GetKeyUp(KeyCode.Alpha2))
		{
			ChooseTeam.Choose(1);
		}
		else if (Input.GetKeyUp(KeyCode.Escape))
		{
			ChooseTeam.SetActive(false);
		}
	}

	private static void Choose(int team)
	{
		HUD.PlayStop();
		C4.SendDropBomb();
		if (PlayerPrefs.GetInt("localplay") > 0)
		{
			BasePlayer.team = team;
			BasePlayer.DevSpawn();
		}
		else
		{
			BasePlayer.team = team;
			Client.cs.send_chooseteam(team);
			ChooseTeam.SetActive(false);
			vp_FPWeaponHandler.cs.m_CurrentWeaponID = 0;
			vp_FPInput.cs.Player.SetWeapon.TryStart<int>(0);
			if (ScoreBoard.gamemode == 1 || ScoreBoard.gamemode == 2 || ScoreBoard.gamemode == 3)
			{
				HUD.SetActive(true);
				SpecCam.SetActive(true);
				SpecCam.SetFPCam();
			}
		}
	}
}
