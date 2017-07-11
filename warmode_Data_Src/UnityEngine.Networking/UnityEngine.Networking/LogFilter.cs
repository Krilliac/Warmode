using System;

namespace UnityEngine.Networking
{
	public class LogFilter
	{
		public enum FilterLevel
		{
			Developer,
			Debug,
			Info,
			Warn,
			Error,
			Fatal
		}

		internal const int Developer = 0;

		public const int Debug = 1;

		public const int Info = 2;

		public const int Warn = 3;

		public const int Error = 4;

		public const int Fatal = 5;

		public static LogFilter.FilterLevel current = LogFilter.FilterLevel.Info;

		private static int s_CurrentLogLevel = 2;

		public static int currentLogLevel
		{
			get
			{
				return LogFilter.s_CurrentLogLevel;
			}
			set
			{
				LogFilter.s_CurrentLogLevel = value;
			}
		}

		internal static bool logDev
		{
			get
			{
				return LogFilter.s_CurrentLogLevel <= 0;
			}
		}

		public static bool logDebug
		{
			get
			{
				return LogFilter.s_CurrentLogLevel <= 1;
			}
		}

		public static bool logInfo
		{
			get
			{
				return LogFilter.s_CurrentLogLevel <= 2;
			}
		}

		public static bool logWarn
		{
			get
			{
				return LogFilter.s_CurrentLogLevel <= 3;
			}
		}

		public static bool logError
		{
			get
			{
				return LogFilter.s_CurrentLogLevel <= 4;
			}
		}

		public static bool logFatal
		{
			get
			{
				return LogFilter.s_CurrentLogLevel <= 5;
			}
		}
	}
}
