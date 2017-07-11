using System;
using UnityEngine;

namespace Facebook.Unity
{
	internal static class FacebookLogger
	{
		private class CustomLogger : IFacebookLogger
		{
			private IFacebookLogger logger;

			public CustomLogger()
			{
				this.logger = new FacebookLogger.CanvasLogger();
			}

			public void Log(string msg)
			{
				if (Debug.isDebugBuild)
				{
					Debug.Log(msg);
					this.logger.Log(msg);
				}
			}

			public void Info(string msg)
			{
				Debug.Log(msg);
				this.logger.Info(msg);
			}

			public void Warn(string msg)
			{
				Debug.LogWarning(msg);
				this.logger.Warn(msg);
			}

			public void Error(string msg)
			{
				Debug.LogError(msg);
				this.logger.Error(msg);
			}
		}

		private class CanvasLogger : IFacebookLogger
		{
			public void Log(string msg)
			{
				Application.ExternalCall("console.log", new object[]
				{
					msg
				});
			}

			public void Info(string msg)
			{
				Application.ExternalCall("console.info", new object[]
				{
					msg
				});
			}

			public void Warn(string msg)
			{
				Application.ExternalCall("console.warn", new object[]
				{
					msg
				});
			}

			public void Error(string msg)
			{
				Application.ExternalCall("console.error", new object[]
				{
					msg
				});
			}
		}

		private const string UnityAndroidTag = "Facebook.Unity.FBDebug";

		internal static IFacebookLogger Instance
		{
			private get;
			set;
		}

		static FacebookLogger()
		{
			FacebookLogger.Instance = new FacebookLogger.CustomLogger();
		}

		public static void Log(string msg)
		{
			FacebookLogger.Instance.Log(msg);
		}

		public static void Log(string format, params string[] args)
		{
			FacebookLogger.Log(string.Format(format, args));
		}

		public static void Info(string msg)
		{
			FacebookLogger.Instance.Info(msg);
		}

		public static void Info(string format, params string[] args)
		{
			FacebookLogger.Info(string.Format(format, args));
		}

		public static void Warn(string msg)
		{
			FacebookLogger.Instance.Warn(msg);
		}

		public static void Warn(string format, params string[] args)
		{
			FacebookLogger.Warn(string.Format(format, args));
		}

		public static void Error(string msg)
		{
			FacebookLogger.Instance.Error(msg);
		}

		public static void Error(string format, params string[] args)
		{
			FacebookLogger.Error(string.Format(format, args));
		}
	}
}
