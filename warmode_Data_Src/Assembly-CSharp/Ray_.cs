using System;
using System.Collections.Generic;
using UnityEngine;

public class Ray_ : MonoBehaviour
{
	private static GameObject CubeMesh;

	public static void Check()
	{
		if (Ray_.CubeMesh == null)
		{
			Ray_.CubeMesh = (Resources.Load("Prefabs/CubeMesh") as GameObject);
		}
		if (Ray_.CubeMesh == null)
		{
			return;
		}
		Ray_.CheckVector(new Vector3((float)UnityEngine.Random.Range(-100, 100), (float)UnityEngine.Random.Range(-100, 100), (float)UnityEngine.Random.Range(-100, 100)), Vector3.right);
		Ray_.CheckVector(new Vector3((float)UnityEngine.Random.Range(-100, 100), (float)UnityEngine.Random.Range(-100, 100), (float)UnityEngine.Random.Range(-100, 100)), Vector3.up);
		Ray_.CheckSpawns();
	}

	public static void CheckVector(Vector3 pos, Vector3 dir)
	{
		GameObject gameObject = UnityEngine.Object.Instantiate(Ray_.CubeMesh, pos, new Quaternion(0f, 0f, 0f, 0f)) as GameObject;
		if (gameObject == null)
		{
			return;
		}
		float num = UnityEngine.Random.Range(25f, 49f);
		Ray ray = new Ray(pos - dir * num, dir);
		RaycastHit raycastHit;
		if (!Physics.Raycast(ray, out raycastHit, num * 2f, vp_Layer.GetBulletBlockers()))
		{
			global::Console.cs.Command("disconnect");
		}
		UnityEngine.Object.Destroy(gameObject);
	}

	public static void CheckSpawns()
	{
		if (Client.Map != "02v2")
		{
			return;
		}
		UnityEngine.Object[] array = UnityEngine.Object.FindObjectsOfType(typeof(Spawn));
		if (array.Length == 0)
		{
			return;
		}
		List<Spawn> list = new List<Spawn>();
		List<Spawn> list2 = new List<Spawn>();
		UnityEngine.Object[] array2 = array;
		for (int i = 0; i < array2.Length; i++)
		{
			Spawn spawn = (Spawn)array2[i];
			if (spawn.Team == 0)
			{
				list.Add(spawn);
			}
			else if (spawn.Team == 1)
			{
				list2.Add(spawn);
			}
		}
		foreach (Spawn current in list)
		{
			foreach (Spawn current2 in list2)
			{
				if (!Physics.Linecast(current.transform.position, current2.transform.position))
				{
					global::Console.cs.Command("disconnect");
					return;
				}
			}
		}
	}
}
