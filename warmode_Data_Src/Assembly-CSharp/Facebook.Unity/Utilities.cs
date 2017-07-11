using Facebook.MiniJSON;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace Facebook.Unity
{
	internal static class Utilities
	{
		public delegate void Callback<T>(T obj);

		private const string WarningMissingParameter = "Did not find expected value '{0}' in dictionary";

		private static Dictionary<string, string> commandLineArguments;

		public static Dictionary<string, string> CommandLineArguments
		{
			get
			{
				if (Utilities.commandLineArguments != null)
				{
					return Utilities.commandLineArguments;
				}
				Dictionary<string, string> dictionary = new Dictionary<string, string>();
				string[] commandLineArgs = Environment.GetCommandLineArgs();
				for (int i = 0; i < commandLineArgs.Length; i++)
				{
					if (commandLineArgs[i].StartsWith("/") || commandLineArgs[i].StartsWith("-"))
					{
						string value = (i + 1 >= commandLineArgs.Length) ? null : commandLineArgs[i + 1];
						dictionary.Add(commandLineArgs[i], value);
					}
				}
				Utilities.commandLineArguments = dictionary;
				return Utilities.commandLineArguments;
			}
		}

		public static bool TryGetValue<T>(this IDictionary<string, object> dictionary, string key, out T value)
		{
			object obj;
			if (dictionary.TryGetValue(key, out obj) && obj is T)
			{
				value = (T)((object)obj);
				return true;
			}
			value = default(T);
			return false;
		}

		public static long TotalSeconds(this DateTime dateTime)
		{
			return (long)(dateTime - new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalSeconds;
		}

		public static T GetValueOrDefault<T>(this IDictionary<string, object> dictionary, string key, bool logWarning = true)
		{
			T result;
			if (!dictionary.TryGetValue(key, out result) && logWarning)
			{
				FacebookLogger.Warn("Did not find expected value '{0}' in dictionary", new string[]
				{
					key
				});
			}
			return result;
		}

		public static string ToCommaSeparateList(this IEnumerable<string> list)
		{
			if (list == null)
			{
				return string.Empty;
			}
			return string.Join(",", list.ToArray<string>());
		}

		public static string AbsoluteUrlOrEmptyString(this Uri uri)
		{
			if (uri == null)
			{
				return string.Empty;
			}
			return uri.AbsoluteUri;
		}

		public static string GetUserAgent(string productName, string productVersion)
		{
			return string.Format(CultureInfo.InvariantCulture, "{0}/{1}", new object[]
			{
				productName,
				productVersion
			});
		}

		public static string ToJson(this IDictionary<string, object> dictionary)
		{
			return Json.Serialize(dictionary);
		}

		public static void AddAllKVPFrom<T1, T2>(this IDictionary<T1, T2> dest, IDictionary<T1, T2> source)
		{
			foreach (T1 current in source.Keys)
			{
				dest[current] = source[current];
			}
		}

		public static AccessToken ParseAccessTokenFromResult(IDictionary<string, object> resultDictionary)
		{
			string valueOrDefault = resultDictionary.GetValueOrDefault(LoginResult.UserIdKey, true);
			string valueOrDefault2 = resultDictionary.GetValueOrDefault(LoginResult.AccessTokenKey, true);
			DateTime expirationTime = Utilities.ParseExpirationDateFromResult(resultDictionary);
			ICollection<string> permissions = Utilities.ParsePermissionFromResult(resultDictionary);
			DateTime? lastRefresh = Utilities.ParseLastRefreshFromResult(resultDictionary);
			return new AccessToken(valueOrDefault2, valueOrDefault, expirationTime, permissions, lastRefresh);
		}

		public static string ToStringNullOk(this object obj)
		{
			if (obj == null)
			{
				return "null";
			}
			return obj.ToString();
		}

		public static string FormatToString(string baseString, string className, IDictionary<string, string> propertiesAndValues)
		{
			StringBuilder stringBuilder = new StringBuilder();
			if (baseString != null)
			{
				stringBuilder.Append(baseString);
			}
			stringBuilder.AppendFormat("\n{0}:", className);
			foreach (KeyValuePair<string, string> current in propertiesAndValues)
			{
				string arg = (current.Value == null) ? "null" : current.Value;
				stringBuilder.AppendFormat("\n\t{0}: {1}", current.Key, arg);
			}
			return stringBuilder.ToString();
		}

		private static DateTime ParseExpirationDateFromResult(IDictionary<string, object> resultDictionary)
		{
			DateTime result;
			if (Constants.IsWeb)
			{
				long valueOrDefault = resultDictionary.GetValueOrDefault(LoginResult.ExpirationTimestampKey, true);
				result = DateTime.UtcNow.AddSeconds((double)valueOrDefault);
			}
			else
			{
				string valueOrDefault2 = resultDictionary.GetValueOrDefault(LoginResult.ExpirationTimestampKey, true);
				int num;
				if (int.TryParse(valueOrDefault2, out num) && num > 0)
				{
					result = Utilities.FromTimestamp(num);
				}
				else
				{
					result = DateTime.MaxValue;
				}
			}
			return result;
		}

		private static DateTime? ParseLastRefreshFromResult(IDictionary<string, object> resultDictionary)
		{
			string valueOrDefault = resultDictionary.GetValueOrDefault("last_refresh", false);
			int num;
			if (int.TryParse(valueOrDefault, out num) && num > 0)
			{
				return new DateTime?(Utilities.FromTimestamp(num));
			}
			return null;
		}

		private static ICollection<string> ParsePermissionFromResult(IDictionary<string, object> resultDictionary)
		{
			string text;
			IEnumerable<object> source;
			if (resultDictionary.TryGetValue(LoginResult.PermissionsKey, out text))
			{
				source = text.Split(new char[]
				{
					','
				});
			}
			else if (!resultDictionary.TryGetValue(LoginResult.PermissionsKey, out source))
			{
				source = new string[0];
				FacebookLogger.Warn("Failed to find parameter '{0}' in login result", new string[]
				{
					LoginResult.PermissionsKey
				});
			}
			return (from permission in source
			select permission.ToString()).ToList<string>();
		}

		private static DateTime FromTimestamp(int timestamp)
		{
			DateTime dateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0);
			return dateTime.AddSeconds((double)timestamp);
		}
	}
}
