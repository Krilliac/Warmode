using System;
using System.Collections.Generic;

namespace Facebook.Unity
{
	internal class AccessTokenRefreshResult : ResultBase, IAccessTokenRefreshResult, IResult
	{
		public AccessToken AccessToken
		{
			get;
			private set;
		}

		public AccessTokenRefreshResult(ResultContainer resultContainer) : base(resultContainer)
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
