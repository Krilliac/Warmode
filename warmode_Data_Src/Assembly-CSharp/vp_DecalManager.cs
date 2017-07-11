using System;
using System.Collections.Generic;
using UnityEngine;

public sealed class vp_DecalManager
{
	public static readonly vp_DecalManager instance;

	private static List<GameObject> m_Decals;

	private static float m_MaxDecals;

	private static float m_FadedDecals;

	private static float m_NonFadedDecals;

	private static float m_FadeAmount;

	public static float MaxDecals
	{
		get
		{
			return vp_DecalManager.m_MaxDecals;
		}
		set
		{
			vp_DecalManager.m_MaxDecals = value;
			vp_DecalManager.Refresh();
		}
	}

	public static float FadedDecals
	{
		get
		{
			return vp_DecalManager.m_FadedDecals;
		}
		set
		{
			if (value > vp_DecalManager.m_MaxDecals)
			{
				Debug.LogError("FadedDecals can't be larger than MaxDecals");
				return;
			}
			vp_DecalManager.m_FadedDecals = value;
			vp_DecalManager.Refresh();
		}
	}

	private vp_DecalManager()
	{
	}

	static vp_DecalManager()
	{
		vp_DecalManager.instance = new vp_DecalManager();
		vp_DecalManager.m_Decals = new List<GameObject>();
		vp_DecalManager.m_MaxDecals = 50f;
		vp_DecalManager.m_FadedDecals = 10f;
		vp_DecalManager.m_NonFadedDecals = 0f;
		vp_DecalManager.m_FadeAmount = 0f;
		vp_DecalManager.Refresh();
	}

	public static void Add(GameObject decal)
	{
		vp_DecalManager.m_Decals.Add(decal);
		vp_DecalManager.FadeAndRemove();
	}

	private static void FadeAndRemove()
	{
		if ((float)vp_DecalManager.m_Decals.Count > vp_DecalManager.m_NonFadedDecals)
		{
			int num = 0;
			while ((float)num < (float)vp_DecalManager.m_Decals.Count - vp_DecalManager.m_NonFadedDecals)
			{
				if (vp_DecalManager.m_Decals[num] != null)
				{
					Color color = vp_DecalManager.m_Decals[num].GetComponent<Renderer>().material.color;
					color.a -= vp_DecalManager.m_FadeAmount;
					vp_DecalManager.m_Decals[num].GetComponent<Renderer>().material.color = color;
				}
				num++;
			}
		}
		if (vp_DecalManager.m_Decals[0] != null)
		{
			if (vp_DecalManager.m_Decals[0].GetComponent<Renderer>().material.color.a <= 0f)
			{
				UnityEngine.Object.Destroy(vp_DecalManager.m_Decals[0]);
				vp_DecalManager.m_Decals.Remove(vp_DecalManager.m_Decals[0]);
			}
		}
		else
		{
			vp_DecalManager.m_Decals.RemoveAt(0);
		}
	}

	private static void Refresh()
	{
		if (vp_DecalManager.m_MaxDecals < vp_DecalManager.m_FadedDecals)
		{
			vp_DecalManager.m_MaxDecals = vp_DecalManager.m_FadedDecals;
		}
		vp_DecalManager.m_FadeAmount = vp_DecalManager.m_MaxDecals / vp_DecalManager.m_FadedDecals / vp_DecalManager.m_MaxDecals;
		vp_DecalManager.m_NonFadedDecals = vp_DecalManager.m_MaxDecals - vp_DecalManager.m_FadedDecals;
	}

	private static void DebugOutput()
	{
		int num = 0;
		int num2 = 0;
		foreach (GameObject current in vp_DecalManager.m_Decals)
		{
			if (current.GetComponent<Renderer>().material.color.a == 1f)
			{
				num++;
			}
			else
			{
				num2++;
			}
		}
		Debug.Log(string.Concat(new object[]
		{
			"Decal count: ",
			vp_DecalManager.m_Decals.Count,
			", Full: ",
			num,
			", Faded: ",
			num2
		}));
	}
}
