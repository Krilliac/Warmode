using System;
using UnityEngine;

public class ItemPreview : MonoBehaviour
{
	private static GameObject WCamera;

	public static GameObject Create(string name)
	{
		GameObject gameObject = ContentLoader_.LoadGameObject(name);
		if (gameObject == null)
		{
			MonoBehaviour.print("not founded " + name);
			return null;
		}
		return UnityEngine.Object.Instantiate(gameObject, gameObject.transform.position, gameObject.transform.rotation) as GameObject;
	}

	public static Texture2D Get()
	{
		if (ItemPreview.WCamera == null)
		{
			ItemPreview.WCamera = GameObject.Find("WCamera");
		}
		if (ItemPreview.WCamera == null)
		{
			return null;
		}
		int num = 256;
		int num2 = 256;
		RenderTexture temporary = RenderTexture.GetTemporary(num, num2, 24);
		RenderTexture.active = temporary;
		ItemPreview.WCamera.GetComponent<Camera>().targetTexture = temporary;
		ItemPreview.WCamera.GetComponent<Camera>().Render();
		Texture2D texture2D = new Texture2D(num, num2, TextureFormat.ARGB32, false);
		texture2D.ReadPixels(new Rect(0f, 0f, (float)num, (float)num2), 0, 0);
		for (int i = 0; i < 128; i++)
		{
			for (int j = 0; j < 256; j++)
			{
				Color pixel = texture2D.GetPixel(i, j);
				Color pixel2 = texture2D.GetPixel(256 - i, j);
				texture2D.SetPixel(256 - i, j, pixel);
				texture2D.SetPixel(i, j, pixel2);
			}
		}
		texture2D.Apply(true);
		return texture2D;
	}

	public static void SetSkin(GameObject go, string name)
	{
		Texture2D textureByName = TEX.GetTextureByName(name);
		Texture2D texture2D = null;
		if (textureByName == null)
		{
			TEX.GetTextureByName("white");
		}
		if (name == "glock17_camo_badboy")
		{
			texture2D = TEX.GetTextureByName(name + "_s");
		}
		Component[] componentsInChildren = go.GetComponentsInChildren(typeof(Renderer));
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			Renderer renderer = (Renderer)componentsInChildren[i];
			renderer.materials[0].SetTexture(0, textureByName);
			if (texture2D)
			{
				renderer.materials[0].SetTexture("_SpecGlossMap", texture2D);
			}
		}
	}
}
