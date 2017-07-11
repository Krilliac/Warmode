using System;

namespace Facebook.Unity.Mobile
{
	internal abstract class MobileFacebookGameObject : FacebookGameObject, IFacebookCallbackHandler, IMobileFacebookCallbackHandler
	{
		private IMobileFacebookImplementation MobileFacebook
		{
			get
			{
				return (IMobileFacebookImplementation)base.Facebook;
			}
		}

		public void OnAppInviteComplete(string message)
		{
			this.MobileFacebook.OnAppInviteComplete(new ResultContainer(message));
		}

		public void OnFetchDeferredAppLinkComplete(string message)
		{
			this.MobileFacebook.OnFetchDeferredAppLinkComplete(new ResultContainer(message));
		}

		public void OnRefreshCurrentAccessTokenComplete(string message)
		{
			this.MobileFacebook.OnRefreshCurrentAccessTokenComplete(new ResultContainer(message));
		}
	}
}
