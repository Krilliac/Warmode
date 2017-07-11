using System;
using UnityEngine;

public class HitEffect : MonoBehaviour
{
	private static bool show;

	private static float time;

	public void PostAwake()
	{
	}

	public static void SetActive()
	{
		HitEffect.time = Time.time + 0.2f;
		HitEffect.show = true;
	}

	public static void Reset()
	{
	}

	private void Update()
	{
		if (!HitEffect.show)
		{
			return;
		}
		if (Time.time > HitEffect.time)
		{
			HitEffect.show = false;
			return;
		}
	}
}
