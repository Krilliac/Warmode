using BestHTTP.Caching;
using System;
using UnityEngine;

public sealed class CacheMaintenanceSample : MonoBehaviour
{
	private enum DeleteOlderTypes
	{
		Days,
		Hours,
		Mins,
		Secs
	}

	private CacheMaintenanceSample.DeleteOlderTypes deleteOlderType = CacheMaintenanceSample.DeleteOlderTypes.Secs;

	private int value = 10;

	private int maxCacheSize = 5242880;

	private void OnGUI()
	{
		GUIHelper.DrawArea(GUIHelper.ClientArea, true, delegate
		{
			GUILayout.BeginHorizontal(new GUILayoutOption[0]);
			GUILayout.Label("Delete cached entities older then", new GUILayoutOption[0]);
			GUILayout.Label(this.value.ToString(), new GUILayoutOption[]
			{
				GUILayout.MinWidth(50f)
			});
			this.value = (int)GUILayout.HorizontalSlider((float)this.value, 1f, 60f, new GUILayoutOption[]
			{
				GUILayout.MinWidth(100f)
			});
			GUILayout.Space(10f);
			this.deleteOlderType = (CacheMaintenanceSample.DeleteOlderTypes)GUILayout.SelectionGrid((int)this.deleteOlderType, new string[]
			{
				"Days",
				"Hours",
				"Mins",
				"Secs"
			}, 4, new GUILayoutOption[0]);
			GUILayout.FlexibleSpace();
			GUILayout.EndHorizontal();
			GUILayout.Space(10f);
			GUILayout.BeginHorizontal(new GUILayoutOption[0]);
			GUILayout.Label("Max Cache Size (bytes): ", new GUILayoutOption[]
			{
				GUILayout.Width(150f)
			});
			GUILayout.Label(this.maxCacheSize.ToString("N0"), new GUILayoutOption[]
			{
				GUILayout.Width(70f)
			});
			this.maxCacheSize = (int)GUILayout.HorizontalSlider((float)this.maxCacheSize, 1024f, 1.048576E+07f, new GUILayoutOption[0]);
			GUILayout.EndHorizontal();
			GUILayout.Space(10f);
			if (GUILayout.Button("Maintenance", new GUILayoutOption[0]))
			{
				TimeSpan deleteOlder = TimeSpan.FromDays(14.0);
				switch (this.deleteOlderType)
				{
				case CacheMaintenanceSample.DeleteOlderTypes.Days:
					deleteOlder = TimeSpan.FromDays((double)this.value);
					break;
				case CacheMaintenanceSample.DeleteOlderTypes.Hours:
					deleteOlder = TimeSpan.FromHours((double)this.value);
					break;
				case CacheMaintenanceSample.DeleteOlderTypes.Mins:
					deleteOlder = TimeSpan.FromMinutes((double)this.value);
					break;
				case CacheMaintenanceSample.DeleteOlderTypes.Secs:
					deleteOlder = TimeSpan.FromSeconds((double)this.value);
					break;
				}
				HTTPCacheService.BeginMaintainence(new HTTPCacheMaintananceParams(deleteOlder, (ulong)((long)this.maxCacheSize)));
			}
		});
	}
}
