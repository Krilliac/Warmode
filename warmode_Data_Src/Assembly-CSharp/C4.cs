using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class C4 : MonoBehaviour
{
	public struct PlantSector
	{
		public float x;

		public float y;

		public float z;

		public float rx;

		public float ry;

		public float rz;

		public PlantSector(float x, float y, float z, float rx, float ry, float rz)
		{
			this.x = x;
			this.y = y;
			this.z = z;
			this.rx = rx;
			this.ry = ry;
			this.rz = rz;
		}

		public PlantSector(GameObject go)
		{
			this.x = go.transform.position.x;
			this.y = go.transform.position.y;
			this.z = go.transform.position.z;
			this.rx = go.transform.localScale.x / 2f;
			this.ry = go.transform.localScale.y / 2f;
			this.rz = go.transform.localScale.z / 2f;
		}
	}

	public static float tDropVisible = 0f;

	public static bool canplant = true;

	public static bool isplanting = false;

	private static bool inplantzone = false;

	private static float planttime = 3f;

	public static float plantstarttime = 0f;

	private static C4.PlantSector[] sector;

	private static List<C4.PlantSector>[] plant;

	private static float pickupradius = 0.9f;

	public static Vector3 bombpos;

	public static float bombangle;

	private static float diffuseradius = 2f;

	public static bool bombplanted = false;

	public static bool bombdropped = false;

	public static bool bombdiffused = false;

	private static bool canpickup = false;

	public static bool isdiffusing = false;

	private static float diffusetime = 10f;

	public static float diffusestarttime = 0f;

	private static float checkTime = 0f;

	private static Transform trPlayer = null;

	private static GameObject placeBomb = null;

	public static GameObject bombGo = null;

	private static bool showDiffuseBar;

	public static int diffuserId = -1;

	private vp_FPCamera csCamera;

	private vp_FPController csController;

	public static AudioClip plantStartSound = new AudioClip();

	private static RaycastHit hit;

	public void PostAwake()
	{
		C4.checkTime = Time.realtimeSinceStartup;
		C4.trPlayer = GameObject.Find("LocalPlayer").transform;
		C4.placeBomb = ContentLoader_.LoadGameObject("c4_place");
		C4.plantStartSound = SND.GetSoundByName("c4_plant");
		C4.showDiffuseBar = false;
		C4.sector = new C4.PlantSector[2];
		C4.plant = new List<C4.PlantSector>[2];
		C4.plant[0] = new List<C4.PlantSector>();
		C4.plant[1] = new List<C4.PlantSector>();
		this.csCamera = (vp_FPCamera)UnityEngine.Object.FindObjectOfType(typeof(vp_FPCamera));
		this.csController = (vp_FPController)UnityEngine.Object.FindObjectOfType(typeof(vp_FPController));
	}

	public static void GetPlants()
	{
		if (ScoreBoard.gamemode != 2)
		{
			return;
		}
		UnityEngine.Object[] array = UnityEngine.Object.FindObjectsOfType(typeof(Sector));
		UnityEngine.Object[] array2 = array;
		for (int i = 0; i < array2.Length; i++)
		{
			Sector sector = (Sector)array2[i];
			Transform transform = sector.gameObject.transform;
			if (sector.SectorAB == 0)
			{
				C4.sector[0] = new C4.PlantSector(sector.gameObject);
			}
			else if (sector.SectorAB == 1)
			{
				C4.sector[1] = new C4.PlantSector(sector.gameObject);
			}
		}
		C4.plant[0].Clear();
		C4.plant[1].Clear();
		UnityEngine.Object[] array3 = UnityEngine.Object.FindObjectsOfType(typeof(Plant));
		UnityEngine.Object[] array4 = array3;
		for (int j = 0; j < array4.Length; j++)
		{
			Plant plant = (Plant)array4[j];
			if (plant.PointAB == 0)
			{
				C4.plant[0].Add(new C4.PlantSector(plant.gameObject));
			}
			else if (plant.PointAB == 1)
			{
				C4.plant[1].Add(new C4.PlantSector(plant.gameObject));
			}
		}
	}

	public static void SetBombPlanted(bool val)
	{
		C4.bombplanted = val;
	}

	public static void SetBombDiffused(bool val)
	{
		C4.bombdiffused = val;
	}

	public static void SetBombDropped(bool val)
	{
		C4.bombdropped = val;
	}

	public static void SetShowDiffuseBar(bool val)
	{
		C4.showDiffuseBar = val;
	}

	public static void SetDiffuserId(int val)
	{
		C4.diffuserId = val;
	}

	public static void CreateBomb(Vector3 pos, float angle_y, bool activate)
	{
		if (Physics.Raycast(pos + new Vector3(0f, 0f, 0f), Vector3.down, out C4.hit))
		{
		}
		C4.bombGo = UnityEngine.Object.Instantiate<GameObject>(C4.placeBomb);
		float y = pos.y;
		pos += new Vector3(0f, 1f, 0f);
		C4.bombGo.transform.position = pos;
		foreach (Transform transform in C4.bombGo.transform)
		{
			if (Physics.Raycast(transform.position, Vector3.down, out C4.hit, 3f, 1) && C4.hit.point.y > y)
			{
				y = C4.hit.point.y;
			}
		}
		pos = new Vector3(pos.x, y, pos.z);
		C4.bombGo.transform.position = pos;
		C4.bombGo.transform.eulerAngles = new Vector3(0f, angle_y, 0f);
		C4.bombGo.name = "bomb";
		if (activate)
		{
			C4.bombGo.AddComponent<C4Place>();
			C4.bombGo.GetComponent<C4Place>().Init();
			C4Place.Activate(true);
		}
		C4.bombpos = pos;
	}

	public static void DestroyBomb()
	{
		if (C4.bombGo == null)
		{
			return;
		}
		UnityEngine.Object.Destroy(C4.bombGo);
	}

	[DebuggerHidden]
	public static IEnumerator KeyboardUnlockDelay()
	{
		return new C4.<KeyboardUnlockDelay>c__Iterator7();
	}

	private void CheckPlant(int val)
	{
		foreach (C4.PlantSector current in C4.plant[val])
		{
			if (Mathf.Abs(C4.trPlayer.position.x - current.x) < current.rx && Mathf.Abs(C4.trPlayer.position.y - current.y) < current.ry && Mathf.Abs(C4.trPlayer.position.z - current.z) < current.rz)
			{
				C4.inplantzone = true;
			}
		}
	}

	private void OnPlanting()
	{
		if (BasePlayer.team != 0)
		{
			return;
		}
		if (!Input.GetMouseButton(0) && C4.bombplanted)
		{
			vp_FPInput.lockKeyboard = false;
		}
		if (BasePlayer.weapon[4] == null)
		{
			HUD.SetBombState(0);
			return;
		}
		C4.inplantzone = false;
		if (C4.plant[0].Count > 0 && Mathf.Abs(C4.trPlayer.position.x - C4.sector[0].x) < C4.sector[0].rx && Mathf.Abs(C4.trPlayer.position.y - C4.sector[0].y) < C4.sector[0].ry && Mathf.Abs(C4.trPlayer.position.z - C4.sector[0].z) < C4.sector[0].rz)
		{
			this.CheckPlant(0);
		}
		if (C4.plant[1].Count > 0 && Mathf.Abs(C4.trPlayer.position.x - C4.sector[1].x) < C4.sector[1].rx && Mathf.Abs(C4.trPlayer.position.y - C4.sector[1].y) < C4.sector[1].ry && Mathf.Abs(C4.trPlayer.position.z - C4.sector[1].z) < C4.sector[1].rz)
		{
			this.CheckPlant(1);
		}
		if (C4.inplantzone)
		{
			HUD.SetBombState(2);
		}
		else
		{
			HUD.SetBombState(1);
		}
		if (BasePlayer.currweapon == null)
		{
			return;
		}
		if (BasePlayer.currweapon.data.wid != 50)
		{
			return;
		}
		if (!Input.GetMouseButton(0))
		{
			if (C4.isplanting)
			{
				vp_FPInput.lockKeyboard = false;
				Client.cs.send_bomb_planted(0);
			}
			C4.isplanting = false;
		}
		if (C4.inplantzone)
		{
			if (Input.GetMouseButtonDown(0) && !C4.isplanting)
			{
				C4.isplanting = true;
				C4.canplant = false;
				vp_FPInput.lockKeyboard = true;
				Client.cs.send_bomb_planting(C4.trPlayer.position.x, C4.trPlayer.position.y, C4.trPlayer.position.z, C4.trPlayer.localEulerAngles.y);
			}
			if (!Input.GetMouseButton(0) || !C4.isplanting || C4.canplant)
			{
			}
		}
	}

	private void OnDiffusing()
	{
		if (BasePlayer.team != 1)
		{
			return;
		}
		if (!Input.GetKey(KeyCode.E))
		{
			if (C4.isdiffusing)
			{
				Client.cs.send_bomb_diffused(0);
			}
			C4.isdiffusing = false;
		}
		if (!C4.bombplanted || C4.bombdiffused)
		{
			return;
		}
		if (Mathf.Abs(C4.trPlayer.position.x - C4.bombpos[0]) < C4.diffuseradius && Mathf.Abs(C4.trPlayer.position.y - C4.bombpos[1]) < C4.diffuseradius && Mathf.Abs(C4.trPlayer.position.z - C4.bombpos[2]) < C4.diffuseradius)
		{
			if (Input.GetKeyDown(KeyCode.E) && !C4.isdiffusing)
			{
				C4.isdiffusing = true;
				Client.cs.send_bomb_diffusing(C4.trPlayer.position.x, C4.trPlayer.position.y, C4.trPlayer.position.z);
			}
			if (!Input.GetKey(KeyCode.E) || C4.isdiffusing)
			{
			}
			return;
		}
	}

	private void OnDrop()
	{
		if (BasePlayer.weapon[4] == null || BasePlayer.currweapon == null)
		{
			return;
		}
		if (BasePlayer.currweapon.data.wid != 50)
		{
			return;
		}
	}

	public static void SendDropBomb()
	{
		if (!BasePlayer.bomb)
		{
			return;
		}
		Client.cs.send_bomb_drop(C4.trPlayer.position.x, C4.trPlayer.position.y, C4.trPlayer.position.z, C4.trPlayer.localEulerAngles.y);
	}

	public static void OnRecvDropBomb(int id)
	{
		PlayerControll.Player[id].bomb = false;
		C4.CreateBomb(new Vector3(C4.bombpos[0], C4.bombpos[1], C4.bombpos[2]), C4.bombangle, false);
		C4.bombdropped = true;
		C4.canpickup = true;
		if (id == Client.ID)
		{
			C4.canpickup = false;
			PlayerControll.Player[Client.ID].bomb = false;
			BasePlayer.bomb = false;
			C4.bombdropped = true;
			BasePlayer.weapon[4] = null;
			if (BasePlayer.weapon[0] != null)
			{
				vp_FPInput.cs.Player.SetWeaponByName.Try(BasePlayer.weapon[0].data.selectName);
			}
			else if (BasePlayer.weapon[1] != null)
			{
				vp_FPInput.cs.Player.SetWeaponByName.Try(BasePlayer.weapon[1].data.selectName);
			}
		}
	}

	private void Update()
	{
		if (C4.showDiffuseBar && Client.ID == C4.diffuserId)
		{
			HUD.SetDiffuseProgress(C4.diffusetime, Time.time - C4.diffusestarttime);
		}
		else
		{
			HUD.SetDiffuseProgress(0f, -1f);
		}
		if (ScoreBoard.gamemode != 2)
		{
			return;
		}
		this.OnPlanting();
		this.OnDiffusing();
	}
}
