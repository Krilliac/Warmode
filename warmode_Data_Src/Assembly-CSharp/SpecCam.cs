using System;
using UnityEngine;

public class SpecCam : MonoBehaviour
{
	public static int speed = 15;

	public static Vector3 position = Vector3.zero;

	public static bool show = false;

	public static int mode = 0;

	public static int forcechasecam = 1;

	public static int FID = -1;

	public static Vector3 rr = Vector3.zero;

	public static float FireTime = 0f;

	private static float lastUpdateTime = 0f;

	public static void Init()
	{
		SpecCam.show = false;
		SpecCam.position = Vector3.zero;
	}

	public static void LateUpdate()
	{
		if (!SpecCam.show)
		{
			BlackScreen.SetActive(false);
			return;
		}
		if (SpecCam.mode == 0)
		{
			SpecCam.FreeCam();
		}
		else if (SpecCam.mode == 1)
		{
			SpecCam.FPCam();
		}
		if (Input.GetKeyUp(KeyCode.Mouse1) && ScoreBoard.gamemode != 3)
		{
			if (SpecCam.mode == 0)
			{
				SpecCam.SetFPCam();
			}
			else
			{
				SpecCam.mode = 0;
				GameObject gameObject = GameObject.Find("WeaponCamera");
				gameObject.GetComponent<Camera>().cullingMask = 0;
			}
		}
		PlayerControll.CheckVisible();
		if (SpecCam.mode == 1 && SpecCam.FID >= 0)
		{
			PlayerControll.vps[SpecCam.FID] = false;
			PlayerControll.vp[SpecCam.FID] = false;
		}
	}

	private static void FreeCam()
	{
		if (SpecCam.forcechasecam == 1 && (BasePlayer.team == 0 || BasePlayer.team == 1))
		{
			BlackScreen.SetActive(true);
		}
		else
		{
			BlackScreen.SetActive(false);
		}
		if (Input.GetKey(vp_FPInput.control[4]))
		{
			SpecCam.speed = 15;
		}
		else
		{
			SpecCam.speed = 7;
		}
		if (Input.GetKey(vp_FPInput.control[1]))
		{
			SpecCam.position += Camera.main.transform.right * -1f * (float)SpecCam.speed * Time.deltaTime;
		}
		if (Input.GetKey(vp_FPInput.control[2]))
		{
			SpecCam.position += Camera.main.transform.forward * -1f * (float)SpecCam.speed * Time.deltaTime;
		}
		if (Input.GetKey(vp_FPInput.control[3]))
		{
			SpecCam.position += Camera.main.transform.right * (float)SpecCam.speed * Time.deltaTime;
		}
		if (Input.GetKey(vp_FPInput.control[0]))
		{
			SpecCam.position += Camera.main.transform.forward * (float)SpecCam.speed * Time.deltaTime;
		}
		if (Input.GetKey(vp_FPInput.control[5]))
		{
			SpecCam.position += Camera.main.transform.up * (float)SpecCam.speed * Time.deltaTime;
		}
		if (Input.GetKey(vp_FPInput.control[6]))
		{
			SpecCam.position += Camera.main.transform.up * -1f * (float)SpecCam.speed * Time.deltaTime;
		}
		Camera.main.transform.position = SpecCam.position;
	}

	public static void SetFPCam()
	{
		SpecCam.mode = 1;
		GameObject gameObject = GameObject.Find("WeaponCamera");
		gameObject.GetComponent<Camera>().cullingMask = -2147483648;
		SpecCam.FindPlayer();
		if (SpecCam.FID < 0)
		{
			SpecCam.mode = 0;
			gameObject.GetComponent<Camera>().cullingMask = 0;
		}
	}

	public static void SetForcechasecam(int val)
	{
		SpecCam.forcechasecam = val;
	}

	private static void FPCam()
	{
		if (Input.GetKeyUp(KeyCode.Mouse0))
		{
			SpecCam.FindPlayer();
		}
		if (SpecCam.FID >= 0)
		{
			if (PlayerControll.Player[SpecCam.FID] == null)
			{
				SpecCam.FID = -1;
			}
			else if (PlayerControll.Player[SpecCam.FID].go == null)
			{
				SpecCam.FID = -1;
			}
		}
		if (SpecCam.FID < 0)
		{
			SpecCam.FindPlayer();
			if (SpecCam.FID < 0)
			{
				SpecCam.mode = 0;
				GameObject gameObject = GameObject.Find("WeaponCamera");
				gameObject.GetComponent<Camera>().cullingMask = 0;
			}
			return;
		}
		float num = PlayerControll.Player[SpecCam.FID].rotation.x;
		if (num >= 270f && num <= 360f)
		{
			num = 360f - num;
		}
		else if (num >= 0f && num <= 90f)
		{
			num = 0f - num;
		}
		float num2 = SpecCam.rr.x;
		if (num2 >= 270f && num2 <= 360f)
		{
			num2 = 360f - num2;
		}
		else if (num2 >= 0f && num2 <= 90f)
		{
			num2 = 0f - num2;
		}
		float num3 = PlayerControll.Player[SpecCam.FID].rotation.y;
		float num4 = SpecCam.rr.y - PlayerControll.Player[SpecCam.FID].rotation.y;
		if (num4 > 180f)
		{
			num3 += 360f;
		}
		if (num4 < -180f)
		{
			num3 -= 360f;
		}
		SpecCam.rr = Vector3.Lerp(new Vector3(num2, SpecCam.rr.y, SpecCam.rr.z), new Vector3(num, num3, PlayerControll.Player[SpecCam.FID].rotation.z), Time.deltaTime * 5f);
		if (SpecCam.rr.x <= 0f)
		{
			SpecCam.rr.x = SpecCam.rr.x * -1f;
		}
		else
		{
			SpecCam.rr.x = 360f - SpecCam.rr.x;
		}
		Camera.main.transform.eulerAngles = SpecCam.rr;
		Crosshair.SetOffsetNull();
	}

	private static void FindPlayer()
	{
		bool flag = false;
		int i = SpecCam.FID + 1;
		if (i >= 16)
		{
			i = 0;
		}
		while (i < 16)
		{
			if (i != Client.ID)
			{
				if (PlayerControll.Player[i] != null)
				{
					if (!(PlayerControll.Player[i].go == null))
					{
						if (PlayerControll.Player[i].DeadFlag != 1)
						{
							if (SpecCam.forcechasecam != 1 || PlayerControll.Player[i].Team == BasePlayer.team)
							{
								SpecCam.FID = i;
								flag = true;
								break;
							}
						}
					}
				}
			}
			i++;
		}
		if (!flag)
		{
			for (i = 0; i < SpecCam.FID; i++)
			{
				if (i != Client.ID)
				{
					if (PlayerControll.Player[i] != null)
					{
						if (!(PlayerControll.Player[i].go == null))
						{
							if (PlayerControll.Player[i].DeadFlag != 1)
							{
								if (SpecCam.forcechasecam != 1 || PlayerControll.Player[i].Team == BasePlayer.team)
								{
									SpecCam.FID = i;
									flag = true;
									break;
								}
							}
						}
					}
				}
			}
		}
		if (!flag)
		{
			return;
		}
		if (GameData.restartroundmode == 1)
		{
			return;
		}
		int currweapon = PlayerControll.Player[SpecCam.FID].currweapon;
		vp_FPInput.cs.Player.SetWeaponByName.Try(WeaponData.GetData(currweapon).selectName);
	}

	public static void Update()
	{
		if (SpecCam.mode == 1)
		{
			SpecCam.FPMove();
		}
		if (!SpecCam.show || SpecCam.mode == 1 || Time.realtimeSinceStartup < SpecCam.lastUpdateTime + 0.2f || BasePlayer.team != 255 || ScoreBoard.gamemode != 3)
		{
			return;
		}
		if (SpecCam.mode == 0)
		{
			SpecCam.lastUpdateTime = Time.realtimeSinceStartup;
			SpecCam.SetFPCam();
		}
	}

	private static void FPMove()
	{
		if (!SpecCam.show)
		{
			return;
		}
		if (SpecCam.FID >= 0)
		{
			if (PlayerControll.Player[SpecCam.FID] == null)
			{
				SpecCam.FID = -1;
			}
			else if (PlayerControll.Player[SpecCam.FID].go == null)
			{
				SpecCam.FID = -1;
			}
		}
		if (SpecCam.FID < 0)
		{
			return;
		}
		PlayerControll.Player[SpecCam.FID].go.SetActive(false);
		vp_FPController.cs.SetPosition(PlayerControll.Player[SpecCam.FID].currPos);
		Crosshair.SetOffsetNull();
		if (Time.time > SpecCam.FireTime)
		{
			vp_FPInput.cs.Player.Attack.TryStop();
		}
	}

	public static void CurrWeapon(int id, int wid)
	{
		if (!SpecCam.show)
		{
			return;
		}
		if (SpecCam.mode == 0)
		{
			return;
		}
		if (SpecCam.FID < 0)
		{
			return;
		}
		if (SpecCam.FID != id)
		{
			return;
		}
		if (GameData.restartroundmode != 1 && BasePlayer.team != 255)
		{
			vp_FPInput.cs.Player.SetWeaponByName.Try(WeaponData.GetData(wid).selectName);
		}
	}

	public static void SetActive(bool val)
	{
		SpecCam.show = val;
		if (!val)
		{
			SpecCam.FID = -1;
			SpecCam.mode = 0;
			GameObject gameObject = GameObject.Find("WeaponCamera");
			gameObject.GetComponent<Camera>().cullingMask = -2147483648;
		}
		else
		{
			vp_FPController vp_FPController = (vp_FPController)UnityEngine.Object.FindObjectOfType(typeof(vp_FPController));
			if (vp_FPController)
			{
				vp_FPController.m_CharacterController.enabled = true;
			}
		}
	}

	public static bool Fire(int id)
	{
		if (!SpecCam.show)
		{
			return false;
		}
		if (SpecCam.mode == 0)
		{
			return false;
		}
		if (SpecCam.FID < 0)
		{
			return false;
		}
		if (SpecCam.FID != id)
		{
			return false;
		}
		vp_FPInput.cs.Player.Attack.TryStart();
		vp_FPInput.CanPistolFire = true;
		SpecCam.FireTime = Time.time + 0.01f;
		return true;
	}
}
