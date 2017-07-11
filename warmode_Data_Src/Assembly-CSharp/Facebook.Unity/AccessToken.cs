using Facebook.MiniJSON;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Facebook.Unity
{
	public class AccessToken
	{
		public static AccessToken CurrentAccessToken
		{
			get;
			internal set;
		}

		public string TokenString
		{
			get;
			private set;
		}

		public DateTime ExpirationTime
		{
			get;
			private set;
		}

		public IEnumerable<string> Permissions
		{
			get;
			private set;
		}

		public string UserId
		{
			get;
			private set;
		}

		public DateTime? LastRefresh
		{
			get;
			private set;
		}

		internal AccessToken(string tokenString, string userId, DateTime expirationTime, IEnumerable<string> permissions, DateTime? lastRefresh)
		{
			if (string.IsNullOrEmpty(tokenString))
			{
				throw new ArgumentNullException("tokenString");
			}
			if (string.IsNullOrEmpty(userId))
			{
				throw new ArgumentNullException("userId");
			}
			if (expirationTime == DateTime.MinValue)
			{
				throw new ArgumentException("Expiration time is unassigned");
			}
			if (permissions == null)
			{
				throw new ArgumentNullException("permissions");
			}
			this.TokenString = tokenString;
			this.ExpirationTime = expirationTime;
			this.Permissions = permissions;
			this.UserId = userId;
			this.LastRefresh = lastRefresh;
		}

		public override string ToString()
		{
			return Utilities.FormatToString(null, base.GetType().Name, new Dictionary<string, string>
			{
				{
					"ExpirationTime",
					this.ExpirationTime.TotalSeconds().ToString()
				},
				{
					"Permissions",
					this.Permissions.ToCommaSeparateList()
				},
				{
					"UserId",
					this.UserId.ToStringNullOk()
				},
				{
					"LastRefresh",
					this.LastRefresh.ToStringNullOk()
				}
			});
		}

		internal string ToJson()
		{
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			dictionary[LoginResult.PermissionsKey] = string.Join(",", this.Permissions.ToArray<string>());
			dictionary[LoginResult.ExpirationTimestampKey] = this.ExpirationTime.TotalSeconds().ToString();
			dictionary[LoginResult.AccessTokenKey] = this.TokenString;
			dictionary[LoginResult.UserIdKey] = this.UserId;
			if (this.LastRefresh.HasValue)
			{
				dictionary["last_refresh"] = this.LastRefresh.Value.TotalSeconds().ToString();
			}
			return Json.Serialize(dictionary);
		}
	}
}
