using Facebook.MiniJSON;
using System;
using System.Collections.Generic;

namespace Facebook.Unity.Mobile
{
	internal abstract class MobileFacebook : FacebookBase, IFacebook, IFacebookResultHandler, IMobileFacebook, IMobileFacebookImplementation, IMobileFacebookResultHandler
	{
		private const string CallbackIdKey = "callback_id";

		private ShareDialogMode shareDialogMode;

		public ShareDialogMode ShareDialogMode
		{
			get
			{
				return this.shareDialogMode;
			}
			set
			{
				this.shareDialogMode = value;
				this.SetShareDialogMode(this.shareDialogMode);
			}
		}

		protected MobileFacebook(CallbackManager callbackManager) : base(callbackManager)
		{
		}

		public abstract void AppInvite(Uri appLinkUrl, Uri previewImageUrl, FacebookDelegate<IAppInviteResult> callback);

		public abstract void FetchDeferredAppLink(FacebookDelegate<IAppLinkResult> callback);

		public abstract void RefreshCurrentAccessToken(FacebookDelegate<IAccessTokenRefreshResult> callback);

		public override void OnLoginComplete(ResultContainer resultContainer)
		{
			LoginResult result = new LoginResult(resultContainer);
			base.OnAuthResponse(result);
		}

		public override void OnGetAppLinkComplete(ResultContainer resultContainer)
		{
			AppLinkResult result = new AppLinkResult(resultContainer);
			base.CallbackManager.OnFacebookResponse(result);
		}

		public override void OnGroupCreateComplete(ResultContainer resultContainer)
		{
			GroupCreateResult result = new GroupCreateResult(resultContainer);
			base.CallbackManager.OnFacebookResponse(result);
		}

		public override void OnGroupJoinComplete(ResultContainer resultContainer)
		{
			GroupJoinResult result = new GroupJoinResult(resultContainer);
			base.CallbackManager.OnFacebookResponse(result);
		}

		public override void OnAppRequestsComplete(ResultContainer resultContainer)
		{
			AppRequestResult result = new AppRequestResult(resultContainer);
			base.CallbackManager.OnFacebookResponse(result);
		}

		public void OnAppInviteComplete(ResultContainer resultContainer)
		{
			AppInviteResult result = new AppInviteResult(resultContainer);
			base.CallbackManager.OnFacebookResponse(result);
		}

		public void OnFetchDeferredAppLinkComplete(ResultContainer resultContainer)
		{
			AppLinkResult result = new AppLinkResult(resultContainer);
			base.CallbackManager.OnFacebookResponse(result);
		}

		public override void OnShareLinkComplete(ResultContainer resultContainer)
		{
			ShareResult result = new ShareResult(resultContainer);
			base.CallbackManager.OnFacebookResponse(result);
		}

		public void OnRefreshCurrentAccessTokenComplete(ResultContainer resultContainer)
		{
			AccessTokenRefreshResult accessTokenRefreshResult = new AccessTokenRefreshResult(resultContainer);
			if (accessTokenRefreshResult.AccessToken != null)
			{
				AccessToken.CurrentAccessToken = accessTokenRefreshResult.AccessToken;
			}
			base.CallbackManager.OnFacebookResponse(accessTokenRefreshResult);
		}

		protected abstract void SetShareDialogMode(ShareDialogMode mode);

		private static IDictionary<string, object> DeserializeMessage(string message)
		{
			return (Dictionary<string, object>)Json.Deserialize(message);
		}

		private static string SerializeDictionary(IDictionary<string, object> dict)
		{
			return Json.Serialize(dict);
		}

		private static bool TryGetCallbackId(IDictionary<string, object> result, out string callbackId)
		{
			callbackId = null;
			object obj;
			if (result.TryGetValue("callback_id", out obj))
			{
				callbackId = (obj as string);
				return true;
			}
			return false;
		}

		private static bool TryGetError(IDictionary<string, object> result, out string errorMessage)
		{
			errorMessage = null;
			object obj;
			if (result.TryGetValue("error", out obj))
			{
				errorMessage = (obj as string);
				return true;
			}
			return false;
		}
	}
}
