using System;
using UnityEngine;

namespace RootMotion
{
	public static class Warning
	{
		public delegate void Logger(string message);

		public static bool logged;

		public static void Log(string message, Warning.Logger logger, bool logInEditMode = false)
		{
			if (!logInEditMode && !Application.isPlaying)
			{
				return;
			}
			if (Warning.logged)
			{
				return;
			}
			if (logger != null)
			{
				logger(message);
			}
			Warning.logged = true;
		}

		public static void Log(string message, Transform context, bool logInEditMode = false)
		{
			if (!logInEditMode && !Application.isPlaying)
			{
				return;
			}
			if (Warning.logged)
			{
				return;
			}
			Debug.LogWarning(message, context);
			Warning.logged = true;
		}
	}
}
