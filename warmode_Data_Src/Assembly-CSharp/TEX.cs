using System;
using System.Collections.Generic;
using UnityEngine;

public class TEX : MonoBehaviour
{
	private static List<Texture2D> texlist = new List<Texture2D>();

	private static Texture2D curTex = null;

	public static void Init()
	{
		TEX.GenerateTexture(Color.black, "black");
		TEX.GenerateTexture(Color.white, "white");
		TEX.GenerateTexture(new Color(0.1f, 0.1f, 0.1f, 1f), "gray0");
		TEX.GenerateTexture(new Color(0.25f, 0.25f, 0.25f, 1f), "gray1");
		TEX.GenerateTexture(new Color(0.35f, 0.35f, 0.35f, 1f), "gray2");
		TEX.GenerateTexture(new Color(1f, 0.2f, 0f, 1f), "red");
		TEX.GenerateTexture(new Color(0f, 0.4f, 1f, 1f), "blue");
		TEX.GenerateTexture(new Color(1f, 1f, 0f, 1f), "yellow");
		TEX.GenerateTexture(new Color(0.3f, 0.3f, 0.3f, 1f), "gray");
		TEX.GenerateTexture(new Color(0.15f, 0.8f, 0.15f, 1f), "green");
		TEX.GenerateTexture(new Color(0f, 0.5f, 1f, 1f), "lightblue");
		TEX.GenerateTexture(new Color(1f, 0.2f, 0f, 1f), "orange");
	}

	public static void GenerateTexture(Color c, string name)
	{
		Texture2D texture2D = new Texture2D(8, 8);
		texture2D.name = name;
		for (int i = 0; i < 8; i++)
		{
			for (int j = 0; j < 8; j++)
			{
				texture2D.SetPixel(i, j, c);
			}
		}
		texture2D.Apply(true);
		TEX.texlist.Add(texture2D);
	}

	public static Texture2D GetTextureByIndex(int index)
	{
		if (TEX.texlist.Count < index)
		{
			return null;
		}
		return TEX.texlist[index];
	}

	public static Texture2D GetTextureByName(string _name)
	{
		foreach (Texture2D current in TEX.texlist)
		{
			if (current.name == _name)
			{
				return current;
			}
		}
		TEX.curTex = null;
		string[] array = _name.Split(new char[]
		{
			'/'
		});
		if (array.Length > 1)
		{
			TEX.curTex = (ContentLoader_.LoadTexture(array[array.Length - 1]) as Texture2D);
		}
		else
		{
			TEX.curTex = (ContentLoader_.LoadTexture(_name) as Texture2D);
		}
		return TEX.curTex;
	}
}
