using System;
using System.Collections.Generic;
using UnityEngine;

public class Batch : MonoBehaviour
{
	private static List<GameObject> arrR = new List<GameObject>();

	private static GameObject obj = null;

	public static void Combine(GameObject map)
	{
		MonoBehaviour.print("Batch Combine " + map.name);
		Batch.arrR.Clear();
		Renderer[] componentsInChildren = map.GetComponentsInChildren<Renderer>();
		Renderer[] array = componentsInChildren;
		for (int i = 0; i < array.Length; i++)
		{
			Renderer renderer = array[i];
			if (!(renderer == null))
			{
				Batch.arrR.Add(renderer.gameObject);
			}
		}
		StaticBatchingUtility.Combine(Batch.arrR.ToArray(), Batch.obj);
	}
}
