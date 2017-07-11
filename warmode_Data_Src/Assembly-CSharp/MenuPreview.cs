using System;
using UnityEngine;

public class MenuPreview : MonoBehaviour
{
	private static bool show;

	private static MenuShop.CShopData currData;

	private static GameObject go;

	private static Rect rBack;

	private static Rect rBackSave;

	private static Rect rButtonSave;

	private static Texture2D tBlack;

	public void LoadEnd()
	{
		MenuPreview.tBlack = TEX.GetTextureByName("black");
	}

	private void OnResize()
	{
		MenuPreview.rBack = new Rect((float)Screen.width / 2f - GUIM.YRES(165f), GUIM.YRES(80f), GUIM.YRES(632f), GUIM.YRES(525f));
		MenuPreview.rBackSave = new Rect(MenuPreview.rBack.x + MenuPreview.rBack.width - GUIM.YRES(200f), MenuPreview.rBack.y + MenuPreview.rBack.height + GUIM.YRES(8f), GUIM.YRES(200f), GUIM.YRES(32f));
		MenuPreview.rButtonSave = new Rect(MenuPreview.rBackSave.x + GUIM.YRES(4f), MenuPreview.rBackSave.y + GUIM.YRES(4f), GUIM.YRES(192f), GUIM.YRES(24f));
	}

	public static void SetActive(bool val)
	{
		MenuPreview.show = val;
		MenuPreview.currData = null;
		if (!MenuPreview.show && MenuPreview.go != null)
		{
			MenuPreview.go.name = string.Concat(new object[]
			{
				"item_",
				Time.time,
				" ",
				UnityEngine.Random.Range(0, 1000)
			});
			UnityEngine.Object.Destroy(MenuPreview.go);
		}
	}

	public static void Preview(MenuShop.CShopData item)
	{
		if (MenuPreview.go != null)
		{
			MenuPreview.go.name = string.Concat(new object[]
			{
				"item_",
				Time.time,
				" ",
				UnityEngine.Random.Range(0, 1000)
			});
			UnityEngine.Object.Destroy(MenuPreview.go);
		}
		MenuPreview.currData = item;
		if (MenuPreview.currData == null)
		{
			return;
		}
		string[] array = MenuPreview.currData.iconname.Split(new char[]
		{
			'_'
		});
		if (array.Length != 3)
		{
			return;
		}
		MenuPreview.go = ItemPreview.Create("w_" + array[0]);
		if (MenuPreview.go == null)
		{
			MonoBehaviour.print("error create preview w_" + array[0]);
			return;
		}
		ItemPreview.SetSkin(MenuPreview.go, MenuPreview.currData.iconname);
		Transform[] componentsInChildren = MenuPreview.go.GetComponentsInChildren<Transform>();
		Transform[] array2 = componentsInChildren;
		for (int i = 0; i < array2.Length; i++)
		{
			Transform transform = array2[i];
			transform.gameObject.layer = 0;
		}
		RotateModel rotateModel = MenuPreview.go.AddComponent<RotateModel>();
		rotateModel.fullrotate = true;
	}

	public static void Draw()
	{
		if (!MenuPreview.show)
		{
			return;
		}
		GUIM.DrawBox(MenuPreview.rBackSave, MenuPreview.tBlack);
		if (GUIM.Button(MenuPreview.rButtonSave, BaseColor.Blue, Lang.Get("_BACK"), TextAnchor.MiddleCenter, BaseColor.White, 1, 12, true))
		{
			MenuShop.CShopData cShopData = MenuPreview.currData;
			Main.HideAll();
			MenuShop.SetActive(true);
			MenuShop.currData = cShopData;
		}
	}
}
