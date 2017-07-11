using System;
using System.Collections.Generic;

namespace Facebook.Unity
{
	internal class LoginResult : ResultBase, ILoginResult, IResult
	{
		public const string LastRefreshKey = "last_refresh";

		public static readonly string UserIdKey = (!Constants.IsWeb) ? "user_id" : "userID";

		public static readonly string ExpirationTimestampKey = (!Constants.IsWeb) ? "expiration_timestamp" : "expiresIn";

		public static readonly string PermissionsKey = (!Constants.IsWeb) ? "permissions" : "grantedScopes";

		public static readonly string AccessTokenKey = (!Constants.IsWeb) ? "access_token" : "accessToken";

		public AccessToken AccessToken
		{
			get;
			private set;
		}

		internal LoginResult(ResultContainer resultContainer) : base(resultContainer)
		{
			if (this.ResultDictionary != null && this.ResultDictionary.ContainsKey(LoginResult.AccessTokenKey))
			{
				this.AccessToken = Utilities.ParseAccessTokenFromResult(this.ResultDictionary);
			}
		}

		public override string ToString()
		{
			return Utilities.FormatToString(base.ToString(), base.GetType().Name, new Dictionary<string, string>
			{
				{
					"AccessToken",
					this.AccessToken.ToStringNullOk()
				}
			});
		}
	}
}
