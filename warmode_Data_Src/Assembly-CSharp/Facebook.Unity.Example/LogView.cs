using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Facebook.Unity.Example
{
	internal class LogView : ConsoleBase
	{
		private static string datePatt = "M/d/yyyy hh:mm:ss tt";

		private static IList<string> events = new List<string>();

		public static void AddLog(string log)
		{
			LogView.events.Insert(0, string.Format("{0}\n{1}\n", DateTime.Now.ToString(LogView.datePatt), log));
		}

		protected void OnGUI()
		{
			GUILayout.BeginVertical(new GUILayoutOption[0]);
			if (base.Button("Back"))
			{
				base.GoBack();
			}
			base.ScrollPosition = GUILayout.BeginScrollView(base.ScrollPosition, new GUILayoutOption[]
			{
				GUILayout.MinWidth((float)ConsoleBase.MainWindowFullWidth)
			});
			GUILayout.TextArea(string.Join("\n", LogView.events.ToArray<string>()), base.TextStyle, new GUILayoutOption[]
			{
				GUILayout.ExpandHeight(true),
				GUILayout.MaxWidth((float)ConsoleBase.MainWindowWidth)
			});
			GUILayout.EndScrollView();
			GUILayout.EndVertical();
		}
	}
}
