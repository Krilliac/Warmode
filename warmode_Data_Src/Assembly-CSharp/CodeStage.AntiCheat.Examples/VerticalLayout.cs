using System;
using UnityEngine;

namespace CodeStage.AntiCheat.Examples
{
	internal class VerticalLayout : IDisposable
	{
		public VerticalLayout(params GUILayoutOption[] options)
		{
			GUILayout.BeginVertical(options);
		}

		public VerticalLayout(GUIStyle style)
		{
			GUILayout.BeginVertical(style, new GUILayoutOption[0]);
		}

		public void Dispose()
		{
			GUILayout.EndHorizontal();
		}
	}
}
