using System;
using UnityEngine;

public class EntControll : MonoBehaviour
{
	private static GameObject pgoEntFG;

	private static GameObject pgoEntFB;

	private static GameObject pgoEntSG;

	public static void Init()
	{
		EntControll.pgoEntFG = ContentLoader_.LoadGameObject("ent_fg");
		EntControll.pgoEntFB = ContentLoader_.LoadGameObject("ent_fb");
		EntControll.pgoEntSG = ContentLoader_.LoadGameObject("ent_sg");
	}

	public static void CreateEnt(int ownerid, int classid, int uid, int type, Vector3 pos, Vector3 rot, Vector3 force, Vector3 torque)
	{
		if (type == 0 && EntControll.pgoEntFG == null)
		{
			return;
		}
		if (type == 1 && EntControll.pgoEntFB == null)
		{
			return;
		}
		if (type == 2 && EntControll.pgoEntSG == null)
		{
			return;
		}
		GameObject gameObject = null;
		if (type == 0)
		{
			gameObject = UnityEngine.Object.Instantiate<GameObject>(EntControll.pgoEntFG);
		}
		if (type == 1)
		{
			gameObject = UnityEngine.Object.Instantiate<GameObject>(EntControll.pgoEntFB);
		}
		if (type == 2)
		{
			gameObject = UnityEngine.Object.Instantiate<GameObject>(EntControll.pgoEntSG);
		}
		gameObject.transform.position = pos;
		gameObject.transform.eulerAngles = rot;
		gameObject.name = "ent_" + uid.ToString();
		gameObject.GetComponent<Rigidbody>().AddForce(force);
		gameObject.GetComponent<Rigidbody>().AddTorque(torque);
		gameObject.GetComponent<BaseEnt>().uid = uid;
		gameObject.GetComponent<BaseEnt>().ownerid = ownerid;
		gameObject.GetComponent<BaseEnt>().type = type;
		if (ScoreBoard.gamemode == 3)
		{
			MeshRenderer component = gameObject.GetComponent<MeshRenderer>();
			ContentLoader_.ReplaceMaterialsFromRes(ref component);
		}
	}
}
