using System;
using UnityEngine;

public class MenuPlayer : MonoBehaviour
{
	private static bool show;

	public static int playermodel = -1;

	public static GameObject pgoPlayer;

	public static GameObject goPlayer;

	private static Texture2D tBlack;

	private static Texture2D tOrange;

	private static Texture2D tBlue;

	public void PostAwake()
	{
		MenuPlayer.tBlack = TEX.GetTextureByName("black");
		MenuPlayer.tBlue = TEX.GetTextureByName("lightblue");
		MenuPlayer.tOrange = TEX.GetTextureByName("orange");
		MenuPlayer.show = false;
		MenuPlayer.playermodel = -1;
		if (ContentLoader_.proceed)
		{
			MenuPlayer.show = true;
		}
	}

	public static void SetActive(bool val)
	{
		MenuPlayer.show = val;
		if (!MenuPlayer.show)
		{
			if (MenuPlayer.goPlayer)
			{
				MenuPlayer.goPlayer.name = string.Concat(new object[]
				{
					"dead_",
					Time.time,
					" ",
					UnityEngine.Random.Range(0, 1000)
				});
				UnityEngine.Object.Destroy(MenuPlayer.goPlayer);
			}
			MenuPlayer.playermodel = -1;
			MenuFriends.SetActive(false);
		}
		else
		{
			MenuPlayer.ChangePlayer(1, 8, 22);
			MenuFriends.SetActive(true);
		}
	}

	public static void ChangePlayer(int index, int wid, int bwid)
	{
		if (MenuPlayer.pgoPlayer == null)
		{
			return;
		}
		if (index == MenuPlayer.playermodel)
		{
			return;
		}
		MenuPlayer.playermodel = index;
		if (MenuPlayer.goPlayer)
		{
			MenuPlayer.goPlayer.name = string.Concat(new object[]
			{
				"dead_",
				Time.time,
				" ",
				UnityEngine.Random.Range(0, 1000)
			});
			UnityEngine.Object.Destroy(MenuPlayer.goPlayer);
		}
		MenuPlayer.goPlayer = (UnityEngine.Object.Instantiate(MenuPlayer.pgoPlayer, new Vector3(0.3f, 0.03f, 0f), new Quaternion(0f, 0f, 0f, 0f)) as GameObject);
		MenuPlayer.goPlayer.transform.eulerAngles = new Vector3(0f, 327f, 0f);
		MenuPlayer.goPlayer.name = string.Concat(new object[]
		{
			"player_",
			index.ToString(),
			"_",
			Time.time
		});
		MenuPlayer.goPlayer.AddComponent<RotateModel>();
		if (index == 0)
		{
			GameObject.Find(MenuPlayer.goPlayer.name + "/player_warcorp").SetActive(false);
		}
		else if (index == 1)
		{
			GameObject.Find(MenuPlayer.goPlayer.name + "/player_merc").SetActive(false);
		}
		int value = 0;
		if (wid == 1)
		{
			value = 2;
		}
		if (wid == 2)
		{
			value = 2;
		}
		if (wid == 3)
		{
			value = 2;
		}
		if (wid == 4)
		{
			value = 2;
		}
		if (wid == 5)
		{
			value = 2;
		}
		if (index == 0)
		{
			GameObject.Find(MenuPlayer.goPlayer.name + "/player_merc").GetComponentInParent<Animator>().SetInteger("w", value);
		}
		else if (index == 1)
		{
			GameObject.Find(MenuPlayer.goPlayer.name + "/player_warcorp").GetComponentInParent<Animator>().SetInteger("w", value);
		}
		Renderer[] componentsInChildren = MenuPlayer.goPlayer.GetComponentsInChildren<Renderer>();
		Renderer[] array = componentsInChildren;
		for (int i = 0; i < array.Length; i++)
		{
			Renderer renderer = array[i];
			if (renderer.gameObject.name == "AimTarget" || renderer.gameObject.name == "PoleTarget")
			{
				renderer.enabled = false;
			}
		}
		if (index == 0 && BaseData.mask_merc != 0)
		{
			MenuPlayer.SetMaskTexture(0, MenuShop.shopdata[BaseData.mask_merc]);
		}
		else if (index == 1 && BaseData.mask_warcorp != 0)
		{
			MenuPlayer.SetMaskTexture(1, MenuShop.shopdata[BaseData.mask_warcorp]);
		}
		MenuPlayer.CreateWeapon(wid, bwid);
	}

	public static void SetMaskTexture(int index, MenuShop.CShopData data)
	{
		GameObject gameObject;
		if (index == 0)
		{
			gameObject = GameObject.Find(MenuPlayer.goPlayer.name + "/player_merc");
		}
		else
		{
			gameObject = GameObject.Find(MenuPlayer.goPlayer.name + "/player_warcorp");
		}
		Texture2D textureByName = TEX.GetTextureByName("_" + data.iconname);
		gameObject.GetComponent<SkinnedMeshRenderer>().materials[0].SetTexture(0, textureByName);
	}

	public static void PreviewMask(MenuShop.CShopData data)
	{
		if (data == null)
		{
			return;
		}
		if (data.section == 3)
		{
			MenuPlayer.ChangePlayer(0, 8, 22);
			MenuPlayer.SetMaskTexture(0, data);
		}
		else if (data.section == 4)
		{
			MenuPlayer.ChangePlayer(1, 8, 22);
			MenuPlayer.SetMaskTexture(1, data);
		}
	}

	public static void SetPosition(float x, float y, float z)
	{
		if (MenuPlayer.goPlayer)
		{
			MenuPlayer.goPlayer.transform.position = new Vector3(x, y, z);
		}
	}

	public static void Draw()
	{
		if (!MenuPlayer.show)
		{
			return;
		}
		GUI.color = new Color(1f, 1f, 1f, 0.8f);
		GUI.DrawTexture(new Rect((float)Screen.width / 2f + GUIM.YRES(180f), GUIM.YRES(96f), GUIM.YRES(16f), GUIM.YRES(16f)), MenuPlayer.tBlack);
		GUI.DrawTexture(new Rect((float)Screen.width / 2f + GUIM.YRES(180f) + GUIM.YRES(20f), GUIM.YRES(96f), GUIM.YRES(16f), GUIM.YRES(16f)), MenuPlayer.tBlack);
		GUI.color = Color.white;
		if (GUIM.Button(new Rect((float)Screen.width / 2f + GUIM.YRES(180f) + 2f, GUIM.YRES(96f) + 2f, GUIM.YRES(16f) - 4f, GUIM.YRES(16f) - 4f), BaseColor.Orange, string.Empty, TextAnchor.MiddleCenter, BaseColor.White, 0, 0, false))
		{
			MenuPlayer.ChangePlayer(0, 8, 22);
		}
		if (GUIM.Button(new Rect((float)Screen.width / 2f + GUIM.YRES(180f) + GUIM.YRES(20f) + 2f, GUIM.YRES(96f) + 2f, GUIM.YRES(16f) - 4f, GUIM.YRES(16f) - 4f), BaseColor.Blue, string.Empty, TextAnchor.MiddleCenter, BaseColor.White, 0, 0, false))
		{
			MenuPlayer.ChangePlayer(1, 8, 22);
		}
	}

	private void LoadEnd()
	{
		MenuPlayer.pgoPlayer = ContentLoader_.LoadGameObject("pwmplayer");
		if (MenuPlayer.pgoPlayer == null)
		{
			MonoBehaviour.print("MenuPlayer::pgoPlayer null");
		}
		if (!BaseData.Auth)
		{
			return;
		}
		Main.HideAll();
		MenuPlayer.SetActive(true);
	}

	public static void CreateWeapon(int wid, int bwid)
	{
		GameObject gameObject = ContentLoader_.LoadGameObject("p_" + MenuShop.shopdata[wid].name2);
		if (gameObject != null)
		{
			GameObject gameObject2 = UnityEngine.Object.Instantiate(gameObject, Vector3.zero, Quaternion.identity) as GameObject;
			GameObject gameObject3 = GameObject.Find(MenuPlayer.goPlayer.name + "/Bip001/Bip001 Pelvis/Bip001 Spine/Bip001 Spine1/Bip001 R Clavicle/Bip001 R UpperArm/Bip001 R Forearm/Bip001 R Hand");
			gameObject2.transform.parent = gameObject3.transform;
			gameObject2.transform.localPosition = gameObject.transform.localPosition;
			gameObject2.transform.localRotation = gameObject.transform.localRotation;
			if (MenuShop.currData != null && MenuShop.currData.section == 5 && wid >= 1 && wid <= 25)
			{
				Component[] componentsInChildren = gameObject2.GetComponentsInChildren(typeof(Renderer));
				for (int i = 0; i < componentsInChildren.Length; i++)
				{
					Renderer renderer = (Renderer)componentsInChildren[i];
					renderer.materials[0].SetTexture(0, TEX.GetTextureByName(MenuShop.currData.iconname));
				}
			}
		}
		GameObject gameObject4 = ContentLoader_.LoadGameObject("b_" + MenuShop.shopdata[bwid].name2);
		if (gameObject != null)
		{
			GameObject gameObject5 = UnityEngine.Object.Instantiate(gameObject4, Vector3.zero, Quaternion.identity) as GameObject;
			GameObject gameObject6 = GameObject.Find(MenuPlayer.goPlayer.name + "/Bip001/Bip001 Pelvis/Bip001 Spine/Bip001 Spine1");
			gameObject5.transform.parent = gameObject6.transform;
			gameObject5.transform.localPosition = gameObject4.transform.localPosition;
			gameObject5.transform.localRotation = gameObject4.transform.localRotation;
		}
	}
}
