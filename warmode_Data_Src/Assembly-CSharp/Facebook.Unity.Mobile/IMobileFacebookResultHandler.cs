using System;

namespace Facebook.Unity.Mobile
{
	internal interface IMobileFacebookResultHandler : IFacebookResultHandler
	{
		void OnAppInviteComplete(ResultContainer resultContainer);

		void OnFetchDeferredAppLinkComplete(ResultContainer resultContainer);

		void OnRefreshCurrentAccessTokenComplete(ResultContainer resultContainer);
	}
}
