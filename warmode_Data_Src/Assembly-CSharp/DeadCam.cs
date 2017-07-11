using System;
using UnityEngine;

public class DeadCam : MonoBehaviour
{
	public static bool show;

	private static GameObject go;

	public static float setspectime;

	public static void LateUpdate()
	{
		if (!DeadCam.show)
		{
			return;
		}
		if (SpecCam.show)
		{
			return;
		}
		if (DeadCam.go == null)
		{
			DeadCam.go = null;
			DeadCam.show = false;
			BasePlayer.deadflag = 0;
			if (Client.ID >= 0)
			{
				PlayerControll.Player[Client.ID].DeadFlag = 0;
			}
			return;
		}
		Camera.main.transform.position = DeadCam.go.transform.position;
		Camera.main.transform.rotation = Quaternion.Euler(DeadCam.go.transform.eulerAngles.x, DeadCam.go.transform.eulerAngles.y + 90f, DeadCam.go.transform.eulerAngles.z + 90f);
		PlayerControll.CheckVisible();
		if (DeadCam.setspectime != 0f && Time.time > DeadCam.setspectime)
		{
			DeadCam.setspectime = 0f;
			SpecCam.SetActive(true);
			SpecCam.SetFPCam();
			Message.badge_name = string.Empty;
		}
	}

	public static void SetActive(bool val)
	{
		if (SpecCam.show)
		{
			return;
		}
		if (val)
		{
			BasePlayer.deadflag = 1;
			HUD.sHealth = "0";
			PlayerControll.Player[Client.ID].DeadFlag = 1;
			vp_FPInput.cs.AllowGameplayInput = false;
			bool deathRD = false;
			if (ScoreBoard.gamemode == 3 && BasePlayer.team == 0)
			{
				deathRD = true;
			}
			DeadCam.go = PlayerControll.CreatePlayerRD(Client.ID, 0, 0, deathRD);
			if (DeadCam.go == null)
			{
				return;
			}
			DeadCam.ChangeLayersRecursively(DeadCam.go, LayerMask.NameToLayer("Hidden"));
			DeadCam.go = GameObject.Find(DeadCam.go.name + "/Bip001/Bip001 Pelvis/Bip001 Spine/Bip001 Spine1/Bip001 Neck/Bip001 Head");
			DeadCam.show = true;
			if (DeadCam.go == null)
			{
				MonoBehaviour.print("gameobject no found");
			}
			vp_FPWeaponHandler.cs.m_CurrentWeaponID = -1;
			vp_FPInput.cs.Player.SetWeapon.TryStart<int>(0);
			ScoreTop.UpdateData();
			GameObject gameObject = GameObject.Find("WeaponCamera");
			gameObject.GetComponent<Camera>().cullingMask = 0;
			Message.SetDead(true);
			PlayerNames.hideradar = true;
			if ((ScoreBoard.gamemode == 1 || ScoreBoard.gamemode == 2 || ScoreBoard.gamemode == 3) && GameData.restartroundmode != 1)
			{
				DeadCam.setspectime = Time.time + 3f;
			}
		}
		else
		{
			DeadCam.go = null;
			DeadCam.show = false;
			BasePlayer.deadflag = 0;
			if (Client.ID != -1)
			{
				PlayerControll.Player[Client.ID].DeadFlag = 0;
			}
			GameObject gameObject2 = GameObject.Find("WeaponCamera");
			gameObject2.GetComponent<Camera>().cullingMask = -2147483648;
			DeadCam.setspectime = 0f;
		}
	}

	public static void ChangeLayersRecursively(GameObject g, int l)
	{
		g.layer = l;
		foreach (Transform transform in g.transform)
		{
			DeadCam.ChangeLayersRecursively(transform.gameObject, l);
		}
	}
}
