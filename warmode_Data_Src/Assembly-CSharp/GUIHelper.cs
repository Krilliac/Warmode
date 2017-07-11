using System;
using UnityEngine;

public static class GUIHelper
{
	private static GUIStyle centerAlignedLabel;

	private static GUIStyle rightAlignedLabel;

	public static Rect ClientArea;

	private static void Setup()
	{
		if (GUIHelper.centerAlignedLabel == null)
		{
			GUIHelper.centerAlignedLabel = new GUIStyle(GUI.skin.label);
			GUIHelper.centerAlignedLabel.alignment = TextAnchor.MiddleCenter;
			GUIHelper.rightAlignedLabel = new GUIStyle(GUI.skin.label);
			GUIHelper.rightAlignedLabel.alignment = TextAnchor.MiddleRight;
		}
	}

	public static void DrawArea(Rect area, bool drawHeader, Action action)
	{
		GUIHelper.Setup();
		GUI.Box(area, string.Empty);
		GUILayout.BeginArea(area);
		if (drawHeader)
		{
			GUIHelper.DrawCenteredText(SampleSelector.SelectedSample.DisplayName);
			GUILayout.Space(5f);
		}
		if (action != null)
		{
			action();
		}
		GUILayout.EndArea();
	}

	public static void DrawCenteredText(string msg)
	{
		GUIHelper.Setup();
		GUILayout.BeginHorizontal(new GUILayoutOption[0]);
		GUILayout.FlexibleSpace();
		GUILayout.Label(msg, GUIHelper.centerAlignedLabel, new GUILayoutOption[0]);
		GUILayout.FlexibleSpace();
		GUILayout.EndHorizontal();
	}

	public static void DrawRow(string key, string value)
	{
		GUIHelper.Setup();
		GUILayout.BeginHorizontal(new GUILayoutOption[0]);
		GUILayout.Label(key, new GUILayoutOption[0]);
		GUILayout.FlexibleSpace();
		GUILayout.Label(value, GUIHelper.rightAlignedLabel, new GUILayoutOption[0]);
		GUILayout.FlexibleSpace();
		GUILayout.EndHorizontal();
	}
}
