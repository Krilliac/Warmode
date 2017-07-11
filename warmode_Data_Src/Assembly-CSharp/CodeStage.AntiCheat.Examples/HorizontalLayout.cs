using System;
using UnityEngine;

namespace CodeStage.AntiCheat.Examples
{
	internal class HorizontalLayout : IDisposable
	{
		public HorizontalLayout(params GUILayoutOption[] options)
		{
			GUILayout.BeginHorizontal(options);
		}

		public void Dispose()
		{
			GUILayout.EndHorizontal();
		}
	}
}
