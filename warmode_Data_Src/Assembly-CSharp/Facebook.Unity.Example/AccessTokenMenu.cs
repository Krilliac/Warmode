using System;

namespace Facebook.Unity.Example
{
	internal class AccessTokenMenu : MenuBase
	{
		protected override void GetGui()
		{
			if (base.Button("Refresh Access Token"))
			{
				FB.Mobile.RefreshCurrentAccessToken(new FacebookDelegate<IAccessTokenRefreshResult>(base.HandleResult));
			}
		}
	}
}
