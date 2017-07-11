using System;

namespace Facebook.Unity.Example
{
	internal class AppLinks : MenuBase
	{
		protected override void GetGui()
		{
			if (base.Button("Get App Link"))
			{
				FB.GetAppLink(new FacebookDelegate<IAppLinkResult>(base.HandleResult));
			}
			if (Constants.IsMobile && base.Button("Fetch Deferred App Link"))
			{
				FB.Mobile.FetchDeferredAppLinkData(new FacebookDelegate<IAppLinkResult>(base.HandleResult));
			}
		}
	}
}
