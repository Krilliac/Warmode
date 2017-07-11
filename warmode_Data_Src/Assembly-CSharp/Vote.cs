using System;
using UnityEngine;

public class Vote : MonoBehaviour
{
	public static bool show = false;

	private static Vector2 mpos = Vector2.zero;

	private static bool voteprocess;

	private static string votetext;

	private static float voteendtime;

	private Texture2D tBlack;

	private Texture2D tWhite;

	private Texture2D tGray0;

	private Texture2D tGray2;

	private Texture2D tBlue;

	private Texture2D tRed;

	private static Rect rBack;

	private static int sid = -1;

	public void PostAwake()
	{
		this.tBlack = TEX.GetTextureByName("black");
		this.tWhite = TEX.GetTextureByName("white");
		this.tGray0 = TEX.GetTextureByName("gray0");
		this.tGray2 = TEX.GetTextureByName("gray2");
		this.tBlue = TEX.GetTextureByName("blue");
		this.tRed = TEX.GetTextureByName("red");
	}

	private static void OnResize()
	{
		float num = (float)((int)GUI2.YRES(2f));
		float num2 = (float)((int)GUI2.YRES(20f));
		float num3 = GUI2.YRES(200f);
		float num4 = (num2 + num) * 16f + num;
		Vote.rBack = new Rect((float)Screen.width / 2f - num3 / 2f, ((float)Screen.height - num4) / 2f, num3, num4);
	}

	private void OnGUI()
	{
		if (Vote.voteprocess)
		{
			if (Time.time > Vote.voteendtime)
			{
				Vote.voteprocess = false;
			}
			GUI2.DrawText(new Rect((float)Screen.width / 2f - GUI2.YRES(200f) / 2f, GUI2.YRES(40f), GUI2.YRES(200f), GUI2.YRES(40f)), Vote.votetext, TextAnchor.MiddleCenter, _Color.Green, 0, 20, true);
		}
		if (!Vote.show)
		{
			return;
		}
		GUI.DrawTexture(Vote.rBack, this.tBlack);
		float num = (float)((int)GUI2.YRES(2f));
		float num2 = (float)((int)GUI2.YRES(20f));
		GUI2.DrawTextRes(new Rect(Vote.rBack.x, Vote.rBack.y - num2, Vote.rBack.width, num2), Lang.Get("_VOTE_KICK"), TextAnchor.MiddleCenter, _Color.White, 0, 12, true);
		for (int i = 0; i < 16; i++)
		{
			if (i == Client.ID)
			{
				GUI.DrawTexture(new Rect(Vote.rBack.x + num, Vote.rBack.y + num + (num2 + num) * (float)i, Vote.rBack.width - num * 2f, num2), this.tGray0);
				GUI2.DrawTextRes(new Rect(Vote.rBack.x + num, Vote.rBack.y + num + (num2 + num) * (float)i, Vote.rBack.width - num * 2f, num2), (i + 1).ToString(), TextAnchor.MiddleLeft, _Color.Black, 0, 12, false);
				GUI2.DrawTextRes(new Rect(Vote.rBack.x + num2, Vote.rBack.y + num + (num2 + num) * (float)i, Vote.rBack.width - num * 2f, num2), BaseData.Name, TextAnchor.MiddleLeft, _Color.White, 0, 12, false);
			}
			else if (PlayerControll.Player[i] == null)
			{
				GUI.DrawTexture(new Rect(Vote.rBack.x + num, Vote.rBack.y + num + (num2 + num) * (float)i, Vote.rBack.width - num * 2f, num2), this.tGray2);
				GUI2.DrawTextRes(new Rect(Vote.rBack.x + num, Vote.rBack.y + num + (num2 + num) * (float)i, Vote.rBack.width - num * 2f, num2), (i + 1).ToString(), TextAnchor.MiddleLeft, _Color.Black, 0, 12, false);
				GUI2.DrawTextRes(new Rect(Vote.rBack.x + num2, Vote.rBack.y + num + (num2 + num) * (float)i, Vote.rBack.width - num * 2f, num2), "-/-", TextAnchor.MiddleLeft, _Color.Black, 0, 12, false);
			}
			else
			{
				if (Vote.sid == i)
				{
					GUI.DrawTexture(new Rect(Vote.rBack.x + num, Vote.rBack.y + num + (num2 + num) * (float)i, Vote.rBack.width - num * 2f, num2), this.tRed);
				}
				else
				{
					GUI.DrawTexture(new Rect(Vote.rBack.x + num, Vote.rBack.y + num + (num2 + num) * (float)i, Vote.rBack.width - num * 2f, num2), this.tWhite);
				}
				GUI2.DrawTextRes(new Rect(Vote.rBack.x + num, Vote.rBack.y + num + (num2 + num) * (float)i, Vote.rBack.width - num * 2f, num2), (i + 1).ToString(), TextAnchor.MiddleLeft, _Color.Black, 0, 12, false);
				GUI2.DrawTextRes(new Rect(Vote.rBack.x + num2, Vote.rBack.y + num + (num2 + num) * (float)i, Vote.rBack.width - num * 2f, num2), PlayerControll.Player[i].Name, TextAnchor.MiddleLeft, _Color.Black, 0, 12, false);
				if (GUI2.HideButton(new Rect(Vote.rBack.x + num2, Vote.rBack.y + num + (num2 + num) * (float)i, Vote.rBack.width - num * 2f, num2)))
				{
					Vote.sid = i;
				}
			}
		}
		GUI.DrawTexture(new Rect(Vote.rBack.x + Vote.rBack.width + GUI2.YRES(8f), Vote.rBack.y + Vote.rBack.height - num2 - num * 2f, GUI2.YRES(100f), num2 + num * 2f), this.tBlack);
		Rect rect = new Rect(Vote.rBack.x + Vote.rBack.width + GUI2.YRES(8f) + num, Vote.rBack.y + Vote.rBack.height - num2 - num, GUI2.YRES(100f) - num * 2f, num2);
		GUI.DrawTexture(rect, this.tBlue);
		GUI2.DrawTextRes(rect, Lang.Get("_SELECT"), TextAnchor.MiddleCenter, _Color.White, 0, 12, false);
		if (GUI2.HideButton(rect))
		{
			Vote_Dialog.cs.SendVoteStart((byte)Vote.sid);
			Vote.SetActive(false);
		}
		GUI.DrawTexture(new Rect(Vote.rBack.x - GUI2.YRES(8f) - num * 2f - GUI2.YRES(100f), Vote.rBack.y + Vote.rBack.height - num2 - num * 2f, GUI2.YRES(100f), num2 + num * 2f), this.tBlack);
		Rect rect2 = new Rect(Vote.rBack.x - GUI2.YRES(8f) - num - GUI2.YRES(100f), Vote.rBack.y + Vote.rBack.height - num2 - num, GUI2.YRES(100f) - num * 2f, num2);
		GUI.DrawTexture(rect2, this.tGray0);
		GUI2.DrawTextRes(rect2, Lang.Get("_CANCEL"), TextAnchor.MiddleCenter, _Color.White, 0, 12, false);
		if (GUI2.HideButton(rect2))
		{
			Vote.SetActive(false);
		}
	}

	public static void Toggle()
	{
		Vote.SetActive(!Vote.show);
	}

	public static void SetActive(bool val)
	{
		Vote.show = val;
		if (Vote.show)
		{
		}
		Vote.OnResize();
		Vote.sid = -1;
		vp_FPCamera.cs.SetMouseFreeze(val);
	}

	public static void SetProcess(string text, float time)
	{
		Vote.voteprocess = true;
		Vote.votetext = text;
		Vote.voteendtime = time;
		Crosshair.SetActive(false);
	}

	public static void HideProcess()
	{
		Vote.voteprocess = false;
	}

	private void Update()
	{
	}
}
