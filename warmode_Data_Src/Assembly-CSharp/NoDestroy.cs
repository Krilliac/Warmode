using System;
using UnityEngine;

public class NoDestroy : MonoBehaviour
{
	private void Awake()
	{
		UnityEngine.Object.DontDestroyOnLoad(base.transform.gameObject);
	}
}
