using System;
using UnityEngine;

public class BlackScreen : MonoBehaviour
{
	private static bool show;

	private static Texture2D tBlack;

	private static Rect rBack;

	public void PostAwake()
	{
		BlackScreen.tBlack = TEX.GetTextureByName("black");
		this.OnResize();
		BlackScreen.show = false;
	}

	public void OnResize()
	{
		BlackScreen.rBack = new Rect(0f, 0f, (float)Screen.width, (float)Screen.height);
	}

	public static void Draw()
	{
		if (!BlackScreen.show)
		{
			return;
		}
	}

	public static void SetActive(bool val)
	{
		BlackScreen.show = val;
	}
}
