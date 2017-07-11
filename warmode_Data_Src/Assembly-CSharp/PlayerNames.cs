using System;
using UnityEngine;

public class PlayerNames : MonoBehaviour
{
	private static bool show = true;

	private GameObject LocalPlayer;

	private Color a = new Color(1f, 1f, 1f, 0.75f);

	private Texture2D tRed;

	private Texture2D tYellow;

	private Texture2D tWhite;

	private Texture2D tRadar;

	private Texture2D tTeam;

	private Texture2D tBombIcon;

	private Rect rRadar;

	public static bool hideradar;

	public void PostAwake()
	{
		this.LocalPlayer = GameObject.Find("LocalPlayer");
		this.tRed = TEX.GetTextureByName("red");
		this.tWhite = TEX.GetTextureByName("white");
		this.tYellow = TEX.GetTextureByName("yellow");
		this.tRadar = TEX.GetTextureByName("warradar");
		this.tTeam = TEX.GetTextureByName("teammate");
		this.tBombIcon = TEX.GetTextureByName("GUI/c4_orange");
		this.OnResize();
	}

	public void OnResize()
	{
		this.rRadar = new Rect((float)Screen.width - GUI2.YRES(104f), GUI2.YRES(8f), GUI2.YRES(96f), GUI2.YRES(96f));
	}

	private void OnGUI()
	{
		if (!PlayerNames.show)
		{
			return;
		}
		if (Client.ID == -1)
		{
			return;
		}
		if (PlayerControll.Player[Client.ID] == null)
		{
			return;
		}
		this.DrawHudName();
		this.DrawHudRadar();
	}

	public static void SetActive(bool val)
	{
		PlayerNames.show = val;
	}

	private void DrawHudName()
	{
		for (int i = 0; i < 16; i++)
		{
			if (PlayerControll.Player[i] != null)
			{
				if (!(PlayerControll.Player[i].go == null))
				{
					if (PlayerControll.Player[i].Team == PlayerControll.Player[Client.ID].Team)
					{
						if (PlayerControll.vps[i])
						{
							this.DrawPlayerName(PlayerControll.Player[i].currPos, PlayerControll.Player[i].Name, PlayerControll.vp[i]);
						}
					}
				}
			}
		}
	}

	private void DrawPlayerName(Vector3 p, string name, bool drawname)
	{
		p.y += 2.5f;
		Vector3 vector = Camera.main.WorldToScreenPoint(p);
		vector.y = (float)Screen.height - vector.y;
		float num = GUI2.CalcSizeRes(name, 0, 8);
		if (drawname)
		{
			GUI.color = this.a;
			GUI2.DrawTextRes(new Rect(vector.x - num / 2f, vector.y, num, GUI2.YRES(8f)), name, TextAnchor.MiddleLeft, _Color.White, 0, 8, true);
			GUI.color = Color.white;
		}
		else if (this.tTeam != null)
		{
			GUI.DrawTexture(new Rect(vector.x - GUI2.YRES(4f), vector.y + GUI2.YRES(9f), GUI2.YRES(8f), GUI2.YRES(8f)), this.tTeam);
		}
	}

	private void DrawHudRadar()
	{
		if (PlayerNames.hideradar)
		{
			return;
		}
		float y = this.LocalPlayer.transform.localRotation.eulerAngles.y;
		GUI.DrawTexture(this.rRadar, this.tRadar);
		Matrix4x4 matrix = GUI.matrix;
		GUIUtility.RotateAroundPivot(360f - y, new Vector2(this.rRadar.center.x, this.rRadar.center.y));
		for (int i = 0; i < 16; i++)
		{
			if (PlayerControll.Player[i] != null)
			{
				if (!(PlayerControll.Player[i].go == null))
				{
					if (PlayerControll.Player[i].Team == PlayerControll.Player[Client.ID].Team)
					{
						this.DrawPlayerRadar(PlayerControll.Player[i].currPos, PlayerControll.Player[i].bomb);
					}
				}
			}
		}
		if (C4.bombdropped && ScoreBoard.gamemode == 2)
		{
			this.DrawBombRadar(C4.bombpos, PlayerControll.Player[Client.ID].Team);
		}
		GUI.matrix = matrix;
	}

	private void DrawPlayerRadar(Vector3 p, bool b)
	{
		Vector3 b2 = this.LocalPlayer.transform.position - p;
		float num = 48f;
		float num2 = 1f;
		float num3 = Vector3.Distance(Vector3.zero, b2);
		if (num3 > num)
		{
			Vector3 vector = b2.normalized * num;
			if (b)
			{
				GUI.DrawTexture(new Rect(this.rRadar.center.x + GUI2.YRES(-vector.x) * num2 - GUI2.YRES(6f), this.rRadar.center.y + GUI2.YRES(vector.z) * num2 - GUI2.YRES(3f), GUI2.YRES(12f), GUI2.YRES(6f)), this.tBombIcon);
			}
			else
			{
				GUI.DrawTexture(new Rect(this.rRadar.center.x + GUI2.YRES(-vector.x) * num2 - GUI2.YRES(1f), this.rRadar.center.y + GUI2.YRES(vector.z) * num2 - GUI2.YRES(1f), GUI2.YRES(2f), GUI2.YRES(2f)), this.tYellow);
			}
		}
		else if (b)
		{
			GUI.DrawTexture(new Rect(this.rRadar.center.x + GUI2.YRES(-b2.x) * num2 - GUI2.YRES(6f), this.rRadar.center.y + GUI2.YRES(b2.z) * num2 - GUI2.YRES(3f), GUI2.YRES(12f), GUI2.YRES(6f)), this.tBombIcon);
		}
		else
		{
			GUI.DrawTexture(new Rect(this.rRadar.center.x + GUI2.YRES(-b2.x) * num2 - GUI2.YRES(1f), this.rRadar.center.y + GUI2.YRES(b2.z) * num2 - GUI2.YRES(1f), GUI2.YRES(2f), GUI2.YRES(2f)), this.tWhite);
		}
	}

	private void DrawBombRadar(Vector3 p, int team)
	{
		Vector3 b = this.LocalPlayer.transform.position - p;
		float num = 48f;
		float num2 = 1f;
		float num3 = Vector3.Distance(Vector3.zero, b);
		if (num3 > num)
		{
			if (team == 1)
			{
				return;
			}
			Vector3 vector = b.normalized * num;
			GUI.DrawTexture(new Rect(this.rRadar.center.x + GUI2.YRES(-vector.x) * num2 - GUI2.YRES(6f), this.rRadar.center.y + GUI2.YRES(vector.z) * num2 - GUI2.YRES(3f), GUI2.YRES(12f), GUI2.YRES(6f)), this.tBombIcon);
		}
		else
		{
			GUI.DrawTexture(new Rect(this.rRadar.center.x + GUI2.YRES(-b.x) * num2 - GUI2.YRES(6f), this.rRadar.center.y + GUI2.YRES(b.z) * num2 - GUI2.YRES(3f), GUI2.YRES(12f), GUI2.YRES(6f)), this.tBombIcon);
		}
	}
}
